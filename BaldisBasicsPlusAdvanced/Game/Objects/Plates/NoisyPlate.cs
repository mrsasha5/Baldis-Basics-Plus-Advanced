using BaldisBasicsPlusAdvanced.Cache;
using BaldisBasicsPlusAdvanced.Game.Objects.Plates.Base;
using System.Collections.Generic;
using UnityEngine;

namespace BaldisBasicsPlusAdvanced.Game.Objects.Plates
{
    public class NoisyPlate : BaseCooldownPlate
    {
        [SerializeField]
        private int generosityCount;

        [SerializeField]
        private float cooldown;

        [SerializeField]
        private int points;

        [SerializeField]
        private bool callsPrincipal;

        [SerializeField]
        private bool levelEditorMode;

        private Cell cell;

        private static List<NoisyPlate> editorPlates;

        private List<NoisyPlate> connectedPlates = new List<NoisyPlate>();

        public bool CallsPrincipal => callsPrincipal;

        protected override void SetValues(PlateData plateData)
        {
            base.SetValues(plateData);
            plateData.targetsPlayer = true;
            //plateData.hasLight = true;
            //plateData.lightColor = Color.red;

            points = 0;
            generosityCount = 0;
            cooldown = 120f;
        }

        protected override void VirtualAwake()
        {
            base.VirtualAwake();
            if (levelEditorMode)
            {
                cell = ec.CellFromPosition(transform.position);

                if (editorPlates == null) editorPlates = new List<NoisyPlate>();

                foreach (NoisyPlate plate in editorPlates)
                {
                    if (plate.cell != null && plate.cell.room != null && plate.cell.room == cell.room)
                    {
                        plate.ConnectTo(this);
                        ConnectTo(plate);
                    }
                }

                editorPlates.Add(this);
            }
        }

        private void OnDestroy()
        {
            if (levelEditorMode)
            {
                editorPlates.Remove(this);
                if (editorPlates.Count == 0) editorPlates = null;
            }
        }

        public void SetLevelEditorMode(bool state)
        {
            levelEditorMode = state;
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

        public void SetGenerosityCount(int count)
        {
            generosityCount = count;
        }

        protected override void SetTextures()
        {
            SetTexturesByBaseName("adv_noisy_plate");
            SetEditorSprite("adv_editor_noisy_plate");
        }

        protected override void VirtualOnPress()
        {
            base.VirtualOnPress();
            audMan.PlaySingle(AssetsStorage.sounds["adv_emergency"]);
            EnvironmentController ec = Singleton<BaseGameManager>.Instance.Ec;
            ec.MakeNoise(transform.position, 127);
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
                Singleton<CoreGameManager>.Instance.AddPoints(points, 0, true);
                generosityCount--;
            }
            SetCooldown(cooldown);
            for (int i = 0; i < connectedPlates.Count; i++) {
                connectedPlates[i].SetCooldown(cooldown);
                if (generosityCount > 0) connectedPlates[i].generosityCount--;
            }
        }

        protected override bool IsPressable(Entity target)
        {
            return base.IsPressable(target) && target.TryGetComponent(out PlayerManager pm) && !pm.Tagged;
        }

    }
}
