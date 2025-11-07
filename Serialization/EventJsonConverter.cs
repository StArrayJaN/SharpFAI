using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using SharpFAI.Events;

namespace SharpFAI.Serialization;

/// <summary>
/// 事件 JSON 转换器：根据 eventType 做多态反序列化，并确保枚举以名字序列化
/// Event JSON converter: polymorphic deserialization by eventType, ensuring enums serialize as names
/// </summary>
public class EventJsonConverter : JsonConverter<BaseEvent>
{
    private static readonly Dictionary<string, Type> EventTypeMap = new()
    {
        { "SetSpeed", typeof(SetSpeed) },
        { "Twirl", typeof(Twirl) },
        { "MultiPlanet", typeof(MultiPlanet) },
        { "Pause", typeof(Pause) },
        { "Bookmark", typeof(Bookmark) },
        { "PositionTrack", typeof(PositionTrack) },
        { "Hold", typeof(Hold) },
        { "MoveCamera", typeof(MoveCamera) },
        { "Unknown", typeof(Unknown) }
    };

    public override bool CanWrite => false;

    /// <summary>
    /// 读取并反序列化事件，根据 eventType 选择具体类型
    /// Read and deserialize event, choose concrete type by eventType
    /// </summary>
    public override BaseEvent ReadJson(JsonReader reader, Type objectType, BaseEvent existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        if (reader.TokenType == JsonToken.Null)
            return new BaseEvent
            {
                Floor = 0,
                EventType = EventType.None
            };

        var jsonObject = JObject.Load(reader);
        
        // 获取事件类型
        var eventTypeToken = jsonObject["eventType"];
        if (eventTypeToken == null)
            throw new JsonSerializationException("Missing 'eventType' property in JSON");

        var eventTypeString = eventTypeToken.ToString();
        /*if (jsonObject.ContainsKey("position"))
        {
            JArray position = (JArray)jsonObject["position"];
            for (int i = 0; i < position.Count; i++)
            {
                if (position[i].ToString() == "null")
                {
                    position[i] = JToken.FromObject("0");
                }
            }
            jsonObject["position"] = position;
        }*/
        
        // 根据事件类型创建相应的实例
        if (!EventTypeMap.TryGetValue(eventTypeString, out var targetType))
        {
            // 对于未知事件类型，创建UnknownEvent实例
            targetType = typeof(Unknown);
        }

        // 使用默认序列化器反序列化到具体类型
        var settings = new JsonSerializerSettings();
        // 枚举序列化为名字，不允许整数形式
        StringEnumConverter stringEnumConverter = new();
        settings.Converters.Add(stringEnumConverter);
        
        var eventInstance = (BaseEvent)jsonObject.ToObject(targetType, JsonSerializer.Create(settings));
        
        // 如果是未知事件类型，保存原始事件类型信息
        if (eventInstance is Unknown unknownEvent && !EventTypeMap.ContainsKey(eventTypeString))
        {
            Enum.TryParse<EventType>(eventTypeString, out var eventType);
            unknownEvent.EventType = eventType;
        }
        
        return eventInstance;
    }

    /// <summary>
    /// 写入序列化由默认序列化器处理
    /// Writing is handled by default serializer
    /// </summary>
    public override void WriteJson(JsonWriter writer, BaseEvent value, JsonSerializer serializer)
    {
        throw new NotImplementedException("Use default serialization.");
    }
    
    /// <summary>
    /// 获取统一的 JSON 设置（包含枚举字符串化与事件转换器）
    /// Get unified JSON settings (string enum + event converter)
    /// </summary>
    public static JsonSerializerSettings GetJsonSettings()
    {
        return new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Include,
            DefaultValueHandling = DefaultValueHandling.Include,
            Converters = { new EventJsonConverter(), new StringEnumConverter() }
        };
    }

    public static T? Deserialize<T>(string json)
    {
        return JsonConvert.DeserializeObject<T>(json, GetJsonSettings());
    }

    public static string Serialize<T>(T obj)
    {
        return JsonConvert.SerializeObject(obj, GetJsonSettings());
    }
}