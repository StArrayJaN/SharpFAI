using Newtonsoft.Json.Linq;
using SharpFAI.Events;
using SharpFAI.Serialization;

namespace SharpFAI.Util;

public static class EventUtils
{
    public static void MoveDecorations(this Level level, int floor, double duration, int opacity, string tag)
    {
        JObject data = new()
        {
            { "duration", duration },
            { "tag", tag },
            { "opacity", opacity }
        };
        level.AddEvent(floor, EventType.MoveDecorations, data);
    }
    
    public static void SetText(this Level level, int floor, string text, string tag)
    {
        JObject data = new();
        data.Add("decText", text);
        data.Add("tag", tag);
        level.AddEvent(floor, EventType.SetText, data);
    }
}