using System;
using System.Collections.Generic;
using UI.Model;
using UI.Model.Templates;
using UnityEngine;

namespace Services
{
    public class TemplateServer : MonoBehaviour, IService
    {
        public SimpleButtonTemplate SimpleButton;
        public TulipTemplate Tulip;
        public TulipEconomyLineTemplate TulipEconomyLine;

        public Dictionary<Type, UIViewTemplateBase> TemplateMapping;

        protected void Awake()
        {
            ServiceLocator.RegisterAsService(this);
            TemplateMapping = new()
            {
                [typeof(SimpleButtonTemplate)] = SimpleButton,
                [typeof(TulipTemplate)] = Tulip,
                [typeof(TulipEconomyLineTemplate)] = TulipEconomyLine,
            };
        }

        public T GetTemplate<T>() where T : UIViewTemplateBase
        {
            if (TemplateMapping.TryGetValue(typeof(T), out UIViewTemplateBase template))
            {
                return (T)template;
            }
            throw new KeyNotFoundException();
        }
    }
}