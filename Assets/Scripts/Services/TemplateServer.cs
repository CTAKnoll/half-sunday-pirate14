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
        public TooltipTemplate Tooltip;
        
        protected void Awake()
        {
            ServiceLocator.RegisterAsService(this);
        }
    }
}