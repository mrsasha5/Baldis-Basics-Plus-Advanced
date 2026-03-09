using HarmonyLib;
using System.Reflection;

namespace BaldisBasicsPlusAdvanced.Helpers
{
    // Planned to be destroyed completely since this terrible solution was made by me for weird reason
    internal class ReflectionHelper
    {
        public static object UseMethod(object instance, string methodName, params object[] parameters)
        {
            return Traverse.Create(instance).Method(methodName, parameters).GetValue();
        }

        public static object NoCache_UseMethod(object instance, string methodName, params object[] parameters)
        {
            MethodInfo method = instance.GetType().GetMethod(methodName, AccessTools.all);
            if (method != null)
            {
                method.Invoke(instance, parameters);
            }
            return null;
        }

        public static T GetValue<T>(object instance, string fieldName)
        {
            return Traverse.Create(instance).Field(fieldName).GetValue<T>();
        }

        public static void SetValue<T>(object instance, string fieldName, T setVal)
        {
            Traverse.Create(instance).Field(fieldName).SetValue(setVal);
        }
        
        public static void Static_SetValue<T>(string fieldName, object setVal)
        {
            Traverse.Create<T>().Field(fieldName).SetValue(setVal);
        }
    }
}
