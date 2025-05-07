using HarmonyLib;
using System;
using System.Reflection;

namespace BaldisBasicsPlusAdvanced.Helpers
{
    public class ReflectionHelper
    {
        public static object UseRequiredMethod(object instance, string methodName, params object[] parameters)
        {
            return Traverse.Create(instance).Method(methodName, parameters).GetValue();
        }

        public static object UseMethod(object instance, string methodName, params object[] parameters)
        {
            if (instance.GetType().GetMethod(methodName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance) != null) //to avoid logs
            {
                return Traverse.Create(instance).Method(methodName, parameters).GetValue();
            }
            return null;
        }

        /*public static T UseMethod<T>(object instance, string methodName, params object[] parameters)
        {
            return Traverse.Create(instance).Method(methodName, parameters).GetValue<T>();
        }*/

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

        // For static fields!
        public static J GetValue<T, J>(string fieldName)
        {
            return Traverse.Create<T>().Field(fieldName).GetValue<J>();
        }

        // For static fields!
        public static void GetValue<T, J>(string fieldName, out J result)
        {
            result = Traverse.Create<T>().Field(fieldName).GetValue<J>();
        }


    }
}
