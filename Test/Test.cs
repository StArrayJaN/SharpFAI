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
    public static void Main2()
    {
        //捅死Yqloss喵
        Level level2 = new Level(pathToLevel: @"D:\ADOFAILevels\level.adofai");
        level2.AddSegmentsAsFloor(2.747474);
        level2.Save();
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