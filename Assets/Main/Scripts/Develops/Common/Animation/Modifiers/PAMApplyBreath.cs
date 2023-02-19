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
    /// PAMApplyBreath stands for Person Animation Modifier Apply Breath
    /// </summary>
    [System.Serializable]
    public class PAMApplyBreath :
        Develops.Common.AnimationModifier
    {

        public AnimationClip breathClip;



        public override void Apply(AnimationClip inputClip, AnimationClip outputClip)
        {

            base.Apply(inputClip, outputClip);



        }

    }

}

#endif