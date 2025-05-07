using BaldisBasicsPlusAdvanced.Cache;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using TMPro;
using UnityEngine;

namespace BaldisBasicsPlusAdvanced.Game.Objects.Plates.Base
{
    public class BaseCooldownPlate : BasePlate
    {

        protected float cooldownTime;

        protected bool locked;

        [SerializeField]
        protected bool ignoreCooldown;

        public void SetIgnoreCooldown(bool state)
        {
            ignoreCooldown = state;
        }

        protected override void SetValues(ref PlateData plateData)
        {
            base.SetValues(ref plateData);
            plateData.showCooldown = true;
        }

        protected override void VirtualUpdate()
        {
            base.VirtualUpdate();
            if (locked && cooldownTime > 0)
            {
                cooldownTime -= Time.deltaTime * Timescale;

                SetVisualCooldown((int)cooldownTime + 1);

                if (cooldownTime < 0)
                {
                    locked = false;
                    text.text = "";
                    OnCooldownEnded();
                }
            }
        }

        protected virtual void OnCooldownEnded()
        {
            audMan.PlaySingle(AssetsStorage.sounds["bell"]);
            SetVisualUses(usedCount, plateData.uses);
        }

        protected void SetCooldown(float cooldown)
        {
            if (ignoreCooldown) return;
            cooldownTime = cooldown;
            locked = true;
            SetVisualCooldown((int)cooldown + 1);
        }

        protected void SetVisualCooldown(int cooldown)
        {
            text.text = string.Join("", cooldown.ToString().Select(num => "<sprite=" + num + ">"));
        }

        protected override bool SetVisualUses(int uses, int maxUses)
        {
            if (!locked) return base.SetVisualUses(uses, maxUses);
            return false;
        }

        protected override bool IsUsable()
        {
            return base.IsUsable() && !locked;
        }
    }
}
