#if UNITY_EDITOR

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;



namespace PROJECT_A11.Develops.Common
{
    /// <summary>
    /// PAMFootsDeltaCurves stands for Person Animation Modifier Foots Delta Curves
    /// </summary>
    [System.Serializable]
    public class PAMFootsDeltaCurves :
        Develops.Common.AnimationModifier
    {

        public override void Apply(AnimationClip inputClip, AnimationClip outputClip)
        {

            base.Apply(inputClip, outputClip);



        }

    }

}

#endif