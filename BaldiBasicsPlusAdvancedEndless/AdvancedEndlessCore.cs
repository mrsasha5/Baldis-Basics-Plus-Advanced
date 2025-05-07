using BaldiEndless;
using BaldisBasicsPlusAdvanced.Cache;
using BepInEx;
using MTM101BaldAPI.Registers;
using System;

namespace BaldiBasicsPlusAdvancedEndless
{

    [BepInPlugin(modId, modName, version)]
    [BepInDependency("baldi.basics.plus.advanced.mod")]
    [BepInDependency("mtm101.rulerp.baldiplus.endlessfloors")]
    public class AdvancedEndlessCore : BaseUnityPlugin
    {
        public const string modId = "baldi.basics.plus.advanced.endless.mod";

        public const string modName = "Baldi's Basics Plus Advanced Edition Endless Floors Integration";

        public const string version = "0.1.3.9";

        private static AdvancedEndlessCore instance;

        public static AdvancedEndlessCore Instance => instance;

        private void Awake()
        {
            EndlessFloorsPlugin.AddGeneratorAction(this.Info, registerContentForEndlessFloors);
        }

        private static void registerContentForEndlessFloors(GeneratorData genData)
        {
            foreach (string name in ObjectsStorage.WeightedItemObjects.Keys)
            {
                if (ObjectsStorage.spawningItemsData[name].spawnsOnRooms)
                {
                    genData.items.Add(ObjectsStorage.WeightedItemObjects[name]);
                }
            }
            genData.randomEvents.AddRange(ObjectsStorage.WeightedEvents.Values);
            genData.objectBuilders.AddRange(ObjectsStorage.WeightedObjectBuilders.Values);
        }


    }
}
