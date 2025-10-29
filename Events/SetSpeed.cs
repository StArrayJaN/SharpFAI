using Newtonsoft.Json;
using SharpFAI.Serialization;

namespace SharpFAI.Events;

public class SetSpeed : BaseEvent
{
    /// <summary>
    /// 速度类型
    /// Speed type
    /// </summary>
    [JsonProperty("speedType")]
    public EventEnums.SpeedType SpeedType;
    
    /// <summary>
    /// 每分钟节拍数
    /// Beats per minute
    /// </summary>
    [JsonProperty("beatsPerMinute")]
    public float BeatsPerMinute;
    
    /// <summary>
    /// BPM 倍率
    /// BPM multiplier
    /// </summary>
    [JsonProperty("bpmMultiplier")]
    public float BpmMultiplier;
    
    /// <summary>
    /// 角度偏移
    /// Angle offset
    /// </summary>
    [JsonProperty("angleOffset")]
    public float AngleOffset;
    
    public SetSpeed(EventEnums.SpeedType speedType = EventEnums.SpeedType.Bpm,
        float beatsPerMinute = 100,
        float bpmMultiplier = 1,
        float angleOffset = 0)
    {
        EventType = EventType.SetSpeed;
        SpeedType = speedType;
        BeatsPerMinute = beatsPerMinute;
        BpmMultiplier = bpmMultiplier;
        AngleOffset = angleOffset;
    }
}