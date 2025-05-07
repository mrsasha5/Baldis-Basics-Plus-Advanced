using BaldisBasicsPlusAdvanced.SavedData;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace BaldisBasicsPlusAdvanced.Compats.LevelEditor
{
    public class AuthenticLabelController : MonoBehaviour
    {

        public SpriteRenderer spriteRenderer;

        private void Awake()
        {
            spriteRenderer.enabled = false;
        }

        private void OnDestroy()
        {
            if (!DataManager.ExtraSettings.authenticMode && Singleton<PlayerFileManager>.Instance.authenticMode)
            {
                Singleton<PlayerFileManager>.Instance.authenticMode = false;
            }
        }

    }
}
