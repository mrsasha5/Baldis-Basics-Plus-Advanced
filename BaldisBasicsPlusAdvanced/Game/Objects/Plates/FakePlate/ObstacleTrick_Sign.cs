using BaldisBasicsPlusAdvanced.Cache.AssetsManagment;
using System.Collections.Generic;
using UnityEngine;

namespace BaldisBasicsPlusAdvanced.Game.Objects.Plates.FakePlate
{
    public class ObstacleTrick_Sign : MonoBehaviour
    {

        //private List<MovementModifier> moveMods = new List<MovementModifier>();

        private MysteriousPlate plate;

        private Collider collider;

        private SpriteRenderer renderer;

        public void Initialize(MysteriousPlate plate, Collider collider, SpriteRenderer renderer)
        {
            this.plate = plate;
            this.collider = collider;
            this.renderer = renderer;
        }
        
        public void Break()
        {
            Destroy(collider.gameObject);
            Destroy(renderer.transform.parent.gameObject);
            plate.AudMan.PlaySingle(AssetsStorage.sounds["bal_break"]);
            plate.EndTrick();
        }

        /*private void OnCollisionEnter(Collision collision)
        {
            if (collision.collider.isTrigger && collision.collider.TryGetComponent(out Entity entity))
            {

            }
        }*/


    }
}
