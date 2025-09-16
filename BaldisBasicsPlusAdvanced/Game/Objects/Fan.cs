using BaldisBasicsPlusAdvanced.Helpers;
using MTM101BaldAPI.Components;
using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using MTM101BaldAPI.Registers;
using BaldisBasicsPlusAdvanced.Game.Components;
using BaldisBasicsPlusAdvanced.Game.Interfaces;
using BaldisBasicsPlusAdvanced.Cache.AssetsManagement;
using BaldisBasicsPlusAdvanced.Extensions;

namespace BaldisBasicsPlusAdvanced.Game.Objects
{
    public class Fan : MonoBehaviour, IClickable<int>, IEntityPrefab, IRepairable, IBreakable
    {
        [SerializeField]
        private Entity entity;

        [SerializeField]
        private AudioManager audMan;

        [SerializeField]
        private RotatedSpriteAnimator animator;

        [SerializeField]
        private SpriteRotator rotator;

        private static Sprite[][] sprites;

        private static float windSpeed = 30f;

        private static int windSize = 12;

        private WindObject wind;

        private EnvironmentController ec;

        private bool activated;

        private Vector3 correctedPosition;

        private bool animating;

        private float time;

        private bool broken;

        public bool Broken => broken;

        public void InitializePrefab(Entity entity, int variant)
        {
            const float pixelsPerUnit = 27f;

            entity.gameObject.SetRigidbody();

            this.entity = entity;

            gameObject.GetComponent<CapsuleCollider>().height = 5f;

            rotator = entity.gameObject.AddComponent<SpriteRotator>();

            ReflectionHelper.SetValue<SpriteRenderer>(rotator, "spriteRenderer", GetComponentInChildren<SpriteRenderer>());

            audMan = gameObject.AddComponent<PropagatedAudioManager>();

            animator = gameObject.AddComponent<RotatedSpriteAnimator>();

            sprites = new Sprite[2][];

            sprites[0] = new Sprite[]
            {
                AssetsHelper.SpriteFromFile("Textures/Objects/Fan/adv_fan_side_2.png", pixelsPerUnit),
                AssetsHelper.SpriteFromFile("Textures/Objects/Fan/adv_fan_face_side_3.png", pixelsPerUnit),
                AssetsHelper.SpriteFromFile("Textures/Objects/Fan/adv_fan_face_1.png", pixelsPerUnit),
                AssetsHelper.SpriteFromFile("Textures/Objects/Fan/adv_fan_face_side_1.png", pixelsPerUnit),
                AssetsHelper.SpriteFromFile("Textures/Objects/Fan/adv_fan_side_1.png", pixelsPerUnit),
                AssetsHelper.SpriteFromFile("Textures/Objects/Fan/adv_fan_rear_side_1.png", pixelsPerUnit),
                AssetsHelper.SpriteFromFile("Textures/Objects/Fan/adv_fan_backside.png", pixelsPerUnit),
                AssetsHelper.SpriteFromFile("Textures/Objects/Fan/adv_fan_rear_side_2.png", pixelsPerUnit),
            };
            sprites[1] = new Sprite[]
            {
                AssetsHelper.SpriteFromFile("Textures/Objects/Fan/adv_fan_side_2.png", pixelsPerUnit),
                AssetsHelper.SpriteFromFile("Textures/Objects/Fan/adv_fan_face_side_4.png", pixelsPerUnit),
                AssetsHelper.SpriteFromFile("Textures/Objects/Fan/adv_fan_face_2.png", pixelsPerUnit),
                AssetsHelper.SpriteFromFile("Textures/Objects/Fan/adv_fan_face_side_2.png", pixelsPerUnit),
                AssetsHelper.SpriteFromFile("Textures/Objects/Fan/adv_fan_side_1.png", pixelsPerUnit),
                AssetsHelper.SpriteFromFile("Textures/Objects/Fan/adv_fan_rear_side_1.png", pixelsPerUnit),
                AssetsHelper.SpriteFromFile("Textures/Objects/Fan/adv_fan_backside.png", pixelsPerUnit),
                AssetsHelper.SpriteFromFile("Textures/Objects/Fan/adv_fan_rear_side_2.png", pixelsPerUnit),
            };
        }

        public void Initialize(EnvironmentController ec, Vector3 position, Quaternion rotation, float livingTime, bool turnOff = true)
        {
            this.ec = ec;

            position = position.CorrectForCell();
            rotation = rotation.Correct();

            transform.rotation = rotation;

            entity.Initialize(ec, position);
            ReflectionHelper.SetValue<bool>(entity, "squished", false);

            wind = ObjectsCreator.CreateWindObject(windSize, windSpeed, true);

            wind.transform.SetParent(transform, false);

            wind.transform.position = position;
            wind.transform.rotation = rotation;

            wind.transform.localPosition += Vector3.forward * 2f;

            audMan.PlaySingle(AssetsStorage.sounds["lock_door_stop"]);

            if (turnOff) SetActivityState(false);

            time = livingTime;

            animator.affectedObject = rotator;
            animator.PopulateAnimations(new Dictionary<string, Sprite[][]>() {
                { "blowing", sprites }
            }, fps: 60);

            animator.SetDefaultAnimation("blowing", 1f);
        }

        private void CorrectSelfPosition(bool playThud = true, bool playOnlyIfPosReallyChanged = true)
        {
            if (playThud && (!playOnlyIfPosReallyChanged || (playOnlyIfPosReallyChanged && transform.position != transform.position.CorrectForCell())))
            {
                audMan.PlaySingle(AssetsStorage.sounds["lock_door_stop"]);
            }
            transform.position = transform.position.CorrectForCell();
            correctedPosition = transform.position;
        }

        private void Update()
        {
            if (activated && !animating && time > 0)
            {
                if (entity.Frozen || entity.Squished || transform.position != correctedPosition) SetActivityState(false);
                time -= Time.deltaTime * ec.EnvironmentTimeScale;
            }

            if (!broken && time < 0)
            {
                Break();
            }
        }

        private void SetActivityState(bool active)
        {
            wind.SetActivityState(active);
            activated = active;
            animator.SetPause(!active);
        }

        public void Turn()
        {
            StartCoroutine(Turning());
            audMan.PlaySingle(AssetsStorage.sounds["clock_wind"]);
        }

        public void Break() {
            broken = true;
            if (activated) audMan.PlaySingle(AssetsStorage.sounds["adv_pah"]);
            SetActivityState(false);
            audMan.PlaySingle(AssetsStorage.sounds["bal_break"]);
        }

        public void Repair()
        {
            broken = false;
            time = 20f;
            PlayerInteractionController.Instance.SetGameTip(0);
        }

        private IEnumerator Turning()
        {
            float time = 1f;

            if (!activated)
            {
                audMan.PlaySingle(AssetsStorage.sounds["adv_inhale"]);
                CorrectSelfPosition();
            }

            animating = true;
            while (time > 0)
            {
                time -= Time.deltaTime * ec.EnvironmentTimeScale;
                yield return null;
            }
            
            if (transform.position == correctedPosition)
            {
                if (activated) audMan.PlaySingle(AssetsStorage.sounds["adv_pah"]);
                SetActivityState(wind.Hidden);
            }

            animating = false;

            yield break;
        }

        public void Clicked(int player)
        {
            if (animating || broken) return;
            Turn();
        }

        public void ClickableSighted(int player)
        {
            if (ClickableHidden()) return;

            ItemManager itm = Singleton<CoreGameManager>.Instance.GetPlayer(0).itm;
            if (broken && itm.items[itm.selectedItem].GetMeta().tags.Contains("adv_repair_tool"))
            {
                PlayerInteractionController.Instance.SetGameTip(0, "Adv_Tip_Repair");
            }
        }

        public void ClickableUnsighted(int player)
        {
            PlayerInteractionController.Instance.SetGameTip(0);
        }

        public bool ClickableHidden()
        {
            ItemManager itm = Singleton<CoreGameManager>.Instance.GetPlayer(0).itm;
            if (broken && itm.items[itm.selectedItem].GetMeta().tags.Contains("adv_repair_tool"))
            {
                return false;
            }

            return (animating || broken);
        }

        public bool ClickableRequiresNormalHeight()
        {
            return false;
        }
    }
}
