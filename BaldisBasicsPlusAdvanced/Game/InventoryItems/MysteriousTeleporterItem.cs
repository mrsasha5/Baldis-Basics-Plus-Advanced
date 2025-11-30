using BaldisBasicsPlusAdvanced.Cache;
using BaldisBasicsPlusAdvanced.Game.Objects.Projectiles;
using UnityEngine;

namespace BaldisBasicsPlusAdvanced.Game.InventoryItems
{
    public class MysteriousTeleporterItem : Item
    {

        private float maxDistance = 5f;

        public override bool Use(PlayerManager pm)
        {
            Physics.Raycast(pm.transform.position, CoreGameManager.Instance.GetCamera(pm.playerNumber).transform.forward, 
                out RaycastHit hit, float.PositiveInfinity, ~pm.gameObject.layer, QueryTriggerInteraction.Ignore);

            float distance = hit.distance > maxDistance ? maxDistance : hit.distance;

            Vector3 pos = pm.transform.position;
            Vector3 forward = Singleton<CoreGameManager>.Instance.GetCamera(pm.playerNumber).transform.forward;

            pos += forward * distance;
            MysteriousTeleporterProjectile projectile =
                Instantiate(ObjectStorage.Objects["mysterious_teleporter"].GetComponent<MysteriousTeleporterProjectile>());
            projectile.Initialize(pm.ec, pos, pm);
            projectile.SetFlying(true);
            projectile.transform.forward = forward;
            projectile.Entity.CopyStatusEffects(pm.plm.Entity);

            //ObjectsCreator.CreateProjectile<TeleporterProjectile>("Projectile", AssetsStorage.sprites["adv_mysterious_teleporter"], pos, pm.ec, forward, 1.5f);
            Destroy(gameObject);
            return true;
        }
    }
}
