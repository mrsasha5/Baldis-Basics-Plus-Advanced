using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace BaldisBasicsPlusAdvanced.Helpers
{
    class ReflectionHelper
    {
        public static object useMethod(object instance, string methodName, params object[] parameters)
        {
            return Traverse.Create(instance).Method(methodName, parameters);//.GetValue();
        }
        public static T getValue<T>(object instance, string fieldName)
        {
            return Traverse.Create(instance).Field(fieldName).GetValue<T>();
        }

        /// <summary>
        /// For static fields!
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        public static J getValue<T, J>(string fieldName)
        {
            return Traverse.Create<T>().Field(fieldName).GetValue<J>();
        }

        public static void getValue<T>(object instance, string fieldName, out T result)
        {
            result = Traverse.Create(instance).Field(fieldName).GetValue<T>();
        }

        /// <summary>
        /// For static fields!
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="instance"></param>
        /// <param name="fieldName"></param>
        /// <param name="result"></param>
        public static void getValue<T, J>(string fieldName, out J result)
        {
            result = Traverse.Create<T>().Field(fieldName).GetValue<J>();
        }

        public static void setValue<T>(object instance, string fieldName, Object setVal)
        {
            Traverse.Create(instance).Field(fieldName).SetValue(setVal);
        }
    }
}
