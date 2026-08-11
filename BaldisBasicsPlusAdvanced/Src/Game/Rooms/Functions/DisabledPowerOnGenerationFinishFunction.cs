namespace BaldisBasicsPlusAdvanced.Game.Rooms.Functions
{
    public class DisabledPowerOnGenerationFinishFunction : RoomFunction
    {

        public override void OnGenerationFinished()
        {
            base.OnGenerationFinished();
            room.SetPower(false);
        }

    }
}
