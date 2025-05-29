namespace BaldisBasicsPlusAdvanced.Game.Rooms.Functions
{
    public class DisabledPowerOnGenerationFinishedFunction : RoomFunction
    {

        public override void OnGenerationFinished()
        {
            base.OnGenerationFinished();
            room.SetPower(false);
        }

    }
}
