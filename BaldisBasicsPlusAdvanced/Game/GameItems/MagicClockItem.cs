using BaldisBasicsPlusAdvanced.Cache;
using BaldisBasicsPlusAdvanced.Game.Components;
using BaldisBasicsPlusAdvanced.Helpers;
using BaldisBasicsPlusAdvanced.Patches;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BaldisBasicsPlusAdvanced.Game.GameItems
{
    public class MagicClockItem : Item
    {
        [SerializeField]
        private float freezingTime = 15f;

        [SerializeField]
        private float unfreezingTime = 5f;

        public override bool Use(PlayerManager pm)
        {
            StartCoroutine(TimeController(pm));
            return true;
        }

        private void Destroy()
        {
            PlayerInteractionController.Instance.SetIgnorePlayerEntitiesInteraction(false);
        }

        private IEnumerator TimeController(PlayerManager pm)
        {
            TimeScaleModifier timeScale = new TimeScaleModifier();
            timeScale.environmentTimeScale = 1f;
            timeScale.npcTimeScale = 1f;

            List<NPC> affectedNPCs = new List<NPC>(pm.ec.Npcs);

            PlayerInteractionController.Instance.SetIgnorePlayerEntitiesInteraction(true);

            /*for (int i = 0; i < affectedNPCs.Count; i++)
            {
                affectedNPCs[i]?.GetComponent<Entity>()?.SetInteractionState(false);
            }*/

            pm.ec.AddTimeScale(timeScale);

            pm.ec.GetAudMan().PlaySingle(AssetsStorage.sounds["adv_time_stops"]);

            float val = 1f;
            float time = freezingTime;
            while (time > 0f)
            {
                if (val > 0f)
                {
                    val -= Time.deltaTime / 2f;
                    if (val < 0f) val = 0f;
                    timeScale.environmentTimeScale = val;
                    timeScale.npcTimeScale = val;
                }
                time -= Time.deltaTime;
                yield return null;
            }

            time = unfreezingTime;
            pm.ec.GetAudMan().PlaySingle(AssetsStorage.sounds["adv_time_starts"]);

            while (time > 0f)
            {
                time -= Time.deltaTime;
                yield return null;
            }

            PlayerInteractionController.Instance.SetIgnorePlayerEntitiesInteraction(false);

            /*for (int i = 0; i < affectedNPCs.Count; i++)
            {
                affectedNPCs[i]?.GetComponent<Entity>()?.SetInteractionState(true);
            }*/

            pm.ec.RemoveTimeScale(timeScale);

            Destroy(gameObject);

            yield break;
        }
    }
}
