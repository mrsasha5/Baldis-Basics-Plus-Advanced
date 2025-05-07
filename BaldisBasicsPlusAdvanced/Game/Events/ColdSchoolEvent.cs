using BaldisBasicsPlusAdvanced.Cache;
using BaldisBasicsPlusAdvanced.Enums;
using BaldisBasicsPlusAdvanced.Game.Components.NPCs;
using BaldisBasicsPlusAdvanced.Game.Systems.Controllers;
using BaldisBasicsPlusAdvanced.GameEventSystem;
using BaldisBasicsPlusAdvanced.Helpers;
using BaldisBasicsPlusAdvanced.Patches;
using MTM101BaldAPI;
using MTM101BaldAPI.Registers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace BaldisBasicsPlusAdvanced.Game.Events
{
    public class ColdSchoolEvent : EventableRandomEvent
    {

        private static int activeEvents;

        public static int ActiveEvents => activeEvents;

        private Fog fog = new Fog();

        private Color fogColor = new Color(0f, 0f, 1f, 1f);

        private AudioManager audMan;

        private MovementModifier npcMoveMod = new MovementModifier(Vector3.zero, 0.25f);

        private MovementModifier playerMod = new MovementModifier(Vector3.zero, 0.25f);

        private float minMultiplier = 0.25f;

        private float maxMultiplier = 1f;

        private Canvas canvasFrozen;

        private Image frozenOverlay;

        private PlayerManager pm;

        private float cooldownTime;

        private float transparency;

        private Dictionary<NPC, BaseController> controlledNpcs = new Dictionary<NPC, BaseController>();

        private Coroutine playerControl;

        public override void ResetConditions()
        {
            base.ResetConditions();
            activeEvents = 0;
        }

        public override void prepareData()
        {
            descriptionKey = "EventAdv_ColdSchool";

            audMan = gameObject.AddComponent<AudioManager>();
            audMan.audioDevice = gameObject.AddComponent<AudioSource>();
            canvasFrozen = Instantiate(ObjectsStorage.Overlays["FrozenOverlay"]);
            frozenOverlay = canvasFrozen.GetComponentInChildren<Image>();
            frozenOverlay.color = new Color(1f, 1f, 1f, 0f);
            //canvasFrozen.gameObject.SetActive(false); //it's automatically disabled
        }

        public override void Begin()
        {
            if (activeEvents < 1)
            {
                pm = Singleton<CoreGameManager>.Instance.GetPlayer(0);
                cancelEventTextOnBegin = true;
                base.Begin();
                StartCoroutine(FadeOnFog());
                audMan.PlaySingle(AssetsStorage.sounds["adv_mysterious_machine"]);
                canvasFrozen.gameObject.SetActive(true);
                canvasFrozen.worldCamera = Singleton<CoreGameManager>.Instance.GetCamera(0).canvasCam;
            } else
            {
                base.Begin();
                setEventText("EventAdvanced_ColdSchool_onMoreEvents");
                audMan.PlaySingle(AssetsStorage.sounds["adv_mysterious_machine"]);
            }
            activeEvents++;
        }

        public override void End()
        {
            base.End();
            if (activeEvents < 2)
            {
                StartCoroutine(FadeOffFog());
                StopCoroutine(playerControl);
            }
            activeEvents--;
        }

        private void addMoveModifiers()
        {
            pm.Am.moveMods.Add(playerMod);
            List<NPC> npcs = getAvailableNPCs();
            foreach (NPC npc in npcs)
            {
                freeze(npc);
            }
        }

        private void removeMoveModifiers()
        {
            pm.Am.moveMods.Remove(playerMod);
            List<NPC> npcs = getAvailableNPCs();
            foreach (NPC npc in npcs)
            {
                unfreeze(npc);
            }
        }

        private List<NPC> getAvailableNPCs()
        {
            List<NPC> npcs = ec.Npcs.FindAll(x => x.GetMeta() == null || !x.GetMeta().tags.Contains("adv_ev_cold_school_immunity"));
            npcs.mix();
            return npcs;
        }

        private void freeze(NPC npc)
        {
            if (controlledNpcs.ContainsKey(npc)) return;
            NPCControllerSystem controllerSystem = npc.getControllerSystem();
            controllerSystem.createController(out FrozennessController frozennessController);
            frozennessController.setInfinityLivingTime();
            controlledNpcs.Add(npc, frozennessController);
        }

        private void unfreeze(NPC npc)
        {
            if (!controlledNpcs.ContainsKey(npc)) return;
            controlledNpcs[npc].setToDestroy();
        }

        private IEnumerator playerController()
        {
            while (true)
            {
                if (cooldownTime > 0 && ec != null) cooldownTime -= Time.deltaTime * ec.EnvironmentTimeScale;

                bool isRunning = ReflectionHelper.getValue<bool>(pm.plm, "running");
                if (playerMod.movementMultiplier < maxMultiplier && isRunning)
                {
                    playerMod.movementMultiplier += 0.06f * Time.deltaTime;
                    setCooldown(2f);
                }
                if (playerMod.movementMultiplier > minMultiplier && !isRunning && cooldownTime < 0)
                {
                    playerMod.movementMultiplier -= 0.02f * Time.deltaTime;
                }
                transparency = (maxMultiplier - playerMod.movementMultiplier) / maxMultiplier;
                frozenOverlay.color = new Color(1f, 1f, 1f, transparency);
                yield return null;
            }
            
        }
        private IEnumerator FadeOnFog()
        {
            ec.AddFog(fog);
            fog.color = fogColor;
            fog.startDist = 5f;
            fog.maxDist = 250f;
            fog.strength = 0f;
            float fogStrength2 = 0f;
            while (fogStrength2 < 1f)
            {
                fogStrength2 += 0.25f * Time.deltaTime;
                fog.strength = fogStrength2;
                ec.UpdateFog();
                frozenOverlay.color = new Color(1f, 1f, 1f, fogStrength2);
                yield return null;
            }

            fogStrength2 = 1f;
            fog.strength = fogStrength2;
            ec.UpdateFog();

            playerControl = StartCoroutine(playerController());
            frozenOverlay.color = new Color(1f, 1f, 1f, fogStrength2);
            if (activeEvents < 2) setEventText(descriptionKey);

            addMoveModifiers();
            playerMod.movementMultiplier = minMultiplier;
            audMan.PlaySingle(AssetsStorage.sounds["adv_frozen"]);
        }

        private IEnumerator FadeOffFog()
        {
            float fogStrength2 = 1f;
            fog.strength = fogStrength2;
            ec.UpdateFog();
            while (fogStrength2 > 0f)
            {
                fogStrength2 -= 0.25f * Time.deltaTime;
                fog.strength = fogStrength2;
                ec.UpdateFog();
                if (fogStrength2 < transparency) {
                    frozenOverlay.color = new Color(1f, 1f, 1f, fogStrength2);
                }
                yield return null;
            }

            fogStrength2 = 0f;
            fog.strength = fogStrength2;
            ec.UpdateFog();
            ec.RemoveFog(fog);
            frozenOverlay.color = new Color(1f, 1f, 1f, fogStrength2);

            removeMoveModifiers();
        }

        public override void onNotebookClaim()
        {
            playerMod.movementMultiplier = maxMultiplier;
            setCooldown(7f);
        }

        private void setCooldown(float cooldownTime)
        {
            if (this.cooldownTime < cooldownTime)
            {
                this.cooldownTime = cooldownTime;
            }
        }
    }
}
