using System;
using System.Collections.Generic;
using System.Text;
using BaldisBasicsPlusAdvanced.Game.Objects.Plates.Base;
using BaldisBasicsPlusAdvanced.Helpers;

namespace BaldisBasicsPlusAdvanced.Game.Objects.Triggers
{
    public class UnpressTimePlateCurrentPositionTrigger : BaseTrigger
    {

        public override void InitializePrefab(int variant)
        {
            base.InitializePrefab(variant);
            CreateRenderer(AssetsHelper.SpriteFromFile(
                "Textures/Objects/Triggers/adv_trigger_low_plate_unpress_time.png", 25f));
        }

        protected override void OnEnvBeginPlay()
        {
            base.OnEnvBeginPlay();
            BaseCooldownPlate[] plates = FindObjectsOfType<BaseCooldownPlate>();

            for (int i = 0; i < plates.Length; i++)
            {
                if (plates[i].transform.position.x == transform.position.x &&
                    plates[i].transform.position.z == transform.position.z)
                {
                    plates[i].Data.timeToUnpress = 0.25f;
                }
            }

        }

    }
}
