using BepInEx;
using BepInEx.Bootstrap;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace BaldisBasicsPlusAdvanced.API
{
    internal class ReflectionEventManager
    {
        private static object[] listeners;

        public static Type[] forbiddenButtonReceivers = new Type[] { typeof(PowerLeverController) };

        public static IEnumerator Init()
        {
            IEnumerator receiversEnumerator = InitListeners();
            receiversEnumerator.MoveNext();
            yield return receiversEnumerator.Current;

            while (receiversEnumerator.MoveNext())
            {
                yield return receiversEnumerator.Current;
            }
        }

        private static IEnumerator InitListeners()
        {
            yield return Chainloader.PluginInfos.Values.Count;
            int counter = 1;
            List<object> _listeners = new List<object>();
            foreach (PluginInfo info in Chainloader.PluginInfos.Values)
            {
                yield return $"Looking for event listeners ({counter}/{Chainloader.PluginInfos.Values.Count})...";
                try
                {
                    MethodInfo _getListenerMethod = info.GetType().GetMethod("AdvancedEdition_GetEventListener",
                        BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, new Type[0], null);
                    if (_getListenerMethod == null) continue;

                    object listener = _getListenerMethod.Invoke(info.Instance, null);
                    if (listener != null)
                    {
                        _listeners.Add(listener);
                    }
                }
                catch { }
                counter++;
            }
            listeners = _listeners.ToArray();
        }

        public static void InvokePreloadingEvents()
        {
            InitForbiddenButtonAcceptors();
        }

        private static void InitForbiddenButtonAcceptors()
        {
            List<Type> receivers = new List<Type>(forbiddenButtonReceivers);
            BindingFlags defFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
            
            for (int i = 0; i < listeners.Length; i++)
            {
                try
                {
                    MethodInfo _method = listeners[i].GetType().GetMethod("GetForbiddenButtonReceivers", defFlags, null, new Type[0], null);
                    if (_method == null) continue;

                    object returnedInstance = _method.Invoke(listeners[i], null);
                    if (returnedInstance is Type[] forbiddenReceivers)
                    {
                        for (int j = 0; j < forbiddenReceivers.Length; j++)
                        {
                            receivers.Add(forbiddenReceivers[i]);
                        }
                    }
                }
                catch { }
            }
            forbiddenButtonReceivers = receivers.ToArray();
        }
    }
}
