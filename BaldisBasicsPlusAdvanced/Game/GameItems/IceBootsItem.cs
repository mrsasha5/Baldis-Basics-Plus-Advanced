using BaldisBasicsPlusAdvanced.Cache;
using BaldisBasicsPlusAdvanced.Game.Components.Player;
using BaldisBasicsPlusAdvanced.Game.Systems.Controllers;
using BaldisBasicsPlusAdvanced.Patches;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace BaldisBasicsPlusAdvanced.Game.GameItems
{
    public class IceBootsItem : Item, IPrefab
    {
        [SerializeField]
        private AudioManager audioManager;

        public AudioManager AudMan => audioManager;

        public void initializePrefab()
        {
            audioManager = gameObject.AddComponent<AudioManager>();
            audioManager.positional = false;
            audioManager.audioDevice = gameObject.AddComponent<AudioSource>();
        }

        public override bool Use(PlayerManager pm)
        {
            PlayerControllerSystem cs = pm.getControllerSystem();

            bool created = cs.createController(out PlayerIceBootsController snowBootsController);
            snowBootsController.setTime(15f);

            if (!created)
            {
                Destroy(gameObject);
                return false;
            }

            snowBootsController.postInitialize(this);
            audioManager.QueueAudio(AssetsStorage.sounds["whoosh"]);

            return true;
        }
        
    }
}
