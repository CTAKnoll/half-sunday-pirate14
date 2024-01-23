using System;
using System.Collections;
using System.Collections.Generic;
using UI.Mission;
using UnityEngine;

namespace UI.Dialogue
{
    public class OptionController : SimpleButtonController
    {
        public OptionController(OptionView view, SimpleButtonModel model = default) : base(view, view.InvokeOptionSelected)
        {

        }
    }
}