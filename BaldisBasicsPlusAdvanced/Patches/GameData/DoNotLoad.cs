/*using BaldisBasicsPlusAdvanced.Cache;
using BaldisBasicsPlusAdvanced.Helpers;
using HarmonyLib;
using MTM101BaldAPI.UI;
using TMPro;
using UnityEngine;

namespace BaldisBasicsPlusAdvanced.Patches.GameData
{
    [HarmonyPatch(typeof(NameManager))]
    internal class DoNotLoad
    {
#warning TODO: REMOVE VERIFICATION ON RELEASE!!!
        [HarmonyPatch("LoadName")]
        [HarmonyPrefix]
        private static bool OnLoadName(NameManager __instance, int fileNo, ref string[] ___nameList)
        {
            if (!PlayerPrefs.HasKey("Advanced_FirstLoad") || 
                (PlayerPrefs.HasKey("Advanced_FirstLoad") && PlayerPrefs.GetString("Advanced_FirstLoad") == "OH NO!"))
            {
                string name = ___nameList[fileNo];
                for (int num = fileNo; num > 0; num--)
                {
                    ___nameList[num] = ___nameList[num - 1];
                }
                ___nameList[0] = name;

                TextMeshProUGUI text = ObjectsCreator.CauseCrash($"HEY <color=#ff0000>{name}!</color>" +
                    "\nDO NOT TELL ABOUT THAT TO ANYONE (except me of course)! IT'S IMPORTANT!" +
                    "\n<color=#ff0000>Code: " + Random.Range(0, int.MaxValue) + "</color=#ff0000>" +
                    "\nIt's a SECOND verification if my dear beta testers really want to help me with a mod!" +
                    "\nThis window WON'T APPEAR AGAIN BUT...! You SHOULD make screenshot (ALT + PrtScSysRq for win) and " +
                    "send it to me (PERSONALLY) to I could know that you have opened the game!" +
                    "\nIf you aren't sure how to make a screenshot, " +
                    "then... make a real photo of the monitor, lol!\nPress ALT + F4 to exit when you do screenshot!",
                    AssetsStorage.sounds["error_maybe"].soundClip);
                __instance.canvas.SetActive(false);
                text.font = BaldiFonts.ComicSans18.FontAsset();
                text.fontSize = BaldiFonts.ComicSans18.FontSize();

                PlayerPrefs.SetString("Advanced_FirstLoad", "99");
                PlayerPrefs.Save();

                return false;
            }
            
            return true;
        }
    }
}
*/