using UnityEngine;

namespace BaldisBasicsPlusAdvanced.Helpers
{
    public class ControlledRNG
    {

        public static bool GenAvailable => GameObject.FindObjectOfType<LevelGenerator>() != null;

        public static System.Random Object => GameObject.FindObjectOfType<LevelGenerator>().controlledRNG;

    }
}
