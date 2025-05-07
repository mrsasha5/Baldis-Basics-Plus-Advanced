using System.Collections.Generic;
using UnityEngine;
using BaldisBasicsPlusAdvanced.Helpers;
using System.Collections;
using BaldisBasicsPlusAdvanced.Cache;
using BaldisBasicsPlusAdvanced.Game.Systems.Controllers;
using UnityEngine.UI;
using System.Reflection;

namespace BaldisBasicsPlusAdvanced.Patches
{
    public static class ClassExtensions
    {
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

        public static void CopyAllValuesTo<T>(this T @object, T target)
        {
            FieldInfo[] originalFields = @object.GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

            for (int i = 0; i < originalFields.Length; i++)
            {
                ReflectionHelper.SetValue(target, originalFields[i].Name, ReflectionHelper.GetValue(@object, originalFields[i].Name));
                //targetFields[i].SetValue(targetFields[i], originalFields[i].GetValue(originalFields[i]));
            }

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

        public static void SpectacularTeleport(this Entity entity, Vector3 pos)
        {
            //old pos
            AudioManager audMan = ObjectsCreator.CreatePropagatedAudMan(entity.transform.position, 2f);
            audMan.PlaySingle(AssetsStorage.sounds["teleport"]);

            entity.Teleport(pos);

            //actually pos
            AudioManager _audMan = ObjectsCreator.CreatePropagatedAudMan(entity.transform.position, 2f);
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

        public static T ConvertTo<T>(this Item item) where T : Item
        {
            return (T)item;
        }

        public static void ToCenter(this Image image)
        {
            image.rectTransform.anchoredPosition = Vector3.zero;
            image.rectTransform.anchorMin = Vector2.one * 0.5f;
            image.rectTransform.anchorMax = Vector2.one * 0.5f;
        }
    }
}
