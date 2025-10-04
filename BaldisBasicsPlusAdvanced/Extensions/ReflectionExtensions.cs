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
            catch (ReflectionTypeLoadException e) //This exception seems platform depend
            {
                types = Array.FindAll(e.Types, x => x != null);
            }

            return types;
        }

    }
}
