using System;
using System.Collections.Generic;
using System.Text;

namespace BaldisBasicsPlusAdvanced.Extensions
{
    public static class PrimitiveTypeExtensions
    {

        public static bool ToBool(this int value)
        {
            return value > 0;
        }

        public static int ToInt(this bool value)
        {
            return value ? 1 : 0;
        }

    }
}
