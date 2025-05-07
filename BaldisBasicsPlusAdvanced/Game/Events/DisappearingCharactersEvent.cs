using BaldisBasicsPlusAdvanced.Cache;
using BaldisBasicsPlusAdvanced.Enums;
using BaldisBasicsPlusAdvanced.Game.Components;
using BaldisBasicsPlusAdvanced.Helpers;
using BaldisBasicsPlusAdvanced.Patches;
using Mono.Security.Cryptography;
using MTM101BaldAPI;
using MTM101BaldAPI.Registers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.EnterpriseServices;
using System.Linq;
using System.Text;
using UnityEngine;

namespace BaldisBasicsPlusAdvanced.Game.Events
{
    public class DisappearingCharactersEvent : BaseRandomEvent
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

        public override void prepareData()
        {
            base.prepareData();
            descriptionKey = "EventAdv_DisappearingCharacters";
            chalkFacesDisappeared = new List<ChalkFace>();
        }

        public override void Begin()
        {
            base.Begin();
            StartCoroutine(disappearEveryone());
            activeEvents++;
        }


        private IEnumerator disappearEveryone()
        {
            float cooldown = 3f;
            float effectTime = 7f;

            List<NPC> npcs = getAvailableNPCs();

            float time = cooldown;
            ObjectsCreator.addChalkCloudEffect(npcs[0], effectTime, ec);
            //addParticlesEffect(npcs[0], effectTime);
            while (npcs.Count > 0)
            {
                time -= Time.deltaTime * ec.EnvironmentTimeScale;
                if (time < 0)
                {
                    NPC npc = npcs[0];
                    if (npcs.Count > 1)
                    {
                        ObjectsCreator.addChalkCloudEffect(npcs[1], effectTime, ec);
                        //addParticlesEffect(npcs[1], effectTime); //to the next npc
                    }
                    disappear(npc, true);
                    npcs.Remove(npc);
                    time = cooldown;
                    if (npc is ChalkFace && !chalkFacesDisappeared.Contains(npc))
                    {
                        chalkFacesDisappeared.Add((ChalkFace)npc);
                        SpriteRenderer chalkRenderer = ReflectionHelper.getValue<SpriteRenderer>(npc, "chalkRenderer");
                        chalkRenderer.gameObject.SetActive(false);
                    }


                }
                yield return null;
            }
            yield break;
        }

        private List<NPC> getAvailableNPCs()
        {
            List<NPC> npcs = ec.Npcs.FindAll(x => x.GetMeta() == null || !x.GetMeta().tags.Contains("adv_ev_disappearing_characters_immunity"));
            npcs.mix(); //new mixing method
            return npcs;
        }

        private void disappear(NPC npc, bool hide)
        {
            if (npc.spriteBase != null) // checking in case someone doesn't use it
            {
                npc.spriteBase.SetActive(!hide);
                PropagatedAudioManager audMan = npc.GetComponent<PropagatedAudioManager>();

                if (audMan != null)
                {
                    if (hide) {
                        audMan.PlaySingle(AssetsStorage.sounds["adv_disappearing"]);
                    } else
                    {
                        audMan.PlaySingle(AssetsStorage.sounds["adv_appearing"]);
                    }
                    
                }
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
                            disappear(npc, false);
                        }
                    } else
                    {
                        disappear(npc, false);
                    }
                    
                }
            }
            activeEvents--;
            
        }

    }
}
