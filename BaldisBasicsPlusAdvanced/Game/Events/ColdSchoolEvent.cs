using BaldisBasicsPlusAdvanced.Cache;
using BaldisBasicsPlusAdvanced.Cache.AssetsManagement;
using BaldisBasicsPlusAdvanced.Extensions;
using BaldisBasicsPlusAdvanced.Game.Systems.Controllers;
using BaldisBasicsPlusAdvanced.Helpers;
using MTM101BaldAPI.Registers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BaldisBasicsPlusAdvanced.Game.Events
{
    public class ColdSchoolEvent : RandomEvent, IPrefab
    {

        private static int activeEvents;

        public static int ActiveEvents => activeEvents;

        private Canvas canvasFrozen;

        [SerializeField]
        private AudioManager audMan;

        [SerializeField]
        private float maxRaycast;

        private Fog fog = new Fog();

        private Color fogColor = new Color(0f, 0f, 1f, 1f);

        private MovementModifier moveMod = new MovementModifier(Vector3.zero, 0.25f);

        private float minMultiplier = 0.25f;

        private float maxMultiplier = 1f;

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

        public void InitializePrefab(int variant)
        {
            audMan = gameObject.AddComponent<AudioManager>();
            audMan.audioDevice = gameObject.AddComponent<AudioSource>();
            maxRaycast = 62.5f;
        }

        private IEnumerator PlayMusicIn(float seconds)
        {
            while (seconds > 0f)
            {
                seconds -= Time.deltaTime;
                yield return null;
            }

            audMan.PlaySingle(AssetsStorage.sounds["creepy_old_computer"]);

            yield break;
        }

        public override void Begin()
        {
            if (canvasFrozen == null)
            {
                canvasFrozen = Instantiate(ObjectsStorage.Overlays["FrozenOverlay"]);
                frozenOverlay = canvasFrozen.GetComponentInChildren<Image>();
                frozenOverlay.color = new Color(1f, 1f, 1f, 0f);
            }
            if (activeEvents < 1)
            {
                pm = Singleton<CoreGameManager>.Instance.GetPlayer(0);
                //cancelEventTextOnBegin = true;
                base.Begin();
                StartCoroutine(FadeOnFog());
                audMan.PlaySingle(AssetsStorage.sounds["adv_mysterious_machine"]);
                StartCoroutine(PlayMusicIn(4f));
                canvasFrozen.gameObject.SetActive(true);
                canvasFrozen.worldCamera = Singleton<CoreGameManager>.Instance.GetCamera(0).canvasCam;
            } else
            {
                base.Begin();
                audMan.PlaySingle(AssetsStorage.sounds["adv_mysterious_machine"]);
            }
            ec.MaxRaycast = maxRaycast;
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
            ec.MaxRaycast = float.PositiveInfinity;
            activeEvents--;
        }

        private void AddMoveModifiers()
        {
            pm.Am.moveMods.Add(moveMod);
            List<NPC> npcs = GetAvailableNPCs();
            foreach (NPC npc in npcs)
            {
                Freeze(npc);
            }
        }

        private void RemoveMoveModifiers()
        {
            pm.Am.moveMods.Remove(moveMod);
            List<NPC> npcs = GetAvailableNPCs();
            foreach (NPC npc in npcs)
            {
                Unfreeze(npc);
            }
        }

        private List<NPC> GetAvailableNPCs()
        {
            List<NPC> npcs = ec.Npcs.FindAll(x => x.TryGetComponent(out Entity _) &&
                (x.GetMeta() == null || !x.GetMeta().tags.Contains(TagsStorage.coldSchoolEventImmunity)));
            npcs.Mix();
            return npcs;
        }

        private void Freeze(NPC npc)
        {
            if (controlledNpcs.ContainsKey(npc)) return;

            NPCControllerSystem controllerSystem = npc.GetControllerSystem();
            controllerSystem.CreateController(out FrozennessController frozennessController);
            frozennessController.SetInfinityLivingTime();
            controlledNpcs.Add(npc, frozennessController);
        }

        private void Unfreeze(NPC npc)
        {
            if (!controlledNpcs.ContainsKey(npc)) return;
            controlledNpcs[npc].SetToDestroy();
        }

        private IEnumerator PlayerController()
        {
            while (true)
            {
                if (cooldownTime > 0 && ec != null) cooldownTime -= Time.deltaTime * ec.EnvironmentTimeScale;

                if (moveMod.movementMultiplier < maxMultiplier && ReflectionHelper.GetValue<bool>(pm.plm, "running"))
                {
                    moveMod.movementMultiplier += 0.06f * Time.deltaTime;
                    SetCooldown(2f);
                }
                if (moveMod.movementMultiplier > minMultiplier && !ReflectionHelper.GetValue<bool>(pm.plm, "running") && cooldownTime < 0)
                {
                    moveMod.movementMultiplier -= 0.02f * Time.deltaTime;
                }
                transparency = (maxMultiplier - moveMod.movementMultiplier) / maxMultiplier;
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

            playerControl = StartCoroutine(PlayerController());
            frozenOverlay.color = new Color(1f, 1f, 1f, fogStrength2);
            //if (activeEvents < 2) setEventText(descriptionKey);

            AddMoveModifiers();
            moveMod.movementMultiplier = minMultiplier;
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

            RemoveMoveModifiers();
        }

        public static void InvokeOnNotebookClaim()
        {
            ColdSchoolEvent[] events = FindObjectsOfType<ColdSchoolEvent>();
            for (int i = 0; i < events.Length; i++)
            {
                if (events[i].Active) events[i].OnNotebookClaim();
            }
        }

        private void OnNotebookClaim()
        {
            moveMod.movementMultiplier = maxMultiplier;
            SetCooldown(7f);
        }

        private void SetCooldown(float cooldownTime)
        {
            if (this.cooldownTime < cooldownTime)
            {
                this.cooldownTime = cooldownTime;
            }
        }
    }
}
