using BaldisBasicsPlusAdvanced.Cache;
using BaldisBasicsPlusAdvanced.Game.Objects.Plates.Base;
using MTM101BaldAPI.Registers;
using System;
using System.Collections.Generic;
using System.Text;
using Unity.Mathematics;
using UnityEngine;

namespace BaldisBasicsPlusAdvanced.Game.Objects.Plates
{
    public class PresentPlate : BaseCooldownPlate
    {
        [SerializeField]
        private bool destroyPickupsOnSpawnItem;

        private List<Pickup> spawnedPickups = new List<Pickup>();

        protected override void setValues(ref PlateData plateData)
        {
            base.setValues(ref plateData);
            plateData.targetPlayer = true;
            plateData.showUses = true;
            plateData.setUses(2);
            destroyPickupsOnSpawnItem = true;
        }

        protected override void setTextures()
        {
            setTexturesByBaseName("adv_present_plate");
            setEditorSprite("adv_editor_present_plate");
        }

        protected override void virtualOnPress()
        {
            base.virtualOnPress();
            spawnRandomItem();
            setCooldown(180f);
        }

        private void spawnRandomItem()
        {
            if (destroyPickupsOnSpawnItem) {
                foreach (Pickup _pickup in spawnedPickups)
                {
                    Destroy(_pickup.gameObject);
                }
                spawnedPickups.Clear();
            }
            ItemMetaData[] metas = ItemMetaStorage.Instance.FindAll(x => x.id != Items.None);
            Pickup pickup = Instantiate(AssetsStorage.pickup, transform.position + Vector3.up * 5f, Quaternion.identity, transform);
            ItemObject item = metas[UnityEngine.Random.Range(0, metas.Length)].value;
            pickup.item = item;
            audMan.PlaySingle(AssetsStorage.sounds["bal_wow"]);
            spawnedPickups.Add(pickup);
        }

    }
}
