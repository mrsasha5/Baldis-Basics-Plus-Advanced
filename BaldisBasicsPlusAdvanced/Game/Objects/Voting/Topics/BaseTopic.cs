using BaldisBasicsPlusAdvanced.Game.Events;
using BaldisBasicsPlusAdvanced.Patches;
using UnityEngine;

namespace BaldisBasicsPlusAdvanced.Game.Objects.Voting.Topics
{
    public abstract class BaseTopic
    {
        protected EnvironmentController ec;

        protected VotingEvent eventInstance;

        private bool active;

        protected bool marksAsActiveOnEndedPost = true;

        public bool Active => active;

        public virtual string Desc => "No description";

        public void Initialize(VotingEvent votingEvent, EnvironmentController ec)
        {
            eventInstance = votingEvent;
            this.ec = ec;
        }

        /// <summary>
        /// Invokes if topic's instance was taken by voting event. It happens during generator works.
        /// </summary>
        public virtual void OnBringUp(System.Random rng)
        {

        }

        public virtual void OnEnvironmentBeginPlay()
        {

        }

        /// <summary>
        /// Updates if Unity invokes Update() from the Voting event.
        /// </summary>
        public virtual void Update()
        {

        }

        /// <summary>
        /// Updates if Unity invokes LateUpdate() from the Voting event.
        /// </summary>
        public virtual void LateUpdate()
        {

        }

        /// <summary>
        /// Invokes after showing voting result.
        /// </summary>
        /// <param name="isWin"></param>
        public virtual void OnVotingEndedPost(bool isWin)
        {
            if (isWin) active = true;
        }
        
        /// <summary>
        /// Invokes when voting event ends.
        /// </summary>
        /// <param name="isWin"></param>
        public virtual void OnVotingEndedPre(bool isWin)
        {
            if (isWin && !marksAsActiveOnEndedPost) active = true;
        }

        /// <summary>
        /// Condition that let know to the Voting event, if it can take it now.
        /// Invokes while generator works.
        /// </summary>
        /// <returns></returns>
        public virtual bool IsAvailable()
        {
            return true;
        }

        /// <summary>
        /// Invokes after destroying Voting event instance.
        /// </summary>
        public virtual void Reset()
        {
            active = false;
        }

        protected void CopyAllBaseValuesTo(BaseTopic topic)
        {
            topic.marksAsActiveOnEndedPost = marksAsActiveOnEndedPost;
        }

        /// <summary>
        /// Used to creating a clone of the registered topic.
        /// Invoke here <see cref="CopyAllBaseValuesTo(BaseTopic)"/> also.
        /// </summary>
        /// <returns></returns>
        public abstract BaseTopic Clone();

    }
}
