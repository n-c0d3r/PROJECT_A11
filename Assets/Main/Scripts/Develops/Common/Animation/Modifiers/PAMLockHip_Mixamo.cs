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
    /// PAMLockHip_Mixamo is a PAMLockHip configured for mixamo animations.
    /// </summary>
    [System.Serializable]
    public class PAMLockHip_Mixamo :
        PAMLockHip
    {

        public override void Apply(AnimationClip inputClip, AnimationClip outputClip)
        {

            hipPositionXCurveBinding = EditorCurveBinding.FloatCurve("", typeof(UnityEngine.Animator), "RootT.x");
            hipPositionYCurveBinding = EditorCurveBinding.FloatCurve("", typeof(UnityEngine.Animator), "RootT.y");
            hipPositionZCurveBinding = EditorCurveBinding.FloatCurve("", typeof(UnityEngine.Animator), "RootT.z");

            hipRotationXCurveBinding = EditorCurveBinding.FloatCurve("", typeof(UnityEngine.Animator), "RootQ.x");
            hipRotationYCurveBinding = EditorCurveBinding.FloatCurve("", typeof(UnityEngine.Animator), "RootQ.y");
            hipRotationZCurveBinding = EditorCurveBinding.FloatCurve("", typeof(UnityEngine.Animator), "RootQ.z");
            hipRotationWCurveBinding = EditorCurveBinding.FloatCurve("", typeof(UnityEngine.Animator), "RootQ.w");

            lockPositionX = true;
            lockPositionY = false;
            lockPositionZ = true;
            lockRotation = false;

            defaultKeyIndex = 0;

            base.Apply(inputClip, outputClip);

        }

    }

}

#endif