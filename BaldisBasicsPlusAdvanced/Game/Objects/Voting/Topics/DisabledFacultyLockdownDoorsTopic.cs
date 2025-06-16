using BaldisBasicsPlusAdvanced.Patches;
using UnityEngine;

namespace BaldisBasicsPlusAdvanced.Game.Objects.Voting.Topics
{
    public class DisabledFacultyLockdownDoorsTopic : BaseTopic
    {

        public override string Desc => "Adv_Text_School_Council_Topic10".Localize();

        public override bool IsAvailable()
        {
            return base.IsAvailable() && GameObject.FindObjectOfType<FacultyOnlyDoor>();
        }

        public override BaseTopic Clone()
        {
            DisabledFacultyLockdownDoorsTopic topic = new DisabledFacultyLockdownDoorsTopic();
            CopyAllBaseValuesTo(topic);
            return topic;
        }

        public override void OnVotingEndedPre(bool isWin)
        {
            base.OnVotingEndedPre(isWin);
            Color color = new Color(0.25f, 0.25f, 0.25f, 1f);
            foreach (FacultyOnlyDoor door in GameObject.FindObjectsOfType<FacultyOnlyDoor>())
            {
                if (!door.IsOpen)
                    door.Open(cancelTimer: true, makeNoise: false);
                door.gameObject.GetComponent<Collider>().enabled = false;
                foreach (SpriteRenderer renderer in door.gameObject.GetComponentsInChildren<SpriteRenderer>())
                {
                    renderer.color = color;
                }
            }
        }

    }
}
