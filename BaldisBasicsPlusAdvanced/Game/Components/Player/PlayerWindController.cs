using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using BaldisBasicsPlusAdvanced.Patches;
using BaldisBasicsPlusAdvanced.Cache;
using BaldisBasicsPlusAdvanced.Helpers;
using System.Linq;
using BaldisBasicsPlusAdvanced.Game.Systems.Controllers;

namespace BaldisBasicsPlusAdvanced.Game.Components.Player
{
    public class PlayerWindController : PlayerController
    {
        private BeltManager playerBelt;

        private Transform windGraphicsParent;

        MeshRenderer[] windGraphics;

        private RaycastHit hit;

        //private float aliveTime;

        private int windMaxSize;

        private float windMaxSpeed;

        private bool blowingAllowed = false;

        private bool windExists = false;

        private int rotationMultiplier = 1;

        public override int MaxCount => 1;

        public void createWind(int windSize, float speed = 12f, float aliveTime = 15f)
        {
            time = aliveTime;
            windMaxSize = windSize;
            windMaxSpeed = speed;
            BeltManager beltManager = UnityEngine.Object.Instantiate(AssetsStorage.windManager, new Vector3(0, 0, windSize / 2 * 10), Quaternion.identity);
            beltManager.name = "PlayerBeltManager";
            beltManager.transform.SetParent(pm.transform, false);

            ReflectionHelper.setValue<Material>(beltManager, "sourceMaterial", AssetsStorage.materials["wind"]);
            ReflectionHelper.setValue<bool>(beltManager, "affectAll", true);

            beltManager.gameObject.SetActive(true);
            beltManager.Initialize();
            beltManager.SetSpeed(speed);

            AudioManager audMan = beltManager.GetComponentInChildren<AudioManager>();
            audMan.positional = false;
            ReflectionHelper.setValue<SoundObject>(audMan, "soundOnStart", new SoundObject[] { AssetsStorage.sounds["blowing"] });

            windGraphicsParent = UnityEngine.Object.Instantiate(AssetsStorage.windGraphicsParent, new Vector3(0, 0, windSize / 2 * 10), Quaternion.identity);
            windGraphicsParent.transform.SetParent(pm.transform, false);

            windGraphics = windGraphicsParent.GetComponentsInChildren<MeshRenderer>();

            ReflectionHelper.setValue<List<MeshRenderer>>(beltManager, "belt", windGraphics.ToList());
            playerBelt = beltManager;

            setSize(windSize);
            windExists = true;
            blowingAllowed = true;
        }

        public void destroyWind(bool playPahSound)
        {
            playerBelt.gameObject.SetActive(false); //to fix bugs when characters moving when belt destroyed. It calls method OnDisable which fixes it.
            UnityEngine.Object.Destroy(playerBelt.gameObject);
            UnityEngine.Object.Destroy(windGraphicsParent.gameObject);
            if (playPahSound)
            {
                AudioManager audMan = ec.getAudMan();
                audMan.PlaySingle(AssetsStorage.sounds["adv_pah"]);
            }

            windExists = false;
        }

        private void setSize(int windSize)
        {
            playerBelt.transform.localPosition = Vector3.forward * (windSize / 2 * 10) * rotationMultiplier;
            windGraphicsParent.transform.localPosition = Vector3.forward * (windSize / 2 * 10) * rotationMultiplier;

            MeshRenderer[] array = windGraphics;
            for (int i = 0; i < array.Length; i++)
            {
                array[i].sharedMaterial = playerBelt.newMaterial;
            }

            playerBelt.BoxCollider.size = Vector3.forward * windSize * 10f + Vector3.right * 5f;
            foreach (MeshRenderer meshRenderer in array)
            {
                meshRenderer.transform.localScale = Vector3.zero + Vector3.forward + Vector3.right * 10f * rotationMultiplier;
                meshRenderer.transform.localScale = meshRenderer.transform.localScale + Vector3.up * windSize * 10f * rotationMultiplier;
            }
        }

        public override void virtualUpdate()
        {
            if (Input.GetKeyDown(KeyCode.B))
            {
                blowingAllowed = !blowingAllowed;
            }

            bool playerHidden = pm.hidden;

            if ((!blowingAllowed || playerHidden) && windExists)
            {
                destroyWind(false);
            }
            else if (blowingAllowed && !playerHidden && !windExists)
            {
                createWind(windMaxSize, windMaxSpeed);
            }

            if (time > 0)
            {
                if (blowingAllowed && !playerHidden)
                {
                    rotationMultiplier = 1;
                    if (Singleton<InputManager>.Instance.GetDigitalInput("LookBack", onDown: false)) rotationMultiplier = -1;

                    time -= Time.deltaTime * ec.EnvironmentTimeScale;

                    playerBelt.SetDirection(pm.transform.forward * rotationMultiplier);
                    //playerBelt.SetNewSpeed(windSpeed, pm.transform.forward);
                    if (Physics.Raycast(pm.transform.position, Singleton<CoreGameManager>.Instance.GetCamera(pm.playerNumber).transform.forward, out hit, float.PositiveInfinity, ObjectsStorage.RaycastMaskIgnorableObjects, QueryTriggerInteraction.Ignore))// && ((int)Math.Round(hit.distance / 10f, MidpointRounding.AwayFromZero) <= windMaxSize))
                    {
                        int currentSize = (int)Math.Round(hit.distance / 10f, MidpointRounding.AwayFromZero);
                        if (currentSize > windMaxSize)
                        {
                            setSize(windMaxSize);
                        }
                        else
                        {
                            setSize((int)Math.Round(hit.distance / 10f, MidpointRounding.AwayFromZero));
                        }
                    }
                    else
                    {
                        setSize(windMaxSize);
                    }
                }
            }
        }

        public override void onDestroying()
        {
            base.onDestroying();
            destroyWind(true);
        }

    }
}
