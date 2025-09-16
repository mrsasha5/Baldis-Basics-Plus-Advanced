using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;

namespace BaldisBasicsPlusAdvanced.Helpers
{
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

        public static object GetValue(object instance, string fieldName)
        {
            return Traverse.Create(instance).Field(fieldName).GetValue();
        }

        public static T GetValue<T>(object instance, string fieldName)
        {
            return Traverse.Create(instance).Field(fieldName).GetValue<T>();
        }

        public static void GetValue<T>(object instance, string fieldName, out T result)
        {
            result = Traverse.Create(instance).Field(fieldName).GetValue<T>();
        }

        public static void SetValue<T>(object instance, string fieldName, T setVal)
        {
            Traverse.Create(instance).Field(fieldName).SetValue(setVal);
        }
        
        public static void Static_SetValue<T>(string fieldName, object setVal)
        {
            Traverse.Create<T>().Field(fieldName).SetValue(setVal);
        }
        
        public static J Static_GetValue<T, J>(string fieldName)
        {
            return Traverse.Create<T>().Field(fieldName).GetValue<J>();
        }

        public static void Static_GetValue<T, J>(string fieldName, out J result)
        {
            result = Traverse.Create<T>().Field(fieldName).GetValue<J>();
        }


    }
}
