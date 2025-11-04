using System;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace SharpFAI.Util;

internal static class MoreMethods
{
    public static double AngleToTime(this double angle, double bpm)
    {
        return angle / 180 * (60 / bpm) * 1000;
    }

    public static double BPMToSecond(this double bpm)
    {
        return 60 / bpm;
    }

    public static float Fmod(this float x, float y)
    {
        return x >= 0 ? x % y : x % y + y;
    }
    
    public static double Fmod(this double x, double y)
    {
        return x >= 0 ? x % y : x % y + y;
    }

    public static float ToFloat(this double x)
    {
        return (float)x;
    }

    public static string[] FindLevelFiles(this string dir)
    {
        return Directory.GetFiles(dir, "*.adofai");
    }

    public static T[] ToArray<T>(this Tuple<T, T> tuple)
    {
        return [tuple.Item1, tuple.Item2];
    }
    
    public static T[] ToArray<T>(this ValueTuple<T, T> tuple)
    {
        return [tuple.Item1, tuple.Item2];
    }

    public static string AsString(this object obj)
    {
        FieldInfo[] fields = obj.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance| BindingFlags.NonPublic | BindingFlags.Static);
        PropertyInfo[] properties = obj.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Static);
        MethodInfo[] methods = obj.GetType().GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic| BindingFlags.Static);
        StringBuilder sb = new StringBuilder();
        sb.AppendLine("Type: " + obj.GetType().Name);
        sb.AppendLine("[");
        sb.AppendLine("Fields:");
        foreach (FieldInfo field in fields)
        {
            try
            {
                var value = field.GetValue(obj);
                if (value is Array)
                {
                    sb.AppendLine("\t" + field.Name + ": " + value + " Count: " + ((Array)value).Length);
                }
                else
                {
                    sb.AppendLine("\t" + field.Name + ": " + value);
                }
            }
            catch (Exception e)
            {
                sb.AppendLine("\t" + field.Name + ": Exception:" + e.Message);
            }
        }
        sb.AppendLine("Properties:");
        foreach (PropertyInfo property in properties)
        {
            try
            {
                var value = property.GetValue(obj);
                if (value is Array)
                {
                    sb.AppendLine("\t" + property.Name + ": " + value + " Count: " + ((Array)value).Length);
                }
                else
                {
                    sb.AppendLine("\t" + property.Name + ": " + value);
                }
            }
            catch (Exception e)
            {
                sb.AppendLine("\t" + property.Name + ": Exception:" + e.Message);
            }
        }
        sb.AppendLine("Methods:");
        foreach (MethodInfo method in methods)
        {
            sb.AppendLine("\t" + method.Name);
        }
        sb.AppendLine("]");
        return sb.ToString();
    }

    public static string AsString(this Array objects)
    {
        StringBuilder sb = new StringBuilder();
        foreach (object obj in objects)
        {
            sb.AppendLine(obj.AsString());
        }
        return sb.ToString();
    }

    public static float ToRGBA(this Color color)
    {
        return (color.A << 24) | (color.R << 16) | (color.G << 8) | color.B;
    }

    public static string FormatJSON(this string s)
    {
        JObject obj = JObject.Parse(s);
        return JsonConvert.SerializeObject(obj, Formatting.Indented);
    }
}