# SharpFAI

ADOFAI Level Parse Library - A C# library for parsing and manipulating ADOFAI (A Dance of Fire and Ice) level files.

ADOFAI关卡解析库 - 用于解析和操作ADOFAI（冰与火之舞）关卡文件的C#库。

## Features / 功能特性

- Parse ADOFAI level files (.adofai) / 解析ADOFAI关卡文件(.adofai)
- Manipulate level settings and events / 操作关卡设置和事件
- Calculate note timings / 计算音符时间
- Add decorations and text / 添加装饰和文本
- Export modified levels / 导出修改后的关卡

## Installation / 安装

```bash
dotnet add package SharpFAI
```

## Usage / 使用方法

```csharp
using SharpFAI;

// Load a level / 加载关卡
var level = new Level("path/to/level.adofai");

// Get level settings / 获取关卡设置
var bpm = level.GetSetting<double>("bpm");
var artist = level.GetSetting<string>("arist");

// Add events / 添加事件
level.AddEvent(10, "Twirl");

// Calculate note times / 计算音符时间
var noteTimes = level.GetNoteTimes();

// Save modified level / 保存修改后的关卡
level.Save("modified-level.adofai");
```

## API Documentation / API文档

Some public methods include bilingual XML documents (English/Chinese) to support Intellisense.

部分公共方法都包含双语XML文档（英文/中文）以支持IntelliSense。

### Level Class / Level类

- `Level(string pathToLevel)` - Initialize level from file path / 从文件路径初始化关卡
- `GetSetting<T>(string setting)` - Get setting value / 获取设置值
- `PutSetting<T>(string setting, T value)` - Set setting value / 设置设置值
- `AddEvent(int floor, string type, JObject data)` - Add event to floor / 向砖块添加事件
- `Save(string filename, bool indent)` - Save level to file / 保存关卡到文件

### LevelUtils Class / LevelUtils类

- `GetNoteTimes(Level level, bool addOffset)` - Calculate note timings / 计算音符时间
- `GetAllSpeedChange(Level level)` - Get speed changes / 获取速度变化
- `GenerateGlidet(int startFloor,Pitch startNote, Pitch endNote, double duration)` - Generate glides / 生成滑音

## License / 许可证

GPL-v3 License

## Author / 作者

StArray

## Contributing / 贡献

Contributions are welcome! Please feel free to submit a Pull Request.

欢迎贡献！请随时提交Pull Request。