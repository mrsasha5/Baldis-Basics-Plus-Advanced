using BaldisBasicsPlusAdvanced.Helpers;
using System;
using UnityEngine;

namespace BaldisBasicsPlusAdvanced.Game.Objects.Triggers
{
    public class BaseTrigger : MonoBehaviour, IPrefab
    {
        [SerializeField]
        private SpriteRenderer renderer;

        private EnvironmentController ec;

        protected virtual Sprite SpriteToSet => null;

        public void InitializePrefab(int variant)
        {
            if (SpriteToSet == null) return;
            renderer = ObjectsCreator.CreateSpriteRendererBase(SpriteToSet);
            renderer.transform.SetParent(transform, false);
        }

        private void Start()
        {
            ec = Singleton<BaseGameManager>.Instance.Ec;
            ec.OnEnvironmentBeginPlay += SafeInvoker;
            if (renderer != null) renderer.enabled = false;
        }

        private void SafeInvoker()
        {
            try
            {
                OnEnvBeginPlay();
            } catch (Exception e)
            {
                AdvancedCore.Logging.LogError(e);
            }
        }

        protected virtual void OnEnvBeginPlay()
        {

        }
    }
}
