using System;
using UnityEngine;

namespace BaldisBasicsPlusAdvanced.Game.FieldTrips.SpecialTrips
{
    public class FieldTripData
    {

        public string sceneName = "FieldTrip";

        public SceneObject sceneObject;

        public Material skyboxMaterial;

        public void SetSkybox(Texture2D texture)
        {
            skyboxMaterial = new Material(Shader.Find("Shader Graphs/Skybox"));
            skyboxMaterial.mainTexture = texture;
        }

        public FieldTripData SetDefaultSkybox()
        {
            skyboxMaterial = Array.Find(Resources.FindObjectsOfTypeAll<Material>(),
                s => s.name == "Skybox" && s.shader == Shader.Find("Shader Graphs/Skybox"));
            return this;
        }

        public FieldTripData Register()
        {
            SpecialTripsRegistryManager.Add(this);
            return this;
        }

    }
}
