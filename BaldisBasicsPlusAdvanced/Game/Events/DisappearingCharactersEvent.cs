using BaldisBasicsPlusAdvanced.Cache;
using BaldisBasicsPlusAdvanced.Cache.AssetsManagement;
using BaldisBasicsPlusAdvanced.Extensions;
using BaldisBasicsPlusAdvanced.Helpers;
using MTM101BaldAPI.Registers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace BaldisBasicsPlusAdvanced.Game.Events
{
    public class DisappearingCharactersEvent : RandomEvent, IPrefab
    {
        private static int activeEvents = 0;

        private static List<ChalkFace> chalkFacesDisappeared;

        public static List<ChalkFace> ChalkFacesDisappeared => chalkFacesDisappeared;

        public static int ActiveEvents => activeEvents;

        public override void ResetConditions()
        {
            base.ResetConditions();
            activeEvents = 0;
            chalkFacesDisappeared.Clear();
        }

        public void InitializePrefab(int variant)
        {
            chalkFacesDisappeared = new List<ChalkFace>();
        }

        public override void Begin()
        {
            base.Begin();
            StartCoroutine(DisappearEveryone());
            activeEvents++;
        }

        private IEnumerator DisappearEveryone()
        {
            float cooldown = 3f;
            float effectTime = 7f;

            List<NPC> npcs = GetAvailableNPCs();

            float time = cooldown;
            ObjectsCreator.AddChalkCloudEffect(npcs[0], effectTime, ec);
            //addParticlesEffect(npcs[0], effectTime);
            while (npcs.Count > 0)
            {
                time -= Time.deltaTime * ec.EnvironmentTimeScale;
                if (time < 0)
                {
                    NPC npc = npcs[0];
                    if (npcs.Count > 1)
                    {
                        ObjectsCreator.AddChalkCloudEffect(npcs[1], effectTime, ec);
                        //addParticlesEffect(npcs[1], effectTime); //to the next npc
                    }
                    Disappear(npc, true);
                    npcs.Remove(npc);
                    time = cooldown;
                    if (npc is ChalkFace && !chalkFacesDisappeared.Contains(npc))
                    {
                        ChalkFace chalkFace = (ChalkFace)npc;
                        chalkFacesDisappeared.Add(chalkFace);
                        SpriteRenderer chalkRenderer = ReflectionHelper.GetValue<SpriteRenderer>(chalkFace, "chalkRenderer");
                        chalkRenderer.gameObject.SetActive(false);
                    }


                }
                yield return null;
            }
            yield break;
        }

        private List<NPC> GetAvailableNPCs()
        {
            List<NPC> npcs = ec.Npcs.FindAll(x => x.GetMeta() == null || 
                !x.GetMeta().tags.Contains(TagsStorage.disappearingCharactersEventImmunity));
            npcs.Mix();
            return npcs;
        }

        private void Disappear(NPC npc, bool hide)
        {
            npc.GetComponent<Entity>().SetVisible(!hide);

            //I leave this sound as unique part of the event
            AudioManager audMan = ObjectsCreator.CreatePropagatedAudMan(Vector3.zero, destroyWhenAudioEnds: true);
            audMan.transform.SetParent(npc.transform, false);
            
            if (hide)
            {
                audMan.PlaySingle(AssetsStorage.sounds["adv_disappearing"]);
            } else
            {
                audMan.PlaySingle(AssetsStorage.sounds["adv_appearing"]);
            }
        }

        public override void End()
        {
            base.End();
            if (activeEvents == 1) //for compability with chaos mode
            {
                StopAllCoroutines();
                foreach (NPC npc in ec.Npcs)
                {
                    if (npc is ChalkFace)
                    {
                        chalkFacesDisappeared.Remove((ChalkFace)npc);
                        ChalkFace chalkFace = (ChalkFace)npc;
                        if (!(chalkFace.state is ChalkFace_Idle))
                        {
                            Disappear(npc, false);
                        }
                    } else
                    {
                        Disappear(npc, false);
                    }
                    
                }
            }
            activeEvents--;
            
        }

    }
}
