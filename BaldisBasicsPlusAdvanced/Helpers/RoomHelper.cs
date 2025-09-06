using System.Collections.Generic;
using UnityEngine;

namespace BaldisBasicsPlusAdvanced.Helpers
{
    public class RoomHelper
    {
        public static T SetupRoomFunction<T>(RoomFunctionContainer container) where T : RoomFunction, new()
        {
            T func = container.gameObject.AddComponent<T>();
            if (ReflectionHelper.GetValue<List<RoomFunction>>(container, "functions") == null)
                ReflectionHelper.SetValue<List<RoomFunction>>(container, "functions", new List<RoomFunction>());
            container.AddFunction(func);
            return func;
        }

        public static Material CreateMapMaterial(string name, Texture2D mapBg)
        {
            Material mapMaterial = new Material(AssetsHelper.LoadAsset<Material>("MapTile_Standard"));
            mapMaterial.SetTexture("_MapBackground", mapBg);
            mapMaterial.shaderKeywords = new string[] { "_KEYMAPSHOWBACKGROUND_ON" };
            mapMaterial.name = name;
            return mapMaterial;
        }

        public static List<IntVector2> CreateVector2Range(IntVector2 vector1, IntVector2 vector2)
        {
            int x1 = vector1.x;
            int z1 = vector1.z;
            List<IntVector2> vector2s = new List<IntVector2>();
            while (x1 < vector2.x)
            {
                while (z1 < vector2.z)
                {
                    vector2s.Add(new IntVector2(x1, z1));
                    z1++;
                }
                z1 = vector1.z;
                x1++;
            }
            return vector2s;
        }
    }
}
