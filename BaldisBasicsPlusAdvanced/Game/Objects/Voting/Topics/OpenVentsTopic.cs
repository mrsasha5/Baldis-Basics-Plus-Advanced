using BaldisBasicsPlusAdvanced.Cache.AssetsManagement;
using BaldisBasicsPlusAdvanced.Helpers;
using BaldisBasicsPlusAdvanced.Patches;
using UnityEngine;

namespace BaldisBasicsPlusAdvanced.Game.Objects.Voting.Topics
{
    public class OpenVentsTopic : BaseTopic
    {

        public override string Desc => "Adv_SC_Topic_Ventilation".Localize();

        public override string BasicInfo => "Adv_SC_Topic_Ventilation_BasicInfo".Localize();

        public override bool IsAvailable()
        {
            return base.IsAvailable() && GameObject.FindObjectOfType<VentController>() != null;
        }

        public override BaseTopic Clone()
        {
            OpenVentsTopic topic = new OpenVentsTopic();
            CopyAllBaseValuesTo(topic);
            return topic;
        }

        public override void OnVotingEndedPre(bool isWin)
        {
            base.OnVotingEndedPre(isWin);
            if (isWin)
            {
                foreach (VentController controller in GameObject.FindObjectsOfType<VentController>())
                {
                    Animator entrance = controller.GetComponentInChildren<Animator>();
                    entrance.enabled = false;

                    entrance.GetComponentInChildren<MeshRenderer>().
                        transform.localRotation = Quaternion.Euler(90f, 0f, 0f);

                    EntitySucker sucker = new GameObject("Sucker").AddComponent<EntitySucker>();
                    sucker.Initialize(ec);
                    sucker.transform.SetParent(entrance.gameObject.transform, false);
                    sucker.maxForce = 40f;

                    AudioManager audMan = ObjectsCreator.CreatePropagatedAudMan(sucker.gameObject);
                    audMan.QueueAudio(AssetsStorage.sounds["vent_vacuum"]);
                    audMan.QueueAudio(AssetsStorage.sounds["vent_travel"]);
                    audMan.SetLoop(true);

                    sucker.CreateSphere(60f);
                    sucker.ignoreAirborne = true; //It's made for you, my "favourite" Buglloons!
                }
            }
        }
    }
}
