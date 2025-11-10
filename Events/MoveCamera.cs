using Newtonsoft.Json;
using SharpFAI.Serialization;

namespace SharpFAI.Events;

public class MoveCamera : BaseEvent
{
    /// <summary>
    /// 持续时间
    /// Duration in seconds
    /// </summary>
    [JsonProperty("duration")]
    public double Duration { get; set; }
    
    /// <summary>
    /// 相对于
    /// Relative to
    /// </summary>
    [JsonProperty("relativeTo")]
    public EventEnums.CamMovementType RelativeTo { get; set; }
    
    /// <summary>
    /// 位置 [x, y]
    /// Position [x, y]
    /// </summary>
    [JsonProperty("position")]
    public object[] Position { get; set; }
    
    /// <summary>
    /// 旋转角度
    /// Rotation angle
    /// </summary>
    [JsonProperty("rotation")]
    public double Rotation { get; set; }
    
    /// <summary>
    /// 缩放百分比
    /// Zoom percentage
    /// </summary>
    [JsonProperty("zoom")]
    public double Zoom { get; set; }
    
    /// <summary>
    /// 角度偏移
    /// Angle offset
    /// </summary>
    [JsonProperty("angleOffset")]
    public double AngleOffset { get; set; }
    
    /// <summary>
    /// 缓动函数
    /// Easing function
    /// </summary>
    [JsonProperty("ease")]
    public EventEnums.Ease Ease { get; set; }
    
    /// <summary>
    /// 事件标签
    /// Event tag
    /// </summary>
    [JsonProperty("eventTag")]
    public string EventTag { get; set; }

    public MoveCamera(int floor = 0,
        double duration = 0,
        EventEnums.CamMovementType relativeTo = EventEnums.CamMovementType.Player,
        object[] position = null, 
        double rotation = 0,
        double zoom = 100,
        double angleOffset = 0,
        EventEnums.Ease ease = EventEnums.Ease.Linear, 
        string eventTag = "")
    {
        Floor = floor;
        EventType = EventType.MoveCamera;
        Duration = duration;
        RelativeTo = relativeTo;
        if (position != null)
        {
            Position = [position[0], position[1]];
        }
        else
        {
            Position = [0,0];
        }
        Rotation = rotation;
        Zoom = zoom;
        AngleOffset = angleOffset;
        Ease = ease;
        EventTag = eventTag;
    }
}