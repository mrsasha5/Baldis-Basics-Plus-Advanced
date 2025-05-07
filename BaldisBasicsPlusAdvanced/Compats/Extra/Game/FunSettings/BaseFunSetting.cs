using UnityEngine;

namespace BaldisBasicsPlusAdvanced.Compats.Extra.Game.FunSettings
{
    public class BaseFunSetting : MonoBehaviour
    {
        protected bool initialized;

        protected EnvironmentController ec;

        //protected System.Random rng;

        private void Awake()
        {
            ec = Singleton<BaseGameManager>.Instance.Ec;
            ec.OnEnvironmentBeginPlay += OnEnvBeginPlay;
            //rng = new System.Random(Singleton<CoreGameManager>.Instance.Seed());
        }

        public virtual void Initialize()
        {
            initialized = true;
        }

        protected virtual void VirtualOnDestroy()
        {

        }

        private void Update()
        {
            VirtualUpdate();
        }

        private void OnDestroy()
        {
            VirtualOnDestroy();
        }

        protected virtual void VirtualUpdate()
        {

        }

        protected virtual void OnEnvBeginPlay()
        {
            if (!initialized) Initialize();
        }

    }
}
