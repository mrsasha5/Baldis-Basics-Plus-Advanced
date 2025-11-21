using HarmonyLib;
using MTM101BaldAPI;

namespace BaldisBasicsPlusAdvanced.Generation.Data
{
    internal class NpcSpawnData : BaseSpawnData<NPC>
    {

        private bool forced;

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

    }
}
