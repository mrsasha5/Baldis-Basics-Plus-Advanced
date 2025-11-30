using System;
using HarmonyLib;
using MTM101BaldAPI;
using MTM101BaldAPI.Registers;
using Newtonsoft.Json;

namespace BaldisBasicsPlusAdvanced.Generation.Data
{
    internal class NpcSpawnData : BaseSpawnData<NPC>
    {
        [JsonProperty]
        private bool forced;

        [JsonProperty("reference")]
        private string Serialization_Npc
        {
            set
            {
                instance = FindInstance(value).value;
            }
        }

        public NpcSpawnData(NPC instance)
        {
            this.instance = instance;
        }

        public NpcSpawnData ForceSpawn()
        {
            forced = true;
            return this;
        }

        public override void Register(string name, int floor, SceneObject sceneObject, CustomLevelObject levelObject)
        {
            if (Instance == null)
                throw new Exception("Object reference is null!");

            int weight = GetWeight(floor, levelObject.type);
            if (weight != 0)
            {
                if (forced)
                {
                    levelObject.forcedNpcs = levelObject.forcedNpcs.AddToArray(Instance);
                }
                else
                {
                    sceneObject.potentialNPCs.Add(new WeightedNPC()
                    {
                        selection = Instance,
                        weight = weight
                    });
                }
            }
        }

        public static NPCMetadata FindInstance(string @enum)
        {
            NPCMetadata meta =
                NPCMetaStorage.Instance.Find(x => x.character == EnumExtensions.GetFromExtendedName<Character>(@enum));

            if (meta == null) throw new Exception("NPC metadata was not found!");
            return meta;
        }

    }
}
