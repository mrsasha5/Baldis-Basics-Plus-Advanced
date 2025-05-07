using BaldisBasicsPlusAdvanced.Cache;
using BaldisBasicsPlusAdvanced.Game.Objects.Plates.Base;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace BaldisBasicsPlusAdvanced.Game.Objects.Plates
{
    public class SugarPlate : CooldownPlateBase
    {

        private List<ActivityModifier> activities = new List<ActivityModifier>();

        private List<MovementModifier> modifiers = new List<MovementModifier>();

        protected override void SetTextures()
        {
            SetTexturesByBaseName("adv_sugar_addiction_plate");
            SetEditorSprite("adv_editor_sugar_addiction_plate");
        }

        protected override void SetValues(ref PlateData plateData)
        {
            base.SetValues(ref plateData);
        }

        protected override void OnFirstTouch(Entity entity)
        {
            base.OnFirstTouch(entity);
            if (IsUsable())
            {
                MovementModifier moveMod = new MovementModifier(Vector3.zero, 1.80f);
                entity.ExternalActivity.moveMods.Add(moveMod);
                activities.Add(entity.ExternalActivity);
                modifiers.Add(moveMod);
                audMan.PlaySingle(AssetsStorage.sounds["adv_boost"]);
                StartCoroutine(StopEffectIn(10f));
                SetCooldown(70f);
            }
        }

        private IEnumerator StopEffectIn(float time)
        {
            while (time > 0)
            {
                time -= Time.deltaTime;
                yield return null;
            }
            activities[0].moveMods.Remove(modifiers[0]);
            activities.RemoveAt(0);
            modifiers.RemoveAt(0);
            yield break;
        }

    }
}
