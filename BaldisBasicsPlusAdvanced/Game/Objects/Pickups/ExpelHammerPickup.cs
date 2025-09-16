using BaldisBasicsPlusAdvanced.Cache;
using BaldisBasicsPlusAdvanced.Cache.AssetsManagement;
using BaldisBasicsPlusAdvanced.Extensions;
using BaldisBasicsPlusAdvanced.Game.Components.UI.Menu;
using BaldisBasicsPlusAdvanced.Helpers;
using BaldisBasicsPlusAdvanced.SaveSystem;
using MTM101BaldAPI.UI;
using TMPro;
using UnityEngine;

namespace BaldisBasicsPlusAdvanced.Game.Objects.Pickups
{
    public class ExpelHammerPickup : BasePickup
    {
        private ChalkboardMenu chalkboardMenu;

        private bool openedMenu;

        private PlayerManager pm;

        private Quaternion rotationOnCloseMenu;

        private MovementModifier zeroMod = new MovementModifier(Vector3.zero, 0f);

        protected override void OnCreationPost()
        {
            renderer.sprite = AssetsStorage.sprites["adv_expel_hammer"];
            purchasable = !LevelDataManager.LevelData.hammerPurchased;
            SetSaleState(purchasable);
            desc = "Adv_Item_ExpelHammer_Desc";
        }

        protected override void VirtualUpdate()
        {
            if (!openedMenu) return;

            if (pm == null) pm = Singleton<CoreGameManager>.Instance.GetPlayer(0);

            if (Vector3.Distance(pm.transform.position, transform.position) > 20f)
            {
                Singleton<BaseGameManager>.Instance.Ec.GetAudMan().PlaySingle(AssetsStorage.sounds["bal_break"]);
                DestroyMenu(0);
            }
        }

        protected override void VirtualClicked(int player)
        {
            base.VirtualClicked(player);
            if (!purchasable)
            {
                Singleton<CoreGameManager>.Instance.GetPlayer(player).GetComponent<Entity>().ExternalActivity.moveMods.Add(zeroMod);

                //lock camera
                Singleton<CoreGameManager>.Instance.GetCamera(player).matchTargetRotation = false;
                rotationOnCloseMenu = Singleton<CoreGameManager>.Instance.GetPlayer(player).transform.rotation;

                Singleton<CoreGameManager>.Instance.GetHud(player).CloseTooltip();
                if (!openedMenu) CreateMenu(player);
                openedMenu = true;
            }
        }

        public override bool ClickableHidden()
        {
            return base.ClickableHidden() || openedMenu;
        }

        public override void OnPurchasing(int spentYTPs)
        {
            base.OnPurchasing(spentYTPs);
            LevelDataManager.LevelData.BuyExpelHammer(spentYTPs);
        }

        private void CreateMenu(int player)
        {
            InitializeMenu(player);
        }

        private void InitializeMenu(int player)
        {
            int boughtHammerPrice = (int)(LevelDataManager.LevelData.boughtHammerPrice * 0.85f);

            chalkboardMenu = Instantiate(ObjectsStorage.Objects["chalkboard_menu"].GetComponent<ChalkboardMenu>());

            Destroy(CursorController.Instance.gameObject); //reinit cursor

            chalkboardMenu.GetText("title").text = Singleton<LocalizationManager>.Instance.GetLocalizedText("Adv_Item_ExpelHammer");
            chalkboardMenu.GetText("info").text = string.Format(
                Singleton<LocalizationManager>.Instance.GetLocalizedText("Adv_Item_ExpelHammer_ReturnPurchase"),
                    boughtHammerPrice
                );

            chalkboardMenu.GetButton("exit").OnPress.AddListener(
                delegate ()
                {
                    DestroyMenu(player);
                }
            );

            TMP_Text agreeText = UIHelpers.CreateText<TextMeshProUGUI>(BaldiFonts.ComicSans24,
                Singleton<LocalizationManager>.Instance.GetLocalizedText("Adv_Text_IAgree"),
                chalkboardMenu.chalkboard.transform, Vector3.up * -70f, false);

            Vector2 size = new Vector2(100, 50);
            agreeText.GetComponent<RectTransform>().sizeDelta = size;
            agreeText.alignment = TextAlignmentOptions.Top;

            StandardMenuButton agreeButton = ObjectsCreator.AddButtonProperties(agreeText, size, true);

            agreeButton.OnPress.AddListener(delegate ()
            {
                LevelDataManager.LevelData.BuyExpelHammer(0, cancelPurchase: true);
                DestroyMenu(player);
                Singleton<CoreGameManager>.Instance.AddPoints(boughtHammerPrice, 0, true);
                SetSaleState(true);
            });

            Singleton<GlobalCam>.Instance.FadeIn(UiTransition.Dither, 0.01666667f);

        }

        private void DestroyMenu(int player)
        {
            Singleton<GlobalCam>.Instance.Transition(UiTransition.Dither, 0.01666667f);
            Destroy(chalkboardMenu.canvas.gameObject);
            Singleton<CoreGameManager>.Instance.GetPlayer(player).GetComponent<Entity>().ExternalActivity.moveMods.Remove(zeroMod);

            //unlock camera
            Singleton<CoreGameManager>.Instance.GetCamera(player).matchTargetRotation = true;
            Singleton<CoreGameManager>.Instance.GetPlayer(player).transform.rotation = rotationOnCloseMenu;

            openedMenu = false;
            nonClickableTime = 1f;
        }
    }
}
