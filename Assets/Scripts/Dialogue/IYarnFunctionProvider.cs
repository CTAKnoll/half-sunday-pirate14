using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;

namespace UI.Dialogue
{
    public interface IYarnFunctionProvider
    {
        void InitYarnFunctions(DialogueRunner dialogueRunner);
    }
}