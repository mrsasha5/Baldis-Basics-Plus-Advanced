using System;
using UnityEngine;

namespace BaldisBasicsPlusAdvanced.Game.Objects.Plates.FakePlate
{
    public class BaseTrick : MonoBehaviour
    {
        private MysteriousPlate plate;
        
        protected bool activated;

        public MysteriousPlate Plate => plate;

        public void Initialize(MysteriousPlate plate)
        {
            this.plate = plate;
        }

        public virtual void OnPostInitialization()
        {

        }

        public virtual void VirtualUpdate()
        {

        }

        public virtual void OnPress()
        {

        }

        public virtual void OnUnpress()
        {

        }

        public virtual void Reset()
        {

        }

        public virtual bool OnEntityCatched(Entity entity)
        {
            return false;
        }

        public virtual void OnEntityEnterTargetZone(Entity entity)
        {

        }

        public virtual void OnEntityStayTargetZone(Entity entity)
        {

        }

        public virtual void OnEntityExitTargetZone(Entity entity)
        {

        }

        public virtual bool IsSelectable()
        {
            return true;
        }

    }
}
