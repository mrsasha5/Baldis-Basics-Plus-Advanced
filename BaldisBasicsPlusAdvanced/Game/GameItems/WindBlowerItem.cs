using BaldisBasicsPlusAdvanced.Cache;
using BaldisBasicsPlusAdvanced.Game.Components.Player;
using BaldisBasicsPlusAdvanced.Game.Systems.Controllers;
using BaldisBasicsPlusAdvanced.Helpers;
using BaldisBasicsPlusAdvanced.Patches;
using Rewired.Utils.Classes.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace BaldisBasicsPlusAdvanced.Game.GameItems
{
    public class WindBlowerItem : Item
    {

        private int windSize = 8;

        private float windSpeed = 20f; 

        public override bool Use(PlayerManager pm)
        {
            PlayerControllerSystem controllerSystem = pm.getControllerSystem();
            bool created = controllerSystem.createController(out PlayerWindController windController);
            if (!created)
            {
                Destroy(base.gameObject);
                return false;
            }
            windController.createWind(windSize, windSpeed);
            Destroy(gameObject);
            return true;
        }
    }
}
