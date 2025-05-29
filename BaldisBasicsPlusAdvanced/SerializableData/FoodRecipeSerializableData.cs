using BaldisBasicsPlusAdvanced.Game.Objects.Plates.KitchenStove;
using BepInEx;
using BepInEx.Bootstrap;
using MTM101BaldAPI;
using MTM101BaldAPI.Registers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace BaldisBasicsPlusAdvanced.SerializableData
{
    [JsonObject]
    public class FoodRecipeSerializableData
    {
        public SerializableItem[] rawFood;

        public SerializableItem[] cookedFood;

        public FoodRecipeData ConvertToStandard(PluginInfo pluginInfo)
        {
            FoodRecipeData data = new FoodRecipeData(pluginInfo);
            List<ItemObject> rawFood = new List<ItemObject>();
            List<ItemObject> cookedFood = new List<ItemObject>();
            for (int i = 0; i < this.rawFood.Length; i++)
            {
                //Debug.Log("Name: " + this.rawFood[i].name);
                //Debug.Log("Uses: " + this.rawFood[i].uses);
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
                //Debug.Log("Name: " + this.cookedFood[i].name);
                //Debug.Log("Uses: " + this.cookedFood[i].uses);
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

        [JsonObject]
        public class SerializableItem
        {
            public string name;

            public string GUID;

            public int uses = 0;
        }

    }
}
