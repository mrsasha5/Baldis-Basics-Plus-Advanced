/*using BaldisBasicsPlusAdvanced.Cache;
using BaldisBasicsPlusAdvanced.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using BaldisBasicsPlusAdvanced.Patches;

namespace BaldisBasicsPlusAdvanced.Game.Objects
{
    public class WindObject : MonoBehaviour
    {
        private BeltManager beltManager;

        private Transform windGraphicsParent;

        MeshRenderer[] windGraphics;

        //18 - ClickableCollidable
        private int layerForRay = ~(LayerMask.GetMask("NPCs", "Player", "Ignore Raycast", "StandardEntities", "ClickableEntities") | 1 << 18);

        private RaycastHit hit;

        //private float aliveTime;

        private float windMaxSize;

        private bool hidden;

        private bool selfUpdate;

        private bool dependsOnObstacles;

        private Transform parent;

        private AudioManager audMan;

        public bool Hidden => hidden;

        public void initialize(float windSize, float speed = 12f, Transform parent = null, bool dependsOnObstacles = true, bool selfUpdate = true)
        {
            windMaxSize = windSize;

            beltManager = Instantiate(AssetsStorage.windManager, new Vector3(0, 0, windSize / 2 * 10), Quaternion.identity);
            beltManager.name = "Adv_WindManager";
            beltManager.transform.SetParent(parent, false);

            ReflectionHelper.setValue<Material>(beltManager, "sourceMaterial", AssetsStorage.materials["wind"]);
            ReflectionHelper.setValue<bool>(beltManager, "affectAll", true);

            beltManager.gameObject.SetActive(true);
            beltManager.Initialize();
            beltManager.SetSpeed(speed);

            Quaternion _rotation = beltManager.transform.rotation;
            _rotation.eulerAngles = Vector3.up * 90f;
            beltManager.transform.rotation = _rotation;

            audMan = beltManager.GetComponentInChildren<AudioManager>();

            audMan.audioDevice.maxDistance = 100f;
            audMan.audioDevice.spatialBlend = 1f;
            audMan.audioDevice.velocityUpdateMode = AudioVelocityUpdateMode.Dynamic;

            audMan.FlushQueue(true);
            audMan.transform.localPosition = new Vector3(0.0f, 5f, -0.5f);

            ReflectionHelper.setValue<SoundObject[]>(audMan, "soundOnStart", new SoundObject[0]);

            windGraphicsParent = Instantiate(AssetsStorage.windGraphicsParent, new Vector3(0, 0, 0), Quaternion.identity);
            windGraphicsParent.transform.SetParent(beltManager.transform, true);
            windGraphicsParent.transform.localPosition = Vector3.zero;

            windGraphics = windGraphicsParent.GetComponentsInChildren<MeshRenderer>();

            ReflectionHelper.setValue<List<MeshRenderer>>(beltManager, "belt", windGraphics.ToList());

            setSize(windSize);

            this.parent = parent;
            this.dependsOnObstacles = dependsOnObstacles;
            this.selfUpdate = selfUpdate;
        }

        private void Update()
        {
            virtualUpdate();
        }

        protected virtual void virtualUpdate()
        {
            if (selfUpdate && !hidden)
            {
                beltManager.SetDirection(parent.forward);
                if (dependsOnObstacles && Physics.Raycast(parent.position, parent.forward, out hit, float.PositiveInfinity, layerForRay, QueryTriggerInteraction.Ignore))// && ((int)Math.Round(hit.distance / 10f, MidpointRounding.AwayFromZero) <= windMaxSize))
                {
                    float currentSize = hit.distance / 10f;
                    if (currentSize > windMaxSize)
                    {
                        setSize(windMaxSize);
                    }
                    else
                    {
                        setSize(currentSize);
                    }
                }
                else
                {
                    setSize(windMaxSize);
                }
            }
        }

        public void setActivityState(bool active)
        {
            beltManager.gameObject.SetActive(active);
            windGraphicsParent.gameObject.SetActive(active);
            hidden = !active;
            if (active)
            {
                audMan.SetLoop(val: true);
                audMan.maintainLoop = true;
                audMan.QueueAudio(AssetsStorage.sounds["blowing"]);
            } else
            {
                audMan.FlushQueue(true);
            }
            virtualUpdate();
        }

        private void setSize(float windSize)
        {
            beltManager.transform.localPosition = Vector3.forward * (windSize / 2 * 10);

            MeshRenderer[] array = windGraphics;
            for (int i = 0; i < array.Length; i++)
            {
                array[i].sharedMaterial = beltManager.newMaterial;
            }

            beltManager.BoxCollider.size = Vector3.right * windSize * 10f + (Vector3.up + Vector3.forward) * 10f;
            foreach (MeshRenderer meshRenderer in array)
            {
                meshRenderer.transform.localScale = Vector3.zero + Vector3.forward + Vector3.right * 10f;
                meshRenderer.transform.localScale = meshRenderer.transform.localScale + Vector3.up * windSize * 10f;
            }
        }

    }
}
*/