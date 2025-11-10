using Newtonsoft.Json;
using SharpFAI.Serialization;

namespace SharpFAI.Events;

public class Pause : BaseEvent
{
    /// <summary>
    /// 暂停时长
    /// Pause duration in seconds
    /// </summary>
    [JsonProperty("duration")]
    public double Duration;
    
    /// <summary>
    /// 倒计时刻度
    /// Countdown ticks
    /// </summary>
    [JsonProperty("countdownTicks")]
    public int CountdownTicks;

    /// <summary>
    /// 角度修正方向
    /// Angle correction direction
    /// </summary>
    [JsonProperty("angleCorrectionDir")]
    public EventEnums.AngleCorrectionDirection AngleCorrectionDir;
    
    public Pause(double duration = 1,
        int countdownTicks = 0, 
        EventEnums.AngleCorrectionDirection angleCorrectionDir = EventEnums.AngleCorrectionDirection.Backward)
    {
        EventType = EventType.Pause;
        Duration = duration;
        CountdownTicks = countdownTicks;
        AngleCorrectionDir = angleCorrectionDir;
    }
}