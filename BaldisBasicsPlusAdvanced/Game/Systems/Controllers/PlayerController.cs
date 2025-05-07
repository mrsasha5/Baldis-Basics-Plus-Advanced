using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace BaldisBasicsPlusAdvanced.Game.Systems.Controllers
{
    public class PlayerController : BaseController
    {

        protected bool invincibleEffect;

        public virtual bool Invincibility => invincibleEffect;

        public virtual void initialize(EnvironmentController ec, PlayerManager pm, PlayerControllerSystem pc)
        {
            this.pm = pm;
            this.ec = ec;
            this.pc = pc;
        }

    }
}
