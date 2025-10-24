using BaldisBasicsPlusAdvanced.Cache.AssetsManagement;
using BaldisBasicsPlusAdvanced.Extensions;
using BaldisBasicsPlusAdvanced.Game.Components.Movement;
using BaldisBasicsPlusAdvanced.Game.Objects.Plates.Base;
using BaldisBasicsPlusAdvanced.Helpers;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace BaldisBasicsPlusAdvanced.Game.Objects.Plates
{
    public class AccelerationPlate : BasePlate, IButtonReceiver
    {
        [SerializeField]
        internal float initialSpeed;

        [SerializeField]
        internal float acceleration;

        [SerializeField]
        private SoundObject audRotate;

        [SerializeField]
        private SoundObject audBoing;

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

        public override void InitializePrefab(int variant)
        {
            base.InitializePrefab(variant);
            audBoing = AssetsStorage.sounds["adv_boing"];
            audRotate = AssetsHelper.LoadAsset<SoundObject>("ShrinkMachine_Door");
        }

        protected override void SetValues(PlateData data)
        {
            base.SetValues(data);
            data.MarkAsCooldownPlate();
            data.allowsToCopyTextures = false;
            //plateData.hasLight = true;
            //plateData.lightColor = new Color(1f, 0.5f, 0f); //orange

            initialSpeed = 120f;
            acceleration = -40f;

            rotationSpeed = 180f;
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

        public void DefineRotateDirection(float angle)
        {
            if (angle > 360f) angle %= 360f;

            if (!potentialAngles.Contains(angle))
            {
                potentialAngles.Add(angle);
            }
        }

        public void ButtonPressed(bool val)
        {
            Rotate();
        }

        public void Rotate()
        {
            if (rotating || potentialAngles.Count == 0) return;

            angleIndex++;

            if (angleIndex > potentialAngles.Count - 1)
            {
                angleIndex = 0;
            }

            rotating = true;
                
            UpdateVisualPressedState(true);

            anglesToStop = Math.Abs(potentialAngles[angleIndex] - meshRenderers[1].transform.eulerAngles.y);

            float plannedAngle = meshRenderers[1].transform.eulerAngles.y + anglesToStop;
            if (plannedAngle >= 360f) plannedAngle = plannedAngle % 360f;

            if (potentialAngles[angleIndex] != plannedAngle)
            {
                anglesToStop += Math.Abs(potentialAngles[angleIndex] - plannedAngle);
            }

            if (anglesToStop >= 360f) anglesToStop = anglesToStop % 360f;

            audMan.PlaySingle(audRotate);
        }

        public void UpdateAngleIndex(float angle)
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

        public void SetRotation(float angle)
        {   
            Quaternion quaternion = default;
            quaternion.eulerAngles = new Vector3(90f, angle, 0f);
            meshRenderers[1].transform.rotation = quaternion;
        }

        protected override void SetTextures()
        {
            SetTexturesByBaseName("adv_acceleration_plate");

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
                    SetRotation(meshRenderers[1].transform.rotation.eulerAngles.y + rotationSpeed * ec.EnvironmentTimeScale * Time.deltaTime);
                } else
                {
                    SetRotation(potentialAngles[angleIndex]);
                    rotating = false;

                    UpdateVisualPressedState(false);
                }
            }
            base.VirtualUpdate();
        }

        protected override void VirtualOnUnpress()
        {
            base.VirtualOnUnpress();
            if (entities.Count <= 0) return;
            audMan.PlaySingle(audBoing);
            float time = Math.Abs(initialSpeed / acceleration);

            foreach (Entity entity in entities)
            {
                if (entity == null) continue;

                Quaternion rot = Quaternion.Euler(0, meshRenderers[1].transform.rotation.eulerAngles.y, 0);

                Vector3 forward = rot * Vector3.forward;

                entity.AddForceWithBehaviour(forward, initialSpeed, acceleration, makesNoises: entity.CompareTag("Player"), 0.75f);
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
