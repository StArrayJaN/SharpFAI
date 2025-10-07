using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;
using SharpFAI.Serialization;

namespace SharpFAI.Util;

/// <summary>
/// Utility class for ADOFAI level calculations and operations
/// ADOFAI关卡实用工具类
/// </summary>
public static class LevelUtils
{
    private static List<double> noteTimesCache = new List<double>();
    private static List<double> noteTimesCacheWithOffset = new List<double>();
    private static List<double> allSpeedChange = new List<double>();
    
    /// <summary>
    /// Calculates note timing for each tile in the level
    /// 计算关卡中每个瓦片的音符时间
    /// </summary>
    /// <param name="level">The level to calculate note times for / 要计算音符时间的关卡</param>
    /// <param name="addOffset">Whether to add the level settings offset to the times (default: false) / 是否将关卡设置中的偏移添加到时间中（默认：false）</param>
    /// <returns>List of note times in milliseconds / 以毫秒为单位的音符时间列表</returns>
    public static List<double> GetNoteTimes(this Level level, bool addOffset = false)
    {
        if (noteTimesCache.Count > 0 && !addOffset) return noteTimesCache;
        if (noteTimesCacheWithOffset.Count > 0 && addOffset) return noteTimesCacheWithOffset;
        var angleDataList = level.angles;
        JArray levelEvents = level.actions;
        JArray parsedChart = new JArray();

        // 初步处理轨道数据
        for (int i = 0; i < angleDataList.Count; i++)
        {
            double angleData = angleDataList[i];
            if (Math.Abs(angleData - 999) < 0.01)
            {
                JObject temp = new JObject
                {
                    ["angle"] = Fmod(angleDataList[i - 1] + 180, 360),
                    ["bpm"] = "unSet",
                    ["direction"] = 0,
                    ["extraHold"] = 0,
                    ["midr"] = true,
                    ["MultiPlanet"] = "-1"
                };

                parsedChart.Add(temp);
            }
            else
            {
                JObject temp = new JObject
                {
                    ["angle"] = Fmod(angleData, 360),
                    ["bpm"] = "unSet",
                    ["direction"] = 0,
                    ["extraHold"] = 0,
                    ["midr"] = false,
                    ["MultiPlanet"] = "-1"
                };

                parsedChart.Add(temp);
            }
        }
        JObject last = new JObject
        {
            ["angle"] = 0,
            ["bpm"] = "unSet",
            ["direction"] = 0,
            ["extraHold"] = 0,
            ["midr"] = false,
            ["MultiPlanet"] = "-1"
        };

        parsedChart.Add(last);

        double bpm = level.GetSetting<double>("bpm");
        double pitch = level.GetSetting<int>("pitch") / 100.0;

        // 处理事件数据
        foreach (var eventValue in levelEvents)
        {
            JObject o = eventValue as JObject;
            if (o != null)
            {
                int tile = (int)o["floor"];
                string eventType = (string)o["eventType"];

                JObject ob = parsedChart[tile] as JObject;
                if (ob != null)
                {
                    switch (eventType)
                    {
                        case "SetSpeed":
                            if ((string)o["speedType"] == "Multiplier")
                            {
                                bpm = (double)o["bpmMultiplier"] * bpm;
                            }
                            else if ((string)o["speedType"] == "Bpm")
                            {
                                bpm = (double)o["beatsPerMinute"] * pitch;
                            }

                            ob["bpm"] = bpm;
                            break;

                        case "Twirl":
                            ob["direction"] = -1;
                            break;

                        case "Pause":
                            ob["extraHold"] = (double)o["duration"] / 2.0;
                            break;

                        case "Hold":
                            ob["extraHold"] = (double)o["duration"];
                            break;

                        case "MultiPlanet":
                            ob["MultiPlanet"] = (string)o["planets"] == "ThreePlanets" ? "1" : "0";
                            break;
                    }
                    parsedChart[tile] = ob;
                }
            }
        }

        double currentBPM = level.GetSetting<double>("bpm") * pitch;
        int direction = 1;

        // 应用全局设置
        foreach (var t in parsedChart)
        {
            JObject ob = t as JObject;
            if (ob != null)
            {
                // 方向处理
                if ((int)ob["direction"] == -1)
                {
                    direction *= -1;
                }

                ob["direction"] = direction;

                // BPM处理
                if ((string)ob["bpm"] == "unSet")
                {
                    ob["bpm"] = currentBPM;
                }
                else
                {
                    currentBPM = (double)ob["bpm"];
                }
            }
        }

        List<double> noteTime = new List<double>();

        double curAngle = 0;
        double curTime = 0;
        bool isMultiPlanet = false;

        foreach (var chartValue in parsedChart)
        {
            JObject o = chartValue as JObject;
            if (o != null)
            {
                curAngle = Fmod(curAngle - 180, 360);
                double curBPM = (double)o["bpm"];
                double destAngle = (double)o["angle"];

                double pAngle = (Fmod(destAngle - curAngle, 360) <= 0.001 || Fmod(destAngle - curAngle, 360) >= 359.999)
                    ? 360
                    : Fmod((curAngle - destAngle) * (int)o["direction"], 360);

                pAngle += (double)o["extraHold"] * 360;

                // 三球处理逻辑
                double angleTemp = pAngle;
                if (isMultiPlanet)
                {
                    pAngle = pAngle > 60 ? pAngle - 60 : pAngle + 300;
                }

                string multiPlanet = (string)o["MultiPlanet"];
                if (multiPlanet != "-1")
                {
                    isMultiPlanet = multiPlanet == "1";
                    pAngle = isMultiPlanet
                        ? (pAngle > 60 ? pAngle - 60 : pAngle + 300)
                        : angleTemp;
                }

                // 计算时间
                double deltaTime = (bool)o["midr"] ? 0 : AngleToTime(pAngle, curBPM);
                curTime += deltaTime;

                curAngle = destAngle;
                noteTime.Add(curTime);
            }
        }
        noteTimesCache = noteTime;
        if (addOffset)
        {
            noteTimesCacheWithOffset = noteTime.Select(t => t + level.GetSetting<int>("offset")).ToList();
        }
        return noteTime;
        double Fmod(double a, double b) => a - b * Math.Floor(a / b);
        double AngleToTime(double angle, double bpm)
        {
            return (angle / 180) * (60 / bpm) * 1000;
        }
    }

    /// <summary>
    /// Gets all BPM changes throughout the level
    /// 获取整个关卡中的所有BPM变化
    /// </summary>
    /// <param name="level">The level to analyze / 要分析的关卡</param>
    /// <returns>List of speed values for each tile / 每个瓦片的速度值列表</returns>
    public static List<double> GetAllSpeedChange(this Level level)
    {
        if (allSpeedChange.Count > 0) return allSpeedChange;
        double[] speeds = new double[level.angleData.Count];
        double speed = level.GetSetting<double>("bpm");
        for (int i = 0; i < level.angles.Count; i++)
        {
            if (level.HasEvents(i,EventType.SetSpeed))
            {
                var a = level.GetEvents(i, EventType.SetSpeed);
                foreach (var o in a)
                {
                    if (o["speedType"].ToObject<string>() == "Multiplier")
                    {
                        speed *= o["bpmMultiplier"].ToObject<double>();
                    }
                    else if (o["speedType"].ToObject<string>() == "Bpm")
                    {
                        speed = o["beatsPerMinute"].ToObject<double>();
                    }
                }
            }
            speeds[i] = speed;
        }

        allSpeedChange = speeds.ToList();
        return speeds.ToList();
    }
    
    /// <summary>
    /// 生成滑音
    /// Generate a glide
    /// </summary>
    /// <param name="level">关卡实例 / Level Instance</param>
    /// <param name="startFloor">开始的砖块 / The starting floor</param>
    /// <param name="startNote">开始的音调 / The starting pitch</param>
    /// <param name="endNote">结束的音调 / The ending</param>
    /// <param name="duration">时长(秒) / Duration (in seconds)</param>
    public static void GenerateGlide(this Level level,int startFloor,Pitch startNote, Pitch endNote, double duration)
    {
        if (startFloor < 0 || startFloor >= level.angles.Count) throw new ArgumentOutOfRangeException($"{nameof(startFloor)}:{startFloor}");
        double current = 0;
        double endFrequency = PitchHelper.GetFrequency(endNote);
        List<double> bpmList = new List<double>();
        while (current < duration - 2 / endFrequency)
        {
            double currentPitch = PitchHelper.GetGlidePitch(startNote, endNote, current / duration);
            current += 1 / currentPitch;
            bpmList.Add(currentPitch * 60);
        }
        bpmList.Add(1 / (duration - current) * 60);

        for (int i = 0; i < bpmList.Count; i++)
        {
            JObject eventInfo = new JObject();
            eventInfo["beatsPerMinute"] = bpmList[i];
            eventInfo["speedType"] = "Bpm";
            if (level.angles.Count-1 < i)
            {
                level.angleData.Add(0);
            }
            level.AddEvent(startFloor + i, EventType.SetSpeed, eventInfo);
        }
    }

    /// <summary>
    /// remove VFXs
    /// 移除视觉效果
    /// </summary>
    /// <param name="level">关卡实例 / Level Instance</param>
    /// <param name="includeDecorations">是否包含装饰 / Whether to include decorations</param>
    /// <param name="includeTracks">是否包含砖块视觉效果 / Whether to include tracks</param> 
    /// <param name="onDelete">删除时的回调（传递被删除项的JSON字符串）；可为 null / Callback invoked on deletion (receives deleted item's JSON string); can be null</param>
    public static void RemoveVFXs(this Level level,bool includeDecorations = false,bool includeTracks = false,Action<string> onDelete = null)
    {
        List<EventType> vfxTypes =
        [
            EventType.SetFilter,
            EventType.SetFilterAdvanced,
            EventType.MoveCamera, 
            EventType.Flash,
            EventType.Bloom,
            EventType.ScreenScroll,
            EventType.ShakeScreen,
            EventType.ScreenTile,
            EventType.CustomBackground,
            EventType.HallOfMirrors,
            EventType.SetFrameRate
        ];
        EventType[] decorationTypes =
        [
            EventType.AddDecoration,
            EventType.AddText,
            EventType.AddObject,
            EventType.AddParticle,
        ];
        if (includeDecorations)
        {
            vfxTypes.AddRange(
            [
                EventType.EmitParticle,
                EventType.SetParticle,
                EventType.SetObject,
                EventType.SetText,
                EventType.SetDefaultText,
                EventType.MoveDecorations, 
            ]);
        }

        if (includeTracks)
        {
            vfxTypes.AddRange([
                EventType.MoveTrack,
                EventType.RecolorTrack, 
                EventType.ColorTrack, 
                EventType.AnimateTrack
            ]);
        }
        for (int i = 0; i < level.actions.Count; i++)
        {
            var action = level.actions[i];
            foreach (var type in vfxTypes)
            {
                if (action["eventType"].ToObject<string>() == type.ToString())
                {
                    level.actions.RemoveAt(i);
                    onDelete?.Invoke(action.ToString());
                    i--;
                    break;
                }
            }
        }

        if (includeDecorations)
        {
            for (int i = 0; i < level.decorations.Count; i++)
            {
                var decoration = level.decorations[i];
                foreach (var type in decorationTypes)
                {
                    if (decoration["eventType"].ToObject<string>() == type.ToString())
                    {
                        onDelete?.Invoke(decoration.ToString());
                        level.decorations.RemoveAt(i);
                        i--;
                        break;
                    }
                }
            }
        }
    }
}