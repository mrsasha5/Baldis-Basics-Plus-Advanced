using System.Collections.Generic;
using System.EnterpriseServices;
using BaldisBasicsPlusAdvanced.Cache.AssetsManagment;
using BaldisBasicsPlusAdvanced.Compats.SpatialElevator.Objects.Interaction;
using BaldisBasicsPlusAdvanced.Helpers;
using BaldisBasicsPlusAdvanced.Patches.UI.Elevator;
using MTM101BaldAPI.Registers;
using MTM101BaldAPI.UI;
using TMPro;
using UnityEngine;

namespace BaldisBasicsPlusAdvanced.Compats.SpatialElevator.Objects
{
    public class SpatialChalkboard : MonoBehaviour
    {

        private AudioManager audMan;

        private MeshRenderer renderer;

        private List<IInteractionObject> allElements = new List<IInteractionObject>();

        private List<InteractionTextObject> buttons = new List<InteractionTextObject>();

        private static SpatialChalkboard instance;

        public static void OnGameManagerInit(BaseGameManager gameManager)
        {
            if (instance != null)
            {

                switch (ElevatorExpelHammerPatch.GetStatus(gameManager))
                {
                    case ElevatorExpelHammerPatch.Status.Available:
                        ElevatorExpelHammerPatch.CreatePages();
                        HideAll(false, animation: true);
                        UpdateList(ElevatorExpelHammerPatch.CurrentPage, animation: true);
                        break;
                    case ElevatorExpelHammerPatch.Status.ShouldBreak:
                        instance.audMan.PlaySingle(AssetsStorage.sounds["bal_break"]);
                        break;
                    default:
                        break;
                }
            }

            //Adv_Expel_Hammer_Pages
        }

        public void Initialize()
        {
            const float distanceOffset = 0.05f;

            instance = this;

            audMan = ObjectsCreator.CreateAudMan(gameObject);
            audMan.ignoreListenerPause = true;
            audMan.useUnscaledPitch = true;

            renderer = ObjectsCreator.CreateQuadRenderer();
            renderer.material.mainTexture = AssetsStorage.sprites["chalkboard_standard"].texture;
            renderer.transform.SetParent(transform, false);
            renderer.transform.localRotation = Quaternion.Euler(0f, 180f, 0f);
            renderer.transform.localScale = new Vector3(25f, 20f, 1f);

            float height = 2.5f;
            float offset = -2.5f;

            Vector3 newArrowSize = new Vector3(64f, 64f, 2f);

            TextMeshPro titleText = 
                ObjectsCreator.CreateSpatialText(
                    BaldiFonts.ComicSans12, new Vector2(20f, 2.5f), transform, new Vector3(0f, 5.5f, distanceOffset));
            titleText.name = "Title";
            titleText.text = string.Format(LocalizationManager.Instance.GetLocalizedText("Adv_ExpelHammer_Character_Info"), 
                ElevatorExpelHammerPatch.FloorsToUnbanValue);
            titleText.transform.localScale = new Vector3(0.8f, 0.8f, 1f);
            allElements.Add(titleText.gameObject.AddComponent<InteractionTextObject>()
                .Assign()
                .SetBoxColliderAppropriateSize());

            SpriteRenderer renderer1 = ObjectsCreator.CreateSpriteRenderer(null, false);
            renderer1.gameObject.transform.SetParent(transform, false);
            renderer1.gameObject.transform.localPosition = new Vector3(-7.8f, -5.5f, distanceOffset);
            InteractionSpriteRendererObject arrowLeft = 
                renderer1.gameObject.AddComponent<InteractionSpriteRendererObject>()
                .Assign()
                .SetSprites(AssetsStorage.sprites["menuArrow0"], AssetsStorage.sprites["menuArrow2"])
                .SetDefaultParameters();
            renderer1.transform.localScale = new Vector3(0.07f, 0.07f, 1f);
            renderer1.GetComponent<BoxCollider>().size = newArrowSize;
            allElements.Add(arrowLeft);

            arrowLeft.onClick =
                delegate ()
                {
                    ElevatorExpelHammerPatch.SetBackPage();
                    UpdateList(ElevatorExpelHammerPatch.CurrentPage, animation: false);
                };

            SpriteRenderer renderer2 = ObjectsCreator.CreateSpriteRenderer(null, false);
            renderer2.gameObject.transform.SetParent(transform, false);
            renderer2.gameObject.transform.localPosition = new Vector3(7.8f, -5.5f, distanceOffset);
            InteractionSpriteRendererObject arrowRight = 
                renderer2.gameObject.AddComponent<InteractionSpriteRendererObject>()
                .Assign()
                .SetSprites(AssetsStorage.sprites["menuArrow1"], AssetsStorage.sprites["menuArrow3"])
                .SetDefaultParameters();
            renderer2.transform.localScale = renderer1.transform.localScale;
            renderer2.GetComponent<BoxCollider>().size = newArrowSize;
            allElements.Add(arrowRight);

            arrowRight.onClick =
                delegate ()
                {
                    ElevatorExpelHammerPatch.SetNextPage();
                    UpdateList(ElevatorExpelHammerPatch.CurrentPage, animation: false);
                };

            for (int i = 0; i < ElevatorExpelHammerPatch.MaxCountOnPage; i++)
            {
                
                TextMeshPro tmpText = 
                    ObjectsCreator.CreateSpatialText(BaldiFonts.ComicSans12, new Vector2(16f, 2.5f), 
                        transform, new Vector3(0f, height, distanceOffset));

                InteractionTextObject textObj = tmpText.gameObject.AddComponent<InteractionTextObject>()
                    .Assign()
                    .SetDefaultParameters();

                tmpText.name = "CharacterText";
                tmpText.text = "OHNO!";

                height += offset;

                allElements.Add(textObj);
                buttons.Add(textObj);
            }

            HideAll(true, animation: false);
        }

        public static void HideAll(bool state, bool animation)
        {
            foreach (IInteractionObject obj in instance.allElements)
            {
                obj.Hide(state, animation);
            }
        }

        public static void UpdateList(List<NPC> npcs, bool animation)
        {
            for (int i = 0; i < instance.buttons.Count; i++)
            {
                instance.buttons[i].onClick = null;
                instance.buttons[i].Hide(true, animation: false);
            }

            for (int i = 0; i < npcs.Count; i++)
            {
                Character character = npcs[i].Character;
                instance.buttons[i].onClick =
                    delegate()
                    {
                        ElevatorExpelHammerPatch.HitCharacter(character);
                        HideAll(true, animation: true);
                        instance.audMan.PlaySingle(AssetsStorage.sounds["adv_boing"]);
                    };
                instance.buttons[i].Tmp.text = LocalizationManager.Instance.GetLocalizedText(npcs[i].GetMeta().nameLocalizationKey);
                instance.buttons[i].Hide(false, animation: animation);
            }
        }

        
    }
}
