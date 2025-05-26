namespace BaldisBasicsPlusAdvanced.Game.NPCs.CrissTheCrystal
{
    public class CrissTheCrystal_StateBase : NpcState
    {
        protected CrissTheCrystal criss;

        public CrissTheCrystal_StateBase(CrissTheCrystal criss) : base(criss)
        {
            this.criss = criss;
        }
    }
}
