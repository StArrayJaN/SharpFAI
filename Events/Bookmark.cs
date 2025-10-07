using SharpFAI.Serialization;

namespace SharpFAI.Events;

public class Bookmark : BaseEvent
{
    public Bookmark()
    {
        EventType = EventType.Bookmark;
    }
}