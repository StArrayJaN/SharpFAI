using System;
using Newtonsoft.Json;
using SharpFAI.Serialization;

namespace SharpFAI.Events;

public class PositionTrack : BaseEvent
{
    [JsonProperty("positionOffset")]
    public float[] PositionOffset { get; set; }
    
    [JsonProperty("relativeTo")]
    public object[] RelativeTo { get; set; }
    
    [JsonProperty("rotation")]
    public float Rotation { get; set; }
    
    [JsonProperty("scale")]
    public float Scale { get; set; }
    
    [JsonProperty("opacity")]
    public float Opacity { get; set; }
    
    [JsonProperty("justThisTile")]
    public bool JustThisTile { get; set; }
    
    [JsonProperty("editorOnly")]
    public bool EditorOnly { get; set; }
    
    [JsonProperty("stickToFloors")]
    public bool StickToFloors { get; set; }
    
    public PositionTrack(float[] positionOffset = null,
        object[] relativeTo = null,
        float rotation = 0,
        float scale = 100,
        float opacity = 100,
        bool justThisTile = false,
        bool editorOnly = false,
        bool stickToFloors = true)
    {
        EventType = EventType.PositionTrack;
        PositionOffset = positionOffset ?? new float[] { 0, 0 };
        RelativeTo = relativeTo ?? new object[] { 0, "ThisTile" };
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
        RelativeTo = new object[] { track, type.ToString() };
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