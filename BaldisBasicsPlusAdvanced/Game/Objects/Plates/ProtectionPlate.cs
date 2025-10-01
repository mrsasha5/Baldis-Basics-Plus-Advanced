using BaldisBasicsPlusAdvanced.Cache.AssetsManagement;
using BaldisBasicsPlusAdvanced.Extensions;
using BaldisBasicsPlusAdvanced.Game.Objects.Plates.Base;
using BaldisBasicsPlusAdvanced.Game.Systems.Controllers;
using UnityEngine;

namespace BaldisBasicsPlusAdvanced.Game.Objects.Plates
{
    public class ProtectionPlate : BasePlate
    {

        [SerializeField]
        private SoundObject audProtection;

        /*protected override void SetValues(PlateData plateData)
        {
            base.SetValues(plateData);
            plateData.hasLight = true;
            plateData.lightColor = Color.white;
        }*/

        public override void InitializePrefab(int variant)
        {
            base.InitializePrefab(variant);
            audProtection = AssetsStorage.sounds["adv_protection"];
        }

        protected override void SetValues(PlateData data)
        {
            base.SetValues(data);
            data.MarkAsCooldownPlate();
        }

        protected override void SetTextures()
        {
            base.SetTextures();
            SetTexturesByBaseName("adv_protection_plate");
            SetEditorSprite("adv_editor_protection_plate");
        }

        protected override void VirtualOnPress()
        {
            base.VirtualOnPress();
            if (entities[0] != null && entities[0].CompareTag("Player"))
            {
                PlayerControllerSystem controllerSystem = entities[0].GetComponent<PlayerControllerSystem>();

                controllerSystem.CreateController(out ShieldController controller);

                controller.SetTime(20f);

                ec.GetAudMan().PlaySingle(audProtection);
                SetCooldown(80f);
            }
        }

    }
}
