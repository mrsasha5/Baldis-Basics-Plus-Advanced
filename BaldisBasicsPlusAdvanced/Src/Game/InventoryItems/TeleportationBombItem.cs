using BaldisBasicsPlusAdvanced.Cache;
using BaldisBasicsPlusAdvanced.Game.Adjusters;
using BaldisBasicsPlusAdvanced.Game.Objects;
using UnityEngine;

namespace BaldisBasicsPlusAdvanced.Game.InventoryItems
{
    public class TeleportationBombItem : Item, IPrefab
    {
        [SerializeField]
        private SoundObject audDrop;

        [SerializeField]
        private Entity entity;

        [SerializeField]
        private CapsuleCollider capsuleCollider;

        [SerializeField]
        private float endHeight = 0.5f;

        [SerializeField]
        private float throwSpeed = 150f;

        [SerializeField]
        private float gravity = 15f;

        [SerializeField]
        private float height = 5f;

        private EnvironmentController ec;

        private bool ready;

        public void InitializePrefab(int variant)
        {
            audDrop = AssetStorage.sounds["banana_slip"];
            entity = new EntityAdjuster(gameObject)
                .SetName("Teleportation Bomb")
                .SetLayer("StandardEntities")
                .AddTrigger(1f)
                .SetLayerCollisionMask((LayerMask)2113541)
                .AddRigidbody()
                .AddDefaultRenderBaseFunction(AssetStorage.sprites["TeleportationBomb_Large"])
                .Build();
            entity.SetGrounded(false);
            capsuleCollider = GetComponent<CapsuleCollider>();
            capsuleCollider.height = 5f;
        }

        private void Update()
        {
            if (!ready)
            {
                height -= gravity * Time.deltaTime * ec.EnvironmentTimeScale;
                entity.UpdateInternalMovement(Vector3.zero);
                if (height <= endHeight)
                {
                    height = endHeight;
                    ready = true;
                    entity.SetGrounded(value: true);
                }
                entity.SetHeight(height);
            }

            if (ready)
            {
                TeleportationHole bomb = Instantiate(ObjectStorage.Objects["teleportation_hole"].GetComponent<TeleportationHole>());
                Vector3 pos = transform.position;
                pos.y = 5f;
                bomb.transform.position = pos;
                bomb.Initialize(ec, endsIn: 10f);
                Destroy(gameObject);
            }
        }

        public override bool Use(PlayerManager pm)
        {
            ec = pm.ec;
            entity.Initialize(pm.ec, pm.transform.position);
            entity.AddForce(new Force(Singleton<CoreGameManager>.Instance.GetCamera(pm.playerNumber).transform.forward, throwSpeed, 
                -throwSpeed));
            entity.CopyStatusEffects(pm.plm.Entity);
            CoreGameManager.Instance.audMan.PlaySingle(audDrop);
            pm.RuleBreak("Bullying", 1f);
            return true;
        }
    }
}