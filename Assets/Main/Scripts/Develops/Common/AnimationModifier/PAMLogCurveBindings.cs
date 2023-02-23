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
    /// PAMLogCurveBindings stands for Person Animation Modifier Log Curve Bindings
    /// </summary>
    [System.Serializable]
    public class PAMLogCurveBindings :
        Develops.Common.AnimationModifier
    {

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        #region Methods
        public override void Apply(AnimationClip inputClip, AnimationClip outputClip)
        {

            base.Apply(inputClip, outputClip);

            var curveBindings = AnimationUtility.GetCurveBindings(inputClip);

            int i = 0;
            foreach (var curveBinding in curveBindings)
            {

                Debug.LogFormat("Curve binding 0: {0}", i);
                Debug.LogFormat("Path: {0}", curveBinding.path);
                Debug.LogFormat("Type: {0}", curveBinding.type);
                Debug.LogFormat("Property Name: {0}", curveBinding.propertyName);

            }

        }
        #endregion

    }

}

#endif