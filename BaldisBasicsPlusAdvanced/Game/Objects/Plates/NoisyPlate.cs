using BaldisBasicsPlusAdvanced.Cache.AssetsManagement;
using BaldisBasicsPlusAdvanced.Game.Objects.Plates.Base;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BaldisBasicsPlusAdvanced.Game.Objects.Plates
{
    public class NoisyPlate : BaseCooldownPlate
    {
        [SerializeField]
        private SoundObject audAlarm;

        [SerializeField]
        private int generosityCount;

        [SerializeField]
        private float cooldown;

        [SerializeField]
        private float resetFacultyColorTime;

        [SerializeField]
        private int points;

        [SerializeField]
        private bool callsPrincipal;

        private RoomController room;

        private List<NoisyPlate> connectedPlates = new List<NoisyPlate>();

        public bool CallsPrincipal => callsPrincipal;

        public override void InitializePrefab(int variant)
        {
            base.InitializePrefab(variant);
            audAlarm = AssetsStorage.sounds["buzz_elv"]; //adv_emergency
        }

        protected override void SetValues(PlateData plateData)
        {
            base.SetValues(plateData);
            plateData.targetsPlayer = true;
            //plateData.hasLight = true;
            //plateData.lightColor = Color.red;

            points = 0;
            generosityCount = 0;
            cooldown = 60f;
            resetFacultyColorTime = 10f;
        }

        protected override void VirtualStart()
        {
            base.VirtualStart();
            room = ec.CellFromPosition(transform.position).room;
        }

        public void OverrideCooldown(float cooldown) { 
            this.cooldown = cooldown;
        }

        public void SetCallsPrincipal(bool state)
        {
            callsPrincipal = state;
        }

        public void ConnectRange(List<NoisyPlate> range)
        {
            List<NoisyPlate> plates = new List<NoisyPlate>(range);
            for (int i = 0; i < plates.Count; i++)
            {
                if (plates[i] == this || connectedPlates.Contains(plates[i]))
                {
                    plates.RemoveAt(i);
                    i--;
                }
            }
            connectedPlates.AddRange(plates);
        }

        public void ConnectTo(NoisyPlate plate)
        {
            if (plate == this || connectedPlates.Contains(plate)) return;
            connectedPlates.Add(plate);
        }

        public void SetPointsReward(int count)
        {
            points = count;
        }

        public void SetGenerosity(int count)
        {
            generosityCount = count;
        }

        protected override void SetTextures()
        {
            SetTexturesByBaseName("adv_noisy_plate");
        }

        protected override void VirtualOnPress()
        {
            base.VirtualOnPress();
            audMan.PlaySingle(audAlarm, 2f);
            ec.MakeNoise(transform.position, 127);

            if (room.Powered)
            {
                for (int i = 0; i < room.lights.Count; i++)
                {
                    room.lights[i].lightColor = Color.red;
                    room.lights[i].SetLight(true);
                }
                StartCoroutine(ResetRoomColor(resetFacultyColorTime));
            }

            if (callsPrincipal)
            {
                for (int i = 0; i < ec.Npcs.Count; i++)
                {
                    if (ec.Npcs[i] is Principal)
                    {
                        ((Principal)ec.Npcs[i]).WhistleReact(transform.position);
                    }
                }
            }

            if (generosityCount > 0)
            {
                CoreGameManager.Instance.AddPoints(points, 0, true);
                generosityCount--;
            }

            SetCooldown(cooldown);
            for (int i = 0; i < connectedPlates.Count; i++) {
                connectedPlates[i].SetCooldown(cooldown);
                if (generosityCount > 0) connectedPlates[i].generosityCount--;
            }
        }

        private IEnumerator ResetRoomColor(float timer)
        {
            while (timer > 0f)
            {
                timer -= Time.deltaTime * ec.EnvironmentTimeScale;
                yield return null;
            }
            for (int i = 0; i < room.lights.Count; i++)
            {
                room.lights[i].lightColor = Color.white;
                if (room.Powered) room.lights[i].SetLight(true);
            }
        }

        protected override bool IsPressable(Entity target)
        {
            return base.IsPressable(target) && target.TryGetComponent(out PlayerManager pm) && !pm.Tagged;
        }

    }
}
