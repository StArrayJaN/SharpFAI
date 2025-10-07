using Newtonsoft.Json;

namespace SharpFAI.Serialization;

/// <summary>
/// 事件基类：所有具体事件的共同字段与序列化行为
/// Base event class: shared fields and serialization behavior for all events
/// </summary>
public class BaseEvent
{
    [JsonProperty("eventType")]
    public EventType EventType;
    
    [JsonProperty("floor")]
    public int Floor;
    
    /// <summary>
    /// 提供一个默认的空事件实例；事件类型为 None，砖块为 0
    /// Provides a default empty event instance; EventType is None and Floor is 0
    /// </summary>
    public static BaseEvent Empty => new BaseEvent
    {
        EventType = EventType.None,
        Floor = 0
    };

    /// <summary>
    /// 将事件序列化为 JSON 字符串（包含枚举为名字）
    /// Serialize event to JSON string (enums as names)
    /// </summary>
    public override string ToString()
    {
        return JsonConvert.SerializeObject(this, EventJsonConverter.GetJsonSettings());
    }
}