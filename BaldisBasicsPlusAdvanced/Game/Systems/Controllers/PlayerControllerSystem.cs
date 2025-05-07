using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace BaldisBasicsPlusAdvanced.Game.Systems.Controllers
{
    public class PlayerControllerSystem : MonoBehaviour
    {

        private PlayerManager pm;

        private EnvironmentController ec;

        private List<PlayerController> controllers = new List<PlayerController>();

        private bool initialized;

        public bool Invincibility
        {
            get
            {
                for (int i = 0; i < controllers.Count; i++)
                {
                    if (controllers[i].Invincibility) return true;
                }
                return false;
            }
        }

        public void initialize(PlayerManager pm)
        {
            this.pm = pm;
            this.ec = pm.ec;
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

        public bool createController<T>() where T : PlayerController, new()
        {
            T controller = new T();
            
            if (controller.MaxCount == -1 || getControllersCount<T>() < controller.MaxCount)
            {
                controller.initialize(ec, pm, this);
                controllers.Add(controller);
                return true;
            }
            return false;
        }

        public bool createController<T>(out T controller) where T : PlayerController, new()
        {
            T _controller = new T();

            controller = _controller;

            if (_controller.MaxCount == -1 || getControllersCount<T>() < _controller.MaxCount)
            {
                _controller.initialize(ec, pm, this);
                controllers.Add(_controller);
                return true;
            }
            return false;
        }

        public void remove<T>(T controller) where T : PlayerController
        {
            controllers.Remove(controller);
        }

        public int getControllersCount<T>() where T : PlayerController
        {
            return controllers.FindAll(x => x.GetType() == typeof(T)).Count;
        }

    }
}
