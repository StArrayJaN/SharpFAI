using System;
using Newtonsoft.Json;
using SharpFAI.Serialization;

namespace SharpFAI.Events;

public class PositionTrack : BaseEvent
{
    /// <summary>
    /// 位置偏移 [x, y]
    /// Position offset [x, y]
    /// </summary>
    [JsonProperty("positionOffset")]
    public double[] PositionOffset;
    
    /// <summary>
    /// 相对于 [轨道索引, 瓦片参考类型]
    /// Relative to [track index, tile reference type]
    /// </summary>
    [JsonProperty("relativeTo")]
    public object[] RelativeTo;
    
    /// <summary>
    /// 旋转角度
    /// Rotation angle
    /// </summary>
    [JsonProperty("rotation")]
    public double Rotation;
    
    /// <summary>
    /// 缩放百分比
    /// Scale percentage
    /// </summary>
    [JsonProperty("scale")]
    public double Scale;
    
    /// <summary>
    /// 不透明度百分比
    /// Opacity percentage
    /// </summary>
    [JsonProperty("opacity")]
    public double Opacity;
    
    /// <summary>
    /// 仅此瓦片
    /// Just this tile
    /// </summary>
    [JsonProperty("justThisTile")]
    public bool JustThisTile;
    
    /// <summary>
    /// 仅编辑器可见
    /// Editor only
    /// </summary>
    [JsonProperty("editorOnly")]
    public bool EditorOnly;
    
    /// <summary>
    /// 粘附到地板
    /// Stick to floors
    /// </summary>
    [JsonProperty("stickToFloors")]
    public bool StickToFloors;
    
    public PositionTrack(double[] positionOffset = null,
        object[] relativeTo = null,
        double rotation = 0,
        double scale = 100,
        double opacity = 100,
        bool justThisTile = false,
        bool editorOnly = false,
        bool stickToFloors = true)
    {
        EventType = EventType.PositionTrack;
        PositionOffset = positionOffset ?? [0, 0];
        RelativeTo = relativeTo ?? [0, "ThisTile"];
        Rotation = rotation;
        Scale = scale;
        Opacity = opacity;
        JustThisTile = justThisTile;
        EditorOnly = editorOnly;
        StickToFloors = stickToFloors;
    }
    
    // 辅助方法来设置 relativeTo 的值
    public void SetRelativeTo(int track, EventEnums.TileRelativeTo type)
    {
        RelativeTo = [track, type.ToString()];
    }
    
    // 辅助方法来获取 track 索引
    public int GetTrack()
    {
        if (RelativeTo != null && RelativeTo.Length >= 1 && RelativeTo[0] is int track)
            return track;
        return 0;
    }
    
    // 辅助方法来获取 tile reference type
    public EventEnums.TileRelativeTo GetTileRelativeTo()
    {
        if (RelativeTo != null && RelativeTo.Length >= 2 && RelativeTo[1] is string typeStr)
        {
            if (Enum.TryParse<EventEnums.TileRelativeTo>(typeStr, out var result))
                return result;
        }
        return EventEnums.TileRelativeTo.ThisTile;
    }
}