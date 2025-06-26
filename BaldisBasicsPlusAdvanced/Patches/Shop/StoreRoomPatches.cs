using BaldisBasicsPlusAdvanced.API;
using BaldisBasicsPlusAdvanced.Cache.AssetsManagment;
using BaldisBasicsPlusAdvanced.Game.Objects.Pickups;
using BaldisBasicsPlusAdvanced.Game.Objects.Plates.KitchenStove;
using BaldisBasicsPlusAdvanced.Helpers;
using HarmonyLib;
using System;
using UnityEngine;

namespace BaldisBasicsPlusAdvanced.Patches.Shop
{
    [HarmonyPatch(typeof(StoreRoomFunction))]
    internal class StoreRoomPatches
    {
        private static StoreRoomFunction storeFunc;

        private static PriceTag priceTagPre;

        [HarmonyPatch("OnGenerationFinished")]
        [HarmonyPostfix]
        private static void OnGenerationFinished(StoreRoomFunction __instance)
        {

            JohnnyKitchenStove stove = GameObject.FindObjectOfType<JohnnyKitchenStove>();
            if (stove != null)
            {
                stove.Assign(__instance);

                IntVector2 pos = __instance.Room.ec.CellFromPosition(stove.transform.position).position;
                pos.x -= 1;
                GameButton button = (GameButton)GameButton.Build(AssetsStorage.gameButton, __instance.Room.ec,
                    pos, Direction.North);
                button.SetUp(stove);
            }
        }

        [HarmonyPatch("Initialize")]
        [HarmonyPostfix]
        private static void OnInitialize(StoreRoomFunction __instance, RoomController room, ref PriceTag ___mapTag, ref SceneObject ___storeData)
        {
            if (___storeData == null) return;

            storeFunc = __instance;
            priceTagPre = ___mapTag;

            int hammerPrice = (int)(___storeData.mapPrice * 1.2f);
            int refreshPrice = ___storeData.totalShopItems * 10;

            PriceTag hammerPriceTag = null;

            if (!PitOverrides.ExpelHammerPickupDisabled)
            {
                hammerPriceTag = CreatePriceTag(hammerPrice.ToString());
                hammerPriceTag.transform.localPosition = new Vector3(20f, 2.65f, 46f);
            }

            if (!PitOverrides.RefreshPickupDisabled)
            {
                PriceTag refreshPriceTag = CreatePriceTag(refreshPrice.ToString());
                refreshPriceTag.transform.localPosition = new Vector3(10f, 2.65f, 46f);

                RefreshPickup refreshPickup = CreatePickup<RefreshPickup>(refreshPriceTag, refreshPrice, new Vector3(10f, 5f, 48f));
                refreshPickup.storeRoomFunc = storeFunc;
            }

            if (Singleton<BaseGameManager>.Instance is PitstopGameManager && !PitOverrides.ExpelHammerPickupDisabled)
            {
                CreatePickup<ExpelHammerPickup>(hammerPriceTag, hammerPrice, new Vector3(20f, 5f, 48f));
            }
            else
            {
                //TAG_Sale
                hammerPriceTag?.SetText(Singleton<LocalizationManager>.Instance.GetLocalizedText("Adv_Tag_Out"));
            }
            
        }

        private static T CreatePickup<T>(PriceTag tag, int price, Vector3 pos) where T : BasePickup
        {
            Pickup pickupComp = GameObject.Instantiate(AssetsStorage.pickup, storeFunc.Room.objectObject.transform);

            T pickup = pickupComp.gameObject.AddComponent<T>();
            pickup.name = "Pickup";
            pickup.Initialize(pickup.GetComponentInChildren<SpriteRenderer>(), tag, price);
            pickup.transform.position = pos;

            pickup.onPickupPurchasing += delegate ()
             {
                 if (BuyingItem(pickup.Price, tag, out int ytpCollected))
                 {
                     pickup.OnPurchasing(ytpCollected);
                 } else if (!ReflectionHelper.GetValue<bool>(storeFunc, "open"))
                 {
                     pickup.OnStealing();
                     if (!ReflectionHelper.GetValue<bool>(storeFunc, "alarmStarted") && pickup.RaiseAlarmDuringRobbery)
                         ReflectionHelper.UseRequiredMethod(storeFunc, "SetOffAlarm");
                 }
             };

            GameObject.Destroy(pickupComp);

            return pickup;
        }

        private static PriceTag CreatePriceTag(string text)
        {
            PriceTag priceTag = GameObject.Instantiate(priceTagPre);
            priceTag.transform.SetParent(storeFunc.Room.objectObject.transform);
            priceTag.SetText(text);
            return priceTag;
        }

        public static void PlayJohnnyUnafforable(StoreRoomFunction func = null)
        {
            if (func == null) func = storeFunc;
            if (func == null) return;

            PropagatedAudioManagerAnimator audMan = 
                ReflectionHelper.GetValue<PropagatedAudioManagerAnimator>(func, "johnnyAudioManager");
            if (!audMan.QueuedUp)
            {
                audMan.QueueRandomAudio(ReflectionHelper.GetValue<SoundObject[]>(func, "audUnafforable"));
            }
        }
        
        public static void PlayJohnnyBuy(StoreRoomFunction func = null)
        {
            if (func == null) func = storeFunc;
            if (func == null) return;

            PropagatedAudioManagerAnimator audMan = 
                ReflectionHelper.GetValue<PropagatedAudioManagerAnimator>(func, "johnnyAudioManager");
            if (!audMan.QueuedUp)
            {
                audMan.QueueRandomAudio(ReflectionHelper.GetValue<SoundObject[]>(func, "audBuy"));
            }

            ReflectionHelper.SetValue<bool>(storeFunc, "itemPurchased", true);
            ReflectionHelper.SetValue<bool>(storeFunc, "playerLeft", false);
        }

        public static bool BuyingItem(int price, PriceTag priceTag, out int ytpCollected)
        {
            ytpCollected = 0;
            if (ReflectionHelper.GetValue<bool>(storeFunc, "open"))
            {
                PropagatedAudioManagerAnimator audMan = ReflectionHelper.GetValue<PropagatedAudioManagerAnimator>(storeFunc, "johnnyAudioManager");
                bool isPossibleToBuy = Singleton<CoreGameManager>.Instance.GetPoints(0) >= price;

                if (!isPossibleToBuy && !Singleton<CoreGameManager>.Instance.johnnyHelped && Math.Abs(price - Singleton<CoreGameManager>.Instance.GetPoints(0)) <= 100)
                {
                    price = Singleton<CoreGameManager>.Instance.GetPoints(0);
                    Singleton<CoreGameManager>.Instance.johnnyHelped = true;
                    audMan.FlushQueue(true);
                    audMan.QueueAudio(ReflectionHelper.GetValue<SoundObject>(storeFunc, "audHelp"));
                } else if (!isPossibleToBuy)
                {
                    if (!audMan.QueuedUp)
                    {
                        audMan.QueueRandomAudio(ReflectionHelper.GetValue<SoundObject[]>(storeFunc, "audUnafforable"));
                    }
                    return false;
                }

                ytpCollected = price;

                Singleton<CoreGameManager>.Instance.audMan.PlaySingle(AssetsStorage.sounds["ytp_pickup_0"]);

                Singleton<CoreGameManager>.Instance.AddPoints(-price, 0, true);

                priceTag.SetText(Singleton<LocalizationManager>.Instance.GetLocalizedText("TAG_Sale"));

                ReflectionHelper.SetValue<bool>(storeFunc, "itemPurchased", true);
                ReflectionHelper.SetValue<bool>(storeFunc, "playerLeft", false);
                return true;
            }
            return false;
        }

    }
}
