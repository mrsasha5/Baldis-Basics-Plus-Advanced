using BaldisBasicsPlusAdvanced.Cache;
using BaldisBasicsPlusAdvanced.Game.Objects.Plates.Base;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace BaldisBasicsPlusAdvanced.Game.Objects.Plates
{
    public class NoisyPlate : BaseCooldownPlate
    {
        private int generosityCount = 2;

        private float cooldown = 120f;

        private int points = 30;

        protected override void setValues(ref PlateData plateData)
        {
            base.setValues(ref plateData);
            plateData.targetPlayer = true;
        }

        protected override void setTextures()
        {
            setTexturesByBaseName("adv_noisy_plate");
            setEditorSprite("adv_editor_noisy_plate");
        }

        protected override void virtualOnPress()
        {
            base.virtualOnPress();
            audMan.PlaySingle(AssetsStorage.sounds["adv_emergency"]);
            EnvironmentController ec = Singleton<BaseGameManager>.Instance.Ec;
            ec.MakeNoise(transform.position, 127);
            if (generosityCount > 0) Singleton<CoreGameManager>.Instance.AddPoints(points, 0, true);
            generosityCount--;
            setCooldown(cooldown);
        }

    }
}
