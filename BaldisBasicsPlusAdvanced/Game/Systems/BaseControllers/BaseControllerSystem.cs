using BaldisBasicsPlusAdvanced.Game.Systems.Controllers;
using System.Collections.Generic;
using UnityEngine;

namespace BaldisBasicsPlusAdvanced.Game.Systems.BaseControllers
{
    public class BaseControllerSystem : MonoBehaviour  
    {
        protected List<BaseController> controllers = new List<BaseController>();

        protected bool initialized;

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

        private void Update()
        {
            if (!initialized || Time.deltaTime <= 0f) return;
            for (int i = 0; i < controllers.Count; i++)
            {
                if (!controllers[i].ToDestroy)
                {
                    controllers[i].VirtualUpdate();
                }
                else
                {
                    BaseController controller = controllers[i];
                    controller.OnPreDestroying();
                    controllers.RemoveAt(i);
                    controller.OnPostDestroying();
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
                    controllers[i].VirtualTriggerEnter(other);
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
                    controllers[i].VirtualTriggerStay(other);
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
                    controllers[i].VirtualTriggerExit(other);
                }
            }
        }

        public bool CreateController<T>() where T : BaseController, new()
        {
            T controller = new T();

            if (controller.MaxCount == -1 || GetAbsoluteControllersCount<T>() < controller.MaxCount)
            {
                if (InitController(controller) != null)
                {
                    controllers.Add(controller);
                    return true;
                }
            }
            return false;
        }

        public bool CreateController<T>(out T controller) where T : BaseController, new()
        {
            T _controller = new T();

            controller = _controller;

            if (controller.MaxCount == -1 || GetAbsoluteControllersCount<T>() < controller.MaxCount)
            {
                if (InitController(_controller) != null)
                {
                    controllers.Add(_controller);
                    return true;
                }
            }
            return false;
        }

        public void RemoveController<T>(T controller) where T : BaseController
        {
            controllers.Remove(controller);
        }

        public int GetControllersCount<T>() where T : BaseController
        {
            return controllers.FindAll(x => x.GetType() == typeof(T) && !x.ToDestroy).Count; 
        }

        public int GetAbsoluteControllersCount<T>() where T : BaseController
        {
            return controllers.FindAll(x => x.GetType() == typeof(T)).Count;
        }

        protected virtual T InitController<T>(T controller) where T : BaseController
        {
            return controller;
        }

    }
}
