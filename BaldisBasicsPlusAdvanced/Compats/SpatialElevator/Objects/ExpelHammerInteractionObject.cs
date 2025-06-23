using BaldisBasicsPlusAdvanced.Helpers;
using BaldisBasicsPlusAdvanced.Patches.UI.Elevator;
using BaldisBasicsPlusAdvanced.Patches;
using UnityEngine;
using BaldisBasicsPlusAdvanced.Cache.AssetsManagment;
using The3DElevator.MonoBehaviours;

namespace BaldisBasicsPlusAdvanced.Compats.SpatialElevator.Objects
{
    public class ExpelHammerInteractionObject : MonoBehaviour, IClickable<int>
    {

        private AudioManager audMan;

        private SpriteRenderer renderer;

        private bool interactionEnabled;

        private static ExpelHammerInteractionObject instance;

        public static void OnGameManagerInit(BaseGameManager gameManager)
        {
            if (instance != null)
            {
                switch (ElevatorExpelHammerPatch.GetStatus(gameManager))
                {
                    case ElevatorExpelHammerPatch.Status.Available:
                        instance.interactionEnabled = true;
                        break;
                    case ElevatorExpelHammerPatch.Status.ShouldBreak:
                        instance.renderer.enabled = false;
                        instance.audMan.PlaySingle(AssetsStorage.sounds["bal_break"]);
                        break;
                    default:
                        break;
                }
            }
        }

        public void Initialize()
        {
            instance = this;

            audMan = ObjectsCreator.CreateAudMan(gameObject);
            audMan.ignoreListenerPause = true;
            audMan.useUnscaledPitch = true;

            renderer = ObjectsCreator.CreateSpriteRenderer(AssetsStorage.sprites["adv_expel_hammer"]);
            renderer.transform.SetParent(transform, false);
            renderer.transform.localScale = Vector3.one * 5f;

            SphereCollider collider = gameObject.AddComponent<SphereCollider>();
            collider.isTrigger = true;
            collider.radius = 5;

        }

        public void Clicked(int player)
        {
            if (!ClickableHidden())
            {
                Canvas canvas = ObjectsCreator.CreateCanvas(setGlobalCam: false);

                canvas.referencePixelsPerUnit = 100f;
                canvas.SetCursorInitiator(setAutoInitiator: true);

                ElevatorExpelHammerPatch.CreateMenuIn(canvas);
            }
        }

        public bool ClickableHidden()
        {
            return !interactionEnabled || GlobalCam.Instance.TransitionActive;
        }

        public bool ClickableRequiresNormalHeight()
        {
            return false;
        }

        public void ClickableSighted(int player)
        {
            
        }

        public void ClickableUnsighted(int player)
        {

        }

        
    }
}
