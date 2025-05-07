using BaldisBasicsPlusAdvanced.Game.Objects.Plates;

namespace BaldisBasicsPlusAdvanced.Game.Objects.Triggers
{
    public class PitStopOverridesTrigger : TriggerBase
    {

        protected override void OnEnvBeginPlay()
        {
            base.OnEnvBeginPlay();
            foreach (AccelerationPlate plate in FindObjectsOfType<AccelerationPlate>())
            {
                plate.initialSpeed = 90f;
                plate.timeToUnpress = 1f;
            }
        }

    }
}
