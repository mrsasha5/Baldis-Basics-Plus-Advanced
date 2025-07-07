using System;
using BaldisBasicsPlusAdvanced.Helpers;
using UnityEngine;

namespace BaldisBasicsPlusAdvanced.Game.Components.UI.MainMenu
{
    public class UpdatesCenterMenu : MonoBehaviour, IPrefab
    {
        public void InitializePrefab(int variant)
        {
            ObjectsCreator.CreateCanvas(gameObject, setGlobalCam: true);
        }
    }
}
