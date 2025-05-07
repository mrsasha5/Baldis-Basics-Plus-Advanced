using BaldisBasicsPlusAdvanced.Game.Systems.BaseControllers;
using BaldisBasicsPlusAdvanced.Patches;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace BaldisBasicsPlusAdvanced.Game.Systems.Controllers
{
    public class NPCControllerSystem : ControllerSystemBase
    {
        private NPC npc;

        private EnvironmentController ec;

        public void Initialize(NPC npc, EnvironmentController ec)
        {
            this.npc = npc;
            this.ec = ec;
            initialized = true;
        }

        protected override T InitController<T>(T controller)
        {
            controller.ec = ec;
            controller.npc = npc;
            controller.entity = npc.GetComponent<Entity>();
            controller.owner = ControllerOwner.NPC;
            //controller.nc = this; npc controller system
            //controller.pc = Singleton<CoreGameManager>.Instance.GetPlayer(0).getControllerSystem(); //player controller system
            controller.controllerSystem = this;
            controller.pm = null; //a
            controller.OnInitialize();
            return controller;
        }
    }
}
