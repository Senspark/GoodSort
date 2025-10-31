using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace Factory
{
    public class UIControllerFactory
    {
        private static UIControllerFactory _instance;
        public static UIControllerFactory Instance => _instance ??= new UIControllerFactory();
        private readonly Dictionary<string, Func<object>> _uiControllers = new();
        
        /// <summary>
        /// Đăng ký controller cho 1 Dialog
        /// </summary>
        public void Register<T>(Func<object> controllerFactory) where T : Component
        {
            _uiControllers[typeof(T).Name] = controllerFactory;
        }

        public T CreateController<T>(string componentType)
        {
            if(_uiControllers.TryGetValue(componentType, out var factory))
            {
                return (T)factory();
            }
            throw new Exception($"Controller {componentType} not found");
        }
        
        public T Instantiate<T>(GameObject prefab) where T : Component
        {
            var instance = UnityEngine.Object.Instantiate(prefab);
            var component = instance.GetComponent<T>();
            var typeName = typeof(T).Name;

            if (!_uiControllers.TryGetValue(typeName, out var factory))
                throw new Exception($"Controller {typeName} not registered");

            var controller = factory();
            var field = typeof(T).GetField("_controller", BindingFlags.Instance | BindingFlags.NonPublic);
            if (field == null)
                throw new Exception($"Field _controller not found in {typeof(T).Name}");

            field.SetValue(component, controller);
            return component;
        }
    }
}