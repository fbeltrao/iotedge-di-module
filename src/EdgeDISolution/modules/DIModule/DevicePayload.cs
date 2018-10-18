using Newtonsoft.Json;
using System;

namespace DIModule
{
    // {"machine":{"temperature":21.813888929581218,"pressure":1.0927215236231766},"ambient":{"temperature":21.294986566898871,"humidity":25},"timeCreated":"2018-10-18T09:05:51.8149607Z"}
    public class DevicePayload
    {
        [JsonProperty("machine")]
        public MachineTelemetry Machine { get; set; }

        [JsonProperty("ambient")]
        public AmbientTelemetry Ambient { get; set; }

        [JsonProperty("timeCreated")]
        public DateTime TimeCreated { get; set; }
    }

    public class MachineTelemetry
    {
        [JsonProperty("temperature")]
        public double Temperature { get; set; }

        [JsonProperty("pressure")]
        public double Pressure { get; set; }
    }

    public class AmbientTelemetry
    {
        [JsonProperty("temperature")]
        public double Temperature { get; set; }

        [JsonProperty("humidity")]
        public double Humidity { get; set; }
    }
}