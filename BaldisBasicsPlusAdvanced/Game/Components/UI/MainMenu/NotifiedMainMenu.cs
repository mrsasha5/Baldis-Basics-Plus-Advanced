﻿using BaldisBasicsPlusAdvanced.SaveSystem;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace BaldisBasicsPlusAdvanced.Game.Components.UI.MainMenu
{
    public class NotifiedMainMenu : MonoBehaviour
    {

        public Image notifImage;

        private void OnEnable()
        {
            if (notifImage != null && !OptionsDataManager.ExtraSettings.showNotif) {
                Destroy(notifImage.gameObject);
                Destroy(this);
            }
        }

    }
}
