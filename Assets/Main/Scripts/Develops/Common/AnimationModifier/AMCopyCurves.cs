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
    /// AMCopyCurves copies all curves in input clip to output clip
    /// </summary>
    [System.Serializable]
    public class AMCopyCurves :
        Develops.Common.AnimationModifier
    {

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        #region Methods
        public override void Apply(AnimationClip inputClip, AnimationClip outputClip)
        {

            base.Apply(inputClip, outputClip);



            var curveBindings_in = AnimationUtility.GetCurveBindings(inputClip);
            var curves_in = new AnimationCurve[curveBindings_in.Length];

            for (int i = 0; i < curveBindings_in.Length; ++i)
            {

                curves_in[i] = AnimationUtility.GetEditorCurve(inputClip, curveBindings_in[i]);

            }

            AnimationUtility.SetEditorCurves(
                outputClip,
                curveBindings_in,
                curves_in
            );

        }
        #endregion

    }

}

#endif