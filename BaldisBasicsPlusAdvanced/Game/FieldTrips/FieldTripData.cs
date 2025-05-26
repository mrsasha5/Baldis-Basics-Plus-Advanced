using BaldisBasicsPlusAdvanced.Game.FieldTrips.Managers;
using System.Linq;
using UnityEngine;

namespace BaldisBasicsPlusAdvanced.Game.FieldTrips
{
    public class FieldTripData
    {

        public string sceneName = "FieldTrip";

        public SceneObject sceneObject;

        public Material skyboxMaterial = Resources.FindObjectsOfTypeAll<Material>().Last(
            (Material s) => s.name == "Skybox" && s.shader == Shader.Find("Shader Graphs/Skybox"));

    }
}
