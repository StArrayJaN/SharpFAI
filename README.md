# SharpFAI

ADOFAI Level Parse Library - A C# library for parsing and manipulating ADOFAI (A Dance of Fire and Ice) level files.

ADOFAI关卡解析库 - 用于解析和操作ADOFAI（冰与火之舞）关卡文件的C#库。

## Features / 功能特性

- Parse ADOFAI level files (.adofai) / 解析ADOFAI关卡文件(.adofai)
- Manipulate level settings and events / 操作关卡设置和事件
- Calculate note timings / 计算音符时间
- Add decorations and text / 添加装饰和文本
- Export modified levels / 导出修改后的关卡
- Create and manipulate floor objects / 创建和操作地板对象

## Installation / 安装

```bash
dotnet add package SharpFAI
```

## Usage / 使用方法

```csharp
using SharpFAI.Serialization;
using SharpFAI.Util;

var level = new Level(pathToLevel:"path/to/level.adofai");

// Get level settings / 获取关卡设置
var bpm = level.GetSetting<double>("bpm");
var artist = level.GetSetting<string>("artist");

// Add events / 添加事件
level.AddEvent(10, EventType.Twirl);

// Calculate note times / 计算音符时间
var noteTimes = level.GetNoteTimes();

// Save modified level / 保存修改后的关卡
level.Save("modified-level.adofai");

// Create a new level / 创建新关卡
var level2 = Level.CreateNewLevel();
level2.Save("new-level.adofai");

// Remove visual effects / 移除视觉效果
level.RemoveVFXs(includeDecorations: true);

// Get floor index by time / 通过时间获取地板索引
int floorIndex = level.GetFloorIndexByNoteTime(5.0); // 5 seconds
```

## API Documentation / API文档

Some public methods include bilingual XML documents (English/Chinese) to support Intellisense.

部分公共方法都包含双语XML文档（英文/中文）以支持IntelliSense。

### Level Class / Level类

- `Level(string pathToLevel)` - Initialize level from file path / 从文件路径初始化关卡
- `Level(Dictionary<string, object> levelInfo)` - Initialize from level info dictionary / 从关卡信息字典初始化
- `CreateNewLevel(string savePath)` - Create a new level with default settings / 创建带有默认设置的新关卡
- `GetSetting<T>(string setting)` - Get setting value / 获取设置值
- `PutSetting<T>(string setting, T value)` - Set setting value / 设置设置值
- `HasSetting(string setting)` - Check if setting exists / 检查设置是否存在
- `SetSong(string songPath)` - Set level song / 设置关卡歌曲
- `AddEvent(int floor, EventType type, JObject data)` - Add event to floor / 向砖块添加事件
- `AddEvent(BaseEvent eventInfo)` - Add event from event object / 从事件对象添加事件
- `RemoveSettings(params string[] settingsToRemove)` - Remove multiple settings / 移除多个设置
- `GetEvents(int floor, EventType type)` - Get events of specific type on floor / 获取特定地板上特定类型的事件
- `GetFloorEvents(int floor)` - Get all events on a floor / 获取特定地板上的所有事件
- `HasEvents(int floor)` - Check if floor has events / 检查地板是否有事件
- `HasEvents(int floor, EventType type)` - Check if floor has specific event type / 检查地板是否有特定类型的事件
- `RemoveEventsIf(Func<BaseEvent, bool> condition)` - Remove events matching condition / 移除满足条件的事件
- `RemoveFloorEvents(int floor, EventType type, int count)` - Remove events from floor / 从地板移除事件
- `AddTextToDecorations(int floor, string text, string tag, bool relativeToScreen, JObject data)` - Add text decoration / 添加文本装饰
- `AddDecoration(int floor, EventType type, string tag, bool relativeToScreen, JObject data)` - Add decoration / 添加装饰
- `Save(string newLevelPath, bool indent)` - Save level to file / 保存关卡到文件
- `ToString(bool indent)` - Convert level to JSON string / 将关卡转换为JSON字符串
- `GetEvents(EventType type)` - Get all events of specific type / 获取特定类型的所有事件
- `GetEventsIf(Func<BaseEvent, bool> condition)` - Get events matching condition / 获取满足条件的事件
- `DeserializeEvents(bool includeDecorations)` - Deserialize events to objects / 将事件反序列化为对象
- `GetAudioPath()` - Get absolute path to audio file / 获取音频文件的绝对路径

### LevelUtils Class / LevelUtils类

- `GetNoteTimes(this Level level, bool addOffset)` - Calculate note timings / 计算音符时间
- `GetAllSpeedChange(this Level level)` - Get speed changes / 获取速度变化
- `GenerateGlide(this Level level, int startFloor, Pitch startNote, Pitch endNote, double duration)` - Generate glides / 生成滑音
- `RemoveVFXs(this Level level, bool includeDecorations, bool includeTracks, Action<string> onDelete)` - Remove visual effects / 移除视觉效果
- `AddCube(this Level level, string cubeImage, Tuple<float, float> position, Tuple<float, float> size, int floorCount, int floor, string tag, bool relativeToScreen)` - Add cube decoration with depth effect / 添加具有深度效果的立方体装饰
- `CreateFloors(this Level level, Vector2 startPosition, bool usePositionTrack)` - Create Floor objects from level data / 从关卡数据创建地板对象
- `GetFloorIndexByNoteTime(this Level level, double noteTimeSecond)` - Get floor index by note time / 通过音符时间获取地板索引

## License / 许可证

GPL-v3 License

## Author / 作者

StArray

## Contributing / 贡献

Contributions are welcome! Please feel free to submit a Pull Request.

欢迎贡献！请随时提交Pull Request。