using BaldisBasicsPlusAdvanced.SaveSystem;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace BaldisBasicsPlusAdvanced.Game.Components.UI.MainMenu
{
    public class NotifiedExtraMenu : MonoBehaviour
    {
        public Image notifImage;

        private void OnDisable()
        {
            if (notifImage != null)
            {
                Destroy(notifImage.gameObject);
                Destroy(this);
                OptionsDataManager.ExtraSettings.showNotif = false; //invokes first than preservation
            }
        }


    }
}
