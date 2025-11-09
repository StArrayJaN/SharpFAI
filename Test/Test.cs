using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Numerics;
using SharpFAI.Framework;
using SharpFAI.Serialization;
using SharpFAI.Util;

namespace SharpFAI.Test;
#if DEBUG
public static class Test
{
    private static readonly double tickToNanoseconds = 1_000_000_000.0 / Stopwatch.Frequency;
    public static void Main2()
    {
        //捅死Yqloss喵
        Level level2 = new Level(pathToLevel: @"D:\ADOFAILevels\level.adofai");
        var _floors = level2.CreateFloors();
        var _noteTimes = level2.GetNoteTimes();
        _noteTimes.ForEach(Console.WriteLine);
        int _currentIndex = 0;
        int events = _currentIndex + 1;
        double startTime = CurrentTimeMilliseconds() - _noteTimes[_currentIndex];
        while (events < _floors.Count)
        {
            double currentTime = CurrentTimeMilliseconds();
            double timeMilliseconds = currentTime - startTime + _noteTimes[_currentIndex];
            while (events < _noteTimes.Count &&
                   _noteTimes[events] <= timeMilliseconds)
            {
                Console.WriteLine($"{_noteTimes[events]} <= {timeMilliseconds}");
                events++;
            }
        }
    }
    // 获取高精度毫秒数（double）
    public static double CurrentTimeMilliseconds()
    {
        long ticks = Stopwatch.GetTimestamp();
        return (double)ticks / Stopwatch.Frequency * 1000.0;
    }
    public static void AddSegmentsAsFloor(this Level level, double multiplier = 2, int count = 32)
    {
        double angle = 180 - 180 / multiplier;
        for (int i = 0; i < count; i++)
        {
            level.angleData.Add(angle * i) ;
            /*if (i % multiplier == 0)
            {
                level.actions.Add(new PositionTrack()
                {
                    Floor = lastFloor + i,
                    PositionOffset = new float[] { i, 0 },
                    EditorOnly = false
                }.ToString());
            }*/
        }
    }
}
#endif