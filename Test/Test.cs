using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;
using SharpFAI.Events;
using SharpFAI.Serialization;
using SharpFAI.Util;

namespace SharpFAI.Test;
#if DEBUG
public class Test
{
    public static void Main2()
    {
        Level level2 = new Level(pathToLevel: @"D:\ADOFAILevels\Difficulty High\Ignis Glacies - aequus libra (High).adofai");
        level2.AddEvent(BaseEvent.Empty);
        Process.Start("D:\\Softwares\\Steam\\steamapps\\common\\A Dance of Fire and Ice\\A Dance of Fire and Ice.exe");
    }
}
#endif