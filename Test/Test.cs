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
            Level level2 = new Level(pathToLevel: @"D:\ADOFAILevels\Arche\Arche\level.adofai");

            var events = level2.DeserializeEvents();
            /*foreach (var event1 in events)
            {
                Console.WriteLine(event1);
            }*/
        }
}