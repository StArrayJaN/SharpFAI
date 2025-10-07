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
    /// Represents an ADOFAI level with parsing and manipulation capabilities
    /// 表示一个ADOFAI关卡，具有解析和操作功能
    /// </summary>
    public class Level
    {
        /// <summary>
        /// JSON object representing level data
        /// 表示关卡数据的JSON对象
        /// </summary>
        public JObject root{ get; }
        
        /// <summary>
        /// JSON object representing level settings
        /// 表示关卡设置的JSON对象
        /// </summary>
        private JObject settings{ get; }
        
        /// <summary>
        /// JSON object representing the angle of level bricks
        /// 表示关卡砖块角度的JSON对象
        /// </summary>
        public JArray angleData{ get; private set; }
        
        /// <summary>
        /// JSON array representing level actions
        /// 表示所有关卡事件的JSON数组
        /// </summary>
        public JArray actions{ get; }
        
        /// <summary>
        /// JSON array representing level decorations
        /// 表示所有关卡装饰的JSON数组
        /// </summary>
        public JArray decorations{ get; }
        
        /// <summary>
        /// Read only list representing the angle of level bricks
        /// 表示关卡砖块角度的只读列表
        /// </summary>
        public IReadOnlyList<double> angles { get; private set; }
        
        /// <summary>
        /// A string representing the path of the level file
        /// 表示关卡文件路径的字符串
        /// </summary>
        public string? pathToLevel;
        
        /// <summary>
        /// Initializes a new instance of the Level class by loading from a file path
        /// 通过从文件路径加载来初始化Level类的新实例
        /// </summary>
        /// <param name="levelInfo">Dictionary representing level information / 表示关卡信息的字典</param>
        /// <param name="pathToLevel">Path to the level file / 关卡文件路径</param>
        /// <exception cref="ArgumentNullException">Thrown when pathToLevel is null / 当levelInfo为null时抛出</exception>
        public Level(Dictionary<string, object>? levelInfo = null, string? pathToLevel = null)
        {
            if (levelInfo == null && pathToLevel != null)
            {
                levelInfo = SimpleJSON.DeserializeFile(pathToLevel);
                this.pathToLevel = pathToLevel;
            }
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
        /// Creates a new level with default settings and saves it to the specified path
        /// 使用默认设置创建一个新关卡并将其保存到指定路径
        /// </summary>
        /// <param name="savePath">Path to save the new level to / 保存新关卡的路径</param>
        /// <returns>A new Level instance / 一个新的Level实例</returns>
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
        /// Initializes angle data from path data when angleData is not present
        /// 当angleData不存在时从路径数据初始化角度数据
        /// </summary>
        private void InitAngleData()
        {
            if (this.angles.Any()) return;
            List<TileAngle> tileAngles = root["pathData"]
                .ToObject<string>()
                .ToCharArray()
                .Select(c => TileAngle.AngleCharMap[c])
                .ToList() ?? new();
            double staticAngle = 0d;
            List<double> angles = new();

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
        /// Gets a setting value with the specified type
        /// 获取指定类型的设置值
        /// </summary>
        /// <typeparam name="T">The type to convert the setting to / 要转换设置的类型</typeparam>
        /// <param name="setting">The setting name / 设置名称</param>
        /// <returns>The setting value converted to type T / 转换为类型T的设置值</returns>
        public T GetSetting<T>(string setting)
        {
            return settings[setting].ToObject<T>();
        }

        /// <summary>
        /// Sets a setting value with the specified type
        /// 设置指定类型的设置值
        /// </summary>
        /// <typeparam name="T">The type of the value to set / 要设置的值的类型</typeparam>
        /// <param name="setting">The setting name / 设置名称</param>
        /// <param name="value">The value to set / 要设置的值</param>
        public void PutSetting<T>(string setting, T value)
        {
            settings[setting]= JToken.FromObject(value);
        }

        /// <summary>
        /// Checks if a setting exists in the level
        /// 检查关卡中是否存在某个设置
        /// </summary>
        /// <param name="setting">The setting name to check / 要检查的设置名称</param>
        /// <returns>True if the setting exists, false otherwise / 如果设置存在则返回true，否则返回false</returns>
        public bool HasSetting(string setting)
        {
            return settings.ContainsKey(setting);
        }
        
        /// <summary>
        /// Sets the song for the level
        /// 设置关卡的曲目
        /// </summary>
        /// <param name="songPath">Path to the song file / 歌曲文件路径</param>
        public void SetSong(string songPath)
        {
            PutSetting("songFilename",Path.GetFileName(songPath));
            File.Copy(songPath, Path.Combine(Path.GetDirectoryName(this.pathToLevel), Path.GetFileName(songPath)), true);
        }

        /// <summary>
        /// Adds an event to the specified floor
        /// 向指定楼层添加事件
        /// </summary>
        /// <param name="floor">The floor number to add the event to / 要添加事件的砖块</param>
        /// <param name="type">The event type / 事件类型</param>
        /// <param name="data">Optional additional data for the event / 事件的可选附加数据</param>
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
        /// Removes multiple settings from the level
        /// 从关卡中移除多个设置
        /// </summary>
        /// <param name="settingsToRemove">Array of setting names to remove / 要移除的设置名称数组</param>
        public void RemoveSettings(params string[] settingsToRemove)
        {
            foreach (string setting in settingsToRemove)
            {
                settings.Remove(setting);
            }
        }

        /// <summary>
        /// Gets all events of a specific type on a specific floor
        /// 获取指定楼层上特定类型的所有事件
        /// </summary>
        /// <param name="floor">The floor number / 砖块</param>
        /// <param name="type">The event type / 事件类型</param>
        /// <returns>List of events matching the criteria / 符合条件的事件列表</returns>
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
        /// Gets all events on a specific floor
        /// 获取指定楼层上的所有事件
        /// </summary>
        /// <param name="floor">The floor number / 砖块</param>
        /// <returns>List of all events on the floor / 该楼层上所有事件的列表</returns>
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
        /// Checks if a floor has any events
        /// 检查楼层是否有任何事件
        /// </summary>
        /// <param name="floor">The floor number to check / 要检查的砖块</param>
        /// <returns>True if the floor has events, false otherwise / 如果楼层有事件则返回true，否则返回false</returns>
        public bool HasEvents(int floor)
        {
            return GetFloorEvents(floor).Any();
        }

        /// <summary>
        /// Checks if a floor has events of a specific type
        /// 检查楼层是否有特定类型的事件
        /// </summary>
        /// <param name="floor">The floor number to check / 要检查的砖块</param>
        /// <param name="type">The event type to check for / 要检查的事件类型</param>
        /// <returns>True if the floor has events of the specified type, false otherwise / 如果楼层有指定类型的事件则返回true，否则返回false</returns>
        public bool HasEvents(int floor, EventType type)
        {
            return GetEvents(floor, type).Any();
        }

        /// <summary>
        /// Removes events of a specific type from a floor
        /// 从楼层中移除特定类型的事件
        /// </summary>
        /// <param name="floor">The floor number / 砖块</param>
        /// <param name="type">The event type to remove / 要移除的事件类型</param>
        /// <param name="count">Number of events to remove (default: 1) / 要移除的事件数量（默认：1）</param>
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
        /// Adds text decoration to the level
        /// 向关卡添加文本装饰
        /// </summary>
        /// <param name="floor">The floor number (default: 0) / 砖块（默认：0）</param>
        /// <param name="text">The text content (default: empty) / 文本内容（默认：空）</param>
        /// <param name="tag">The decoration tag (default: empty) / 装饰标签（默认：空）</param>
        /// <param name="relativeToScreen">Whether relative to screen or tile (default: false) / 是否相对于屏幕或瓦片（默认：false）</param>
        /// <param name="data">Optional additional data / 可选的附加数据</param>
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
        /// Adds an image decoration to the level
        /// 向关卡添加图像装饰
        /// </summary>
        /// <param name="floor">The floor number (default: 0) / 砖块（默认：0）</param>
        /// <param name="type">The decoration type (default: "AddDecoration") / 装饰类型（默认："AddDecoration"）</param>
        /// <param name="tag">The decoration tag (default: empty) / 装饰标签（默认：空）</param>
        /// <param name="relativeToScreen">Whether relative to screen or tile (default: false) / 是否相对于屏幕或瓦片（默认：false）</param>
        /// <param name="data">Optional additional data / 可选的附加数据</param>
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
            if (data != null)
            {
                foreach (var kvpair in data)
                {
                    newDecoration[kvpair.Key] = kvpair.Value;
                }
            }
            newDecoration["relativeTo"] = relativeToScreen ? "Camera" : "Tile";
            newDecoration["depth"] = -1;
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
        private string ToString(bool indent = true)
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
        public JArray GetEvents(string type)
        {
            JArray events = new JArray();
            foreach (JObject action in actions)
            {
                if (action["eventType"].Value<string>() == type)
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
            List<BaseEvent> baseEvents = new();
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