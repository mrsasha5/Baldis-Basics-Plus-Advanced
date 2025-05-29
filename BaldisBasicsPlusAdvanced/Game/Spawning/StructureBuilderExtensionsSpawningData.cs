using BaldisBasicsPlusAdvanced.Helpers;
using System.Collections.Generic;
using System.Linq;

namespace BaldisBasicsPlusAdvanced.Game.Spawning
{
    public class StructureBuilderExtensionsSpawningData : BaseSpawningData
    {
        private StructureBuilder builder;

        private Dictionary<int, StructureParameters> parameters = new Dictionary<int, StructureParameters>();

        public StructureBuilder StructureBuilder => builder;

        public StructureBuilderExtensionsSpawningData(string key, StructureBuilder builder) : base(key)
        {
            this.builder = builder;
        }

        public StructureBuilderExtensionsSpawningData SetStructureParameters(int floor, StructureParameters parameters)
        {
            if (this.parameters.ContainsKey(floor))
            {
                this.parameters[floor] = parameters;
            }
            else
            {
                this.parameters.Add(floor, parameters);
            }
            return this;
        }

        public StructureParameters GetStructureParameters(int floor)
        {
            if (parameters.Count == 0) return null;

            if (parameters.ContainsKey(floor))
            {
                return parameters[floor];
            }
            else
            {
                int nearest = MathHelper.FindNearestValue(parameters.Keys.ToArray(), floor);
                return parameters[nearest];
            }
        }
    }
}
