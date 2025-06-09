using BaldisBasicsPlusAdvanced.Cache;
using BaldisBasicsPlusAdvanced.Cache.AssetsManagment;
using BaldisBasicsPlusAdvanced.Game.Components.UI;
using BaldisBasicsPlusAdvanced.Game.FieldTrips.SpecialTrips;
using BaldisBasicsPlusAdvanced.Game.FieldTrips.SpecialTrips.Farm.NPCs;
using BaldisBasicsPlusAdvanced.Game.FieldTrips.SpecialTrips.Farm.Objects;
using BaldisBasicsPlusAdvanced.Game.FieldTrips.SpecialTrips.Managers;
using BaldisBasicsPlusAdvanced.Helpers;
using BaldisBasicsPlusAdvanced.Managers;
using BaldisBasicsPlusAdvanced.Patches;
using MTM101BaldAPI.Registers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace BaldisBasicsPlusAdvanced.Game.FieldTrips.SpecialTrips.Farm
{
    public class FarmFieldTripManager : BaseFieldTripManager, IPrefab
    {
        [SerializeField]
        private Sprite reaperIconGauge;

        [SerializeField]
        private WeightedGameObject[] signs;

        [SerializeField]
        private WeightedItemObject[] items;

        [SerializeField]
        private Baldi baldiPre;

        [SerializeField]
        private Reaper reaperPre;

        [SerializeField]
        private IntVector2 minMaxSigns;

        [SerializeField]
        private IntVector2 minMaxItems;

        [SerializeField]
        private TileShapeMask signShapes;

        [SerializeField]
        private TileShapeMask itemShapes;

        [SerializeField]
        private float reaperBaseTime;

        [SerializeField]
        private float timePerCell;

        public static string farmTripMusicKey;

        private bool flagIsReached;

        private bool coverageFuncRemoved;

        private RoomController room;

        private Reaper reaper;

        private FinishFlag[] flags;

        private bool firstInit = true;

        private HudGauge gauge;

        private System.Random rng;

        protected override void CloseFieldTrip(bool showFieldTripScreen = true)
        {
            base.CloseFieldTrip(showFieldTripScreen);
            for (int i = 0; i < ec.Players.Length; i++)
            {
                ec.Players[i]?.itm.Disable(false); //TURN ON
            }
            if (!flagIsReached)
                FieldTripsLoader.onGameLoadedBack += 
                    () => FieldTripsLoader.PrevEc.GetAudMan().PlaySingle(AssetsStorage.sounds["bal_game_over"]);
            gauge?.Deactivate();
            //MusicManager.Instance.MidiPlayer.MPTK_ChannelVolumeSet(1, 1f);
            //MusicManager.Instance.MidiPlayer.MPTK_ChannelVolumeSet(5, 1f);
            //MusicManager.Instance.MidiPlayer.MPTK_ChannelVolumeSet(6, 1f);
        }

        public void InitializePrefab(int variant)
        {
            reaperIconGauge = AssetsHelper.SpriteFromFile("Textures/Gauges/adv_gauge_reaper.png");
            baldiPre = AssetsStorage.genericBaldi;
            reaperPre = ObjectsStorage.Objects["farm_reaper"].GetComponent<Reaper>();
            reaperBaseTime = 60f;
            timePerCell = 2f;
            minMaxSigns = new IntVector2(5, 12);
            minMaxItems = new IntVector2(5, 12);
            signShapes = TileShapeMask.Corner | TileShapeMask.End;
            itemShapes = TileShapeMask.End;

            signs = new WeightedGameObject[]
            {
                new WeightedGameObject()
                {
                    selection = ObjectsStorage.Objects["farm_sign1"],
                    weight = 100
                }
            };

            items = new WeightedItemObject[]
            {
                new WeightedItemObject()
                {
                    selection = ItemMetaStorage.Instance.Find(x => x.id == Items.Points).itemObjects[0],
                    weight = 100
                }
            };
        }

        private void Update()
        {
            if (!coverageFuncRemoved && room != null)
            {
                coverageFuncRemoved = true;
                if (room.functions.TryGetComponent(out CoverRoomFunction coverFunc))
                {
                    room.functions.RemoveFunction(coverFunc);
                }
                
            }
        }

        public override void Initialize()
        {
            if (firstInit)
            {
                ec.name = "EC_FieldTrip_Farm";
                room = FindObjectOfType<RoomController>();

                /*if (room.functions.TryGetComponent(out SkyboxRoomFunction skyboxFunc))
                {
                    Destroy(skyboxFunc.skybox.gameObject);
                    room.functions.RemoveFunction(skyboxFunc);
                }
                if (room.functions.TryGetComponent(out StaminaBoostRoomFunction staminaBoost))
                {
                    room.functions.RemoveFunction(staminaBoost);
                }*/

                rng = new System.Random(Singleton<CoreGameManager>.Instance.Seed());
                AudioListener.pause = true;
                PropagatedAudioManager.paused = true;
                firstInit = false;
                StartCoroutine(MazeGenerator());
                return;
            }

            base.Initialize();
            
            for (int i = 0; i < ec.Players.Length; i++)
            {
                ec.Players[i]?.Teleport(Vector3.one * 5f);
                ec.Players[i]?.itm.Disable(true); //TURN ON
            }

            GameObject quadObj = GameObject.CreatePrimitive(PrimitiveType.Quad);
            quadObj.name = "GrassField";
            MeshRenderer renderer = quadObj.GetComponent<MeshRenderer>();
            renderer.material = new Material(AssetsStorage.graphsStandardShader);
            renderer.material.mainTexture = AssetsStorage.textures["grass"];
            renderer.material.mainTextureScale = Vector2.one * 100f;
            quadObj.transform.position = Vector3.up * -1f;
            quadObj.transform.eulerAngles = new Vector3(90f, 0f, 0f);
            quadObj.transform.localScale = new Vector3(2000f, 2000f, 1f);//new Vector3(100000f, 100000f, 1f);
            Destroy(quadObj.GetComponent<Collider>()); //just isn't needed

            float startX = room.position.x * 10f;
            float startZ = (room.position.z + room.size.z) * 10f;
            float endX = (room.position.x + room.size.x) * 10f;
            float endZ = room.position.z * 10f;

            void CreateCollider(Vector3 position, Vector3 size)
            {
                GameObject colObj = new GameObject("ColliderBarrier");
                colObj.transform.position = position;
                BoxCollider collider = colObj.AddComponent<BoxCollider>();
                collider.size = size;
            }

            float centerZ = (startZ + endZ) / 2f;
            float centerX = (startX + endX) / 2f;

            CreateCollider(new Vector3(startX - 5f, 0f, centerZ), 
                new Vector3(10f, 10f, room.size.z * 10f));
            CreateCollider(new Vector3(endX + 5f, 0f, centerZ),
                new Vector3(10f, 10f, room.size.z * 10f));
            CreateCollider(new Vector3(centerX, 0f, endZ - 5f),
                new Vector3(room.size.x * 10f, 10f, 10f));
            CreateCollider(new Vector3(centerX, 0f, startZ + 5f),
                new Vector3(room.size.x * 10f, 10f, 10f));
        }

        public void SpawnBaldi()
        {
            ec.SpawnNPC(baldiPre, new IntVector2(0, 0));
            Baldi baldi = ec.GetBaldi();
            baldi.GetAngry(25f);
            if (baldi.behaviorStateMachine.currentState is Baldi_Chase)
            {
                ReflectionHelper.SetValue(baldi.behaviorStateMachine.currentState, "delayTimer", 0f);
            }
        }

        private IEnumerator Timer()
        {
            ec.FindPath(ec.CellFromPosition(new IntVector2(0, 0)),
                ec.CellFromPosition(flags[0].transform.position), PathType.Nav, out List<Cell> path, out bool success);

            float time = reaperBaseTime + path.Count * timePerCell;
            float baseTime = time;
            if (!success) time = 10f;

            gauge = CoreGameManager.Instance.GetHud(0).gaugeManager.ActivateNewGauge(reaperIconGauge, time);

            float defaultSpeed = 1f;

            Singleton<MusicManager>.Instance.SetSpeed(defaultSpeed);

            while (time > 0f)
            {
                time -= Time.deltaTime * ec.EnvironmentTimeScale;
                gauge?.SetValue(baseTime, time);

                if (time <= 10f && time >= 0f)
                {
                    if (defaultSpeed * time / 10f > 0.1f) Singleton<MusicManager>.Instance.SetSpeed(defaultSpeed * time / 10f);
                    //or else there will be a lot of logs without condition
                }

                yield return null;
            }

            gauge?.Deactivate();

            Singleton<MusicManager>.Instance.StopMidi();
            reaper.Begin();
        }

        public void OnWin()
        {
            flagIsReached = true;
            Time.timeScale = 0f;
            AudioListener.pause = true;
            PropagatedAudioManager.paused = true;

            Singleton<MusicManager>.Instance.StopMidi();

            ChalkboardMenu chalkMenu = Instantiate(ObjectsStorage.Objects["chalkboard_menu"].GetComponent<ChalkboardMenu>());

            Destroy(CursorController.Instance.gameObject); //reinit cursor

            chalkMenu.GetText("title").text = "Adv_FieldTrip_Farm_Win_Title".Localize();
            chalkMenu.GetText("info").text = "Adv_FieldTrip_Farm_Win_Desc".Localize();
            chalkMenu.GetButton("exit").OnPress.AddListener(delegate () { OnExitButtonPress(chalkMenu.gameObject); });

            AudioSource audio = chalkMenu.gameObject.AddComponent<AudioSource>();
            audio.ignoreListenerPause = true;

            audio.PlayOneShot(AssetsStorage.sounds["adv_mus_win"].soundClip);
        }

        private void OnExitButtonPress(GameObject menuToDestroy)
        {
            Destroy(menuToDestroy);
            FieldTripsLoader.onGameLoadedBack += delegate
            {
                FieldTripsLoader.PrevEc.GetAudMan().PlaySingle(AssetsStorage.sounds["bal_wow"]);
            };
            GiveRewards();
            CloseFieldTrip();
        }

        private void GiveRewards()
        {
            List<ItemObject> items = new List<ItemObject>();

            foreach (ItemMetaData meta in ItemMetaStorage.Instance.FindAll(x => x.tags.Contains("adv_perfect")))
            {
                if (!items.Contains(meta.value))
                {
                    items.Add(meta.value);
                }
            }

            List<ItemObject> pointsItems = ItemMetaStorage.Instance.Find(x => x.id == Items.Points).itemObjects.ToList();

            RoomController room = FindObjectOfType<FieldTripEntranceRoomFunction>(true).Room;
            List<Cell> cells = new List<Cell>(room.cells);

            int itemsCounter = 3;

            while (items.Count > 0 && cells.Count > 0 && itemsCounter > 0)
            {
                Pickup pickup = Instantiate(AssetsStorage.pickup, cells[0].CenterWorldPosition, Quaternion.identity, room.transform);
                cells.RemoveAt(0);

                ItemObject item = items[UnityEngine.Random.Range(0, items.Count)];
                pickup.AssignItem(item);
                items.Remove(item);

                itemsCounter--;
            }

            int pointsCounter = 3;

            while (pointsItems.Count > 0 && cells.Count > 0 && pointsCounter > 0)
            {
                Pickup pickup = Instantiate(AssetsStorage.pickup, cells[0].CenterWorldPosition, Quaternion.identity, room.transform);
                cells.RemoveAt(0);

                ItemObject item = pointsItems[UnityEngine.Random.Range(0, pointsItems.Count)];
                pickup.AssignItem(item);
                pointsItems.Remove(item);

                pointsCounter--;
            }
        }

        public override void OnLoadingScreenDestroying()
        {
            base.OnLoadingScreenDestroying();

            AudioListener.pause = false;
            PropagatedAudioManager.paused = false;

            reaper = Instantiate(reaperPre);
            reaper.Initialize(this, ec);
            reaper.gameObject.SetActive(false);
            reaper.transform.position = new Vector3(-25f, -10f, -25f);
            reaper.transform.rotation = Quaternion.Euler(0f, 230f, 0f);
            //reaper.transform.localScale = new Vector3(1f, 1f, 1f);

            flags = FindObjectsOfType<FinishFlag>();
            foreach (FinishFlag flag in flags)
            {
                flag.Initialize(this);
            }
            
            RoomController room = FindObjectOfType<RoomController>();

            StartCoroutine(Timer());

            //MusicManager.Instance.MidiPlayer.MPTK_ChannelVolumeSet(1, 0f);
            //MusicManager.Instance.MidiPlayer.MPTK_ChannelVolumeSet(5, 0f);
            //MusicManager.Instance.MidiPlayer.MPTK_ChannelVolumeSet(6, 0f);

            Singleton<MusicManager>.Instance.PlayMidi(AssetsStorage.campingMidi.text, loop: true);
        }

        public override void CollectNotebooks(int count)
        {
            
        }

        //Eller's algorithm
        private IEnumerator MazeGenerator()
        {
            ec.CullingManager.SetActive(false);

            int setsCounter = 0;
            Dictionary<Cell, int> cellSets = new Dictionary<Cell, int>();

            int GetCellSet()
            {
                setsCounter++;
                return setsCounter;
            }

            bool CellExists(int _x, int _z)
            {
                return ec.ContainsCoordinates(_x, _z) && ec.CellFromPosition(_x, _z).room == room;
            }

            int startX = room.position.x;
            int startZ = room.position.z + room.size.z - 1;
            int endX = room.position.x + room.size.x - 1;
            int endZ = room.position.z;

            int x = startX;
            int z = startZ;

            void SetStartPos()
            {
                x = startX;
                z = startZ;
            }

            //Debug.Log($"Room size: x: {room.size.x} z: {room.size.z}");
            //Debug.Log($"Room position: x: {room.position.x} z: {room.position.z}");
            //Debug.Log($"Room max size: x: {room.maxSize.x} z: {room.maxSize.z}");
            //Debug.Log($"Current position: x: {x} z: {z}");

            while (x <= endX)
            {
                cellSets.Add(ec.CellFromPosition(x, z), GetCellSet());
                x++;
            }
            x = startX;
            z = startZ;

            yield return null;

#warning navigation problem is unsolved

            SetStartPos();

            while (z >= endZ)
            {
                while (x <= endX)
                {
                    if (rng.NextDouble() > 0.5d)
                    {
                        ec.CreateCell(ec.CellFromPosition(x, z).ConstBin | 2, room.transform, new IntVector2(x, z), room); //creates right wall
                        if (CellExists(x + 1, z))
                        {
                            ec.CreateCell(ec.CellFromPosition(x + 1, z).ConstBin | 8, room.transform, new IntVector2(x + 1, z), room); //creates left wall
                        }
                    }
                    else if (CellExists(x + 1, z) && 
                        cellSets[ec.CellFromPosition(x + 1, z)] == cellSets[ec.CellFromPosition(x, z)])
                    {
                        ec.CreateCell(ec.CellFromPosition(x, z).ConstBin | 2, room.transform, new IntVector2(x, z), room); //creates right wall
                        if (CellExists(x + 1, z))
                        {
                            ec.CreateCell(ec.CellFromPosition(x + 1, z).ConstBin | 8, room.transform, new IntVector2(x + 1, z), room); //creates left wall
                        }
                    } 
                    else if (CellExists(x + 1, z))
                    {
                        int set = cellSets[ec.CellFromPosition(x, z)];
                        int setToReplace = cellSets[ec.CellFromPosition(x + 1, z)];
                        for (int _x = startX; _x <= endX; _x++)
                        {
                            if (cellSets[ec.CellFromPosition(_x, z)] == setToReplace)
                            {
                                cellSets[ec.CellFromPosition(_x, z)] = set;
                            }
                        }
                    }

                    x++;
                }
                x = startX;

                while (x <= endX)
                {
                    if (rng.NextDouble() > 0.5d)
                    {
                        int set = cellSets[ec.CellFromPosition(x, z)];
                        int counter = 0;
                        for (int _x = startX; _x <= endX; _x++)
                        {
                            if (cellSets[ec.CellFromPosition(_x, z)] == set
                                && (ec.CellFromPosition(_x, z).ConstBin & 4) == 0) counter++; //if cells with same sets haven't a down wall
                        }

                        if (counter >= 2)
                        {
                            ec.CreateCell(ec.CellFromPosition(x, z).ConstBin | 4, room.transform, new IntVector2(x, z), room); //creates down wall
                            if (CellExists(x, z - 1))
                            {
                                ec.CreateCell(ec.CellFromPosition(x, z - 1).ConstBin | 1, room.transform, new IntVector2(x, z - 1), room); //creates up wall
                            }
                        }
                    }

                    x++;
                }
                x = startX;

                if (z == endZ) //if row is last
                {
                    while (x <= endX)
                    {
                        if (CellExists(x + 1, z))
                        {
                            Cell cell = ec.CellFromPosition(x, z);
                            Cell rightCell = ec.CellFromPosition(x + 1, z);
                            int set = cellSets[cell];
                            int rightSet = cellSets[rightCell];

                            if (set != rightSet)
                            {
                                ec.CreateCell(cell.ConstBin & ~2, room.transform, new IntVector2(x, z), room); //destroys east

                                ec.CreateCell(rightCell.ConstBin & ~8, room.transform, new IntVector2(x + 1, z), room); //destroys west

                                for (int _x = startX; _x < endX; _x++) //looks like it makes no sense because it's end row, but ok
                                {
                                    if (cellSets[ec.CellFromPosition(_x, z)] == rightSet)
                                    {
                                        cellSets[ec.CellFromPosition(_x, z)] = set;
                                    }
                                }
                            }
                        }
                        x++;
                    }
                    x = startX;
                    break;
                }

                z--;

                while (x <= endX) //copies the row's set datas
                {
                    int set = cellSets[ec.CellFromPosition(x, z + 1)];

                    cellSets.Add(ec.CellFromPosition(x, z), set);

                    if ((ec.CellFromPosition(x, z + 1).ConstBin & 4) != 0) //has down wall?
                    {
                        cellSets[ec.CellFromPosition(x, z)] = GetCellSet();
                    }

                    x++;
                }
                x = startX;

                yield return null;
            }

            List<Cell> cells = room.GetTilesOfShape(signShapes, 
                CellCoverage.North | CellCoverage.East | CellCoverage.South | CellCoverage.West |
                CellCoverage.Up | CellCoverage.Down | CellCoverage.Center, true);
            int signsCounter = rng.Next(minMaxSigns.x, minMaxSigns.z + 1);
            while (signsCounter > 0 && cells.Count > 0)
            {
                Cell cell = cells[rng.Next(0, cells.Count)];
                Instantiate(WeightedGameObject.ControlledRandomSelection(signs, rng))
                    .transform.position = cell.CenterWorldPosition;
                cell.HardCoverEntirely();
                cells.Remove(cell);
                signsCounter--;
            }

            cells = room.GetTilesOfShape(itemShapes,
                CellCoverage.North | CellCoverage.East | CellCoverage.South | CellCoverage.West |
                CellCoverage.Up | CellCoverage.Down | CellCoverage.Center, true);
            int itemsCounter = rng.Next(minMaxItems.x, minMaxItems.z + 1);
            while (itemsCounter > 0 && cells.Count > 0)
            {
                Cell cell = cells[rng.Next(0, cells.Count)];
                Pickup pickup = Instantiate(AssetsStorage.pickup, room.transform);
                pickup.transform.position = cell.CenterWorldPosition;
                pickup.AssignItem(WeightedItemObject.ControlledRandomSelection(items, rng));

                cells.Remove(cell);
                cell.HardCoverEntirely();
                itemsCounter--;
            }

            ec.CullingManager.SetActive(false);

            Initialize();
            FieldTripsLoader.OnTripReady();

            cellSets.Clear();
            cellSets = null;

            GC.Collect();

            yield return null;
        }
    }
}
