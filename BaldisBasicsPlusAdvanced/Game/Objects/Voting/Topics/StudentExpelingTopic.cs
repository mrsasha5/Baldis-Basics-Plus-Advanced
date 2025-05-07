using BaldisBasicsPlusAdvanced.Helpers;
using BaldisBasicsPlusAdvanced.Patches;
using BaldisBasicsPlusAdvanced.Patches.UI.Elevator;
using BaldisBasicsPlusAdvanced.SavedData;
using MTM101BaldAPI.Registers;
using System.Collections.Generic;

namespace BaldisBasicsPlusAdvanced.Game.Objects.Voting.Topics
{
    public class StudentExpelingTopic : TopicBase
    {
        private string nameKey;

        private Character character;

        private bool isBanned;

        public override string Desc => string.Format("Adv_Text_School_Council_Topic1".Localize(), nameKey.Localize());

        public override bool SaveToNextFloors => true;

        public override void Initialize()
        {
            base.Initialize();
            List<NPC> potentialNPCs = ElevatorExpelHammerPatch.GetPotentialCharacters();
            NPC chosenNPC = potentialNPCs[ControlledRNG.Object.Next(0, potentialNPCs.Count)];

            nameKey = chosenNPC.GetMeta().nameLocalizationKey;
            character = chosenNPC.Character;
        }

        public override void OnLoadNextLevel(bool afterPit)
        {
            base.OnLoadNextLevel(afterPit);
            if (!afterPit && !isBanned)
            {
                isBanned = true;
                LevelDataManager.LevelData.BanCharacter(character, -1);
            }
        }

    }
}
