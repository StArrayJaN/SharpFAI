using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using SharpFAI.Util;

namespace SharpFAI.Test;

public class Test
{
        public static void Main2()
        {
            Level level = Level.CreateNewLevel("D:\\test.adofai");
            level.GenerateGlide(1,Pitch.C2,Pitch.A4,10);
            level.Save();
        }
}