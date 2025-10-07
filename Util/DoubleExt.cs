namespace SharpFAI.Util;

internal static class DoubleExt
{
    public static double AngleToTime(this double angle, double bpm)
    {
        return (angle / 180) * (60 / bpm) * 1000;
    }
}