using BaldisBasicsPlusAdvanced.API;
using BepInEx;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BaldisBasicsPlusAdvanced.Game.Objects.Plates.KitchenStove
{
    /// <summary>
    /// Please do not try to add for the one array more than "MaxFoodCount" from the KitchenStove property.
    /// I just will cut the array.
    /// </summary>
    public class FoodRecipeData
    {
        private ItemObject[] rawFood;

        private ItemObject[] cookedFood;

        private bool soundOverridden;

        private SoundObject sound;

        private float? coolingTime;

        private float? cookingTime;

        internal Action<KitchenStove> onKitchenStoveActivatingPre;

        internal Action<KitchenStove> onKitchenStoveActivatingPost;

        internal Action<KitchenStove> onKitchenStoveDeactivatingPre;

        internal Action<KitchenStove> onKitchenStoveDeactivatingPost;

        internal List<PluginInfo> pluginInfos = new List<PluginInfo>();

        public ItemObject[] RawFood => rawFood;

        public ItemObject[] CookedFood => cookedFood;

        public SoundObject Sound => sound;

        public float? CoolingTime => coolingTime;

        public float? CookingTime => cookingTime;

        public bool SoundOverridden => soundOverridden;

        /// <summary>
        /// Initialization of the class.
        /// </summary>
        /// <param name="pluginInfo"></param>
        public FoodRecipeData(PluginInfo pluginInfo)
        {
            this.pluginInfos.Add(pluginInfo);
            this.rawFood = new ItemObject[0];
            this.cookedFood = new ItemObject[0];
        }

        /// <summary>
        /// Sets raw items which are required to cook cooked ones!
        /// </summary>
        /// <param name="rawFood"></param>
        /// <returns></returns>
        public FoodRecipeData SetRawFood(params ItemObject[] rawFood)
        {
            if (rawFood.Length > KitchenStove.MaxFoodCount)
            {
                AdvancedCore.Logging.LogWarning(
                    $"Recipe can't contain more than {KitchenStove.MaxFoodCount} items in the one array. Cutting the rawFood array!");
                ItemObject[] newArray = new ItemObject[KitchenStove.MaxFoodCount];
                for (int i = 0; i < newArray.Length; i++)
                {
                    newArray[i] = rawFood[i];
                }
                rawFood = newArray;
            }
            this.rawFood = rawFood;
            return this;
        }

        /// <summary>
        /// Sets cooked items which player gets after cooking recipe!
        /// </summary>
        /// <param name="cookedFood"></param>
        /// <returns></returns>
        public FoodRecipeData SetCookedFood(params ItemObject[] cookedFood)
        {
            if (cookedFood.Length > KitchenStove.MaxFoodCount)
            {
                AdvancedCore.Logging.LogWarning(
                    $"Recipe can't contain more than {KitchenStove.MaxFoodCount} items in the one array. Cutting the cookedFood array!");
                ItemObject[] newArray = new ItemObject[KitchenStove.MaxFoodCount];
                for (int i = 0; i < newArray.Length; i++)
                {
                    newArray[i] = rawFood[i];
                }
                cookedFood = newArray;
            }
            this.cookedFood = cookedFood;
            return this;
        }

        /// <summary>
        /// Adds delegate which will be invoked before starting cooking process.
        /// Post is false: Invokes on KitchenStove.Activate()
        /// Post is true: Invokes on KitchenStove.OnActivatingPost()
        /// </summary>
        /// <param name="action"></param>
        /// <param name="post"></param>
        /// <returns></returns>
        public FoodRecipeData AddListenerOnKitchenStoveActivating(Action<KitchenStove> action, bool post)
        {
            if (post) onKitchenStoveActivatingPost += action;
            else onKitchenStoveActivatingPre += action;
            return this;
        }

        /// <summary>
        /// Adds delegate which will be invoked on finishing cooking process.
        /// Post is false: Invokes on KitchenStove.OnDeactivatingPre()
        /// Post is true: Invokes on KitchenStove.OnDeactivatingPost()
        /// </summary>
        /// <param name="action"></param>
        /// <param name="post"></param>
        /// <returns></returns>
        public FoodRecipeData AddListenerOnKitchenStoveDeactivating(Action<KitchenStove> action, bool post)
        {
            if (post) onKitchenStoveDeactivatingPost += action;
            else onKitchenStoveDeactivatingPre += action;
            return this;
        }

        /// <summary>
        /// Overrides cooling time for that recipe! Johnny's Kitchen Stove ignores that!
        /// </summary>
        /// <param name="coolingTime"></param>
        /// <returns></returns>
        public FoodRecipeData OverrideCoolingTime(float coolingTime)
        {
            this.coolingTime = coolingTime;
            return this;
        }

        /// <summary>
        /// Overrides cooking time for that recipe!
        /// </summary>
        /// <param name="cookingTime"></param>
        /// <returns></returns>
        public FoodRecipeData OverrideCookingTime(float cookingTime)
        {
            this.cookingTime = cookingTime;
            return this;
        }

        /// <summary>
        /// Overrides sound when recipe is cooked!
        /// </summary>
        /// <param name="sound">If equals null then audio manager won't be playing anything!</param>
        /// <returns></returns>
        public FoodRecipeData OverrideSound(SoundObject sound)
        {
            soundOverridden = true;
            this.sound = sound;
            return this;
        }

        /// <summary>
        /// Usually used by Kitchen Stove. With 99% chance you won't be needing in using that.
        /// </summary>
        /// <param name="pickups"></param>
        /// <returns></returns>
        public bool ContainsListOfPickups(List<Pickup> pickups)
        {
            List<ItemObject> rawFood = new List<ItemObject>();

            for (int i = 0; i < pickups.Count; i++)
            {
                rawFood.Add(pickups[i].item);
            }

            return ContainsListOfRawFood(rawFood);
        }

        /// <summary>
        /// Usually used by Kitchen Stove. With 99% chance you won't be needing in using that.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public bool IsEqual(FoodRecipeData data)
        {
            if (IsIdentical(data) &&
                data.ContainsListOfCookedFood(cookedFood.ToList()) && cookedFood.Length == data.cookedFood.Length)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Usually used by Kitchen Stove. With 99% chance you won't be needing in using that.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public bool IsIdentical(FoodRecipeData data)
        {
            if (data.ContainsListOfRawFood(rawFood.ToList()) && rawFood.Length == data.rawFood.Length)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Usually used by Kitchen Stove. With 99% chance you won't be needing in using that.
        /// </summary>
        /// <param name="cookedFood"></param>
        /// <returns></returns>
        public bool ContainsListOfCookedFood(List<ItemObject> cookedFood)
        {
            List<ItemObject> _cookedFood = new List<ItemObject>(this.cookedFood);
            for (int i = 0; i < cookedFood.Count; i++)
            {
                if (!_cookedFood.Contains(cookedFood[i])) return false;
                else _cookedFood.Remove(cookedFood[i]);
            }
            return true;
        }


        /// <summary>
        /// Usually used by Kitchen Stove. With 99% chance you won't be needing in using that.
        /// </summary>
        /// <param name="rawFood"></param>
        /// <returns></returns>
        public bool ContainsListOfRawFood(List<ItemObject> rawFood)
        {
            List<ItemObject> _rawFood = new List<ItemObject>(this.rawFood);
            for (int i = 0; i < rawFood.Count; i++)
            {
                if (!_rawFood.Contains(rawFood[i])) return false;
                else _rawFood.Remove(rawFood[i]);
            }
            return true;
        }

        /// <summary>
        /// Another way of registering recipe! Use that one which you prefer! ApiManager.CreateKitchenStoveRecipe(...) or this method!
        /// </summary>
        /// <returns></returns>
        public bool RegisterRecipe()
        {
            return ApiManager.CreateKitchenStoveRecipe(this);
        }

    }
}
