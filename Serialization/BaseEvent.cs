using System;
using Newtonsoft.Json;

namespace SharpFAI.Serialization;

/// <summary>
/// 事件基类：所有具体事件的共同字段与序列化行为
/// Base event class: shared fields and serialization behavior for all events
/// </summary>
public class BaseEvent
{
    /// <summary>
    /// 事件类型
    /// Event type
    /// </summary>
    [JsonProperty("eventType")]
    public EventType EventType;
    
    /// <summary>
    /// 所在地板（砖块）编号
    /// Floor (tile) number
    /// </summary>
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

    /// <summary>
    /// 将事件转换为指定的具体事件类型
    /// Convert event to specific event type
    /// </summary>
    /// <typeparam name="T">目标事件类型 / Target event type</typeparam>
    /// <returns>转换后的事件实例 / Converted event instance</returns>
    /// <exception cref="InvalidCastException">当事件类型不匹配时抛出 / Thrown when event type doesn't match</exception>
    public T ToEvent<T>() where T : BaseEvent
    {
        if (EventType.ToString() == typeof(T).Name)
        {
            return JsonConvert.DeserializeObject<T>(ToString());
        }
        throw new InvalidCastException("Unknow event type: " + EventType);
    }
}