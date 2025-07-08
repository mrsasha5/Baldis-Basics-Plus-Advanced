using BaldisBasicsPlusAdvanced.Cache.AssetsManagement;
using BaldisBasicsPlusAdvanced.Game.Components.Movement;
using BaldisBasicsPlusAdvanced.Game.Objects.Plates.Base;
using BaldisBasicsPlusAdvanced.Helpers;
using BaldisBasicsPlusAdvanced.Patches;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace BaldisBasicsPlusAdvanced.Game.Objects.Plates
{
    public class AccelerationPlate : BaseCooldownPlate, IButtonReceiver
    {
        [SerializeField]
        internal float initialSpeed;

        [SerializeField]
        internal float acceleration;

        //[SerializeField]
        //internal float timeToUnpress;

        [SerializeField]
        private Material arrowDeactivatedMat;

        [SerializeField]
        private Material arrowActivatedMat;

        [SerializeField]
        private float rotationSpeed;

        private float cooldown = 20f;

        private float ignoringTime;

        private bool rotating;

        private List<float> potentialAngles = new List<float>();

        private float anglesToStop;

        private int angleIndex;

        public bool IsRotatable => potentialAngles.Count > 1;

        protected override bool UnpressesWithNoReason => base.UnpressesWithNoReason || cooldownTime <= 0f;

        public void ConnectButton(GameButtonBase button)
        {

        }

        protected override void SetValues(PlateData plateData)
        {
            base.SetValues(plateData);
            plateData.showsCooldown = true;
            plateData.allowsToCopyTextures = false;
            //plateData.hasLight = true;
            //plateData.lightColor = new Color(1f, 0.5f, 0f); //orange

            initialSpeed = 120f;
            acceleration = -40f;

            rotationSpeed = 50f;
        }

        public void ButtonPressed(bool val)
        {
            Rotate();
        }

        public void Rotate()
        {
            if (rotating || potentialAngles.Count == 0) return;

            angleIndex++;

            if (angleIndex > potentialAngles.Count - 1) angleIndex = 0;

            rotating = true;
                
            UpdateVisualPressedState(true);

            anglesToStop = Math.Abs(potentialAngles[angleIndex] - meshRenderers[1].transform.eulerAngles.y);

            float plannedAngle = meshRenderers[1].transform.eulerAngles.y + anglesToStop;
            if (plannedAngle >= 360f) plannedAngle = plannedAngle % 360f; //conversion to 360 angle system

            if (potentialAngles[angleIndex] != plannedAngle)
            {
                anglesToStop += Math.Abs(potentialAngles[angleIndex] - plannedAngle);
            }

            audMan.FlushQueue(true);
            audMan.QueueAudio(AssetsStorage.sounds["adv_turning_start"]);
            audMan.QueueAudio(AssetsStorage.sounds["adv_turning_loop"]);
            audMan.SetLoop(true);
        }

        public void SetAngleIndexByAngle(float angle)
        {
            for (int i = 0; i < potentialAngles.Count; i++)
            {
                if (potentialAngles[i] == angle)
                {
                    angleIndex = i;
                    break;
                }
            }
        }

        public void SetForwardByAngle(float angle)
        {   
            Quaternion quaternion = default;
            quaternion.eulerAngles = new Vector3(90f, angle, 0f);
            meshRenderers[1].transform.rotation = quaternion;
        }

        public void InitializePotentialDirections()
        {
            Cell mainCell = ec.CellFromPosition(transform.position);

            for (int i = 0; i < 4; i++)
            {
                IntVector2 pos = mainCell.position + ((Direction)i).ToIntVector2();
                if (!ec.ContainsCoordinates(pos)) continue;

                Cell potentialCell = ec.CellFromPosition(pos);
                if (!potentialCell.Null && !potentialCell.HasWallInDirection(((Direction)i).GetOpposite()))
                {
                    potentialAngles.Add(i * 90f);
                }
            }

        }

        public void ChooseBestRotation()
        {
            /*Cell mainCell = ec.CellFromPosition(transform.position);
            int[] lengths = new int[4];
            for (int i = 0; i < 4; i++)
            {
                IntVector2 pos = mainCell.position + ((Direction)i).ToIntVector2();
                if (!ec.ContainsCoordinates(pos)) continue;

                Cell nextCell = ec.CellFromPosition(pos);

                while (nextCell != null && !nextCell.Null && !nextCell.HasWallInDirection(((Direction)i).GetOpposite())
                    && ((nextCell.HardCoverageBin & Directions.ToBinary(((Direction)i).GetOpposite())) == 0))
                {
                    pos += ((Direction)i).ToIntVector2();
                    lengths[i] += 1;
                    nextCell = ec.CellFromPosition(pos);
                }
            }
            int index = Array.IndexOf(lengths, MathHelper.FindMaxValue(lengths));
            SetForwardByAngle(lengths[index] * 90f);*/
            Vector3[] forwards = new Vector3[] { Vector3.forward, Vector3.back, Vector3.left, Vector3.right };
            float[] angles = new float[] { 0f, 180f, 270f, 90f };
            float lastDistance = float.NegativeInfinity;
            int chosen = 0;
            for (int i = 0; i < forwards.Length; i++)
            {
                Physics.Raycast(transform.position, forwards[i], out RaycastHit hit, float.PositiveInfinity, LayersHelper.ignorableCollidableObjects, QueryTriggerInteraction.Ignore);
                if (hit.distance > lastDistance)
                {
                    chosen = i;
                    lastDistance = hit.distance;
                }
            }
            SetForwardByAngle(angles[chosen]);
        }

        protected override void SetTextures()
        {
            SetTexturesByBaseName("adv_acceleration_plate");
            SetEditorSprite("adv_editor_acceleration_plate");

            InitializeRenderer();

            meshRenderers[1].transform.localPosition += Vector3.up * 0.001f;

            arrowDeactivatedMat = new Material(deactivatedMaterial);
            arrowActivatedMat = new Material(deactivatedMaterial);

            arrowDeactivatedMat.mainTexture = AssetsStorage.textures["adv_acceleration_plate_deactivated_arrow"];
            arrowActivatedMat.mainTexture = AssetsStorage.textures["adv_acceleration_plate_activated_arrow"];

            UpdateVisualPressedState(false, playSound: false);
        }

        protected override void VirtualUpdate()
        {
            if (ignoringTime >= 0) ignoringTime -= Time.deltaTime * Timescale;

            if (rotating)
            {
                if (potentialAngles.Count > 1 && anglesToStop > 0f)
                {
                    anglesToStop -= rotationSpeed * Timescale * Time.deltaTime;
                    SetForwardByAngle(meshRenderers[1].transform.rotation.eulerAngles.y + rotationSpeed * ec.EnvironmentTimeScale * Time.deltaTime);
                } else
                {
                    SetForwardByAngle(potentialAngles[angleIndex]);
                    rotating = false;

                    UpdateVisualPressedState(false);

                    audMan.FlushQueue(true);
                    audMan.QueueAudio(AssetsStorage.sounds["adv_turning_end"]);
                }
            }
            base.VirtualUpdate();
        }

        protected override void VirtualOnUnpress()
        {
            base.VirtualOnUnpress();
            if (entities.Count <= 0) return;
            audMan.PlaySingle(AssetsStorage.sounds["adv_boing"]);
            float time = Math.Abs(initialSpeed / acceleration);

            foreach (Entity entity in entities)
            {
                if (entity == null) continue;

                Quaternion rot = Quaternion.Euler(0, meshRenderers[1].transform.rotation.eulerAngles.y, 0);

                Vector3 forward = rot * Vector3.forward;

                entity.AddForceWithBehaviour(forward, initialSpeed, acceleration, makesNoises: entity.CompareTag("Player"), 0.75f);

                /*Force force = new Force(forward, initialSpeed, acceleration);
                entity.AddForce(force);

                ForcedEntityBehaviour behaviour = entity.gameObject.AddComponent<ForcedEntityBehaviour>();
                behaviour.Initialize(Singleton<BaseGameManager>.Instance.Ec, time);
                behaviour.PostInit(entity, force, forward, 0.75f, behaviour.DefaultSlamDistance, makesNoises: entity is PlayerEntity, time: time);*/
        }
        SetCooldown(cooldown);
            ignoringTime = 1f;
        }

        protected override void UpdateVisualPressedState(bool active, bool playSound = true)
        {
            if (active)
            {
                meshRenderers[1].material = arrowActivatedMat;
            } else
            {
                meshRenderers[1].material = arrowDeactivatedMat;
            }
            base.UpdateVisualPressedState(active, playSound);
        }

        protected override bool IsPressable(Entity target)
        {
            return base.IsPressable(target) && !rotating && ignoringTime <= 0f;
        }
    }
}
