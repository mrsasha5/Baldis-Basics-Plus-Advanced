using Newtonsoft.Json;

namespace BaldisBasicsPlusAdvanced.Generation.Data
{
    [JsonObject(MemberSerialization.OptIn)]
    internal struct WeightData
    {
        [JsonProperty]
        public int floor;

        [JsonProperty]
        public int weight;

        public WeightData(int floor, int weight)
        {
            this.floor = floor;
            this.weight = weight;
        }
    }
}
