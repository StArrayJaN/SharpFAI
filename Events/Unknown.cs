using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using SharpFAI.Serialization;

namespace SharpFAI.Events;

/// <summary>
/// 表示未知或不支持的事件类型
/// Represents unknown or unsupported event type
/// </summary>
public class Unknown : BaseEvent
{
    /// <summary>
    /// 原始 JSON 数据（用于保留未知属性）
    /// Original JSON data (for preserving unknown properties)
    /// </summary>
    [JsonExtensionData] public Dictionary<string, object> AdditionalData;
}