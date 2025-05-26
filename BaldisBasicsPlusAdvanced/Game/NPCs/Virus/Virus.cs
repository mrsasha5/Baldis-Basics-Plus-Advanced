/*using BaldisBasicsPlusAdvanced.Cache;
using BaldisBasicsPlusAdvanced.Game.Components;
using BaldisBasicsPlusAdvanced.Game.Objects;
using BaldisBasicsPlusAdvanced.Helpers;
using System.Collections;
using UnityEngine;

namespace BaldisBasicsPlusAdvanced.Game.NPCs.Virus
{
    public class Virus : NPC, IPrefab
    {
        [SerializeField]
        private CustomPickupBob pickupBob;

        //private float lastSpeed;

        //[SerializeField]
        //private float maxAchooAddendSize;

        public void InitializePrefab(int variant)
        {
            spriteRenderer[0].sprite = AssetsHelper.SpriteFromFile("Textures/Npcs/Chohu/adv_npc_chohu.png", 50f);
            pickupBob = spriteRenderer[0].gameObject.AddComponent<CustomPickupBob>();
            pickupBob.divider = 1f;

            //maxAchooAddendSize = 1f;
        }

        public override void Initialize()
        {
            base.Initialize();
            behaviorStateMachine.ChangeState(new Virus_Wandering(this));
        }

        protected override void VirtualUpdate()
        {
            base.VirtualUpdate();
            //pickupBob.speed = 0.15f * navigator.speed; //TODO: FIX THE CRAZY SPEED DURING CHARACTER SPEED UPDATE
            //if (pickupBob.speed < 2f) pickupBob.speed = 2f;
        }

        public void SetWalkSpeed()
        {
            navigator.SetSpeed(15f);
            RefreshPickupAnimation();
        }

        public void SetRunningSpeed()
        {
            navigator.SetSpeed(25f);
            RefreshPickupAnimation();
        }

        public void Achoo()
        {
            //StartAchooAnimation(1f);
            CreateInfection(15f);
        }

        private void OnDisable()
        {
            spriteRenderer[0].transform.localScale = Vector3.one;
        }

        public void RefreshPickupAnimation()
        {
            pickupBob.speed = 0.15f * navigator.speed;
            pickupBob.val = 0f;
            if (pickupBob.speed < 2f) pickupBob.speed = 2f;
        }

        private void CreateInfection(float time)
        {
            InfectionZone infection = GameObject.Instantiate(ObjectsStorage.Objects["infection_zone"].GetComponent<InfectionZone>(),
                transform.position, Quaternion.identity);
            infection.Initialize(ec, time);
        }
    }
}
*/