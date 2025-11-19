using System;
using SharpFAI.Serialization;

namespace SharpFAI.Test;
#pragma warning disable CS1591
#if DEBUG
public static class Test
{
    public static void Main2()
    {
        Level level2 = new Level(pathToLevel: @"D:\ADOFAILevels\kiss my lips\level.adofai");
        var events = level2.GetEventsIf(a => a.EventType == EventType.Twirl);
        Console.WriteLine(events.Count);
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