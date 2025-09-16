using System.Collections.Generic;
using BaldisBasicsPlusAdvanced.Cache.AssetsManagement;
using BaldisBasicsPlusAdvanced.Extensions;
using BaldisBasicsPlusAdvanced.Helpers;
using UnityEngine;

namespace BaldisBasicsPlusAdvanced.Game.Objects.Voting.Topics
{
    public class OpenVentsTopic : BaseTopic
    {

        private List<VentController> vents;

        private float time;

        public override string Desc => "Adv_SC_Topic_Ventilation".Localize();

        public override string BasicInfo => "Adv_SC_Topic_Ventilation_BasicInfo".Localize();

        public override bool IsAvailable()
        {
            return base.IsAvailable() && GameObject.FindObjectOfType<VentController>() != null;
        }

        public override void Update()
        {
            base.Update();
            if (Active && time > 0f)
            {
                time -= Time.deltaTime * ec.EnvironmentTimeScale;
                if (time <= 0f)
                {
                    foreach (VentController controller in vents)
                    {
                        Animator entrance = controller.GetComponentInChildren<Animator>();
                        entrance.enabled = true;

                        /*entrance.GetComponentInChildren<MeshRenderer>().
                            transform.localRotation = Quaternion.Euler(0f, 0f, 0f);*/

                        GameObject.Destroy(entrance.gameObject.GetComponentInChildren<AdvancedEntityPull>().gameObject);
                    }
                }
            }
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
                time = Random.Range(120f, 180f);
                vents = new List<VentController>();
                foreach (VentController controller in GameObject.FindObjectsOfType<VentController>())
                {
                    Animator entrance = controller.GetComponentInChildren<Animator>();
                    entrance.enabled = false;

                    entrance.GetComponentInChildren<MeshRenderer>().
                        transform.localRotation = Quaternion.Euler(90f, 0f, 0f);

                    AdvancedEntityPull pull = new GameObject("EntityPull").AddComponent<AdvancedEntityPull>();
                    pull.Initialize(ec);
                    pull.transform.SetParent(entrance.gameObject.transform, false);
                    pull.maxForce = 40f;

                    AudioManager audMan = ObjectsCreator.CreatePropagatedAudMan(pull.gameObject);
                    audMan.QueueAudio(AssetsStorage.sounds["vent_vacuum"]);
                    audMan.QueueAudio(AssetsStorage.sounds["vent_travel"]);
                    audMan.SetLoop(true);

                    pull.CreateSphere(60f);
                    vents.Add(controller);
                }
            }
        }

    }
}
