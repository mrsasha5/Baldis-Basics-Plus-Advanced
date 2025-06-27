using BaldisBasicsPlusAdvanced.Cache.AssetsManagment;
using BaldisBasicsPlusAdvanced.Game.Systems.Controllers;
using BaldisBasicsPlusAdvanced.Patches;
using UnityEngine;

namespace BaldisBasicsPlusAdvanced.Game.InventoryItems
{
    public class WindBlowerItem : Item, IPrefab
    {
        [SerializeField]
        private int windSize;

        [SerializeField]
        private float windSpeed;

        public void InitializePrefab(int variant)
        {
            windSize = 8;
            windSpeed = 20f;
        }

        public override bool Use(PlayerManager pm)
        {
            PlayerControllerSystem controllerSystem = pm.GetControllerSystem();
            bool created = controllerSystem.CreateController(out PlayerWindController windController);
            if (!created)
            {
                pm.ec.GetAudMan().PlaySingle(AssetsStorage.sounds["error_maybe"]);
                Destroy(gameObject);
                return false;
            }
            windController.CreateWind(windSize, windSpeed);
            windController.SetTime(15f);
            Destroy(gameObject);
            return true;
        }
    }
}
