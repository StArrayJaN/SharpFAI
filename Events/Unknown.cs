using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using SharpFAI.Serialization;

namespace SharpFAI.Events;

/// <summary>
/// 表示未知或不支持的事件类型
/// </summary>
public class Unknown : BaseEvent
{
    /// <summary>
    /// 原始JSON数据（用于保留未知属性）
    /// </summary>
    [JsonExtensionData] public Dictionary<string, object> AdditionalData;
}