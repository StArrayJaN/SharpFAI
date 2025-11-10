using System;
using System.Diagnostics;
using System.Linq;
using SharpFAI.Serialization;
using SharpFAI.Util;

namespace SharpFAI.Test;
#if DEBUG
public static class Test
{
    public static void Main2()
    {
        //捅死Yqloss喵
        Level level2 = new Level(pathToLevel: @"D:\ADOFAILevels\kiss my lips\level.adofai");
        level2.DeserializeEvents().ToList().ForEach(Console.WriteLine);
    }
    
    public static void AddSegmentsAsFloor(this Level level, double multiplier = 2, int count = 32)
    {
        double angle = 180 - 180 / multiplier;
        for (int i = 0; i < count; i++)
        {
            level.angleData.Add(angle * i) ;
        }
    }
}
#endif