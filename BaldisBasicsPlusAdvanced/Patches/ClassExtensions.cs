using System.Collections.Generic;
using UnityEngine;
using BaldisBasicsPlusAdvanced.Helpers;
using System.Collections;
using BaldisBasicsPlusAdvanced.Cache;
using BaldisBasicsPlusAdvanced.Game.Systems.Controllers;
using UnityEngine.UI;
using System.Reflection;
using System;
using BaldisBasicsPlusAdvanced.Game.Components.Movement;

namespace BaldisBasicsPlusAdvanced.Patches
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

        public static void SetSpeedEffect(this Entity entity, float multiplier, float time)
        {
            entity.StartCoroutine(SpeedController(entity, multiplier, time));
        }

        private static IEnumerator SpeedController(Entity entity, float multiplier, float time)
        {
            MovementModifier moveMod = new MovementModifier(Vector3.zero, multiplier);

            EnvironmentController ec = ReflectionHelper.GetValue<EnvironmentController>(entity, "environmentController");

            entity?.ExternalActivity.moveMods.Add(moveMod);

            while (time > 0f)
            {
                time -= Time.deltaTime * ec.EnvironmentTimeScale;
                yield return null;
            }

            entity?.ExternalActivity.moveMods.Remove(moveMod);

        }

        public static bool IsEntity(this Collider collider)
        {
            return (collider.gameObject.layer == LayersHelper.standardEntities) || (collider.gameObject.layer == LayersHelper.clickableEntities);
        }

        private static IEnumerator ShutAnimation(LockdownDoor door, float speed)
        {
            ReflectionHelper.SetValue<bool>(door, "moving", true);

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

            ReflectionHelper.SetValue<bool>(door, "moving", false);

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

            ReflectionHelper.SetValue<bool>(door, "moving", true);

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

            ReflectionHelper.SetValue<bool>(door, "moving", false);

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

                    ReflectionHelper.SetValue<IEnumerator>(door, "doorMover", enumerator);

                    door.StartCoroutine(enumerator);

                    door.aTile.Mute(door.direction, false);
                    door.bTile.Mute(door.direction.GetOpposite(), false);
                } else //Shut method analog
                {
                    door.open = false;

                    if (door.closeBlocks) door.Block(true); //based on Door class

                    IEnumerator enumerator = ShutAnimation(door, speed);

                    ReflectionHelper.SetValue<IEnumerator>(door, "doorMover", enumerator);

                    door.StartCoroutine(enumerator);
                }
            }
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
            behaviour.Initialize(Singleton<BaseGameManager>.Instance.Ec, entity, force, direction, minSlamMagnitude, behaviour.DefaultSlamDistance, makesNoises);
        }

        public static void CopyAllValuesTo<T>(this T @object, T target)
        {
            FieldInfo[] originalFields = @object.GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

            for (int i = 0; i < originalFields.Length; i++)
            {
                ReflectionHelper.SetValue(target, originalFields[i].Name, ReflectionHelper.GetValue(@object, originalFields[i].Name));
                //targetFields[i].SetValue(targetFields[i], originalFields[i].GetValue(originalFields[i]));
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

        public static void SetDirection(this BeltManager belt, Vector3 vector)
        {
            MovementModifier moveMod = ReflectionHelper.GetValue<MovementModifier>(belt, "moveMod");
            moveMod.movementAddend.x = vector.x * belt.Speed;
            moveMod.movementAddend.z = vector.z * belt.Speed;
        }

        public static void SetNewSpeed(this BeltManager belt, float speed, Vector3 vector)
        {
            ReflectionHelper.SetValue<float>(belt, "speed", speed);
            belt.TextureSlider.speed.y = (0f - speed) / 10f;
            belt.SetDirection(vector);
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
            entity.StartCoroutine(ChaoticTeleporting(entity));
        }

        private static IEnumerator ChaoticTeleporting(this Entity entity)
        {
            int teleports = UnityEngine.Random.Range(12, 16);
            int teleportCount = 0;
            float baseTime = 0.2f;
            float currentTime = 0;//baseTime;
            float increaseFactor = 1.1f;
            EnvironmentController ec = ReflectionHelper.GetValue<EnvironmentController>(entity, "environmentController");
            while (teleportCount < teleports)
            {
                currentTime -= Time.deltaTime;
                if (currentTime <= 0f)
                {
                    entity.SpectacularTeleport
                        (ec.RandomCell(includeOffLimits: false, includeWithObjects: false, useEntitySafeCell: true)
                        .FloorWorldPosition + Vector3.up * 5f);
                    teleportCount++;
                    baseTime *= increaseFactor;
                    currentTime = baseTime;
                }

                yield return null;
            }
            yield break;
        }

        public static void RandomTeleport(this Entity entity)
        {
            entity.SpectacularTeleport(Singleton<BaseGameManager>.Instance.Ec.
                RandomCell(includeOffLimits: false, includeWithObjects: false, useEntitySafeCell: true)
                        .FloorWorldPosition + Vector3.up * 5f);
        }

        public static void SpectacularTeleport(this Entity entity, Vector3 pos)
        {
            //old pos
            AudioManager audMan = ObjectsCreator.CreatePropagatedAudMan(entity.transform.position, destroyWhenAudioEnds: true);
            audMan.PlaySingle(AssetsStorage.sounds["teleport"]);

            entity.Teleport(pos);

            //actually pos
            AudioManager _audMan = ObjectsCreator.CreatePropagatedAudMan(entity.transform.position, destroyWhenAudioEnds: true);
            _audMan.PlaySingle(AssetsStorage.sounds["teleport"]);
        }

        /*public static void SetGrounded(this ITM_NanaPeel nanaPeel)
        {
            Entity entity = ReflectionHelper.GetValue<Entity>(nanaPeel, "entity");

            Vector3 position = new Vector3(entity.transform.position.x, ReflectionHelper.GetValue<Entity, float>("physicalHeight"), entity.transform.position.z);

            entity.transform.position = position;

            //PrivateDataHelper.SetValue<Vector3>(entity, "velocity", Vector3.zero);
            ReflectionHelper.SetValue<Vector3>(entity, "previousPosition", entity.transform.position);

            entity.UpdateInternalMovement(Vector3.zero);

            ReflectionHelper.SetValue<float>(nanaPeel, "height", ReflectionHelper.GetValue<float>(nanaPeel, "endHeight"));
            ReflectionHelper.SetValue<bool>(nanaPeel, "ready", true);

            Force force = ReflectionHelper.GetValue<Force>(nanaPeel, "force");

            try
            {
                entity.RemoveForce(force);
            } catch (Exception) { }

            entity.SetGrounded(true);
            entity.SetHeight(ReflectionHelper.GetValue<float>(nanaPeel, "height"));

            ReflectionHelper.SetValue<float>(nanaPeel, "time", ReflectionHelper.GetValue<float>(nanaPeel, "maxTime"));
        }*/

        public static bool Exists(this LocalizationManager localizationManager, string checkName)
        {
            Dictionary<string, string> localizedText = ReflectionHelper.GetValue<Dictionary<string, string>>(localizationManager,
                "localizedText");
            return localizedText.ContainsKey(checkName);
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
                if (itm.items[i] != null && !(itm.items[i].itemType == global::Items.None))
                {
                    itemsCount++;
                }
            }
            return itemsCount;
        }

        public static void ToCenter(this Image image)
        {
            image.rectTransform.anchoredPosition = Vector3.zero;
            image.rectTransform.anchorMin = Vector2.one * 0.5f;
            image.rectTransform.anchorMax = Vector2.one * 0.5f;
        }
    }
}
