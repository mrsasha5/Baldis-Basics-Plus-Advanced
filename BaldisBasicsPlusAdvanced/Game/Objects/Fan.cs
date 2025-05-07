/*using BaldisBasicsPlusAdvanced.Cache;
using BaldisBasicsPlusAdvanced.Helpers;
using MTM101BaldAPI.Components;
using System;
using System.Collections;
using System.Text;
using UnityEngine;
using System.Collections.Generic;
using MTM101BaldAPI.Registers;

namespace BaldisBasicsPlusAdvanced.Game.Objects
{
    public class Fan : MonoBehaviour, IClickable<int>
    {
        [SerializeField]
        private Entity entity;

        [SerializeField]
        private AudioManager audMan;

        [SerializeField]
        private RotatedSpriteAnimator animator;

        [SerializeField]
        private SpriteRotator rotator;

        private static float windSpeed = 30f;

        private static int windSize = 12;

        private WindObject wind;

        private EnvironmentController ec;

        private bool activated;

        private Vector3 correctedPosition;

        private static int[] angles = new int[] {
            0,
            90,
            180,
            270,
            360
        };

        private bool animating;

        private float time;

        private bool broken;

        public bool Broken => broken;

        public void prefabInitialization(Entity entity, SpriteRenderer spriteRenderer)
        {
            Rigidbody rb = entity.gameObject.AddComponent<Rigidbody>();
            rb.useGravity = false;
            rb.angularDrag = 0;
            rb.constraints = RigidbodyConstraints.FreezeAll;
            rb.freezeRotation = true;
            rb.isKinematic = true;
            rb.mass = 0;

            this.entity = entity;

            gameObject.GetComponent<CapsuleCollider>().height = 5f;

            rotator = entity.gameObject.AddComponent<SpriteRotator>();

            ReflectionHelper.setValue<SpriteRenderer>(rotator, "spriteRenderer", spriteRenderer);

            audMan = gameObject.AddComponent<PropagatedAudioManager>();

            animator = gameObject.AddComponent<RotatedSpriteAnimator>();
        }

        public void initialize(EnvironmentController ec, Vector3 position, Quaternion rotation, float livingTime, bool turnOff = true)
        {
            this.ec = ec;

            position = correctPosition(position);
            rotation = correctRotation(rotation);

            transform.rotation = rotation;

            entity.Initialize(ec, position);
            ReflectionHelper.setValue<bool>(entity, "squished", false);

            ReflectionHelper.setValue<bool>(entity, "persistent", false);

            wind = ObjectsCreator.createWindObject(windSize, windSpeed, true);

            wind.transform.position = position;
            wind.transform.rotation = rotation;

            wind.transform.SetParent(transform, true);

            wind.transform.localPosition += Vector3.forward * 2f;

            audMan.PlaySingle(AssetsStorage.sounds["lock_door_stop"]);

            if (turnOff) setActivityState(false);

            time = livingTime;

            Sprite[][] sprites = new Sprite[2][];

            sprites[0] = new Sprite[]
            {
                AssetsStorage.sprites["adv_fan_side_2"],
                AssetsStorage.sprites["adv_fan_face_side_3"],
                AssetsStorage.sprites["adv_fan_face_1"],
                AssetsStorage.sprites["adv_fan_face_side_1"],
                AssetsStorage.sprites["adv_fan_side_1"],
                AssetsStorage.sprites["adv_fan_rear_side_1"],
                AssetsStorage.sprites["adv_fan_backside"],
                AssetsStorage.sprites["adv_fan_rear_side_2"]
            };
            sprites[1] = new Sprite[]
            {
                AssetsStorage.sprites["adv_fan_side_2"],
                AssetsStorage.sprites["adv_fan_face_side_4"],
                AssetsStorage.sprites["adv_fan_face_2"],
                AssetsStorage.sprites["adv_fan_face_side_2"],
                AssetsStorage.sprites["adv_fan_side_1"],
                AssetsStorage.sprites["adv_fan_rear_side_1"],
                AssetsStorage.sprites["adv_fan_backside"],
                AssetsStorage.sprites["adv_fan_rear_side_2"]
            };

            animator.affectedObject = rotator;
            animator.PopulateAnimations(new Dictionary<string, Sprite[][]>() {
                { "blowing", sprites }
            }, fps: 60);

            animator.SetDefaultAnimation("blowing", 1f);
        }

        private void correctSelfPosition(bool playThud = true, bool playOnlyIfPosReallyChanged = true)
        {
            if (playThud && (!playOnlyIfPosReallyChanged || (playOnlyIfPosReallyChanged && transform.position != correctPosition(transform.position))))
            {
                audMan.PlaySingle(AssetsStorage.sounds["lock_door_stop"]);
            }
            transform.position = correctPosition(transform.position);
            correctedPosition = transform.position;
        }

        private Vector3 correctPosition(Vector3 position)
        {
            IntVector2 _position = ec.CellFromPosition(position).position;

            if (_position != null)
            {
                position.x = _position.x * 10f + 5f;
                position.z = _position.z * 10f + 5f;
                position.y = 5f;
            }

            return position;
        }

        private Quaternion correctRotation(Quaternion rotation)
        {
            float setY = 0f;

            float lastDifference = float.PositiveInfinity;

            for (int i = 0; i < angles.Length; i++)
            {
                float difference = Math.Abs(angles[i] - rotation.eulerAngles.y);

                if (difference < lastDifference)
                {
                    setY = angles[i];
                    lastDifference = difference;
                }
            }

            rotation.eulerAngles = new Vector3(rotation.eulerAngles.x, setY, rotation.eulerAngles.z);

            return rotation;
        }

        private void Update()
        {
            if (activated && !animating && time > 0)
            {
                if (entity.Frozen || entity.Squished || transform.position != correctedPosition) setActivityState(false);
                time -= Time.deltaTime * ec.EnvironmentTimeScale;
            }

            if (!broken && time < 0)
            {
                breakFan();
            }
        }

        private void setActivityState(bool active)
        {
            wind.setActivityState(active);
            activated = active;
            animator.SetPause(!active);
        }

        public void turn()
        {
            StartCoroutine(turning());
            audMan.PlaySingle(AssetsStorage.sounds["clock_wind"]);
        }

        public void breakFan() {
            broken = true;
            if (activated) audMan.PlaySingle(AssetsStorage.sounds["adv_pah"]);
            setActivityState(false);
            audMan.PlaySingle(AssetsStorage.sounds["bal_break"]);
        }

        public void repair(float livingTime, bool playXylophone = true)
        {
            broken = false;
            time = livingTime;
            if (playXylophone) audMan.PlaySingle(AssetsStorage.sounds["xylophone"]);
        }

        private IEnumerator turning()
        {
            float time = 1f;

            if (!activated)
            {
                audMan.PlaySingle(AssetsStorage.sounds["adv_inhale"]);
                correctSelfPosition();
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
                setActivityState(wind.Hidden);
            }

            animating = false;

            yield break;
        }

        public void Clicked(int player)
        {
            if (animating || broken) return;
            turn();
        }

        public void ClickableSighted(int player)
        {
        }

        public void ClickableUnsighted(int player)
        {
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
*/