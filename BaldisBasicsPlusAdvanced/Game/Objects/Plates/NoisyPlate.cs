using BaldisBasicsPlusAdvanced.Cache;
using BaldisBasicsPlusAdvanced.Game.Objects.Plates.Base;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace BaldisBasicsPlusAdvanced.Game.Objects.Plates
{
    public class NoisyPlate : CooldownPlateBase
    {
        [SerializeField]
        private int generosityCount;

        [SerializeField]
        private float cooldown;

        [SerializeField]
        private int points;

        public void SetPointsReward(int count)
        {
            points = count;
        }

        public void SetGenerosityCount(int count)
        {
            generosityCount = count;
        }

        protected override void SetValues(ref PlateData plateData)
        {
            base.SetValues(ref plateData);
            plateData.targetPlayer = true;

            points = 0;
            generosityCount = 0;
            cooldown = 120f;
        }

        protected override void SetTextures()
        {
            SetTexturesByBaseName("adv_noisy_plate");
            SetEditorSprite("adv_editor_noisy_plate");
        }

        protected override void VirtualOnPress()
        {
            base.VirtualOnPress();
            audMan.PlaySingle(AssetsStorage.sounds["adv_emergency"]);
            EnvironmentController ec = Singleton<BaseGameManager>.Instance.Ec;
            ec.MakeNoise(transform.position, 127);
            if (generosityCount > 0)
            {
                Singleton<CoreGameManager>.Instance.AddPoints(points, 0, true);
                generosityCount--;
            }
            SetCooldown(cooldown);
        }

    }
}
