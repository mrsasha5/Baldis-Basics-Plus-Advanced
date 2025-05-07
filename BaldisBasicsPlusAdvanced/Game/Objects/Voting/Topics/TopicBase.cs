
namespace BaldisBasicsPlusAdvanced.Game.Objects.Voting.Topics
{

    public class TopicBase
    {
        public virtual string Desc => "";

        public virtual bool SaveToNextFloors => false;

        //public virtual bool ActivatesImmediately => false;

        public virtual void Initialize()
        {

        }

        public virtual void OnEnvBeginPlay(EnvironmentController ec)
        {

        }

        public virtual void OnVotingEndedSuccessfully()
        {

        }

        public virtual void OnLoadNextLevel(bool afterPit)
        {

        }

        public virtual void OnDestroying()
        {

        }

        public virtual void OnLoad()
        {

        }
        public virtual void OnSave()
        {

        }

    }
}
