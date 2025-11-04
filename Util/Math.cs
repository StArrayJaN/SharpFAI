using System;

namespace SharpFAI.Util;

public class MathF
{
    public const double PI = 3.1415926535897932384626433832795028841971693993751058209749445923078164062862089986280348253421170679;
    
    public static float Lerp(float a, float b, float t)
    {
        return a + (b - a) * t;
    }

    public static float Cos(float a,bool degrees = false)
    {
        if (degrees)
            a = a * PI.ToFloat() / 180;
        return Math.Cos(a).ToFloat();
    }
    
    public static float Cos(double a,bool degrees = false)
    {
        if (degrees)
            a = a * PI / 180;
        return Math.Cos(a).ToFloat();
    }

    public static float Sin(float a,bool degrees = false)
    {
        if (degrees)
            a = a * PI.ToFloat() / 180;
        return Math.Sin(a).ToFloat();
    }
    
    public static float Sin(double a, bool degrees = false)
    {
        if (degrees)
            a = a * PI / 180;
        return Math.Sin(a).ToFloat();
    }

    public static float Pow(float a, float b)
    {
        return Math.Pow(a, b).ToFloat();
    }

    public static float Pow(double a, double b)
    {
        return Math.Pow(a, b).ToFloat();
    }
    
}