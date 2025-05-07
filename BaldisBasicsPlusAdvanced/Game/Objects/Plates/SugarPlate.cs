using BaldisBasicsPlusAdvanced.Cache;
using BaldisBasicsPlusAdvanced.Game.Objects.Plates.Base;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace BaldisBasicsPlusAdvanced.Game.Objects.Plates
{
    public class SugarPlate : BaseCooldownPlate
    {

        private List<ActivityModifier> activities = new List<ActivityModifier>();

        private List<MovementModifier> modifiers = new List<MovementModifier>();

        protected override void setTextures()
        {
            setTexturesByBaseName("adv_sugar_addiction_plate");
            setEditorSprite("adv_editor_sugar_addiction_plate");
        }

        protected override void setValues(ref PlateData plateData)
        {
            base.setValues(ref plateData);
        }

        protected override void onFirstTouch(Entity entity)
        {
            base.onFirstTouch(entity);
            if (isUsable())
            {
                MovementModifier moveMod = new MovementModifier(Vector3.zero, 1.50f);
                entity.ExternalActivity.moveMods.Add(moveMod);
                activities.Add(entity.ExternalActivity);
                modifiers.Add(moveMod);
                audMan.PlaySingle(AssetsStorage.sounds["adv_boost"]);
                StartCoroutine(stopEffectIn(10f));
                setCooldown(60f);
            }
        }

        private IEnumerator stopEffectIn(float time)
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
