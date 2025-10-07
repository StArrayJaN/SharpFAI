using SharpFAI.Serialization;

namespace SharpFAI.Events;

public class Twirl : BaseEvent
{
    public Twirl()
    {
        EventType = EventType.Twirl;
    }
}