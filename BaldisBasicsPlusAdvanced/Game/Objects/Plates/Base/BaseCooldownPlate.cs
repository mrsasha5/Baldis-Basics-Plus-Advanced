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
        //[SerializeField]
        //protected Transform counterBase;

        //[SerializeField]
        //protected TextMeshPro counterText;

        protected float cooldownTime;

        protected bool locked;

        protected override void setValues(ref PlateData plateData)
        {
            base.setValues(ref plateData);
            plateData.showCooldown = true;
        }

        protected override void virtualUpdate()
        {
            base.virtualUpdate();
            if (locked && cooldownTime > 0)
            {
                cooldownTime -= Time.deltaTime * Timescale;

                setVisualCooldown((int)cooldownTime + 1);

                if (cooldownTime < 0)
                {
                    locked = false;
                    text.text = "";
                    onCooldownEnded();
                }
            }
        }

        protected virtual void onCooldownEnded()
        {
            audMan.PlaySingle(AssetsStorage.sounds["bell"]);
            setVisualUses(usedCount, plateData.uses);
        }

        protected virtual void setCooldown(float cooldown)
        {
            cooldownTime = cooldown;
            locked = true;
            setVisualCooldown((int)cooldown + 1);
        }

        protected void setVisualCooldown(int cooldown)
        {
            text.text = string.Join("", cooldown.ToString().Select(num => "<sprite=" + num + ">"));
        }

        protected override bool setVisualUses(int uses, int maxUses)
        {
            if (!locked) return base.setVisualUses(uses, maxUses);
            return false;
        }

        protected override bool isUsable()
        {
            return base.isUsable() && !locked;
        }
    }
}
