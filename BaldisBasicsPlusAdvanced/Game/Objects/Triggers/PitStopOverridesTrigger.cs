using BaldisBasicsPlusAdvanced.Game.Objects.Plates;

namespace BaldisBasicsPlusAdvanced.Game.Objects.Triggers
{
    public class PitStopOverridesTrigger : BaseTrigger
    {

        protected override void OnEnvBeginPlay()
        {
            base.OnEnvBeginPlay();
            foreach (AccelerationPlate plate in FindObjectsOfType<AccelerationPlate>())
            {
                plate.initialSpeed = 90f;
                plate.Data.timeToUnpress = 1f;
            }
        }

    }
}
