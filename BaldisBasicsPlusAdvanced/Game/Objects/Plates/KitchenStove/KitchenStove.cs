using BaldisBasicsPlusAdvanced.Cache.AssetsManagment;
using BaldisBasicsPlusAdvanced.Game.Components;
using BaldisBasicsPlusAdvanced.Game.Components.UI;
using BaldisBasicsPlusAdvanced.Game.Objects.Plates.Base;
using BaldisBasicsPlusAdvanced.Helpers;
using BaldisBasicsPlusAdvanced.Patches;
using BaldisBasicsPlusAdvanced.SerializableData;
using MTM101BaldAPI;
using MTM101BaldAPI.Registers;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.ParticleSystem;

namespace BaldisBasicsPlusAdvanced.Game.Objects.Plates.KitchenStove
{
    public class KitchenStove : BasePlate, IButtonReceiver
    {
        [SerializeField]
        private SoundObject audBurningStart;

        [SerializeField]
        private SoundObject audBurningLoop;

        [SerializeField]
        private SoundObject audBurningEnd;

        [SerializeField]
        private NavMeshObstacle obstacle;

        [SerializeField]
        private ParticleSystem particleSystem;

        [SerializeField]
        private InteractionObject interaction;

        private static int maxFoodCount = 4;

        private static List<FoodRecipeData> datas = new List<FoodRecipeData>();

        [SerializeField]
        private float coolingTime;

        [SerializeField]
        private float burningTime;

        [SerializeField]
        private bool activeStateOverridden;

        private List<Pickup> pickups;

        private FoodRecipeData currentRecipe;

        private float activeTime;

        private bool available;

        private bool active;

        private bool isHot;

        //private bool updatesColor;

        private Cell cell;

        public static int MaxFoodCount => maxFoodCount;

        internal static List<FoodRecipeData> Datas => datas;

        protected override bool UpdatesPressedState => false;

        protected override bool VisualPressedStateOverridden => true;

        protected override bool VisualActiveStateOverridden => activeStateOverridden;

        public override bool IsUsable => base.IsUsable && !active && available;

        public override void InitializePrefab(int variant)
        {
            base.InitializePrefab(variant);

            obstacle = gameObject.AddComponent<NavMeshObstacle>();
            obstacle.shape = NavMeshObstacleShape.Box;
            obstacle.size = new Vector3(10f, 10f, 10f);
            obstacle.carving = true; //let's cut out nav mesh!
            obstacle.enabled = false;

            audBurningStart = AssetsStorage.sounds["adv_burning_start"];
            audBurningLoop = AssetsStorage.sounds["adv_burning_loop"];
            audBurningEnd = AssetsStorage.sounds["adv_pah"];//AssetsStorage.sounds["adv_burning_end"];

            particleSystem = new GameObject("ParticleSystem").AddComponent<ParticleSystem>();

            particleSystem.transform.SetParent(transform);
            particleSystem.transform.localPosition = Vector3.up * -5f;

            ParticleSystemRenderer renderer = particleSystem.GetComponent<ParticleSystemRenderer>();
            renderer.material = AssetsHelper.LoadAsset<Material>("DustTest");
            renderer.material.shader = AssetsStorage.graphsStandardShader;
            renderer.material.SetColor(new Color(0.88f, 0.34f, 0.13f));

            MainModule main = particleSystem.main;
            main.playOnAwake = false;
            main.gravityModifier = -4f;
            // main.startLifetimeMultiplier = 2.1f;
            main.startLifetime = 2f;
            main.startSize = 3f;

            main.startSpeed = 0f; //no!!!! I need to have a normal up direction. 

            ShapeModule shape = particleSystem.shape;
            shape.shapeType = ParticleSystemShapeType.Sphere;
            shape.radius = 5f;

            EmissionModule emission = particleSystem.emission;
            emission.enabled = true;
            emission.rateOverTime = 80f;

            VelocityOverLifetimeModule velocityOverLifetime = particleSystem.velocityOverLifetime;
            velocityOverLifetime.enabled = true;
            velocityOverLifetime.y = 24f;
            velocityOverLifetime.radialMultiplier = 1.5f;

            //bruh
            /*SizeOverLifetimeModule sizeOverLifetime = particleSystem.sizeOverLifetime;
            sizeOverLifetime.enabled = true;
            sizeOverLifetime.separateAxes = true;

            AnimationCurve curve = new AnimationCurve();
            curve.AddKey(0f, 3f);
            curve.AddKey(1.5f, 0f);

            sizeOverLifetime.z = new MinMaxCurve(1f, curve);*/

            burningTime = 10f;
            coolingTime = 10f;
            activeStateOverridden = true;

            interaction = new GameObject("InteractionObject").AddComponent<InteractionObject>();
            interaction.transform.SetParent(transform, false);
            interaction.transform.localPosition = Vector3.up * 5f;

            SphereCollider collider = interaction.gameObject.AddComponent<SphereCollider>();
            collider.radius = 3f;
            collider.isTrigger = true;

            interaction.stove = this;
            interaction.gameObject.SetActive(false);
        }

        public void ConnectButton(GameButtonBase button)
        {
        }

        public static void LoadRecipesFromAssets()
        {
            string[] filesPath = Directory.GetFiles(AssetsHelper.modPath + "Premades/Recipes/KitchenStove/", 
                "*.json", SearchOption.AllDirectories);

            int ignoredRecipes = 0;
            int brokenRecipes = 0;

            foreach (string path in filesPath)
            {
                try
                {
                    FoodRecipeSerializableData data = JsonConvert.DeserializeObject<FoodRecipeSerializableData>(File.ReadAllText(path));

                    if (!data.ConvertToStandard(AdvancedCore.Instance.Info).RegisterRecipe())
                    {
                        AdvancedCore.Logging.LogWarning($"Ignored recipe from {path}. Recipe with same raw food already exists.");
                        ignoredRecipes++;
                    }
                } catch (Exception e)
                {
                    AdvancedCore.Logging.LogWarning($"Cannot load recipe from {path}");
                    AdvancedCore.Logging.LogError($"Error: {e}");
                    brokenRecipes++;
                }
                
            }

            if (brokenRecipes > 0)
                Singleton<NotificationManager>.Instance.Queue(
                    $"Can't load {brokenRecipes} recipes from assets!",
                    AssetsStorage.sounds["elv_buzz"],
                    time: 5f);

            if (ignoredRecipes > 0)
                Singleton<NotificationManager>.Instance.Queue(
                    $"Ignored {ignoredRecipes} recipes. Recipes with these raw food sets already exist!",
                    AssetsStorage.sounds["elv_buzz"],
                    time: 5f);

        }

        protected override void VirtualAwake()
        {
            base.VirtualAwake();
            visualActiveState = false;
            available = true;
            interaction.gameObject.SetActive(true);
            pickups = new List<Pickup>();
        }

        protected override void SetValues(PlateData plateData)
        {
            base.SetValues(plateData);
            plateData.allowsToCopyTextures = false;
        }

        protected override void VirtualUpdate()
        {
            base.VirtualUpdate();
            if (activeTime > 0f)
            {
                activeTime -= Time.deltaTime * Timescale;

                if (activeTime <= 0f)
                {
                    activeStateOverridden = false;
                    UpdateVisualActiveState(false);
                    activeStateOverridden = true;
                }
            }
        }

        private Pickup InitializePickup()
        {
            Vector3 position = transform.position + Vector3.up * 5f;

            Vector3? offset = GetOffset(pickups.Count + 1);

            if (offset != null) position += (Vector3)offset;

            Pickup _pickup = Instantiate(AssetsStorage.pickup, position, Quaternion.identity, transform);

            //I hate current pickup code base
            PickupClickOverrider controllerPre = _pickup.gameObject.AddComponent<PickupClickOverrider>();
            controllerPre.stove = this;

            GameObject pickupObj = _pickup.gameObject;
            Destroy(_pickup);
            Pickup pickup = pickupObj.AddComponent<Pickup>();
            _pickup.CopyAllValuesTo(pickup);

            controllerPre.pickup = pickup;

            pickups.Add(pickup);

            return pickup;
        }

        public void CreatePickup(ItemObject item)
        {
            Pickup pickup = InitializePickup();
            pickup.AssignItem(item);
            pickup.OnItemCollected += OnItemCollected;
        }

        public bool CurrentSetFitsWithAndWithout(ItemObject item, out FoodRecipeData recipeData, params ItemObject[] expelledItems)
        {
            List<Pickup> pickups = new List<Pickup>(this.pickups);
            for (int i = 0; i < expelledItems.Length; i++)
            {
                Pickup pickup = pickups.Find(x => x.item == expelledItems[i]);
                if (pickup != null)
                {
                    pickups.Remove(pickup);
                }
            }

            if (pickups.Count + 1 - expelledItems.Length < maxFoodCount)
            {
                recipeData =
                    datas.Find(x => x.ContainsListOfPickups(pickups) && x.RawFood.Contains(item) && pickups.Count < x.RawFood.Length);
                return recipeData != null;
            }
            recipeData = null;
            return false;
        }

        public bool CurrentSetFitsWith(ItemObject item, out FoodRecipeData recipeData)
        {
            if (pickups.Count + 1 < maxFoodCount)
            {
                recipeData = 
                    datas.Find(x => x.ContainsListOfPickups(pickups) && x.RawFood.Contains(item) && pickups.Count < x.RawFood.Length);
                return recipeData != null;
            }
            recipeData = null;
            return false;
        }

        private void OnItemCollected(Pickup pickup, int player)
        {
            if (pickup.item.itemType == Items.None)
            {
                DestroyPickup(pickup);
                interaction.gameObject.SetActive(true);
            }
        }

        protected override void VirtualTriggerStay(Collider other)
        {
            base.VirtualTriggerStay(other);
            if (other.TryGetComponent(out Entity entity) && ((isHot && IsPressable(entity)) || active))
            {
                entity.AddForce(new Force((entity.transform.position - transform.position).normalized, 10f, -10f));
            }
        }

        public void ButtonPressed(bool val)
        {
            if (IsUsable) Activate();
        }

        public void Activate()
        {
            activeStateOverridden = false;
            UpdateVisualActiveState(true);
            activeStateOverridden = true;

            for (int i = 0; i < pickups.Count; i++)
            {
                pickups[i].GetComponent<Collider>().enabled = false;
            }

            interaction.gameObject.SetActive(false);

            obstacle.enabled = true;

            cell = ec.CellFromPosition(transform.position);
            if (cell != null)
            {
                foreach (Cell cell in ec.GetCellNeighbors(cell.position))
                {
                    cell.Block(Directions.DirFromVector3(
                        (this.cell.TileTransform.position - cell.TileTransform.position).normalized, 0f), block: true);
                }
            }

            currentRecipe = datas.Find(x => x.ContainsListOfPickups(pickups) && pickups.Count == x.RawFood.Length);

            currentRecipe?.onKitchenStoveActivatingPre?.Invoke(this);
        }

        private void OnActivating()
        {
            particleSystem.Play();
            active = true;
            available = false;
            if (currentRecipe != null && currentRecipe.CookingTime != null) activeTime = (float)currentRecipe.CookingTime;
            else activeTime = burningTime;

            audMan.FlushQueue(true);
            audMan.QueueAudio(audBurningStart);
            audMan.QueueAudio(audBurningLoop);
            audMan.SetLoop(true);

            currentRecipe?.onKitchenStoveActivatingPost?.Invoke(this);
        }

        private void OnDeactivatingPre()
        {
            particleSystem.Stop();

            audMan.FlushQueue(true);
            audMan.QueueAudio(audBurningEnd);
            active = false;

            for (int i = 0; i < pickups.Count; i++)
            {
                DestroyPickup(pickups[i]);
                i--;
            }

            if (currentRecipe != null)
            {
                for (int i = 0; i < currentRecipe.CookedFood.Length; i++)
                {
                    CreatePickup(currentRecipe.CookedFood[i]);
                }

                if (pickups.Count == 1) pickups[0].transform.localPosition = Vector3.up * 5f; 

                if (currentRecipe.SoundOverridden)
                {
                    if (currentRecipe.Sound != null) audMan.PlaySingle(currentRecipe.Sound);
                } else
                    audMan.PlaySingle(AssetsStorage.sounds["adv_magic_1"]);

                currentRecipe.onKitchenStoveDeactivatingPre?.Invoke(this);
            }

            if (pickups.Count == 0) interaction.gameObject.SetActive(true);
        }

        private void OnDeactivatingPost()
        {
            available = true;

            obstacle.enabled = false;

            if (cell != null)
            {
                foreach (Cell cell in ec.GetCellNeighbors(cell.position))
                {
                    cell.Block(Directions.DirFromVector3(
                        (this.cell.TileTransform.position - cell.TileTransform.position).normalized, 0f), block: false);
                }
            }

            currentRecipe?.onKitchenStoveDeactivatingPost?.Invoke(this);
            currentRecipe = null;
        }

        protected override IEnumerator ColorAnimator(bool setState)
        {
            Color color = Color.red;
            float val = setState ? 1f : 0f;

            //updatesColor = true;

            if (setState)
            {
                while (val > 0f)
                {
                    val -= Time.deltaTime * Timescale * colorTransitionSpeed;
                    if (val < 0f) val = 0f;

                    color.g = val;
                    color.b = val;

                    UpdateMeshColors(ref color);
                    yield return null;
                }

                color = Color.red;

                //updatesColor = false;

                UpdateMeshColors(ref color);

                isHot = true;

                OnActivating();
            } else
            {
                OnDeactivatingPre();

                float time = (currentRecipe != null && currentRecipe.CoolingTime != null) ? (float)currentRecipe.CoolingTime : coolingTime;

                while (time > 0f)
                {
                    time -= Time.deltaTime * Timescale;
                    yield return null;
                }

                while (val < 1f)
                {
                    val += Time.deltaTime * Timescale * colorTransitionSpeed;
                    if (val > 1f) val = 1f;

                    color.g = val;
                    color.b = val;

                    UpdateMeshColors(ref color);
                    yield return null;
                }

                //updatesColor = false;

                isHot = false;

                color = Color.white;
                UpdateMeshColors(ref color);

                OnDeactivatingPost();
            }
            
        }

        /*private void UpdatePickupPositions()
        {
            Vector3 position = transform.position + Vector3.up * 5f;

            for (int i = 0; i < pickups.Count; i++)
            {
                Vector3? offset = GetOffset(i + 1);

                if (offset != null) pickups[i].transform.position += (Vector3)offset;
            }
        }*/

        private Vector3? GetOffset(int offsetNum)
        { 
            for (int i = 0; i < pickups.Count; i++)
            {
                if (pickups[i].transform.position == (transform.position + Vector3.up * 5f + CalculateOffset(offsetNum)))
                {
                    offsetNum = -1;
                    break;
                }
            }

            if (offsetNum == -1)
            {
                for (int i = 0; i < pickups.Count; i++)
                {
                    if (pickups[i].transform.position != (transform.position + Vector3.up * 5f + CalculateOffset(i + 1)))
                    {
                        offsetNum = i + 1;
                        break;
                    }
                }
            }

            if (offsetNum != -1) return CalculateOffset(offsetNum);

            return null;
        }

        private Vector3? CalculateOffset(int n)
        {
            float val = 3f;
            switch (n)
            {
                case 1:
                    return (Vector3.right + Vector3.back) * val;
                case 2:
                    return (Vector3.right + Vector3.forward) * val;
                case 3:
                    return (Vector3.left + Vector3.back) * val;
                case 4:
                    return (Vector3.left + Vector3.forward) * val;
                default:
                    return null;
            }
        }

        protected override void SetTextures()
        {
            deactivatedMaterial.mainTexture = AssetsStorage.textures["adv_kitchen_stove"];
        }

        private void DestroyPickup(Pickup pickup)
        {
            pickups.Remove(pickup);
            Destroy(pickup.gameObject);
        }

        public class PickupClickOverrider : BasePickupController
        {
            public KitchenStove stove;

            public override void Clicked(int player)
            {
                base.Clicked(player);
                PlayerManager playerMan = Singleton<CoreGameManager>.Instance.GetPlayer(player);
                if (playerMan.itm.InventoryFull() && !stove.CurrentSetFitsWithAndWithout(playerMan.itm.items[playerMan.itm.selectedItem],
                    out FoodRecipeData _, pickup.item))
                {
                    Singleton<BaseGameManager>.Instance.Ec.GetAudMan().PlaySingle(AssetsStorage.sounds["error_maybe"]);
                } else
                {
                    pickup.Clicked(player);
                }
            }
        }

        public class InteractionObject : MonoBehaviour, IClickable<int>
        {
            public KitchenStove stove;

            public void Clicked(int player)
            {
                if (!ClickableHidden())
                {
                    ItemManager inventory = Singleton<CoreGameManager>.Instance.GetPlayer(player).itm;
                    ItemObject selectedItem = inventory.items[inventory.selectedItem];

                    if (stove.CurrentSetFitsWith(selectedItem, out FoodRecipeData recipe))
                    {
                        stove.CreatePickup(selectedItem);

                        inventory.RemoveItem(inventory.selectedItem);
                        stove.Ec.GetAudMan().PlaySingle(AssetsStorage.sounds["slap"]);
                    } else
                    {
                        stove.Ec.GetAudMan().PlaySingle(AssetsStorage.sounds["error_maybe"]);
                    }
                }
            }

            public bool ClickableHidden()
            {
                return !stove.IsUsable;
            }

            public bool ClickableRequiresNormalHeight()
            {
                return true;
            }

            public void ClickableSighted(int player)
            {
                if (!ClickableHidden())
                {
                    PlayerInteractionController.Instance.SetGameTip(player, "Adv_Tip_KitchenStove");
                }
            }

            public void ClickableUnsighted(int player)
            {
                PlayerInteractionController.Instance.SetGameTip(player);
            }
        }

        public class BasePickupController : MonoBehaviour, IClickable<int>
        {
            public Pickup pickup;

            public virtual void Clicked(int player)
            {

            }

            public bool ClickableHidden()
            {
                return pickup.ClickableHidden();
            }

            public bool ClickableRequiresNormalHeight()
            {
                return pickup.ClickableRequiresNormalHeight();
            }

            public void ClickableSighted(int player)
            {

            }

            public void ClickableUnsighted(int player)
            {

            }
        }
    }
}
