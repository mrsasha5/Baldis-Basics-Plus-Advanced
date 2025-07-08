using BaldisBasicsPlusAdvanced.Cache.AssetsManagement;
using System.Linq;
using UnityEngine;

namespace BaldisBasicsPlusAdvanced.Game.Objects.Plates.Base
{
    public class BaseCooldownPlate : BasePlate
    {
        [SerializeField]
        protected SoundObject audCooldownEnds;

        [SerializeField]
        protected int cooldownIgnores;

        protected float cooldownTime;

        protected bool locked;

        public override bool IsUsable => base.IsUsable && !locked;

        public override void InitializePrefab(int variant)
        {
            base.InitializePrefab(variant);
            audCooldownEnds = AssetsStorage.sounds["bell"];
        }

        protected override void OnTurnOff()
        {
            base.OnTurnOff();
            if (text != null) text.text = "";
        }

        public void SetIgnoreCooldown(bool state)
        {
            if (state)
            {
                cooldownIgnores++;
            } else
            {
                cooldownIgnores--;
            }

            if (cooldownIgnores < 0) cooldownIgnores = 0;
        }

        protected override void SetValues(PlateData plateData)
        {
            base.SetValues(plateData);
            plateData.showsCooldown = true;
        }

        protected override void VirtualUpdate()
        {
            base.VirtualUpdate();
            if (locked && turnOffs <= 0 && cooldownTime > 0f)
            {
                cooldownTime -= Time.deltaTime * Timescale;

                SetVisualCooldown((int)cooldownTime + 1);

                if (cooldownTime <= 0f)
                {
                    OnCooldownEnded();
                }
            }
        }

        public virtual void OnCooldownEnded()
        {
            locked = false;
            cooldownTime = 0f;
            if (text != null) text.text = "";
            if (audCooldownEnds != null) audMan.PlaySingle(audCooldownEnds);
            SetVisualUses(usedCount, plateData.uses);
            UpdateVisualActiveState();
        }

        public void SetCooldown(float cooldown)
        {
            if (cooldownIgnores > 0) return;
            cooldownTime = cooldown;
            locked = true;
            SetVisualCooldown((int)cooldown + 1);
            UpdateVisualActiveState();
        }

        protected bool SetVisualCooldown(int cooldown)
        {
            if (!Data.showsCooldown) return false;
            text.text = string.Join("", cooldown.ToString().Select(num => "<sprite=" + num + ">"));
            return true;
        }

        protected override bool SetVisualUses(int uses, int maxUses)
        {
            if (!locked) return base.SetVisualUses(uses, maxUses);
            return false;
        }
    }
}
