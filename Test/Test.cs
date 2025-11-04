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
public class Test
{
    public static void Main2()
    {
        Level level2 = new Level(pathToLevel: @"D:\ADOFAILevels\Scattered Faith\level.adofai");
        Floor.length = 5;
        Floor.width = 5 / 2f;
        var floors = level2.CreateFloors();
        var polygons = new List<Floor.Polygon>();
        var positions = new List<Vector2>();
        
        foreach (var floor in floors.Take(2))
        {
            Console.WriteLine(floor.GeneratePolygons());
        }
        //RenderAndSave(polygons, positions.ToArray(),"D:\\test.png");
        //Process.Start("D:\\test.png");

    }
}
#endif