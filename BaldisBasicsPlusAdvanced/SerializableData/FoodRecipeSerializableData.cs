using BaldisBasicsPlusAdvanced.Game.Objects.Plates.KitchenStove;
using BepInEx;
using BepInEx.Bootstrap;
using MTM101BaldAPI;
using MTM101BaldAPI.Registers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace BaldisBasicsPlusAdvanced.SerializableData
{
    [JsonObject]
    public class FoodRecipeSerializableData
    {
        public SerializableItem[] rawFood;

        public SerializableItem[] cookedFood;

        /// <summary>
        /// Loads Kitchen Stove's recipe from path.
        /// May cause exception if something will go wrong during deserializing or reading data from file!
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static FoodRecipeSerializableData LoadFromPath(string path)
        {
            return JsonConvert.DeserializeObject<FoodRecipeSerializableData>(File.ReadAllText(path));
        }

        /// <summary>
        /// Converts <see cref="FoodRecipeSerializableData"/> to <see cref="FoodRecipeData"/>.
        /// </summary>
        /// <param name="pluginInfo">This parameter is used to get items firstly from your mod if recipe contains item with modded enum.
        /// If you are using item from other mod, then use field "GUID" from <see cref="SerializableItem"/> to get item from special mod!
        /// Otherwise converter will be thinking that you want to get item from your mod.</param>
        /// <returns></returns>
        public FoodRecipeData ConvertToStandard(PluginInfo pluginInfo)
        {
            FoodRecipeData data = new FoodRecipeData(pluginInfo);
            List<ItemObject> rawFood = new List<ItemObject>();
            List<ItemObject> cookedFood = new List<ItemObject>();
            for (int i = 0; i < this.rawFood.Length; i++)
            {
                if (string.IsNullOrEmpty(this.rawFood[i].GUID) && Enum.TryParse(this.rawFood[i].name, out Items @enum))
                {
                    ItemMetaData meta = ItemMetaStorage.Instance.FindByEnum(@enum);
                    if (this.rawFood[i].uses > 0)
                        rawFood.Add(meta.itemObjects[this.rawFood[i].uses - 1]);
                    else rawFood.Add(meta.value);
                } else
                {
                    ItemMetaData meta = null;

                    if (string.IsNullOrEmpty(this.rawFood[i].GUID))
                    {
                        meta = ItemMetaStorage.Instance.FindByEnumFromMod(EnumExtensions.GetFromExtendedName<Items>(
                        this.rawFood[i].name),
                        pluginInfo);
                    } else meta = ItemMetaStorage.Instance.FindByEnumFromMod(EnumExtensions.GetFromExtendedName<Items>(
                        this.rawFood[i].name),
                        Chainloader.PluginInfos[this.rawFood[i].GUID]);

                    if (this.rawFood[i].uses > 0)
                        rawFood.Add(meta.itemObjects[this.rawFood[i].uses - 1]);
                    else rawFood.Add(meta.value);
                }
            }
            for (int i = 0; i < this.cookedFood.Length; i++)
            {
                if (string.IsNullOrEmpty(this.cookedFood[i].GUID) && Enum.TryParse(this.cookedFood[i].name, out Items @enum))
                {
                    ItemMetaData meta = ItemMetaStorage.Instance.FindByEnum(@enum);
                    if (this.cookedFood[i].uses > 0)
                        cookedFood.Add(meta.itemObjects[this.cookedFood[i].uses - 1]);
                    else cookedFood.Add(meta.value);
                }
                else
                {
                    ItemMetaData meta = null;

                    if (string.IsNullOrEmpty(this.cookedFood[i].GUID))
                    {
                        meta = ItemMetaStorage.Instance.FindByEnumFromMod(EnumExtensions.GetFromExtendedName<Items>(
                        this.cookedFood[i].name),
                        pluginInfo);
                    }
                    else meta = ItemMetaStorage.Instance.FindByEnumFromMod(EnumExtensions.GetFromExtendedName<Items>(
                        this.cookedFood[i].name),
                        Chainloader.PluginInfos[this.cookedFood[i].GUID]);

                    if (this.cookedFood[i].uses > 0)
                        cookedFood.Add(meta.itemObjects[this.cookedFood[i].uses - 1]);
                    else cookedFood.Add(meta.value);
                }
            }

            data.SetRawFood(rawFood.ToArray())
                .SetCookedFood(cookedFood.ToArray());

            return data;
        }

        /// <summary>
        /// Nothing special, just a class which contains item's data for recipe
        /// </summary>
        [JsonObject]
        public class SerializableItem
        {
            /// <summary>
            /// That's enum actually.
            /// </summary>
            public string name;

            /// <summary>
            /// Use it if you need to use item from other mod.
            /// </summary>
            public string GUID;

            public int uses = 0;
        }

    }
}
