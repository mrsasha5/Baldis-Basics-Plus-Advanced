using BaldisBasicsPlusAdvanced.Extensions;
using UnityEngine;

namespace BaldisBasicsPlusAdvanced.Game.Objects.Voting.Topics
{
    public class DisabledFacultyLockdownDoorsTopic : BaseTopic
    {

        public override string Desc => "Adv_SC_Topic_FacultySuperDoors".Localize();

        public override string BasicInfo => "Adv_SC_Topic_FacultySuperDoors_BasicInfo".Localize();

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
                door.gameObject.GetComponent<Collider>().enabled = false;
                foreach (SpriteRenderer renderer in door.gameObject.GetComponentsInChildren<SpriteRenderer>())
                {
                    renderer.color = color;
                }
            }
        }

    }
}
