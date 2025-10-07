using Newtonsoft.Json;
using SharpFAI.Serialization;

namespace SharpFAI.Events;

public class Pause : BaseEvent
{
    /// <summary>
    /// 暂停时长（秒）
    /// Pause duration in seconds
    /// </summary>
    [JsonProperty("duration")]
    public float Duration { get; set; }
    
    /// <summary>
    /// 倒计时刻度
    /// Countdown ticks
    /// </summary>
    [JsonProperty("countdownTicks")]
    public int CountdownTicks { get; set; }

    /// <summary>
    /// 角度修正方向
    /// Angle correction direction
    /// </summary>
    [JsonProperty("angleCorrectionDir")]
    public EventEnums.AngleCorrectionDirection AngleCorrectionDir { get; set; } =
        EventEnums.AngleCorrectionDirection.Backward;
    
    public Pause(float duration = 1,
        int countdownTicks = 0, 
        EventEnums.AngleCorrectionDirection angleCorrectionDir = EventEnums.AngleCorrectionDirection.Backward)
    {
        EventType = EventType.Pause;
        Duration = duration;
        CountdownTicks = countdownTicks;
        AngleCorrectionDir = angleCorrectionDir;
    }
}