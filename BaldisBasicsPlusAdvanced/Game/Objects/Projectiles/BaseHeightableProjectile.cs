using UnityEngine;

namespace BaldisBasicsPlusAdvanced.Game.Objects.Projectiles
{
    public class BaseHeightableProjectile : BaseProjectile
    {

        [SerializeField]
        protected float heightMin;

        [SerializeField]
        protected float heightMax;

        [SerializeField]
        protected float rotationSpeed;

        [SerializeField]
        protected float heightSpeed;

        [SerializeField]
        protected bool increasesHeight;

        protected float height;

        protected float rotationAngle;

        public override void InitializePrefab(int variant)
        {
            base.InitializePrefab(1);
            SetHeightValues(min: 2f, max: 8f);
            rotationSpeed = 720f;
            heightSpeed = 7f;
            increasesHeight = true;
        }

        public void SetHeightValues(float min, float max)
        {
            heightMin = min;
            heightMax = max;
        }

        public void SetHeight(float height)
        {
            this.height = height;
            entity.SetHeight(height);
        }

        protected override void VirtualUpdate()
        {
            base.VirtualUpdate();

            if (flying)
            {
                if (increasesHeight)
                {
                    height += Time.deltaTime * ec.EnvironmentTimeScale * heightSpeed;
                    entity.SetHeight(height);

                    if (height > heightMax) increasesHeight = false;
                }
                else
                {
                    height += Time.deltaTime * ec.EnvironmentTimeScale * -heightSpeed;
                    entity.SetHeight(height);

                    if (height < heightMin) Destroy();
                }

                if (rotationAngle >= 360f) rotationAngle = 0f;

                rotationAngle += Time.deltaTime * ec.EnvironmentTimeScale * rotationSpeed;

                renderer.SetSpriteRotation(rotationAngle);
            }

        }

        protected virtual void Destroy()
        {
            SetFlying(false);
            Destroy(gameObject);
        }
    }
}
