using Newtonsoft.Json;
using SharpFAI.Serialization;
using static SharpFAI.Serialization.EventEnums;

namespace SharpFAI.Events;

/// <summary>
/// 自由漫游事件
/// FreeRoam event
/// </summary>
public class FreeRoam : BaseEvent
{
    /// <summary>
    /// 持续时间
    /// Duration in beats
    /// </summary>
    [JsonProperty("duration")]
    public float Duration;

    /// <summary>
    /// 大小
    /// Size of the free roam area [width, height]
    /// </summary>
    [JsonProperty("size")]
    public int[] Size;

    /// <summary>
    /// 位置偏移
    /// Position offset [x, y]
    /// </summary>
    [JsonProperty("positionOffset")]
    public float[] PositionOffset;

    /// <summary>
    /// 退出时间
    /// Time to exit the free roam mode
    /// </summary>
    [JsonProperty("outTime")]
    public float OutTime;

    /// <summary>
    /// 退出缓动
    /// Easing function for exit animation
    /// </summary>
    [JsonProperty("outEase")]
    public EventEnums.Ease OutEase;

    /// <summary>
    /// 节拍命中音效
    /// Hit sound for on-beat actions
    /// </summary>
    [JsonProperty("hitsoundOnBeats")]
    public EventEnums.HitSound HitsoundOnBeats;

    /// <summary>
    /// 节拍外命中音效
    /// Hit sound for off-beat actions
    /// </summary>
    [JsonProperty("hitsoundOffBeats")]
    public EventEnums.HitSound HitsoundOffBeats;

    /// <summary>
    /// 倒计时刻度
    /// Countdown ticks before entering free roam
    /// </summary>
    [JsonProperty("countdownTicks")]
    public int CountdownTicks;

    /// <summary>
    /// 角度修正方向
    /// Direction for angle correction
    /// </summary>
    [JsonProperty("angleCorrectionDir")]
    public EventEnums.AngleCorrectionDirection AngleCorrectionDir;

    /// <summary>
    /// 初始化一个新的自由漫游事件
    /// Initializes a new instance of the FreeRoam event
    /// </summary>
    /// <param name="duration">持续时间 (Duration in beats)</param>
    /// <param name="size">漫游区域大小 [宽度, 高度] (Size of the free roam area [width, height])</param>
    /// <param name="positionOffset">位置偏移 [x, y] (Position offset)</param>
    /// <param name="outTime">退出时间 (Time to exit)</param>
    /// <param name="outEase">退出缓动函数 (Exit easing function)</param>
    /// <param name="hitsoundOnBeats">节拍命中音效 (On-beat hit sound)</param>
    /// <param name="hitsoundOffBeats">节拍外命中音效 (Off-beat hit sound)</param>
    /// <param name="countdownTicks">倒计时刻度 (Countdown ticks)</param>
    /// <param name="angleCorrectionDir">角度修正方向 (Angle correction direction)</param>
    public FreeRoam(
        float duration = 16,
        int[]? size = default,
        float[]? positionOffset = default,
        float outTime = 4,
        Ease outEase = Ease.InOutSine,
        HitSound hitsoundOnBeats = HitSound.None,
        HitSound hitsoundOffBeats = HitSound.None,
        int countdownTicks = 4,
        AngleCorrectionDirection angleCorrectionDir = AngleCorrectionDirection.Backward)
    {
        EventType = EventType.FreeRoam;
        Duration = duration;
        Size = size ?? [4, 4];
        PositionOffset = positionOffset ?? [0f, 0f];
        OutTime = outTime;
        OutEase = outEase;
        HitsoundOnBeats = hitsoundOnBeats;
        HitsoundOffBeats = hitsoundOffBeats;
        CountdownTicks = countdownTicks;
        AngleCorrectionDir = angleCorrectionDir;
    }
}