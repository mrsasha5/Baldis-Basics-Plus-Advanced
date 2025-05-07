using BaldisBasicsPlusAdvanced.Cache;
using BaldisBasicsPlusAdvanced.Game.Objects.Plates.Base;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BaldisBasicsPlusAdvanced.Game.Objects.Plates
{
    public class SlowdownPlate : CooldownPlateBase
    {
        private List<ActivityModifier> activities = new List<ActivityModifier>();

        private List<MovementModifier> modifiers = new List<MovementModifier>();

        protected override void SetTextures()
        {
            SetTexturesByBaseName("adv_slowdown_plate");
            SetEditorSprite("adv_editor_slowdown_plate");
        }

        protected override void VirtualOnPress()
        {
            base.VirtualOnPress();
            Entity entity = entities[0];
            MovementModifier moveMod = new MovementModifier(Vector3.zero, 0.25f);
            entity.ExternalActivity.moveMods.Add(moveMod);
            activities.Add(entity.ExternalActivity);
            modifiers.Add(moveMod);
            audMan.PlaySingle(AssetsStorage.sounds["adv_metal_blow"]);
            StartCoroutine(StopEffectIn(10f));
            SetCooldown(60f);
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
