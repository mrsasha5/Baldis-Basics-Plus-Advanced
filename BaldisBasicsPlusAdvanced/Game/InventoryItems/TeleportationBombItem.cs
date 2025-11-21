using BaldisBasicsPlusAdvanced.Cache;
using BaldisBasicsPlusAdvanced.Game.Adjusters;
using BaldisBasicsPlusAdvanced.Game.Objects;
using UnityEngine;

namespace BaldisBasicsPlusAdvanced.Game.InventoryItems
{
    public class TeleportationBombItem : Item, IPrefab
    {
        [SerializeField]
        private Entity entity;

        [SerializeField]
        private AudioManager audMan;

        [SerializeField]
        private CapsuleCollider capsuleCollider;

        [SerializeField]
        private float beginsInTime;

        private float endHeight = 0.5f;

        private float throwSpeed = 80f; //50f

        private float gravity = 15f; //10f

        private EnvironmentController ec;

        private float height = 5f;

        private bool ready;

        public void InitializePrefab(int variant)
        {
            entity = new EntityAdjuster(gameObject)
                .SetName("Teleportation Bomb")
                .SetLayer("StandardEntities")
                .AddTrigger(1f)
                .SetLayerCollisionMask((LayerMask)2113541)
                .AddRigidbody()
                .AddDefaultRenderBaseFunction(AssetStorage.sprites["adv_teleportation_bomb"])
                .Build();

            entity.SetGrounded(false);

            audMan = gameObject.AddComponent<PropagatedAudioManager>();

            capsuleCollider = GetComponent<CapsuleCollider>();

            capsuleCollider.height = 5f;

            beginsInTime = 3f;
        }

        private void Update()
        {
            if (beginsInTime <= 0f) return;

            if (!ready)
            {
                height -= gravity * Time.deltaTime * ec.EnvironmentTimeScale;
                entity.UpdateInternalMovement(Vector3.zero);
                if (height <= endHeight)
                {
                    height = endHeight;
                    ready = true;
                    entity.SetGrounded(value: true);
                    audMan.PlaySingle(AssetStorage.sounds["teleport"]);
                }

                entity.SetHeight(height);
            }

            if (beginsInTime > 0f)
            {
                beginsInTime -= Time.deltaTime * ec.EnvironmentTimeScale;

                if (beginsInTime <= 0f)
                {
                    TeleportationHole bomb = Instantiate(ObjectStorage.Objects["teleportation_bomb"].GetComponent<TeleportationHole>());
                    Vector3 pos = transform.position;
                    pos.y = 5f;
                    bomb.transform.position = pos;
                    bomb.Initialize(ec, endsIn: 10f);
                    Destroy(gameObject);
                }
            }
        }

        public override bool Use(PlayerManager pm)
        {
            ec = pm.ec;
            entity.Initialize(pm.ec, pm.transform.position);
            entity.AddForce(new Force(Singleton<CoreGameManager>.Instance.GetCamera(pm.playerNumber).transform.forward, throwSpeed, -throwSpeed));
            return true;
        }
    }
}