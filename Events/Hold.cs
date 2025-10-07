using Newtonsoft.Json;
using SharpFAI.Serialization;

namespace SharpFAI.Events;

public class Hold : BaseEvent
{
    /// <summary>
    /// 持续时间（秒）
    /// Duration in seconds
    /// </summary>
    [JsonProperty("duration")]
    public float Duration { get; set; }
    
    /// <summary>
    /// 距离倍数（百分比）
    /// Distance multiplier (percentage)
    /// </summary>
    [JsonProperty("distanceMultiplier")]
    public int DistanceMultiplier { get; set; }
    
    /// <summary>
    /// 是否播放落地动画
    /// Whether to play landing animation
    /// </summary>
    [JsonProperty("landingAnimation")]
    public bool LandingAnimation { get; set; }
    
    public Hold(float duration = 0,
        int distanceMultiplier = 100,
        bool landingAnimation = false)
    {
        EventType = EventType.Hold;
        Duration = duration;
        DistanceMultiplier = distanceMultiplier;
        LandingAnimation = landingAnimation;
    }
}