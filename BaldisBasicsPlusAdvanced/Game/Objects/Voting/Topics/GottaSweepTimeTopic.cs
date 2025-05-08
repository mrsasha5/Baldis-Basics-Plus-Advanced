using BaldisBasicsPlusAdvanced.Helpers;
using BaldisBasicsPlusAdvanced.Patches;
using UnityEngine;

namespace BaldisBasicsPlusAdvanced.Game.Objects.Voting.Topics
{
    public class GottaSweepTimeTopic : BaseTopic
    {

        public override string Desc => "Adv_Text_School_Council_Topic5".Localize();

        public override bool IsAvailable()
        {
            return base.IsAvailable() && ec.npcsToSpawn.Find(x => x is GottaSweep) != null;
        }

        public override void OnVotingEndedPost(bool isWin)
        {
            base.OnVotingEndedPost(isWin);
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
