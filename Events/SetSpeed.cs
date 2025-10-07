using Newtonsoft.Json;
using SharpFAI.Serialization;

namespace SharpFAI.Events;

public class SetSpeed : BaseEvent
{
    [JsonProperty("speedType")]
    public EventEnums.SpeedType SpeedType { get; set; }
    
    [JsonProperty("beatsPerMinute")]
    public float BeatsPerMinute { get; set; }
    
    [JsonProperty("bpmMultiplier")]
    public float BpmMultiplier { get; set; }
    
    [JsonProperty("angleOffset")]
    public float AngleOffset { get; set; }
    
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