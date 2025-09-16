using BaldisBasicsPlusAdvanced.Extensions;
using BaldisBasicsPlusAdvanced.Helpers;
using UnityEngine;

namespace BaldisBasicsPlusAdvanced.Game.Objects.Voting.Topics
{
    public class GottaSweepTimeTopic : BaseTopic
    {

        public override string Desc => "Adv_SC_Topic_GottaSweep".Localize();

        public override string BasicInfo => "Adv_SC_Topic_GottaSweep_BasicInfo".Localize();

        public override bool IsAvailable()
        {
            return base.IsAvailable() && ec.npcsToSpawn.Find(x => x is GottaSweep) != null;
        }

        public override void OnVotingEndedPre(bool isWin)
        {
            base.OnVotingEndedPre(isWin);
            if (isWin)
            {
                foreach (GottaSweep gottaSweep in GameObject.FindObjectsOfType<GottaSweep>())
                {
                    ReflectionHelper.SetValue<float>(gottaSweep, "minDelay", 0f);
                    ReflectionHelper.SetValue<float>(gottaSweep, "maxDelay", 0f);
                    ReflectionHelper.SetValue<float>(gottaSweep, "minActive", float.MaxValue);
                    ReflectionHelper.SetValue<float>(gottaSweep, "maxActive", float.MaxValue);
                    if (gottaSweep.behaviorStateMachine.currentState is GottaSweep_Wait)
                    {
                        new GottaSweep_SweepingTime(gottaSweep, gottaSweep);
                    }
                }
            }
        }

        public override BaseTopic Clone()
        {
            GottaSweepTimeTopic topic = new GottaSweepTimeTopic();
            CopyAllBaseValuesTo(topic);
            return topic;
        }
    }
}
