using BaldisBasicsPlusAdvanced.Cache;
using BaldisBasicsPlusAdvanced.Helpers;
using HarmonyLib;
using MTM101BaldAPI.UI;
using System;
using System.Collections.Generic;
using System.Text;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

namespace BaldisBasicsPlusAdvanced.Patches.Shop
{
    [HarmonyPatch(typeof(StoreScreen))]
    internal class StorePricesPatch
    {

        private static Dictionary<int, int> percents = new Dictionary<int, int>();

        private static int[] potentialPercents = new int[] { 10, 15, 20, 25 };

        private static List<GameObject> toDestroyLater = new List<GameObject>();

        private static int baseItemPrice;

        private static bool setBasePriceOnPost;

        private static void clearAll()
        {
            percents.Clear();
            foreach (GameObject gameObject in toDestroyLater)
            {
                GameObject.Destroy(gameObject);
            }
        }

        [HarmonyPatch("Start")]
        [HarmonyPostfix]
        private static void onStart(StoreScreen __instance, ref Image[] ___forSaleImage, ref TMP_Text[] ___itemPrice, ref ItemObject[] ___itemForSale,
            ref ItemObject ___defaultItem)
        {
            clearAll();

            int maxCount = UnityEngine.Random.Range(1, 3);

            int pricesAdded = 0;

            for (int i = 0; i < ___itemForSale.Length; i++)
            {
                if (pricesAdded >= maxCount) break;

                if (___itemForSale[i] == ___defaultItem || ___itemForSale[i] == null) continue;

                if (!(UnityEngine.Random.Range(0, 100) >= 90)) continue;

                Image image = ___forSaleImage[i];

                int percent = potentialPercents[UnityEngine.Random.Range(0, potentialPercents.Length)];

                if (UnityEngine.Random.Range(0, 100) == 99) percent = 50; //99!!!

                Image percentImage = UIHelpers.CreateImage(AssetsStorage.sprites["adv_item_discount"], image.transform, new Vector3(15f, 20f, 0f), false, 0.75f);
                percentImage.name = "Percent background";
                TextMeshProUGUI percentText = UIHelpers.CreateText<TextMeshProUGUI>(BaldiFonts.ComicSans12, "-" + percent + "%", image.transform, new Vector3(105f, 2f, 0f), false);
                percentText.name = "Percent text";

                toDestroyLater.Add(percentImage.gameObject);
                toDestroyLater.Add(percentText.gameObject);

                percents.Add(i, percent);
                pricesAdded++;

                //s - strikethrough
                if (___itemForSale[i].price >= 10000)
                {
                    ___itemPrice[i].text = "<s>" + ___itemForSale[i].price + "</s>\n"
                        + ___itemForSale[i].price * (100 - percents[i]) / 100;

                    Vector2 newPos = ___itemPrice[i].transform.localPosition;
                    newPos.x -= 3;
                    newPos.y += 8;
                    ___itemPrice[i].transform.localPosition = newPos;
                    ___itemPrice[i].transform.SetSiblingIndex(15);
                } else
                {
                    ___itemPrice[i].text = "<s>" + ___itemForSale[i].price + "</s> "
                        + ___itemForSale[i].price * (100 - percents[i]) / 100;

                    if (___itemPrice[i].text.Length == 16)
                    {
                        Vector2 newPos = ___itemPrice[i].transform.localPosition;
                        newPos.x -= 3;
                        ___itemPrice[i].transform.localPosition = newPos;
                    }
                }
                
            }

        }

        [HarmonyPatch("BuyItem")]
        [HarmonyPrefix]
        private static void onBuyItemPre(StoreScreen __instance, int val, ref ItemObject[] ___itemForSale, ref int ___ytps)
        {
            if (percents.ContainsKey(val))
            {
                int newPrice = ___itemForSale[val].price * (100 - percents[val]) / 100;
                if (___ytps >= newPrice)
                {
                    baseItemPrice = ___itemForSale[val].price;
                    ___itemForSale[val].price = newPrice;
                    setBasePriceOnPost = true;
                }
            }
        }

        [HarmonyPatch("BuyItem")]
        [HarmonyPostfix]
        private static void onBuyItemPost(StoreScreen __instance, int val, ref ItemObject[] ___itemForSale)
        {
            if (setBasePriceOnPost)
            {
                ___itemForSale[val].price = baseItemPrice;
                setBasePriceOnPost = false;
            }
        }

    }
}
