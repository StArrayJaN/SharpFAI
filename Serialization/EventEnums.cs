namespace SharpFAI.Serialization;

/// <summary>
/// 事件用到的枚举集合
/// Enum collection used by events
/// </summary>
public class EventEnums
{
    public enum PlanetCount
    {
        TwoPlanets = 2,
        ThreePlanets = 3
    }
    public enum SpeedType
    {
        Bpm,
        Multiplier
    }
    public enum AngleCorrectionDirection
    {
        Backward = -1,
        None,
        Forward
    }
    public enum TileRelativeTo
    {
        ThisTile,
        Start,
        End
    }
}