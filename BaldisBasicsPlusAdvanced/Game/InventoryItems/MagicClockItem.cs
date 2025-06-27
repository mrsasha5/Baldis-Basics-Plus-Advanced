using BaldisBasicsPlusAdvanced.Cache;
using BaldisBasicsPlusAdvanced.Cache.AssetsManagment;
using BaldisBasicsPlusAdvanced.Game.Components;
using BaldisBasicsPlusAdvanced.Helpers;
using BaldisBasicsPlusAdvanced.Patches;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BaldisBasicsPlusAdvanced.Game.InventoryItems
{
    public class MagicClockItem : Item
    {
        [SerializeField]
        private float freezingTime = 15f;

        [SerializeField]
        private float unfreezingTime = 5f;

        [SerializeField]
        private float restoringWorldTime = 5f;

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

            HudGauge gauge = Singleton<CoreGameManager>.Instance.GetHud(0).gaugeManager
                            .ActivateNewGauge(ObjectsStorage.ItemObjects["MagicClock"].itemSpriteSmall, freezingTime + unfreezingTime);

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
                gauge?.SetValue(freezingTime + unfreezingTime, time + unfreezingTime);
                yield return null;
            }

            time = unfreezingTime;
            pm.ec.GetAudMan().PlaySingle(AssetsStorage.sounds["adv_time_starts"]);

            while (time > 0f)
            {
                time -= Time.deltaTime;
                gauge?.SetValue(freezingTime + unfreezingTime, time);
                yield return null;
            }

            PlayerInteractionController.Instance.SetIgnorePlayerEntitiesInteraction(false);

            /*for (int i = 0; i < affectedNPCs.Count; i++)
            {
                affectedNPCs[i]?.GetComponent<Entity>()?.SetInteractionState(true);
            }*/

            gauge?.Deactivate();

            time = restoringWorldTime;

            while (time > 0f)
            {
                time -= Time.deltaTime;

                timeScale.npcTimeScale = 1f - (time / restoringWorldTime);

                if (timeScale.npcTimeScale > 1f) timeScale.npcTimeScale = 1f;

                timeScale.environmentTimeScale = timeScale.npcTimeScale;

                yield return null;
            }

            pm.ec.RemoveTimeScale(timeScale);

            Destroy(gameObject);
        }
    }
}
