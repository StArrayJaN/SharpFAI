using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Newtonsoft.Json.Linq;
using SharpFAI.Events;
using SharpFAI.Framework;
using SharpFAI.Serialization;

namespace SharpFAI.Util;

/// <summary>
/// Utility class for ADOFAI level calculations and operations
/// ADOFAI关卡实用工具类
/// </summary>
public static class LevelUtils
{
    private static List<double> noteTimesCache = [];
    private static List<double> noteTimesCacheWithOffset = [];
    private static List<double> allSpeedChange = [];

    public static event Action<int, string> progressCallback;

    private class ParsedChart
    {
        public double angle;
        public double bpm;
        public int direction;
        public double extraHold;
        public bool midr;
        public int multiPlanet;
    }

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
        List<ParsedChart> parsedChart = [];

        // 初步处理轨道数据
        for (int i = 0; i < angleDataList.Count; i++)
        {
            double angleData = angleDataList[i];
            if (Math.Abs(angleData - 999) < 0.01)
            {
                parsedChart.Add(new()
                {
                    angle = Fmod(angleDataList[i - 1] + 180, 360),
                    bpm = 0,
                    direction = 0,
                    extraHold = 0,
                    midr = true,
                    multiPlanet = -1
                });
            }
            else
            {
                parsedChart.Add(new()
                {
                    angle = Fmod(angleData, 360),
                    bpm = 0,
                    direction = 0,
                    extraHold = 0,
                    midr = false,
                    multiPlanet = -1
                });
            }
        }

        // 添加最后一个标记点
        parsedChart.Add(new()
        {
            angle = 0,
            bpm = 0,
            direction = 0,
            extraHold = 0,
            midr = false,
            multiPlanet = -1
        });


        double bpm = level.GetSetting<double>("bpm");
        double pitch = level.GetSetting<int>("pitch") / 100.0;
        var levelEvents = level.DeserializeEvents();

        // 处理事件数据
        foreach (var eventValue in levelEvents)
        {
            var o = eventValue;
            if (o != null)
            {
                int tile = o.Floor;
                var eventType = o.EventType;

                var ob = parsedChart[tile];
                //if (ob == null) continue;
                switch (eventType)
                {
                    case EventType.SetSpeed:
                        var eventObj = eventValue.ToEvent<SetSpeed>();
                        if (eventObj.SpeedType == EventEnums.SpeedType.Multiplier)
                        {
                            bpm = eventObj.BpmMultiplier * bpm;
                        }
                        else if (eventObj.SpeedType == EventEnums.SpeedType.Bpm)
                        {
                            bpm = eventObj.BeatsPerMinute * pitch;
                        }

                        ob.bpm = bpm;
                        break;

                    case EventType.Twirl:
                        ob.direction = -1;
                        break;

                    case EventType.Pause:
                        ob.extraHold = eventValue.ToEvent<Pause>().Duration / 2.0;
                        break;

                    case EventType.Hold:
                        ob.extraHold = eventValue.ToEvent<Hold>().Duration;
                        break;

                    case EventType.MultiPlanet:
                        ob.multiPlanet =
                            eventValue.ToEvent<MultiPlanet>().Planets == EventEnums.PlanetCount.ThreePlanets ? 1 : 0;
                        break;

                    case EventType.FreeRoam:
                        ob.extraHold = (eventValue.ToEvent<FreeRoam>().Duration - 1) / 2.0;
                        break;
                }

                parsedChart[tile] = ob;
            }
        }

        double currentBPM = level.GetSetting<double>("bpm") * pitch;
        int direction = 1;

        // 应用全局设置
        foreach (var t in parsedChart)
        {
            var ob = t;
            //if (ob == null) continue;
            // 方向处理
            if (ob.direction == -1)
            {
                direction *= -1;
            }

            ob.direction = direction;

            // BPM处理
            if (ob.bpm == 0)
            {
                ob.bpm = currentBPM;
            }
            else
            {
                currentBPM = ob.bpm;
            }
        }

        List<double> noteTime = [];

        double curAngle = 0;
        double curTime = 0;
        bool isMultiPlanet = false;

        foreach (var chartValue in parsedChart)
        {
            var o = chartValue;

            curAngle = Fmod(curAngle - 180, 360);
            double curBPM = o.bpm;
            double destAngle = o.angle;

            double pAngle = Fmod(destAngle - curAngle, 360) <= 0.001 || Fmod(destAngle - curAngle, 360) >= 359.999
                ? 360
                : Fmod((curAngle - destAngle) * o.direction, 360);

            pAngle += o.extraHold * 360;

            // 三球处理逻辑
            double angleTemp = pAngle;
            if (isMultiPlanet)
            {
                pAngle = pAngle > 60 ? pAngle - 60 : pAngle + 300;
            }

            int multiPlanet = o.multiPlanet;
            if (multiPlanet != -1)
            {
                isMultiPlanet = multiPlanet == 1;
                pAngle = isMultiPlanet
                    ? pAngle > 60 ? pAngle - 60 : pAngle + 300
                    : angleTemp;
            }

            // 计算时间
            double deltaTime = o.midr ? 0 : AngleToTime(pAngle, curBPM);
            curTime += deltaTime;

            curAngle = destAngle;
            noteTime.Add(curTime);
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
            return angle / 180 * (60 / bpm) * 1000;
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
            if (level.HasEvents(i, EventType.SetSpeed))
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
    public static void GenerateGlide(this Level level, int startFloor, Pitch startNote, Pitch endNote, double duration)
    {
        if (startFloor < 0 || startFloor >= level.angles.Count)
            throw new ArgumentOutOfRangeException($"{nameof(startFloor)}:{startFloor}");
        double current = 0;
        double endFrequency = PitchHelper.GetFrequency(endNote);
        List<double> noteTimes = [];
        while (current < duration - 2 / endFrequency)
        {
            double currentPitch = PitchHelper.GetGlidePitch(startNote, endNote, current / duration);
            current += 1 / currentPitch;
            noteTimes.Add(currentPitch * 60);
        }

        noteTimes.Add(1 / (duration - current) * 60);

        for (int i = 0; i < noteTimes.Count; i++)
        {
            if (level.angles.Count - 1 < i)
            {
                level.angleData.Add(0);
            }

            level.AddEvent(new SetSpeed()
            {
                Floor = startFloor + i,
                BeatsPerMinute = noteTimes[i],
                SpeedType = EventEnums.SpeedType.Bpm
            });
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
    public static void RemoveVFXs(this Level level, bool includeDecorations = false, bool includeTracks = false,
        Action<string> onDelete = null)
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

    /// <summary>
    /// Add a cube decorations with depth effect to the level / 向关卡添加一个具有深度效果的立方体装饰
    /// </summary>
    /// <param name="level">The level to add the cube to / 要添加立方体的关卡</param>
    /// <param name="cubeImage">The image path or name of the cube texture / 立方体纹理的图像路径或名称</param>
    /// <param name="position">The initial position (x, y) of the cube / 立方体的初始位置(x,y)</param>
    /// <param name="size">The initial size (width, height) of the cube / 立方体的初始大小(宽度,高度)</param>
    /// <param name="floorCount">The number of depth layers to create (default: 100) / 要创建的深度层数（默认：100）</param>
    /// <param name="floor">The floor number (default: 0) / 地板编号（默认：0）</param>
    /// <param name="tag">The decoration tag (default: empty) / 装饰标签（默认：空）</param>
    /// <param name="relativeToScreen">Whether relative to screen or tile (default: false) / 是否相对于屏幕或瓦片（默认：false）</param>
    /// <remarks>
    /// This method creates a parallax effect by generating multiple layers of the same cube,
    /// each with different scale and parallax values based on their depth.
    /// 此方法通过生成同一立方体的多个层来创建视差效果，
    /// 每层根据其深度具有不同的缩放和视差值。
    /// </remarks>
    public static void AddCube(this Level level, string cubeImage, Tuple<float, float> position,
        Tuple<float, float> size, int floorCount = 100, int floor = 0, string tag = "", bool relativeToScreen = false)
    {
        for (int i = 0; i < floorCount; i++)
        {
            float depthRatio = (float)i / (floorCount - 1);
            float parallaxValue = 100 * depthRatio;
            float scaleMultiplier = 1.0f - depthRatio * 0.5f; // 最远处缩小到50%
            float scaledWidth = size.Item1 * scaleMultiplier;
            float scaledHeight = size.Item2 * scaleMultiplier;
            JObject data = new JObject
            {
                ["decorationImage"] = cubeImage,
                ["position"] = new JArray(position.Item1, position.Item2),
                ["scale"] = new JArray(scaledWidth, scaledHeight),
                ["parallax"] = new JArray(parallaxValue, parallaxValue), // 水平和垂直使用相同的平行值
                ["depth"] = i, // 设置深度，确保正确的渲染顺序
                ["syncFloorDepth"] = false
            };
            level.AddDecoration(floor, tag: tag, relativeToScreen: relativeToScreen, data: data);
        }
    }

    /// <summary>
    /// Creates Floor objects from the level's angle data with positions and properties
    /// 从关卡的角度数据创建Floor对象，包含位置和属性
    /// </summary>
    /// <param name="level">The level to create floors from / 要创建地板的关卡</param>
    /// <param name="startPosition">Starting position for the first floor (default: Vector2.Zero) / 第一个地板的起始位置（默认：Vector2.Zero）</param>
    /// <param name="usePositionTrack">Whether to apply PositionTrack events to floor positions / 是否将PositionTrack事件应用于地板位置</param>
    /// <returns>List of Floor objects representing the level's path / 表示关卡路径的Floor对象列表</returns>
    /// <remarks>
    /// This method processes the level's angle data, events (SetSpeed, PositionTrack),
    /// and generates Floor instances with correct positions, angles, BPM, and mesh data.
    /// 此方法处理关卡的角度数据、事件（SetSpeed、PositionTrack），
    /// 并生成具有正确位置、角度、BPM和网格数据的Floor实例。
    /// </remarks>
    public static List<Floor> CreateFloors(this Level level, Vector2 startPosition = default,
        bool usePositionTrack = false)
    {
        List<Floor> floors = new List<Floor>();
        progressCallback?.Invoke(0, "获取打击时间点");
        var noteTimes = level.GetNoteTimes();
        double[] anglesArray = level.angles.ToArray();
        List<bool> midSpins = new();
        for (int i = 0; i < anglesArray.Length; i++)
        {
            midSpins.Add(anglesArray[i].Equals(999));
            if (anglesArray[i].Equals(999))
            {
                anglesArray[i] = anglesArray[i - 1] + 180;
            }
        }

        int n = anglesArray.Length + 1;
        double[] SetSpeedBpm = new Double[n];
        double[] SetSpeedMultiplier = new Double[n];
        bool[] setSpeedIsMultiplier = new Boolean[n];
        for (int i = 0; i < n; i++)
        {
            SetSpeedBpm[i] = 0;
            SetSpeedMultiplier[i] = 0;
            setSpeedIsMultiplier[i] = false;
        }

        Vector2 startPos = new Vector2(startPosition.X, startPosition.Y);
        progressCallback?.Invoke(25, "初始化事件");
        List<BaseEvent> allEvents = level.DeserializeEvents().ToList();
        for (int a = 0; a < allEvents.Count; a++)
        {
            BaseEvent eventObj = allEvents[a];
            int floor = eventObj.Floor;
            if (eventObj is SetSpeed setSpeed)
            {
                if (setSpeed.SpeedType == EventEnums.SpeedType.Multiplier)
                {
                    SetSpeedMultiplier[floor] = setSpeed.BpmMultiplier;
                    setSpeedIsMultiplier[floor] = true;
                }
                else
                {
                    SetSpeedBpm[floor] = setSpeed.BeatsPerMinute;
                    setSpeedIsMultiplier[floor] = false;
                }
            }
        }

        // 1. 并行批量创建Floor及其Mesh
        Floor[] tileArr = new Floor[n];
        Vector2[] posArr = new Vector2[n];
        float[] angle1Arr = new float[n];
        float[] angle2Arr = new float[n];
        progressCallback?.Invoke(50, "初始化位置");
        List<PositionTrack> positionTracks = allEvents.Where(e => e is PositionTrack).Cast<PositionTrack>().ToList();
        for (int i = 0; i < n; i++)
        {
            float angle1 = (i == anglesArray.Length) ? (float)anglesArray[i - 1] : (float)anglesArray[i];
            float angle2 = (i == 0) ? 0 : (float)anglesArray[i - 1];
            // 先判断当前floor是否有PositionTrack，若有则修正startPos
            if (usePositionTrack)
            {
                foreach (var positionTrack in positionTracks)
                {
                    if (positionTrack.Floor == i && !positionTrack.EditorOnly)
                    {
                        Vector2 position = new Vector2(positionTrack.PositionOffset[0].ToFloat(),
                            positionTrack.PositionOffset[1].ToFloat());
                        startPos += new Vector2(position.X * Floor.length * 2, position.Y * Floor.length * 2);
                    }
                }
            }

            posArr[i] = new(startPos.X, startPos.Y);
            angle1Arr[i] = angle1;
            angle2Arr[i] = angle2;
            Vector2 step = new Vector2(Floor.length * 2 * FloatMath.Cos(angle1, true),
                Floor.length * 2 * FloatMath.Sin(angle1, true));
            startPos += (step);
            Floor tile = new Floor(angle1Arr[i], angle2Arr[i] - 180, posArr[i]);
            if (i == anglesArray.Length)
            {
                tile.isMidspin = (false);
            }
            else
            {
                tile.isMidspin = (midSpins[i]);
            }

            tile.angle = i == anglesArray.Length ? (float)anglesArray[i - 1] + 180 : (float)anglesArray[i];
            tileArr[i] = tile;
        }

        progressCallback?.Invoke(75, "初始化变速与链表关系");
        double bpm = level.GetSetting<double>("bpm");
        bool isCW = true;
        for (int i = 0; i < tileArr.Length; i++)
        {
            Floor tile = tileArr[i];
            if (setSpeedIsMultiplier[i] && SetSpeedMultiplier[i] != 0)
            {
                bpm *= SetSpeedMultiplier[i];
            }
            else if (SetSpeedBpm[i] != 0)
            {
                bpm = SetSpeedBpm[i];
            }

            tile.bpm = bpm;
            if (level.HasEvents(i, EventType.Twirl))
            {
                isCW = !isCW;
            }

            tile.isCW = isCW;
            tile.index = i;
            if (i < tileArr.Length - 1)
            {
                tile.nextFloor = tileArr[i + 1];
            }

            if (i > 0)
            {
                tile.lastFloor = tileArr[i - 1];
            }

            tile.entryTime = noteTimes[i];
            tile.renderOrder = -i;
            tile.events = allEvents.Where(x => x.Floor == i).ToList();
            floors.Add(tile);
            progressCallback?.Invoke(75, $"初始化变速与链表关系({i}/{tileArr.Length - 1})");
        }

        progressCallback?.Invoke(100, "完成");
        return floors;
    }

    /// <summary>
    /// Get the floor index corresponding to a specific note time in milliseconds
    /// 获取与特定音符时间（以毫秒为单位）对应的地板索引
    /// </summary>
    /// <param name="level">The level to analyze / 要分析的关卡</param>
    /// <param name="noteTimeSecond">The note time in seconds / 以秒为单位的音符时间</param>
    /// <returns>The floor index / 地板索引</returns>
    public static int GetFloorIndexByNoteTime(this Level level, double noteTimeSecond)
    {
        List<double> noteTimes = level.GetNoteTimes(true);
        double targetTime = noteTimeSecond * 1000;

        int left = 0;
        int right = noteTimes.Count - 1;

        // 如果目标时间大于所有元素，直接返回最后一个索引
        if (targetTime >= noteTimes[right])
        {
            return right;
        }

        while (left <= right)
        {
            int mid = left + (right - left) / 2;

            if (noteTimes[mid] >= targetTime)
            {
                // 如果mid是第一个满足条件的元素，或者是当前元素满足条件但前一个不满足
                if (mid == 0 || noteTimes[mid - 1] < targetTime)
                {
                    return mid;
                }

                right = mid - 1;
            }
            else
            {
                left = mid + 1;
            }
        }

        return noteTimes.Count - 1;
    }
}