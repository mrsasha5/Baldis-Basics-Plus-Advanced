using BaldisBasicsPlusAdvanced.SavedData;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace BaldisBasicsPlusAdvanced.Game.Components.UI.Menu
{
    public class NotifiedMainMenu : MonoBehaviour
    {

        public Image notifImage;

        private void OnEnable()
        {
            if (notifImage != null && !DataManager.ExtraSettings.showNotif) {
                Destroy(notifImage.gameObject);
                Destroy(this);
            }
        }

    }
}
