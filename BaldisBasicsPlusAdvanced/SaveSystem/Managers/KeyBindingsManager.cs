using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BaldisBasicsPlusAdvanced.SaveSystem.Data;
using UnityEngine;

namespace BaldisBasicsPlusAdvanced.SaveSystem.Managers
{
    public class KeyBindingsManager
    {
        private static string keyBindingsFile = "bindings.txt";

        private static Dictionary<string, KeyBindingData> keyBindings = new Dictionary<string, KeyBindingData>();

        public static Dictionary<string, KeyBindingData> Keys => keyBindings;

        public static void Load()
        {
            SetBindingDefaultValues();

            if (File.Exists(SaveSystemCore.Path + keyBindingsFile))
            {
                try
                {
                    string[] lines = File.ReadAllLines(SaveSystemCore.Path + keyBindingsFile);
                    foreach (string line in lines)
                    {
                        //0 - id
                        //1 - button
                        string[] values = line.Split(new char[] { ' ' });
                        if (keyBindings.ContainsKey(values[0]))
                        {
                            keyBindings[values[0]].OverrideButton((KeyCode)Enum.Parse(typeof(KeyCode), values[1]));
                        }
                    }
                }
                catch
                {
                    SetBindingDefaultValues();
                    RewriteBindings();
                }
            }
            else
            {
                RewriteBindings();
            }
        }

        public static void SetBindingDefaultValues()
        {
            PrepareKeyBinding("wind_blower_switch", "Adv_KeyBind_WindBlowerSwitch", "Adv_KeyBind_WindBlowerSwitch_Desc", KeyCode.B);
            PrepareKeyBinding("balloon_pop_action", "Adv_KeyBind_BalloonPopAction", "Adv_KeyBind_BalloonPopAction_Desc", KeyCode.P);
            //PrepareKeyBinding("update_mod", "Adv_KeyBind_UpdateMod", "Adv_KeyBind_UpdateMod_Desc", KeyCode.U);
            /*for (int i = 0; i < 10; i++)
            {
                PrepareKeyBinding("test" + i, "TEST" + i, "Test?", KeyCode.T);
            }*/
        }

        public static void RewriteBindings()
        {
            List<KeyBindingData> data = keyBindings.Values.ToList();
            string text = "";
            for (int i = 0; i < data.Count; i++)
            {
                text += $"{data[i].Id} {data[i].Button}\n";
            }
            File.WriteAllText(SaveSystemCore.Path + keyBindingsFile, text);
        }

        private static void PrepareKeyBinding(string id, string locName, string locDesc, KeyCode key)
        {
            if (!Keys.ContainsKey(id))
            {
                KeyBindingData data = new KeyBindingData(id, locName, locDesc, key);
                keyBindings.Add(id, data);
            }
            else
            {
                keyBindings[id].OverrideButton(key);
            }
        }

    }
}
