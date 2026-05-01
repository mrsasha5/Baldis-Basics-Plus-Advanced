using System;
using UnityEngine;

namespace BaldisBasicsPlusAdvanced.Game.Activities
{
    // Added temporarily to restore compatibility with one mod
    [Obsolete("Please stop using that", true)]
    public class AdvancedMathMachine : MathMachine
    {
        [SerializeField]
        private int maxAnswer;

        public void GenerateProblem()
        {

        }
    }
}
