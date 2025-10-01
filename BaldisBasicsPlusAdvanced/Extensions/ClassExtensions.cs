using System.Collections.Generic;
using UnityEngine;
using BaldisBasicsPlusAdvanced.Helpers;
using System.Collections;
using BaldisBasicsPlusAdvanced.Game.Systems.Controllers;
using UnityEngine.UI;
using System.Reflection;
using System;
using BaldisBasicsPlusAdvanced.Game.Components.Movement;
using BaldisBasicsPlusAdvanced.Cache.AssetsManagement;
using BaldisBasicsPlusAdvanced.Game.Components.UI;
using MTM101BaldAPI.UI;
using System.Linq;

namespace BaldisBasicsPlusAdvanced.Extensions
{
    public static class ClassExtensions
    {
        private static int[] angles = new int[] {
            0,
            90,
            180,
            270,
            360
        };

        public static Rigidbody SetRigidbody(this GameObject gameObject)
        {
            Rigidbody rb = gameObject.AddComponent<Rigidbody>();
            rb.useGravity = false;
            rb.angularDrag = 0;
            rb.constraints = RigidbodyConstraints.FreezeAll;
            rb.freezeRotation = true;
            rb.isKinematic = true;
            rb.mass = 0;
            return rb;
        }

        public static void RemoveFunction(this RoomFunctionContainer controller, RoomFunction function)
        {
            ReflectionHelper.GetValue<List<RoomFunction>>(controller, "functions").Remove(function);
            UnityEngine.Object.Destroy(function);
        }

        public static void SetSpeedEffect(this Entity entity, float multiplier, float time, Sprite gaugeIcon = null)
        {
            entity.StartCoroutine(SpeedController(entity, multiplier, time, gaugeIcon));
        }

        private static IEnumerator SpeedController(Entity entity, float multiplier, float time, Sprite gaugeIcon)
        {
            HudGauge gauge = null;
            if (gaugeIcon != null)
                gauge = Singleton<CoreGameManager>.Instance.GetHud(0).gaugeManager.ActivateNewGauge(gaugeIcon, time);

            float baseTime = time;

            MovementModifier moveMod = new MovementModifier(Vector3.zero, multiplier);

            EnvironmentController ec = ReflectionHelper.GetValue<EnvironmentController>(entity, "environmentController");

            entity?.ExternalActivity.moveMods.Add(moveMod);

            while (time > 0f)
            {
                time -= Time.deltaTime * ec.EnvironmentTimeScale;
                gauge?.SetValue(baseTime, time);
                yield return null;
            }

            entity?.ExternalActivity.moveMods.Remove(moveMod);
            gauge?.Deactivate();
        }

        public static int GetWeight(this Dictionary<int, int> weights, int floor)
        {
            if (weights.Count == 0) return 0;

            if (weights.ContainsKey(floor))
            {
                return weights[floor];
            }
            else
            {
                int nearestFloor = MathHelper.FindNearestValue(weights.Keys.ToArray(), floor);

                return weights[nearestFloor];
            }
        }

        public static bool IsEntity(this Collider collider)
        {
            return collider.gameObject.layer == LayersHelper.standardEntities || collider.gameObject.layer == LayersHelper.clickableEntities;
        }

        private static IEnumerator ShutAnimation(LockdownDoor door, float speed)
        {
            ReflectionHelper.SetValue(door, "moving", true);

            AudioManager audMan = door.GetComponent<AudioManager>();
            Collider collider = door.GetComponent<Collider>();
            MeshRenderer renderer = door.GetComponentInChildren<MeshRenderer>();

            audMan.QueueAudio(AssetsStorage.sounds["lockdown_door"], playImmediately: true);
            audMan.SetLoop(val: true);

            while (renderer.transform.position.y > 0f)
            {
                yield return null;
                renderer.transform.position -= Vector3.up * (speed * Time.deltaTime * door.ec.EnvironmentTimeScale);
                if (renderer.transform.position.y <= ReflectionHelper.GetValue<float>(door, "collisionHeight") && !collider.enabled)
                {
                    collider.enabled = true;
                }
            }

            ReflectionHelper.SetValue(door, "moving", false);

            collider.enabled = true;
            renderer.transform.position -= Vector3.up * (0f - renderer.transform.position.y);

            door.aTile.Mute(door.direction, block: true);
            door.bTile.Mute(door.direction.GetOpposite(), block: true);

            audMan.FlushQueue(endCurrent: true);
            audMan.PlaySingle(AssetsStorage.sounds["lock_door_stop"]);
        }

        private static IEnumerator OpenAnimation(LockdownDoor door, float speed)
        {
            AudioManager audMan = door.GetComponent<AudioManager>();
            Collider collider = door.GetComponent<Collider>();
            MeshRenderer renderer = door.GetComponentInChildren<MeshRenderer>();

            if (renderer.transform.position.y >= door.originalHeight)
            {
                renderer.transform.position -= Vector3.up * (renderer.transform.position.y - door.originalHeight);
                collider.enabled = false;
                yield break;
            }

            ReflectionHelper.SetValue(door, "moving", true);

            audMan.QueueAudio(AssetsStorage.sounds["lockdown_door"], playImmediately: true);
            audMan.SetLoop(val: true);

            while (renderer.transform.position.y < door.originalHeight)
            {
                yield return null;
                renderer.transform.position += Vector3.up * (speed * Time.deltaTime * door.ec.EnvironmentTimeScale);
                if (renderer.transform.position.y > ReflectionHelper.GetValue<float>(door, "collisionHeight") && collider.enabled)
                {
                    collider.enabled = false;
                }
            }

            ReflectionHelper.SetValue(door, "moving", false);

            collider.enabled = false;
            renderer.transform.position -= Vector3.up * (renderer.transform.position.y - door.originalHeight);
            audMan.FlushQueue(endCurrent: true);
            audMan.PlaySingle(AssetsStorage.sounds["lock_door_stop"]);
        }

        public static void Toggle(this LockdownDoor door, float speed, bool toggleEvenItMoves = false, bool? shut = null)
        {
            bool isMoving = ReflectionHelper.GetValue<bool>(door, "moving");
            if (!isMoving || toggleEvenItMoves)
            {
                if (isMoving)
                {
                    door.StopCoroutine(ReflectionHelper.GetValue<IEnumerator>(door, "doorMover"));
                }

                bool open = shut == null ? !door.open : !(bool)shut;

                if (open) //Open method analog
                {
                    door.open = true;

                    if (door.closeBlocks) door.Block(false); //based on Door class

                    IEnumerator enumerator = OpenAnimation(door, speed);

                    ReflectionHelper.SetValue(door, "doorMover", enumerator);

                    door.StartCoroutine(enumerator);

                    door.aTile.Mute(door.direction, false);
                    door.bTile.Mute(door.direction.GetOpposite(), false);
                } else //Shut method analog
                {
                    door.open = false;

                    if (door.closeBlocks) door.Block(true); //based on Door class

                    IEnumerator enumerator = ShutAnimation(door, speed);

                    ReflectionHelper.SetValue(door, "doorMover", enumerator);

                    door.StartCoroutine(enumerator);
                }
            }
        }

        public static Vector3 GetVector3FromCellPosition(this IntVector2 vec2)
        {
            Vector3 vector3 = new Vector3();
            vector3.x = vec2.x * 10f + 5f;
            vector3.z = vec2.z * 10f + 5f;
            return vector3;
        }

        public static Vector3 CorrectForCell(this Vector3 position, float y = 5f)
        {
            EnvironmentController ec = Singleton<BaseGameManager>.Instance.Ec;

            IntVector2 _vector = ec.CellFromPosition(position).position;

            if (_vector != null)
            {
                position.x = _vector.x * 10f + 5f;
                position.z = _vector.z * 10f + 5f;
                position.y = y;
            }

            return position;
        }

        public static Quaternion Correct(this Quaternion rotation)
        {
            float setY = 0f;
            float lastDifference = float.PositiveInfinity;

            for (int i = 0; i < angles.Length; i++)
            {
                float difference = Math.Abs(angles[i] - rotation.eulerAngles.y);

                if (difference < lastDifference)
                {
                    setY = angles[i];
                    lastDifference = difference;
                }
            }

            rotation.eulerAngles = new Vector3(rotation.eulerAngles.x, setY, rotation.eulerAngles.z);

            return rotation;
        }

        public static void AddForceWithBehaviour(this Entity entity, Vector3 direction, float initialSpeed, float acceleration, bool makesNoises, float minSlamMagnitude = 0.25f)
        {
            Force force = new Force(direction, initialSpeed, acceleration);
            entity.AddForce(force);

            ForcedEntityBehaviour behaviour = entity.gameObject.AddComponent<ForcedEntityBehaviour>();
            behaviour.Initialize(
                BaseGameManager.Instance.Ec, entity, force, direction, minSlamMagnitude, 
                behaviour.DefaultSlamDistance, makesNoises);
        }

        public static void CopyAllValuesTo<T>(this T @object, T target)
        {
            FieldInfo[] originalFields = @object.GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

            for (int i = 0; i < originalFields.Length; i++)
            {
                ReflectionHelper.SetValue(target, originalFields[i].Name, ReflectionHelper.GetValue(@object, originalFields[i].Name));
            }

        }

        public static RoomGroup SetCeilingTex(this RoomGroup group, Texture2D tex, int weight)
        {
            group.ceilingTexture = new WeightedTexture2D[] {
                new WeightedTexture2D()
                {
                    selection = tex,
                    weight = weight
                }
            };
            return group;
        }

        public static RoomGroup SetWallTex(this RoomGroup group, Texture2D tex, int weight)
        {
            group.wallTexture = new WeightedTexture2D[] {
                new WeightedTexture2D()
                {
                    selection = tex,
                    weight = weight
                }
            };
            return group;
        }

        public static RoomGroup SetFloorTex(this RoomGroup group, Texture2D tex, int weight)
        {
            group.floorTexture = new WeightedTexture2D[] {
                new WeightedTexture2D()
                {
                    selection = tex,
                    weight = weight
                }
            };
            return group;
        }

        public static string Localize(this string text)
        {
            return Singleton<LocalizationManager>.Instance.GetLocalizedText(text);
        }

        public static void SetAnyDirection(this BeltManager belt, Vector3 vector)
        {
            MovementModifier moveMod = ReflectionHelper.GetValue<MovementModifier>(belt, "moveMod");
            moveMod.movementAddend.x = vector.x * belt.Speed;
            moveMod.movementAddend.z = vector.z * belt.Speed;
        }

        public static void SetNewSpeed(this BeltManager belt, float speed, Vector3 vector)
        {
            ReflectionHelper.SetValue(belt, "speed", speed);
            belt.TextureSlider.speed.y = (0f - speed) / 10f;
            belt.SetAnyDirection(vector);
        }

        public static void Mix<T>(this List<T> list)
        {
            List<T> inputObjects = new List<T>(list);
            List<T> objects = new List<T>();
            while (inputObjects.Count > 0)
            {
                int chosen = UnityEngine.Random.Range(0, inputObjects.Count);
                T obj = inputObjects[chosen];
                objects.Add(obj);
                inputObjects.Remove(obj);
            }
            list.Clear();
            list.AddRange(objects);
        }

        public static void ControlledMix<T>(this List<T> list, System.Random cRng)
        {
            List<T> inputObjects = new List<T>(list);
            List<T> objects = new List<T>();
            
            while (inputObjects.Count > 0)
            {
                int chosen = cRng.Next(0, inputObjects.Count);
                T obj = inputObjects[chosen];
                objects.Add(obj);
                inputObjects.Remove(obj);
            }
            list.Clear();
            list.AddRange(objects);
        }

        public static void DangerousTeleportation(this Entity entity)
        {
            entity.StartCoroutine(entity.ChaoticTeleporting());
        }

        private static IEnumerator ChaoticTeleporting(this Entity entity)
        {
            int teleports = UnityEngine.Random.Range(12, 16);
            int teleportCount = 0;

            float baseTime = 0.2f;
            float currentTime = 0;
            float increaseFactor = 1.1f;

            EnvironmentController ec = ReflectionHelper.GetValue<EnvironmentController>(entity, "environmentController");

            entity.SetInteractionState(false);
            entity.SetFrozen(true);

            while (teleportCount < teleports)
            {
                currentTime -= Time.deltaTime;
                if (currentTime <= 0f)
                {
                    entity.SoundTeleport(ec.RandomCell(includeOffLimits: false, includeWithObjects: false, useEntitySafeCell: true)
                        .CenterWorldPosition);
                    teleportCount++;
                    baseTime *= increaseFactor;
                    currentTime = baseTime;
                }

                yield return null;
            }

            entity.SetInteractionState(true);
            entity.SetFrozen(false);
        }

        public static void RandomTeleport(this Entity entity)
        {
            entity.SoundTeleport(BaseGameManager.Instance.Ec.
                RandomCell(includeOffLimits: false, includeWithObjects: false, useEntitySafeCell: true)
                        .FloorWorldPosition + Vector3.up * 5f);
        }

        public static void SoundTeleport(this Entity entity, Vector3 pos)
        {
            //Old pos
            AudioManager audMan = ObjectsCreator.CreatePropagatedAudMan(entity.transform.position, destroyWhenAudioEnds: true);
            audMan.PlaySingle(AssetsStorage.sounds["teleport"]);

            entity.Teleport(pos);

            //Actual pos
            AudioManager _audMan = ObjectsCreator.CreatePropagatedAudMan(entity.transform.position, destroyWhenAudioEnds: true);
            _audMan.PlaySingle(AssetsStorage.sounds["teleport"]);
        }

        public static SpriteRenderer RemoveBillboard(this SpriteRenderer renderer)
        {
            renderer.material = new Material(AssetsStorage.materials["sprite_standard_no_billboard"]);
            return renderer;
        }

        public static AudioManager GetAudMan(this EnvironmentController ec)
        {
            return ReflectionHelper.GetValue<AudioManager>(ec, "audMan");
        }

        public static PlayerControllerSystem GetControllerSystem(this PlayerManager pm)
        {
            return pm.GetComponent<PlayerControllerSystem>();
        }

        public static NPCControllerSystem GetControllerSystem(this NPC npc)
        {
            return npc.GetComponent<NPCControllerSystem>();
        }

        public static int CountItems(this ItemManager itm)
        {
            int itemsCount = 0;

            for (int i = 0; i < itm.items.Length; i++)
            {
                if (itm.items[i] != null && !(itm.items[i].itemType == Items.None))
                {
                    itemsCount++;
                }
            }
            return itemsCount;
        }

        public static CursorInitiator SetCursorInitiator(this Canvas canvas, bool setAutoInitiator = false)
        {
            CursorInitiator cursorInitiator = UIHelpers.AddCursorInitiatorToCanvas(canvas);

            if (setAutoInitiator)
                canvas.gameObject.AddComponent<CursorAutoInitiator>().initiator = cursorInitiator;

            return cursorInitiator;
        }

        public static void ToCenter(this RectTransform rect)
        {
            rect.anchoredPosition = Vector3.zero;
            rect.anchorMin = Vector2.one * 0.5f;
            rect.anchorMax = Vector2.one * 0.5f;
        }

        public static void ToCenter(this Image image) => image.rectTransform.ToCenter();
    }
}
