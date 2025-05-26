using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

namespace BaldisBasicsPlusAdvanced.Helpers
{
    public class MathHelpher
    {

        public static int FindNearestValue(int[] values, int value)
        {
            int difference = Mathf.Abs(values[0] - value);
            int nearest = values[0];

            for (int i = 0; i < values.Length; i++)
            {
                if (Mathf.Abs(values[i] - value) < difference)
                {
                    difference = Mathf.Abs(values[i] - value);
                    nearest = values[i];
                }
            }
            return nearest;
        }

        public static int FindMaxValue(int[] values)
        {
            int val = values[0];
            for (int i = 0; i < values.Length; i++)
            {
                if (values[i] > val)
                {
                    val = values[i];
                }
            }
            return val;
        }

        public static int FindMaxValue(List<int> values)
        {
            int val = values[0];
            for (int i = 0; i < values.Count; i++)
            {
                if (values[i] > val)
                {
                    val = values[i];
                }
            }
            return val;
        }

        public static float FindMaxValue(float[] values)
        {
            float val = values[0];
            for (int i = 0; i < values.Length; i++)
            {
                if (values[i] > val)
                {
                    val = values[i];
                }
            }
            return val;
        }

        public static float FindMaxValue(List<float> values)
        {
            float val = values[0];
            for (int i = 0; i < values.Count; i++)
            {
                if (values[i] > val)
                {
                    val = values[i];
                }
            }
            return val;
        }

    }
}
