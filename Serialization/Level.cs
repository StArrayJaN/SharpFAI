using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace SharpFAI.Serialization
{
    /// <summary>
    /// 表示一个 ADOFAI 关卡，具有解析和操作功能
    /// Represents an ADOFAI level with parsing and manipulation capabilities
    /// </summary>
    public class Level
    {
        /// <summary>
        /// 表示关卡数据的 JSON 对象
        /// JSON object representing level data
        /// </summary>
        public JObject root{ get; }
        
        /// <summary>
        /// 表示关卡设置的 JSON 对象
        /// JSON object representing level settings
        /// </summary>
        private JObject settings{ get; }
        
        /// <summary>
        /// 表示关卡砖块角度的 JSON 数组
        /// JSON array representing the angle of level tiles
        /// </summary>
        public JArray angleData{ get; private set; }
        
        /// <summary>
        /// 表示所有关卡事件的 JSON 数组
        /// JSON array representing level actions
        /// </summary>
        public JArray actions{ get; }
        
        /// <summary>
        /// 表示所有关卡装饰的 JSON 数组
        /// JSON array representing level decorations
        /// </summary>
        public JArray decorations{ get; }
        
        /// <summary>
        /// 表示关卡砖块角度的只读列表
        /// Read-only list representing the angle of level tiles
        /// </summary>
        public IReadOnlyList<double> angles { get; private set; }
        
        /// <summary>
        /// 表示关卡文件路径的字符串
        /// Path to the level file
        /// </summary>
        public string? pathToLevel;
        
        /// <summary>
        /// 通过关卡信息字典初始化 Level 类的新实例
        /// Initializes a new instance of the Level class from level information dictionary
        /// </summary>
        /// <param name="levelInfo">表示关卡信息的字典 / Dictionary representing level information</param>
        public Level(Dictionary<string, object>? levelInfo)
        {
            root = JObject.Parse(
                JsonConvert.SerializeObject(levelInfo));
            actions = root["actions"].ToObject<JArray>();
            if (root.ContainsKey("angleData"))
            {
                angleData = root["angleData"].ToObject<JArray>();
                angles = angleData.ToObject<List<double>>();
            } 
            else if (root.ContainsKey("pathData"))
            {
                InitAngleData();
            }
            settings = root["settings"].ToObject<JObject>();
            if (settings["version"].Value<int>() > 10)
            {
                decorations = root["decorations"].ToObject<JArray>();
            }
        }

        /// <summary>
        /// 通过文件路径加载并初始化 Level 类的新实例
        /// Initializes a new instance of the Level class by loading from a file path
        /// </summary>
        /// <param name="pathToLevel">关卡文件路径 / Path to the level file</param>
        public Level(string? pathToLevel) : this(SimpleJSON.DeserializeFile(pathToLevel))
        {
            this.pathToLevel = pathToLevel;
        }

        /// <summary>
        /// 使用默认设置创建一个新关卡并可选保存到指定路径
        /// Creates a new level with default settings and optionally saves it to the specified path
        /// </summary>
        /// <param name="savePath">保存新关卡的路径（可选）/ Path to save the new level to (optional)</param>
        /// <returns>新的 Level 实例 / A new Level instance</returns>
        public static Level CreateNewLevel(string? savePath = null)
        {
            JObject root = new();
            root["angleData"] = new JArray([0,0,0,0,0,0,0,0,0,0]);
            root["settings"] = JObject.FromObject(new
            {
                version = 14,
                author = "Created Level by SharpFAI",
                bpm = 100,
                offset = 0,
            });
            root["actions"] = new JArray();
            root["decorations"] = new JArray();
            if (savePath != null)
            {
                File.WriteAllText(savePath, root.ToString());
            }
            return new(JsonConvert.DeserializeObject<Dictionary<string,object>>(root.ToString()));
        }

        /// <summary>
        /// 当 angleData 不存在时从路径数据初始化角度数据
        /// Initializes angle data from path data when angleData is not present
        /// </summary>
        private void InitAngleData()
        {
            if (this.angles.Any()) return;
            List<TileAngle> tileAngles = root["pathData"]
                .ToObject<string>()
                .ToCharArray()
                .Select(c => TileAngle.AngleCharMap[c])
                .ToList() ?? [];
            double staticAngle = 0d;
            List<double> angles = [];

            foreach (TileAngle angle in tileAngles) {
                if (angle == TileAngle.NONE) {
                    angles.Add(angle.Angle);
                    continue;
                }
                staticAngle = angle.Relative ? generalizeAngle(staticAngle + 180 - angle.Angle) : angle.Angle;
                angles.Add(staticAngle);
            }
            this.angles = angles;
            root.Remove("pathData");
            angleData = JArray.FromObject(angles);
            root.Add("angleData", angleData);
            
            double generalizeAngle(double angle) {
                angle -= (int) (angle / 360) * 360;
                return angle < 0 ? angle + 360 : angle;
            }
        }

        /// <summary>
        /// 获取指定类型的设置值
        /// Gets a setting value with the specified type
        /// </summary>
        /// <typeparam name="T">要转换设置的类型 / The type to convert the setting to</typeparam>
        /// <param name="setting">设置名称 / The setting name</param>
        /// <returns>转换为类型 T 的设置值 / The setting value converted to type T</returns>
        public T GetSetting<T>(string setting)
        {
            return settings[setting].ToObject<T>();
        }

        /// <summary>
        /// 设置指定类型的设置值
        /// Sets a setting value with the specified type
        /// </summary>
        /// <typeparam name="T">要设置的值的类型 / The type of the value to set</typeparam>
        /// <param name="setting">设置名称 / The setting name</param>
        /// <param name="value">要设置的值 / The value to set</param>
        public void PutSetting<T>(string setting, T value)
        {
            settings[setting]= JToken.FromObject(value);
        }

        /// <summary>
        /// 检查关卡中是否存在某个设置
        /// Checks if a setting exists in the level
        /// </summary>
        /// <param name="setting">要检查的设置名称 / The setting name to check</param>
        /// <returns>如果设置存在则返回 true，否则返回 false / True if the setting exists, false otherwise</returns>
        public bool HasSetting(string setting)
        {
            return settings.ContainsKey(setting);
        }
        
        /// <summary>
        /// 设置关卡的曲目
        /// Sets the song for the level
        /// </summary>
        /// <param name="songPath">歌曲文件路径 / Path to the song file</param>
        public void SetSong(string songPath)
        {
            PutSetting("songFilename",Path.GetFileName(songPath));
            File.Copy(songPath, Path.Combine(Path.GetDirectoryName(this.pathToLevel), Path.GetFileName(songPath)), true);
        }

        /// <summary>
        /// 向指定地板添加事件
        /// Adds an event to the specified floor
        /// </summary>
        /// <param name="floor">要添加事件的地板编号 / The floor number to add the event to</param>
        /// <param name="type">事件类型 / The event type</param>
        /// <param name="data">事件的可选附加数据 / Optional additional data for the event</param>
        public void AddEvent(int floor, EventType type, JObject data = null)
        {
            JObject newEvent = new JObject();
            newEvent["floor"] = floor;
            newEvent["eventType"] = type.ToString();
            if (data != null)
            {
                foreach (var kvpair in data)
                {
                    newEvent[kvpair.Key] = kvpair.Value;
                }
            }
            actions.Add(newEvent);
        }

        /// <summary>
        /// 添加事件
        /// Adds an event
        /// </summary>
        /// <param name="eventInfo">要添加的事件信息 / Event information to be added</param>
        public void AddEvent(BaseEvent eventInfo)
        {
            JObject newEvent = JObject.Parse(eventInfo.ToString());
            actions.Add(newEvent);
        }
        
        /// <summary>
        /// 从关卡中移除多个设置
        /// Removes multiple settings from the level
        /// </summary>
        /// <param name="settingsToRemove">要移除的设置名称数组 / Array of setting names to remove</param>
        public void RemoveSettings(params string[] settingsToRemove)
        {
            foreach (string setting in settingsToRemove)
            {
                settings.Remove(setting);
            }
        }

        /// <summary>
        /// 获取指定地板上特定类型的所有事件
        /// Gets all events of a specific type on a specific floor
        /// </summary>
        /// <param name="floor">地板编号 / The floor number</param>
        /// <param name="type">事件类型 / The event type</param>
        /// <returns>符合条件的事件列表 / List of events matching the criteria</returns>
        public List<JObject> GetEvents(int floor, EventType type)
        {
            var events = new List<JObject>();
            foreach (JObject action in actions)
            {
                if (action["floor"].Value<int>() == floor && action["eventType"].Value<string>() == type.ToString())
                {
                    events.Add(action);
                }
            }
            return events;
        }

        /// <summary>
        /// 获取指定地板上的所有事件
        /// Gets all events on a specific floor
        /// </summary>
        /// <param name="floor">地板编号 / The floor number</param>
        /// <returns>该地板上所有事件的列表 / List of all events on the floor</returns>
        public List<JObject> GetFloorEvents(int floor)
        {
            var events = new List<JObject>();
            foreach (JObject action in actions)
            {
                if (action["floor"].Value<int>() == floor)
                {
                    events.Add(action);
                }
            }
            return events;
        }

        /// <summary>
        /// 检查地板是否有任何事件
        /// Checks if a floor has any events
        /// </summary>
        /// <param name="floor">要检查的地板编号 / The floor number to check</param>
        /// <returns>如果地板有事件则返回 true，否则返回 false / True if the floor has events, false otherwise</returns>
        public bool HasEvents(int floor)
        {
            return GetFloorEvents(floor).Any();
        }

        /// <summary>
        /// 检查地板是否有特定类型的事件
        /// Checks if a floor has events of a specific type
        /// </summary>
        /// <param name="floor">要检查的地板编号 / The floor number to check</param>
        /// <param name="type">要检查的事件类型 / The event type to check for</param>
        /// <returns>如果地板有指定类型的事件则返回 true，否则返回 false / True if the floor has events of the specified type, false otherwise</returns>
        public bool HasEvents(int floor, EventType type)
        {
            return GetEvents(floor, type).Any();
        }

        /// <summary>
        /// 从地板中移除特定类型的事件
        /// Removes events of a specific type from a floor
        /// </summary>
        /// <param name="floor">地板编号 / The floor number</param>
        /// <param name="type">要移除的事件类型 / The event type to remove</param>
        /// <param name="count">要移除的事件数量（默认：1）/ Number of events to remove (default: 1)</param>
        public void RemoveFloorEvents(int floor, EventType type, int count = 1)
        {
            int eventsRemoved = 0;
            foreach (JObject action in actions)
            {
                if (action["floor"].Value<int>() == floor && action["eventType"].Value<string>() == type.ToString())
                    actions.Remove(action);
                if (++eventsRemoved == count) break;
            }
        }

        /// <summary>
        /// 向关卡添加文本装饰
        /// Adds text decoration to the level
        /// </summary>
        /// <param name="floor">地板编号（默认：0）/ The floor number (default: 0)</param>
        /// <param name="text">文本内容（默认：空）/ The text content (default: empty)</param>
        /// <param name="tag">装饰标签（默认：空）/ The decoration tag (default: empty)</param>
        /// <param name="relativeToScreen">是否相对于屏幕或瓦片（默认：false）/ Whether relative to screen or tile (default: false)</param>
        /// <param name="data">可选的附加数据 / Optional additional data</param>
        public void AddTextToDecorations(int floor = 0, string text = "", string tag = "",bool relativeToScreen = false, JObject data = null)
        {
            //example 
            JObject newText = new JObject();
            newText["decText"] = text;
            newText["font"] = "Default";
            if (data != null)
            {
                foreach (var kvpair in data)
                {
                    newText[kvpair.Key] = kvpair.Value;
                }
            }
            AddDecoration(floor,EventType.AddText, tag, relativeToScreen, newText);
        }

        /// <summary>
        /// 向关卡添加装饰
        /// Adds a decoration to the level
        /// </summary>
        /// <param name="floor">地板编号（默认：0）/ The floor number (default: 0)</param>
        /// <param name="type">装饰类型（默认：AddDecoration）/ The decoration type (default: AddDecoration)</param>
        /// <param name="tag">装饰标签（默认：空）/ The decoration tag (default: empty)</param>
        /// <param name="relativeToScreen">是否相对于屏幕或瓦片（默认：false）/ Whether relative to screen or tile (default: false)</param>
        /// <param name="data">可选的附加数据 / Optional additional data</param>
        public void AddDecoration(int floor = 0, EventType type = EventType.AddDecoration, string tag = "", bool relativeToScreen = false, JObject data = null)
        {
            JObject newDecoration = new JObject();
            if (!relativeToScreen)
            {
                newDecoration["floor"] = floor;
            }
            newDecoration["eventType"] = type.ToString();
            newDecoration["tag"] = tag;
            newDecoration["decorationImage"] = "";
            newDecoration["relativeTo"] = relativeToScreen ? "Camera" : "Tile";
            newDecoration["depth"] = -1;
            if (data != null)
            {
                foreach (var kvpair in data)
                {
                    newDecoration[kvpair.Key] = kvpair.Value;
                }
            }
            decorations.Add(newDecoration);
        }

        /// <summary>
        /// Saves the level to a file
        /// 将关卡保存到文件
        /// </summary>
        /// <param name="newLevelPath">The file path to save as / 保存的文件路径 </param>
        /// <param name="indent">Whether to format with indentation (default: true) / 是否使用缩进格式化（默认：true）</param>
        public void Save(string newLevelPath = null, bool indent = true)
        {
            if (newLevelPath == null && pathToLevel != null)
            {
                newLevelPath = Path.Combine(Path.GetDirectoryName(pathToLevel),"level-modified.adofai");
            } 
            else if (newLevelPath == null && pathToLevel == null)
            {
                throw new ArgumentNullException(nameof(newLevelPath));
            }
            File.WriteAllText(newLevelPath, ToString(indent));
        }

        /// <summary>
        /// Converts the level to JSON string representation
        /// 将关卡转换为JSON字符串表示
        /// </summary>
        /// <param name="indent">Whether to format with indentation (default: true) / 是否使用缩进格式化（默认：true）</param>
        /// <returns>JSON string representation of the level / 关卡的JSON字符串表示</returns>
        public string ToString(bool indent = true)
        {
            root["actions"] = actions;
            root["angleData"] = angleData;
            root["settings"] = settings;
            if (decorations != null)
            {
                root["decorations"] = decorations;
            }
            JsonSerializerSettings serializerSettings = new JsonSerializerSettings();
            serializerSettings.Formatting = Formatting.Indented;
            if (!indent)
            {
                serializerSettings.Formatting = Formatting.None;
            }
            return JsonConvert.SerializeObject(root, serializerSettings);
        }

        /// <summary>
        /// Converts the level to JSON string
        /// 将关卡转换为JSON字符串
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return ToString(false);
        }

        /// <summary>
        /// Gets all events of a specific type from the level
        /// 从关卡中获取特定类型的所有事件
        /// </summary>
        /// <param name="type">The event type to search for / 要搜索的事件类型</param>
        /// <returns>Array of events matching the specified type / 匹配指定类型的事件数组</returns>
        public JArray GetEvents(EventType type)
        {
            JArray events = [];
            foreach (JObject action in actions)
            {
                if (action["eventType"].Value<EventType>() == type)
                {
                    events.Add(action);
                }
            }
            return events;
        }


        /// <summary>
        /// Deserializes level events into strongly-typed objects; optionally includes decorations when requested
        /// 将关卡事件反序列化为强类型对象；可选按需包含装饰
        /// </summary>
        /// <param name="includeDecorations">Whether to also deserialize decorations / 是否同时反序列化装饰</param>
        /// <returns>Read-only list of events in the same order as actions (and decorations if included); returns empty list when none / 只读的事件列表，顺序与 actions（以及包含时的 decorations）一致；若无事件返回空列表而非 null</returns>
        public IReadOnlyList<BaseEvent> DeserializeEvents(bool includeDecorations = false)
        {
            List<BaseEvent> baseEvents = [];
            baseEvents.AddRange(JsonConvert.DeserializeObject<BaseEvent[]>(actions.ToString(), EventJsonConverter.GetJsonSettings()));
            if (includeDecorations && decorations != null)
            {
                baseEvents.AddRange(JsonConvert.DeserializeObject<BaseEvent[]>(decorations.ToString(), EventJsonConverter.GetJsonSettings()));
            }
            return baseEvents; 
        }
    }
 
    internal class TileAngle
    {
        public static readonly TileAngle _0 = new TileAngle('R', 0, false);
        public static readonly TileAngle _15 = new TileAngle('p', 15, false);
        public static readonly TileAngle _30 = new TileAngle('J', 30, false);
        public static readonly TileAngle _45 = new TileAngle('E', 45, false);
        public static readonly TileAngle _60 = new TileAngle('T', 60, false);
        public static readonly TileAngle _75 = new TileAngle('o', 75, false);
        public static readonly TileAngle _90 = new TileAngle('U', 90, false);
        public static readonly TileAngle _105 = new TileAngle('q', 105, false);
        public static readonly TileAngle _120 = new TileAngle('G', 120, false);
        public static readonly TileAngle _135 = new TileAngle('Q', 135, false);
        public static readonly TileAngle _150 = new TileAngle('H', 150, false);
        public static readonly TileAngle _165 = new TileAngle('W', 165, false);
        public static readonly TileAngle _180 = new TileAngle('L', 180, false);
        public static readonly TileAngle _195 = new TileAngle('x', 195, false);
        public static readonly TileAngle _210 = new TileAngle('N', 210, false);
        public static readonly TileAngle _225 = new TileAngle('Z', 225, false);
        public static readonly TileAngle _240 = new TileAngle('F', 240, false);
        public static readonly TileAngle _255 = new TileAngle('V', 255, false);
        public static readonly TileAngle _270 = new TileAngle('D', 270, false);
        public static readonly TileAngle _285 = new TileAngle('Y', 285, false);
        public static readonly TileAngle _300 = new TileAngle('B', 300, false);
        public static readonly TileAngle _315 = new TileAngle('C', 315, false);
        public static readonly TileAngle _330 = new TileAngle('M', 330, false);
        public static readonly TileAngle _345 = new TileAngle('A', 345, false);
        public static readonly TileAngle _5 = new TileAngle('5', 108, true);
        public static readonly TileAngle _6 = new TileAngle('6', 252, true);
        public static readonly TileAngle _7 = new TileAngle('7', 900.0 / 7.0, true);
        public static readonly TileAngle _8 = new TileAngle('8', 360 - 900.0 / 7.0, true);
        public static readonly TileAngle R60 = new TileAngle('t', 60, true);
        public static readonly TileAngle R120 = new TileAngle('h', 120, true);
        public static readonly TileAngle R240 = new TileAngle('j', 240, true);
        public static readonly TileAngle R300 = new TileAngle('y', 300, true);
        public static readonly TileAngle NONE = new TileAngle('!', 999, true);

        public static readonly Dictionary<char, TileAngle> AngleCharMap = new Dictionary<char, TileAngle>();

        static TileAngle()
        {
            foreach (var field in typeof(TileAngle).GetFields(BindingFlags.Public | BindingFlags.Static))
            {
                if (field.FieldType == typeof(TileAngle))
                {
                    var tileAngle = (TileAngle)field.GetValue(null);
                    if (tileAngle != null)
                    {
                        AngleCharMap[tileAngle.CharCode] = tileAngle;
                    }
                }
            }
        }

        private TileAngle(char charCode, double angle, bool relative)
        {
            CharCode = charCode;
            Angle = angle;
            Relative = relative;
        }
        private char CharCode { get; }
        public double Angle { get; }
        public readonly bool Relative;
    }
}