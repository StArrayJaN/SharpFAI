using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;
using SharpFAI.Serialization;
using SharpFAI.Util;

namespace SharpFAI.Test;

public class Test
{
    
    public static void Main2()
    {
        /*Level level2 = new Level(pathToLevel: @"D:\ADOFAILevels\Arche\Arche\level.adofai");

        var events = level2.DeserializeEvents();
        /*foreach (var event1 in events)
        {
            Console.WriteLine(event1);
        }#1#*/
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

        var level2 = Level.CreateNewLevel();
        level2.Save("new-level.adofai");
    }
}