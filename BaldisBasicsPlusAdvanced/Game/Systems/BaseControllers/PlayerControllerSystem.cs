using BaldisBasicsPlusAdvanced.Game.Systems.BaseControllers;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace BaldisBasicsPlusAdvanced.Game.Systems.Controllers
{
    public class PlayerControllerSystem : BaseControllerSystem
    {
        private PlayerManager pm;

        private EnvironmentController ec;

        public void Initialize(PlayerManager pm)
        {
            this.pm = pm;
            this.ec = pm.ec;
            initialized = true;
        }

        protected override T InitController<T>(T controller)
        {
            controller.ec = ec;
            controller.pm = pm;
            controller.entity = pm.GetComponent<PlayerEntity>();
            controller.owner = ControllerOwner.Player;
            controller.controllerSystem = this;
            //controller.pc = this; player controller
            controller.npc = null; //no
            if (controller.Initializable)
            {
                controller.OnInitialize();
                return controller;
            }
            return null;
        }

    }
}
