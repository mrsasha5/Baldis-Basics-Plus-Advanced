using BaldisBasicsPlusAdvanced.Cache;
using BaldisBasicsPlusAdvanced.Cache.AssetsManagement;
using BaldisBasicsPlusAdvanced.Game.Objects.Plates.Base;
using MTM101BaldAPI.Registers;
using System.Collections.Generic;
using UnityEngine;

namespace BaldisBasicsPlusAdvanced.Game.Objects.Plates
{
    public class PresentPlate : BasePlate
    {
        [SerializeField]
        private bool destroyPickupsOnSpawnItem;

        private List<Pickup> spawnedPickups = new List<Pickup>();

        protected override void SetValues(PlateData plateData)
        {
            base.SetValues(plateData);
            //plateData.hasLight = true;
            //plateData.lightColor = Color.blue;
            plateData.SetUses(1);
            destroyPickupsOnSpawnItem = true;
        }

        protected override void SetTextures()
        {
            SetTexturesByBaseName("adv_present_plate");
        }

        protected override void VirtualOnPress()
        {
            base.VirtualOnPress();
            SpawnRandomItem();
        }

        private void SpawnRandomItem()
        {
            if (destroyPickupsOnSpawnItem) {
                foreach (Pickup _pickup in spawnedPickups)
                {
                    Destroy(_pickup.gameObject);
                }
                spawnedPickups.Clear();
            }

            ItemMetaData[] metas = ItemMetaStorage.Instance.FindAll(x => x.id != Items.None && x.flags != ItemFlags.InstantUse && 
                !x.tags.Contains(TagsStorage.forbiddenPresent) && !x.tags.Contains(TagsStorage.narrowlyFunctional));

            if (metas.Length > 0)
            {
                Pickup pickup = Instantiate(AssetsStorage.pickup, transform.position + Vector3.up * 5f, Quaternion.identity, transform);

                ItemObject item = metas[UnityEngine.Random.Range(0, metas.Length)].value;
                pickup.item = item;
                audMan.PlaySingle(AssetsStorage.sounds["bal_wow"]);
                spawnedPickups.Add(pickup);
            }
            
        }

        protected override bool IsPressable(Entity target)
        {
            return base.IsPressable(target) && target.CompareTag("Player");
        }

    }
}
