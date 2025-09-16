using System;
using System.Collections.Generic;
using System.Reflection;

namespace BaldisBasicsPlusAdvanced.Extensions
{
    public static class ReflectionExtensions
    {

        public static Type[] TryGetAllTypes(this Assembly ass)
        {
            Type[] types;
            try { types = ass.GetTypes(); }
            catch (ReflectionTypeLoadException e) { types = e.Types; } //This exception seems platform depend

            return types;
        }

    }
}
