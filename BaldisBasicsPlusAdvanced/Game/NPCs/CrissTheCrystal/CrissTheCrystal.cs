using BaldisBasicsPlusAdvanced.Cache.AssetsManagment;
using BaldisBasicsPlusAdvanced.Game.Systems.BaseControllers;
using BaldisBasicsPlusAdvanced.Game.Systems.Controllers;
using BaldisBasicsPlusAdvanced.Helpers;
using BaldisBasicsPlusAdvanced.Patches;
using MTM101BaldAPI;
using MTM101BaldAPI.AssetTools;
using MTM101BaldAPI.Components;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

namespace BaldisBasicsPlusAdvanced.Game.NPCs.CrissTheCrystal
{
    public class CrissTheCrystal : NPC, IPrefab
    {
        [SerializeField]
        private SoundObject[] audWanderSounds;

        [SerializeField]
        private SoundObject[] audAttackSounds;

        [SerializeField]
        private SoundObject[] audAfterAttackSounds;

        //[SerializeField]
        //private SoundObject[] audAfterAttackSounds;

        [SerializeField]
        private SoundObject audLaserStart;

        [SerializeField]
        private SoundObject audLaserLoop;

        [SerializeField]
        private SoundObject audLaserEnd;

        [SerializeField]
        private Sprite[] idleSprites;

        [SerializeField]
        private Sprite[] walkingSprites;

        [SerializeField]
        private Sprite[] crazyRunningSprites;

        [SerializeField]
        private Sprite[] goingToShootSprites;

        [SerializeField]
        private Sprite[] loopedShootingSprites;

        [SerializeField]
        private Sprite[] turnsCrazySprites;

        [SerializeField]
        private Sprite gaugeIcon;

        [SerializeField]
        private WindowObject windowObjectPre;

        [SerializeField]
        private AudioManager audMan;

        [SerializeField]
        private CustomSpriteAnimator animator;

        [SerializeField]
        private Vector2 minMaxCooldown;

        [SerializeField]
        private float blindCooldown;

        [SerializeField]
        private float blindTime;

        [SerializeField]
        private float destroyingWallTime;

        [SerializeField]
        private float laserSize;

        [SerializeField]
        private float walkSpeed;

        [SerializeField]
        private float runSpeed;

        [SerializeField]
        private float pitchSpeed;

        private LaserObject laser;

        public Vector2 MinMaxCooldown => minMaxCooldown;

        public AudioManager AudMan => audMan;

        public LaserObject Laser => laser;

        public CustomSpriteAnimator Animator => animator;

        public WindowObject WindowPre => windowObjectPre;

        public void InitializePrefab(int variant)
        {
            windowObjectPre = ObjectCreators.CreateWindowObject(
                "Big Hole",
                AssetsHelper.TextureFromFile("Textures/Windows/BigHole/adv_window_big_hole.png"),
                AssetsHelper.TextureFromFile("Textures/Windows/BigHole/adv_window_big_hole.png"),
                AssetsHelper.TextureFromFile("Textures/Windows/BigHole/adv_window_big_hole_mask.png")
            );
            windowObjectPre.windowPre = Instantiate(windowObjectPre.windowPre);
            windowObjectPre.windowPre.gameObject.ConvertToPrefab(true);
            windowObjectPre.windowPre.openOnStart = true;

            audMan = gameObject.GetComponent<AudioManager>();
            animator = gameObject.AddComponent<CustomSpriteAnimator>();
            blindCooldown = 1f;
            laserSize = 5f;
            blindTime = 10f;
            walkSpeed = 10f;
            runSpeed = 30f;
            pitchSpeed = 0.15f;
            destroyingWallTime = 2f;

            minMaxCooldown = new Vector2(30f, 60f);

            audLaserStart = AssetsStorage.sounds["adv_laser_start"];
            audLaserLoop = AssetsStorage.sounds["adv_laser_loop"];
            audLaserEnd = AssetsStorage.sounds["adv_laser_end"];

            animator.spriteRenderer = spriteRenderer[0];
            spriteRenderer[0].transform.localPosition = Vector3.zero;

            Sprite[] sprites =
                AssetLoader.SpritesFromSpritesheet(4, 4, 25f, Vector2.one * 0.5f, AssetsStorage.textures["adv_criss_the_crystal"]);

            Sprite[] crazySprites =
                AssetLoader.SpritesFromSpritesheet(3, 3, 25f, Vector2.one * 0.5f, AssetsStorage.textures["adv_criss_the_crystal_crazy"]);

            gaugeIcon = AssetsHelper.SpriteFromFile("Textures/Gauges/adv_gauge_crystal_blindness.png");

            turnsCrazySprites = new Sprite[4];

            crazyRunningSprites = new Sprite[6];

            for (int i = 0; i < turnsCrazySprites.Length; i++)
            {
                turnsCrazySprites[i] = crazySprites[i];
            }

            for (int i = 0; i <= 2; i++)
            {
                crazyRunningSprites[i] = crazySprites[i + 3];
            }

            crazyRunningSprites[3] = crazySprites[3];
            crazyRunningSprites[4] = crazySprites[6];
            crazyRunningSprites[5] = crazySprites[7];

            idleSprites = new Sprite[4];
            walkingSprites = new Sprite[4];
            for (int i = 0; i < idleSprites.Length; i++)
            {
                idleSprites[i] = sprites[i];
                walkingSprites[i] = sprites[i];
            }

            goingToShootSprites = new Sprite[7];
            for (int i = 0; i < goingToShootSprites.Length; i++)
            {
                goingToShootSprites[i] = sprites[i + 3];
            }
            loopedShootingSprites = new Sprite[4];
            for (int i = 0; i < loopedShootingSprites.Length; i++)
            {
                loopedShootingSprites[i] = sprites[i + 11];
            }

            audWanderSounds = AssetsHelper.LoadSounds("Voices/Criss", "Criss_Wander", SoundType.Voice, Color.cyan)
                .ToArray();
            audAttackSounds = AssetsHelper.LoadSounds("Voices/Criss", "Criss_Attack", SoundType.Voice, Color.cyan)
                .ToArray();
            audAfterAttackSounds = AssetsHelper.LoadSounds("Voices/Criss", "Criss_AfterAttack", SoundType.Voice, Color.cyan)
                .ToArray();

            spriteRenderer[0].sprite = walkingSprites[0];

            //GameObject laserObj = new GameObject("Laser");

            //laserObj.transform.SetParent(transform, false);

            //laser = laserObj.AddComponent<LaserObject>();
        }

        public override void Initialize()
        {
            base.Initialize();
            animator.PopulateAnimations(
                new Dictionary<string, Sprite[]>()
                {
                    { "Idle", idleSprites },
                    { "Walking", walkingSprites },
                    { "PreparingToShoot", goingToShootSprites },
                    { "Shooting", loopedShootingSprites },
                    { "TurnsCrazy", turnsCrazySprites },
                    { "Crazy_Running", crazyRunningSprites}
                }, fps: 10
            );
            animator.SetDefaultAnimation("Idle", 1f);

            GameObject laserObj = new GameObject("Criss Laser");
            laser = laserObj.AddComponent<LaserObject>();
            laser.Initialize(this);
            laser.SetActive(false, playSound: false);

            CrissTheCrystal_Wandering state = new CrissTheCrystal_Wandering(this);
            behaviorStateMachine.ChangeState(state);

            state.SetTime(0f);
        }

        public void PlayRandomVoice(string name)
        {
            switch (name)
            {
                case "Wander":
                    audMan.PlayRandomAudio(audWanderSounds);
                    break;
                case "Attack":
                    audMan.PlayRandomAudio(audAttackSounds);
                    break;
                case "AfterAttack":
                    audMan.PlayRandomAudio(audAfterAttackSounds);
                    break;
            }
        }
        
        public void SetWalkSpeed()
        {
            navigator.maxSpeed = walkSpeed;
            navigator.SetSpeed(walkSpeed);
        }

        public void SetRunSpeed()
        {
            navigator.maxSpeed = runSpeed;
            navigator.SetSpeed(runSpeed);
        }

        public void TryDestroyCollider(Collider collider)
        {
            if (collider != null)
            {
                if (collider.transform.CompareTag("Wall"))
                {
                    Direction direction = Directions.DirFromVector3(collider.transform.forward, 5f);
                    Cell cell = ec.CellFromPosition(IntVector2.GetGridPosition(
                        collider.transform.position + collider.transform.forward * 5f));
                    Cell cell2 = ec.CellFromPosition(IntVector2.GetGridPosition(
                        collider.transform.position + collider.transform.forward * -5f));
                    if (ec.ContainsCoordinates(
                        IntVector2.GetGridPosition(collider.transform.position + collider.transform.forward * 5f)) &&
                        !cell.Null &&
                        cell.HasWallInDirection(direction.GetOpposite()) &&
                        !cell.WallHardCovered(direction.GetOpposite()) && 
                        ec.ContainsCoordinates(IntVector2.GetGridPosition(
                            collider.transform.position + collider.transform.forward * -5f)) &&
                        !cell2.Null &&
                        !cell2.WallHardCovered(direction))
                    {

                        ec.BuildWindow(cell2, direction, windowObjectPre);
                    }
                }
                else if (collider.transform.parent != null)
                {
                    if (collider.transform.parent.CompareTag("Window"))
                        collider.transform.parent.GetComponent<Window>()
                            .Break(true); //Why it makes noise? Then ask First Prize why he makes, when
                                        //he tries to push player to a wall
                    else if (collider.transform.parent.TryGetComponent(out StandardDoor door))
                    {
                        door.Unlock();
                        door.Open(true, false);
                        door.doors = new MeshRenderer[0];
                        door.colliders = new MeshCollider[0];
                        door.audMan.volumeModifier = 0f; //Haha, no.
                        door.tag = "Untagged";
                        //GameObject.Destroy(door); //Still can't do that because it registered not only by some room
                        //but it may be registered by some mod too
                        MeshRenderer[] renderers = collider.transform.parent.GetComponentsInChildren<MeshRenderer>();
                        for (int i = 0; i < renderers.Length; i++)
                        {
                            Material[] materials = renderers[i].materials;
                            materials[0] = windowObjectPre.mask;
                            materials[1] = windowObjectPre.open[0];
                            renderers[i].materials = materials;
                        }

                        Collider trigger = collider.transform.parent.GetComponent<Collider>();

                        if (trigger != null) Destroy(trigger);

                        Collider[] colliders = collider.transform.parent.GetComponentsInChildren<MeshCollider>();
                        for (int i = 0; i < colliders.Length; i++)
                        {
                            GameObject.Destroy(colliders[i]);
                        }
                    }
                    else if (collider.transform.parent.TryGetComponent(out SwingDoor swingDoor))
                    {
                        swingDoor.Unlock();
                        swingDoor.Open(true, false);
                        swingDoor.doors = new MeshRenderer[0];
                        swingDoor.colliders = new MeshCollider[0];
                        swingDoor.audMan.volumeModifier = 0f; //Haha, no.
                        swingDoor.tag = "Untagged";
                        //GameObject.Destroy(door); //Still can't do that because it registered not only by some room
                        //but it may be registered by some mod too
                        MeshRenderer[] renderers = collider.transform.parent.GetComponentsInChildren<MeshRenderer>();
                        for (int i = 0; i < renderers.Length; i++)
                        {
                            Material[] materials = renderers[i].materials;
                            materials[0] = windowObjectPre.mask;
                            materials[1] = windowObjectPre.open[0];
                            renderers[i].materials = materials;
                        }

                        Collider trigger = collider.transform.parent.GetComponent<Collider>();

                        if (trigger != null) Destroy(trigger);

                        Collider[] colliders = collider.transform.parent.GetComponentsInChildren<MeshCollider>();
                        for (int i = 0; i < colliders.Length; i++)
                        {
                            GameObject.Destroy(colliders[i]);
                        }
                    }

                }
            }
        }

        public class LaserObject : MonoBehaviour
        {
            private BoxCollider collider;

            private LineRenderer laserRenderer;

            private CrissTheCrystal criss;

            private List<BaseControllerSystem> systems = new List<BaseControllerSystem>();

            private List<float> times = new List<float>();

            private List<Collider> triggeredColliders = new List<Collider>();

            private List<float> triggeredTimes = new List<float>();

            private Vector3 _direction;

            private float _distance;

            public void Initialize(CrissTheCrystal criss)
            {
                this.criss = criss;
                laserRenderer = gameObject.AddComponent<LineRenderer>();
                laserRenderer.startWidth = 0.8f;//0.1977f;
                laserRenderer.endWidth = 0.8f;//0.1977f;
                laserRenderer.allowOcclusionWhenDynamic = false;
                laserRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                laserRenderer.receiveShadows = false;
                laserRenderer.material = new Material(AssetsStorage.materials["black_behind"]); //black_behind
                laserRenderer.material.SetColor(Color.white);

                collider = gameObject.AddComponent<BoxCollider>();
                collider.isTrigger = true;

                gameObject.SetRigidbody();
            }

            private void Update()
            {
                if (criss == null)
                {
                    Destroy(gameObject);
                    return;
                }

                transform.position = criss.transform.position;
                for (int i = 0; i < systems.Count; i++)
                {
                    if (times[i] > 0f)
                    {
                        times[i] -= Time.deltaTime * criss.TimeScale;

                        if (times[i] <= 0f)
                        {
                            systems[i].CreateController(out BlindController blind);
                            blind.SetTime(criss.blindTime);
                            if (systems[i] is PlayerControllerSystem)
                                blind.SetGauge(
                                    CoreGameManager.Instance.GetHud(0).gaugeManager.ActivateNewGauge(criss.gaugeIcon, criss.blindTime));
                        }
                    }
                }

                for (int i = 0; i < triggeredColliders.Count; i++)
                {
                    if (triggeredTimes[i] > 0f)
                    {
                        triggeredTimes[i] -= Time.deltaTime * criss.TimeScale;

                        if (triggeredTimes[i] <= 0f)
                        {
                            criss.TryDestroyCollider(triggeredColliders[i]);
                        }
                    }
                }
                criss.audMan.pitchModifier += Time.deltaTime * criss.TimeScale * criss.pitchSpeed;
            }

            public void SetActive(bool state, bool playSound = true)
            {
                gameObject.SetActive(state);

                if (state && playSound)
                {
                    criss.audMan.FlushQueue(true);
                    criss.audMan.QueueAudio(criss.audLaserStart);
                    criss.audMan.QueueAudio(criss.audLaserLoop);
                    criss.audMan.SetLoop(true);
                }
                else if (!state)
                {
                    systems.Clear();
                    times.Clear();
                    triggeredColliders.Clear();
                    triggeredTimes.Clear();
                    criss.audMan.pitchModifier = 1f;
                    if (playSound)
                    {
                        criss.audMan.FlushQueue(true);
                        criss.audMan.PlaySingle(criss.audLaserEnd);
                    }
                }
            }

            private void OnTriggerEnter(Collider collider)
            {
                if (collider.TryGetComponent(out BaseControllerSystem system))
                {
                    if (!systems.Contains(system) && collider.gameObject != criss.gameObject)
                    {
                        systems.Add(system);
                        times.Add(criss.blindCooldown);
                    }
                }
                else if (!triggeredColliders.Contains(collider))
                {
                    triggeredColliders.Add(collider);
                    triggeredTimes.Add(criss.destroyingWallTime);
                }
                    
            }

            private void OnTriggerExit(Collider collider)
            {
                if (collider.TryGetComponent(out BaseControllerSystem system))
                {
                    if (systems.Contains(system) && collider.gameObject != criss.gameObject)
                    {
                        int index = systems.IndexOf(system);
                        systems.RemoveAt(index);
                        times.RemoveAt(index);
                    }
                }
                else if (triggeredColliders.Contains(collider))
                {
                    int index = triggeredColliders.IndexOf(collider);
                    triggeredColliders.RemoveAt(index);
                    triggeredTimes.RemoveAt(index);
                }
            }

            public void SetLaserPositions(Vector3[] positions)
            {
                laserRenderer.SetPositions(positions);

                _distance = Vector3.Distance(positions[0], positions[1]);

                _direction = (positions[1] - positions[0]).normalized;

                collider.size = (Vector3.right + Vector3.up) * criss.laserSize + Vector3.forward * _distance;
                collider.center = Vector3.forward * _distance / 2f;
                
                transform.rotation = Quaternion.Euler(0f, math.atan2(_direction.x, _direction.z) * 180f / math.PI, 0f);
            }
        }

    }
}
