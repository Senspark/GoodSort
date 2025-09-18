using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace Factory
{
    public class UIControllerFactory
    {
        private readonly Dictionary<string, Func<object>> _uiControllers = new();
        
        /// <summary>
        /// Đăng ký controller cho 1 component type
        /// </summary>
        public void Register<T>(Func<object> controllerFactory) where T : Component
        {
            _uiControllers[typeof(T).Name] = controllerFactory;
        }

        /// <summary>
        /// Tạo controller theo tên component
        /// </summary>
        public T CreateController<T>(string componentType)
        {
            if(_uiControllers.TryGetValue(componentType, out var factory))
            {
                return (T)factory();
            }
            throw new Exception($"Controller {componentType} not found");
        }
        
        /// <summary>
        /// Instantiate prefab và thêm controller vào component có _controller field
        /// </summary>
        public T Instantiate<T>(GameObject prefab) where T : Component
        {
            var instance = UnityEngine.Object.Instantiate(prefab);
            var component = instance.GetComponent<T>();
            if(component == null)
            {
                throw new Exception($"Component {typeof(T).Name} not found");
            }
            var typeName = typeof(T).Name;
            if(!_uiControllers.TryGetValue(typeName, out var factory))
            {
                throw new Exception($"Controller {typeName} not registered");
            }

            var field = typeof(T).GetField("_controller", BindingFlags.Instance | BindingFlags.NonPublic);
            if (field != null)
            {
                field.SetValue(component, factory);
            }
            else
            {
                throw new Exception($"Field _controller not found in {typeof(T).Name}");
            }

            return component;
        }
    }
}