using BaldisBasicsPlusAdvanced.Cache.AssetsManagement;
using BaldisBasicsPlusAdvanced.Extensions;
using BaldisBasicsPlusAdvanced.Game.Systems.Controllers;
using System;
using UnityEngine;

namespace BaldisBasicsPlusAdvanced.Game.InventoryItems
{
    public class IceBootsItem : Item, IPrefab
    {
        [SerializeField]
        private AudioManager audioManager;

        public AudioManager AudMan => audioManager;

        public void InitializePrefab(int variant)
        {
            audioManager = gameObject.AddComponent<AudioManager>();
            audioManager.positional = false;
            audioManager.audioDevice = gameObject.AddComponent<AudioSource>();
        }

        public override bool Use(PlayerManager pm)
        {
            PlayerControllerSystem cs = pm.GetControllerSystem();

            bool created = cs.CreateController(out IceBootsController snowBootsController);
            snowBootsController.SetTime(15f);

            if (!created)
            {
                Destroy(gameObject);
                return false;
            }

            snowBootsController.RegisterOnDestroy(OnDestroyEvent, false);

            audioManager.QueueAudio(AssetsStorage.sounds["whoosh"]);

            return true;
        }

        private void OnDestroyEvent()
        {
            GameObject.Destroy(gameObject);
        }
    }
}
