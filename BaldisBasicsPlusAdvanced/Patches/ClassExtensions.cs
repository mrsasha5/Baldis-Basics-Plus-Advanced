using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using UnityEngine;
using BaldisBasicsPlusAdvanced.Helpers;
using System.Collections;
using BaldisBasicsPlusAdvanced.Cache;
using HarmonyLib;
using UnityEngine.UIElements;
using BaldisBasicsPlusAdvanced.Game.Systems.Controllers;
using UnityEngine.Events;

namespace BaldisBasicsPlusAdvanced.Patches
{
    public static class ClassExtensions
    {

        public static void SetDirection(this BeltManager belt, Vector3 vector)
        {
            MovementModifier moveMod = ReflectionHelper.getValue<MovementModifier>(belt, "moveMod");
            moveMod.movementAddend.x = vector.x * belt.Speed;
            moveMod.movementAddend.z = vector.z * belt.Speed;
        }

        public static void SetNewSpeed(this BeltManager belt, float speed, Vector3 vector)
        {
            ReflectionHelper.setValue<float>(belt, "speed", speed);
            belt.TextureSlider.speed.y = (0f - speed) / 10f;
            belt.SetDirection(vector);
        }

        public static void mix<T>(this List<T> list)
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

        public static void controlledMix<T>(this List<T> list, System.Random cRng)
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

        public static void dangerousTeleportation(this NPC npc)
        {
            npc.StartCoroutine(teleport(npc));
        }

        private static IEnumerator teleport(this NPC npc)
        {
            int teleports = UnityEngine.Random.Range(12, 16);
            int teleportCount = 0;
            float baseTime = 0.2f;
            float currentTime = 0;//baseTime;
            float increaseFactor = 1.1f;
            while (teleportCount < teleports)
            {
                currentTime -= Time.deltaTime;
                if (currentTime <= 0f)
                {
                    //old pos
                    AudioManager audMan = ObjectsCreator.createPropagatedAudMan(npc.transform.position, 2f);
                    audMan.PlaySingle(AssetsStorage.sounds["teleport"]);

                    npc.transform.position = npc.ec.RandomCell(includeOffLimits: false, includeWithObjects: false, useEntitySafeCell: true).FloorWorldPosition + Vector3.up * 5f;
                    //actually pos
                    AudioManager _audMan = ObjectsCreator.createPropagatedAudMan(npc.transform.position, 2f);
                    _audMan.PlaySingle(AssetsStorage.sounds["teleport"]);
                    teleportCount++;
                    baseTime *= increaseFactor;
                    currentTime = baseTime;
                }

                yield return null;
            }
            yield break;
        }

        public static void setGrounded(this ITM_NanaPeel nanaPeel)
        {
            Entity entity = ReflectionHelper.getValue<Entity>(nanaPeel, "entity");

            Vector3 position = new Vector3(entity.transform.position.x, ReflectionHelper.getValue<Entity, float>("physicalHeight"), entity.transform.position.z);

            entity.transform.position = position;

            //PrivateDataHelper.SetValue<Vector3>(entity, "velocity", Vector3.zero);
            ReflectionHelper.setValue<Vector3>(entity, "previousPosition", entity.transform.position);

            entity.UpdateInternalMovement(Vector3.zero);

            ReflectionHelper.setValue<float>(nanaPeel, "height", ReflectionHelper.getValue<float>(nanaPeel, "endHeight"));
            ReflectionHelper.setValue<bool>(nanaPeel, "ready", true);

            Force force = ReflectionHelper.getValue<Force>(nanaPeel, "force");

            try
            {
                entity.RemoveForce(force);
            } catch (Exception) { }

            entity.SetGrounded(true);
            entity.SetHeight(ReflectionHelper.getValue<float>(nanaPeel, "height"));

            ReflectionHelper.setValue<float>(nanaPeel, "time", ReflectionHelper.getValue<float>(nanaPeel, "maxTime"));
        }

        public static bool exists(this LocalizationManager localizationManager, string checkName)
        {
            Dictionary<string, string> localizedText = ReflectionHelper.getValue<Dictionary<string, string>>(localizationManager, "localizedText");
            return localizedText.ContainsKey(checkName);
        }

        public static AudioManager getAudMan(this EnvironmentController ec)
        {
            return ReflectionHelper.getValue<AudioManager>(ec, "audMan");
        }

        public static PlayerControllerSystem getControllerSystem(this PlayerManager pm)
        {
            return pm.GetComponent<PlayerControllerSystem>();
        }

        public static NPCControllerSystem getControllerSystem(this NPC npc)
        {
            return npc.GetComponent<NPCControllerSystem>();
        }

        public static int countItems(this ItemManager itm)
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

        public static T convertTo<T>(this Item item) where T : Item
        {
            return (T)item;
        }
    }
}
