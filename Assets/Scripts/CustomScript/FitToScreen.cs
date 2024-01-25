using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KC_Custom
{
    [ExecuteInEditMode]
    public class FitToScreen : ARectTransform
    {
        [SerializeField] public bool updateOnEditorMode = true;
    }
}