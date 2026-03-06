using MTM101BaldAPI.Reflection;
using System;
using System.Reflection;

namespace BaldisBasicsPlusAdvanced.Extensions
{
    public static class ReflectionExtensions
    {
        public static Type[] TryGetAllTypes(this Assembly ass)
        {
            Type[] types;
            try { types = ass.GetTypes(); }
            catch (ReflectionTypeLoadException e)
            {
                types = Array.FindAll(e.Types, x => x != null);
            }
            return types;
        }

        // The main reason why I added it: my laziness.
        // I do not want each time to cast it, because it requires more actions when I need to use fields/methods from class I want.
        // this.ReflectionGetValue<Material>("correctMat").SetMainTexture(AssetStorage.textures["AMM_Front_Correct"]);
        // ((Material)this.ReflectionGetValue("correctMat")).SetMainTexture(AssetStorage.textures["AMM_Front_Correct"]);
        public static T ReflectionGetValue<T>(this object obj, string name)
        {
            return (T)obj.ReflectionGetVariable(name);
        }

        // It was made truly for staying familiar name lol.        
        public static void ReflectionSetValue(this object obj, string name, object value)
        {
            obj.ReflectionSetVariable(name, value);
        }

        // Same thing.
        public static object ReflectionGetValue(this object obj, string name)
        {
            return obj.ReflectionGetVariable(name);
        }

        public static T GetValue<T>(this FieldInfo fieldInfo, object obj)
        {
            return (T)fieldInfo.GetValue(obj);
        }
    }
}
