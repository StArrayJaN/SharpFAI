namespace SharpFAI.Serialization;

#pragma warning disable CS1591
/// <summary>
/// 事件用到的枚举集合
/// Enum collection used by events
/// </summary>
public static class EventEnums
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
    public enum CamMovementType
    {
        Player,
        Tile,
        Global,
        LastPosition,
        LastPositionNoRotation
    }
    public enum Ease
    {
        Unset,
        Linear,
        InSine,
        OutSine,
        InOutSine,
        InQuad,
        OutQuad,
        InOutQuad,
        InCubic,
        OutCubic,
        InOutCubic,
        InQuart,
        OutQuart,
        InOutQuart,
        InQuint,
        OutQuint,
        InOutQuint,
        InExpo,
        OutExpo,
        InOutExpo,
        InCirc,
        OutCirc,
        InOutCirc,
        InElastic,
        OutElastic,
        InOutElastic,
        InBack,
        OutBack,
        InOutBack,
        InBounce,
        OutBounce,
        InOutBounce,
        Flash,
        InFlash,
        OutFlash,
        InOutFlash,
        INTERNAL_Zero,
        INTERNAL_Custom
    }
    
    public enum HitSound
    {
        Hat,
        Kick,
        Shaker,
        Sizzle,
        Chuck,
        ShakerLoud,
        None,
        Hammer,
        KickChroma,
        SnareAcoustic2,
        Sidestick,
        Stick,
        ReverbClack,
        Squareshot,
        PowerDown,
        PowerUp,
        KickHouse,
        KickRupture,
        HatHouse,
        SnareHouse,
        SnareVapor,
        ClapHit,
        ClapHitEcho,
        ReverbClap,
        FireTile,
        IceTile,
        VehiclePositive,
        VehicleNegative
    }
}