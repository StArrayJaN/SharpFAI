using Newtonsoft.Json;
using SharpFAI.Serialization;

namespace SharpFAI.Events;

public class Hold : BaseEvent
{
    /// <summary>
    /// 持续时间
    /// Duration in seconds
    /// </summary>
    [JsonProperty("duration")] 
    public double Duration;

    /// <summary>
    /// 距离
    /// Distance multiplier (percentage)
    /// </summary>
    [JsonProperty("distanceMultiplier")] 
    public int DistanceMultiplier;
    
    /// <summary>
    /// 是否播放落地动画
    /// Whether to play landing animation
    /// </summary>
    [JsonProperty("landingAnimation")]
    public bool LandingAnimation;
    
    public Hold(double duration = 0,
        int distanceMultiplier = 100,
        bool landingAnimation = false)
    {
        EventType = EventType.Hold;
        Duration = duration;
        DistanceMultiplier = distanceMultiplier;
        LandingAnimation = landingAnimation;
    }
}