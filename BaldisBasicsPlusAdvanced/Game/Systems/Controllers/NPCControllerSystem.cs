using BaldisBasicsPlusAdvanced.Patches;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace BaldisBasicsPlusAdvanced.Game.Systems.Controllers
{
    public class NPCControllerSystem : MonoBehaviour
    {
        private NPC npc;

        private EnvironmentController ec;

        private List<NPCController> controllers = new List<NPCController>();

        private bool initialized;

        public void initialize(NPC npc, EnvironmentController ec)
        {
            this.npc = npc;
            this.ec = ec;
            initialized = true;
        }

        private void Update()
        {
            if (!initialized || Time.deltaTime <= 0f) return;
            for (int i = 0; i < controllers.Count; i++)
            {
                if (!controllers[i].ToDestroy)
                {
                    controllers[i].virtualUpdate();
                }
                else
                {
                    controllers[i].onDestroying();
                    controllers.RemoveAt(i);
                }
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!initialized || Time.deltaTime <= 0f) return;
            for (int i = 0; i < controllers.Count; i++)
            {
                if (!controllers[i].ToDestroy)
                {
                    controllers[i].virtualTriggerEnter(other);
                }
            }
        }

        private void OnTriggerStay(Collider other)
        {
            if (!initialized || Time.deltaTime <= 0f) return;
            for (int i = 0; i < controllers.Count; i++)
            {
                if (!controllers[i].ToDestroy)
                {
                    controllers[i].virtualTriggerStay(other);
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (!initialized || Time.deltaTime <= 0f) return;
            for (int i = 0; i < controllers.Count; i++)
            {
                if (!controllers[i].ToDestroy)
                {
                    controllers[i].virtualTriggerExit(other);
                }
            }
        }

        public bool createController<T>() where T : NPCController, new()
        {
            T controller = new T();

            if (controller.MaxCount == -1 || getControllersCount<T>() < controller.MaxCount)
            {
                controller.initialize(npc, Singleton<CoreGameManager>.Instance.GetPlayer(0).getControllerSystem());
                controllers.Add(controller);
                return true;
            }
            return false;
        }

        public bool createController<T>(out T controller) where T : NPCController, new()
        {
            T _controller = new T();

            controller = _controller;

            if (_controller.MaxCount == -1 || getControllersCount<T>() < _controller.MaxCount)
            {
                _controller.initialize(npc, Singleton<CoreGameManager>.Instance.GetPlayer(0).getControllerSystem());
                controllers.Add(_controller);
                return true;
            }
            return false;
        }

        public void remove<T>(T controller) where T : NPCController
        {
            controllers.Remove(controller);
        }

        public int getControllersCount<T>() where T : NPCController
        {
            return controllers.FindAll(x => x.GetType() == typeof(T)).Count;
        }
    }
}
