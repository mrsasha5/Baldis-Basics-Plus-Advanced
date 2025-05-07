using BaldisBasicsPlusAdvanced.Game.Systems.Controllers;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace BaldisBasicsPlusAdvanced.Game.Components.NPCs
{
    public class PushedNpcController : NPCController
    {

        private float slamSpeed = 50f;

        private MovementModifier moveMod = new MovementModifier(Vector3.zero, 0f);

        public override void initialize(NPC npc, PlayerControllerSystem pc)
        {
            base.initialize(npc, pc);
            entity.ExternalActivity.moveMods.Add(moveMod);
        }

        public override void virtualUpdate()
        {
            base.virtualUpdate();
            if (npc.Navigator.speed >= slamSpeed && Physics.Raycast(npc.transform.position, npc.transform.forward, out RaycastHit raycastHit, 5f, 2097152, QueryTriggerInteraction.Collide) && raycastHit.transform.CompareTag("Window"))
            {
                raycastHit.transform.GetComponent<Window>().Break(makeNoise: true);
            }
        }

        public override void onDestroying()
        {
            base.onDestroying();
            entity.ExternalActivity.moveMods.Remove(moveMod);
        }

    }
}
