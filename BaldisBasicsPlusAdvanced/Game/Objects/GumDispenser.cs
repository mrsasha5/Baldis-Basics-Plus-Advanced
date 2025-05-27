using BaldisBasicsPlusAdvanced.Cache;
using BaldisBasicsPlusAdvanced.Game.Objects.Projectiles;
using BaldisBasicsPlusAdvanced.Helpers;
using MTM101BaldAPI;
using MTM101BaldAPI.Components;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BaldisBasicsPlusAdvanced.Game.Objects
{
    public class GumDispenser : TileBasedObject, IButtonReceiver, IPrefab
    {
        [SerializeField]
        private AudioManager audMan;

        [SerializeField]
        private float cooldownTime;

        [SerializeField]
        private int maxUses;

        [SerializeField]
        private int maxCrashedGums;

        [SerializeField]
        private Renderer renderer;

        private float time;

        private int uses;

        private IEnumerator animator;

        private List<GumProjectile> potentialGumsToDestroy = new List<GumProjectile>();

        public void InitializePrefab(int variant)
        {
            InitializeRenderer();

            BoxCollider collider = gameObject.AddComponent<BoxCollider>(); //for the editor
            collider.isTrigger = true;
            collider.size = Vector3.one * 5f;
            collider.center = Vector3.up * 5f;

            audMan = gameObject.AddComponent<PropagatedAudioManager>();

            cooldownTime = 30f;
            maxUses = 5;
            maxCrashedGums = 5;
        }

        private void Awake()
        {
            uses = maxUses;
        }

        private void Update()
        {
            if (uses < maxUses && time > 0f)
            {
                time -= Time.deltaTime * ec.EnvironmentTimeScale;

                if (time <= 0f)
                {
                    uses++;
                    time = cooldownTime;
                }
            }
        }

        public void ButtonPressed(bool val)
        {
            if (uses > 0)
            {
                uses--;
                if (time <= 0f) time = cooldownTime;
                GumProjectile gum = Object.Instantiate(ObjectsStorage.Objects["gum"].GetComponent<GumProjectile>());
                gum.Initialize(ec, transform.position - transform.forward * 2f, this);
                gum.Reset();
                gum.transform.forward = transform.forward;

                potentialGumsToDestroy.Add(gum);

                audMan.PlaySingle(AssetsStorage.sounds["spit"]);

                if (animator != null)
                {
                    StopCoroutine(animator);
                }
                animator = ColorAnimator();
                StartCoroutine(animator);

                if (potentialGumsToDestroy.Count > maxCrashedGums)
                {
                    for (int i = 0; i < potentialGumsToDestroy.Count; i++)
                    {
                        if (!potentialGumsToDestroy[i].Flying && !potentialGumsToDestroy[i].AttachedToSomebody)
                        {
                            Destroy(potentialGumsToDestroy[i].gameObject);
                            potentialGumsToDestroy.RemoveAt(i);
                            i--;
                            if (potentialGumsToDestroy.Count <= potentialGumsToDestroy.Count) break;
                        }
                    }
                }
            }
        }

        private IEnumerator ColorAnimator()
        {
            Color color = Color.red;
            float val = 0f;

            while (val < 1f)
            {
                val += Time.deltaTime * ec.EnvironmentTimeScale;
                if (val > 1f) val = 1f;

                color.g = val;
                color.b = val;

                renderer.material.SetColor(color);
                yield return null;
            }

            renderer.material.SetColor(Color.white);
        }

        private void InitializeRenderer()
        {
            GameObject childObj = new GameObject("Renderer");
            childObj.transform.SetParent(gameObject.transform, false);
            childObj.transform.localScale = new Vector3(10f, 10f, 1f);
            childObj.transform.localPosition = Vector3.up * 5f + Vector3.forward * -5f;

            Quaternion rotation = childObj.transform.rotation;
            rotation.eulerAngles = new Vector3(0f, 180f, 0f);
            childObj.transform.rotation = rotation;

            renderer = childObj.AddComponent<MeshRenderer>();
            renderer.material = new Material(AssetsStorage.materials["belt"]);
            renderer.material.mainTexture = AssetsStorage.textures["adv_gum_dispenser"];

            MeshFilter meshFilter = childObj.AddComponent<MeshFilter>();
            meshFilter.mesh = AssetsStorage.meshes["quad"];
        }


    }
}
