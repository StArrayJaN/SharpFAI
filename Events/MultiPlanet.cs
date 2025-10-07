using Newtonsoft.Json;
using SharpFAI.Serialization;

namespace SharpFAI.Events;

public class MultiPlanet : BaseEvent
{
    /// <summary>
    /// 行星数量
    /// Number of planets
    /// </summary>
    [JsonProperty("planets")]
    public EventEnums.PlanetCount Planets { get; set; }
    
    public MultiPlanet(EventEnums.PlanetCount planets = EventEnums.PlanetCount.TwoPlanets)
    {
        EventType = EventType.MultiPlanet;
        Planets = planets;
    }
}