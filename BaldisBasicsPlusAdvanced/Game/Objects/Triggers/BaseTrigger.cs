using BaldisBasicsPlusAdvanced.Helpers;
using MTM101BaldAPI.UI;
using System;
using TMPro;
using UnityEngine;

namespace BaldisBasicsPlusAdvanced.Game.Objects.Triggers
{
    public class BaseTrigger : MonoBehaviour, IPrefab
    {
        [SerializeField]
        protected SpriteRenderer renderer;

        private EnvironmentController ec;

        public virtual void InitializePrefab(int variant)
        {

        }

        protected void CreateRenderer(Sprite sprite)
        {
            renderer = ObjectCreator.CreateSpriteRendererBase(sprite);
            renderer.transform.SetParent(transform, false);
        }

        private void Start()
        {
            ec = BaseGameManager.Instance.Ec;
            ec.OnEnvironmentBeginPlay += OnEnvBeginPlay;
            if (renderer != null) Destroy(renderer.gameObject);
            VirtualStart();
        }

        protected virtual void VirtualStart()
        {

        }

        protected virtual void OnEnvBeginPlay()
        {

        }
    }
}
