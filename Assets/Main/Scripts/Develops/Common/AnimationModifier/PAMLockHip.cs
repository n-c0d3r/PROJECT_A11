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
    /// PAMLockHip stands for Person Animation Modifier Lock Hip
    /// </summary>
    [System.Serializable]
    public class PAMLockHip :
        Develops.Common.AnimationModifier
    {

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        #region Fields
        public EditorCurveBinding hipPositionXCurveBinding;
        public EditorCurveBinding hipPositionYCurveBinding;
        public EditorCurveBinding hipPositionZCurveBinding;

        public EditorCurveBinding hipRotationXCurveBinding;
        public EditorCurveBinding hipRotationYCurveBinding;
        public EditorCurveBinding hipRotationZCurveBinding;
        public EditorCurveBinding hipRotationWCurveBinding;

        public bool lockPositionX = true;
        public bool lockPositionY = false;
        public bool lockPositionZ = true;
        public bool lockRotation = false;

        public int defaultKeyIndex = 0;
        #endregion

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



            AnimationCurve hipPositionXCurve_in = AnimationUtility.GetEditorCurve(inputClip, hipPositionXCurveBinding);
            AnimationCurve hipPositionYCurve_in = AnimationUtility.GetEditorCurve(inputClip, hipPositionYCurveBinding);
            AnimationCurve hipPositionZCurve_in = AnimationUtility.GetEditorCurve(inputClip, hipPositionZCurveBinding);

            AnimationCurve hipRotationXCurve_in = AnimationUtility.GetEditorCurve(inputClip, hipRotationXCurveBinding);
            AnimationCurve hipRotationYCurve_in = AnimationUtility.GetEditorCurve(inputClip, hipRotationYCurveBinding);
            AnimationCurve hipRotationZCurve_in = AnimationUtility.GetEditorCurve(inputClip, hipRotationZCurveBinding);
            AnimationCurve hipRotationWCurve_in = AnimationUtility.GetEditorCurve(inputClip, hipRotationWCurveBinding);



            Keyframe[] hipPositionXCurveKFs_out = new Keyframe[hipPositionXCurve_in.keys.Length];
            Keyframe[] hipPositionYCurveKFs_out = new Keyframe[hipPositionYCurve_in.keys.Length];
            Keyframe[] hipPositionZCurveKFs_out = new Keyframe[hipPositionZCurve_in.keys.Length];

            Keyframe[] hipRotationXCurveKFs_out = new Keyframe[hipRotationXCurve_in.keys.Length];
            Keyframe[] hipRotationYCurveKFs_out = new Keyframe[hipRotationYCurve_in.keys.Length];
            Keyframe[] hipRotationZCurveKFs_out = new Keyframe[hipRotationZCurve_in.keys.Length];
            Keyframe[] hipRotationWCurveKFs_out = new Keyframe[hipRotationWCurve_in.keys.Length];



            if (lockPositionX)
            {
                for (int i = 0; i < hipPositionXCurve_in.keys.Length; ++i)
                {

                    hipPositionXCurveKFs_out[i] = hipPositionXCurve_in.keys[defaultKeyIndex];
                    hipPositionXCurveKFs_out[i].inTangent = 0.0f;
                    hipPositionXCurveKFs_out[i].outTangent = 0.0f;

                }
            }
            else
            {
                for (int i = 0; i < hipPositionXCurve_in.keys.Length; ++i)
                {

                    hipPositionXCurveKFs_out[i] = hipPositionXCurve_in.keys[i];

                }
            }
            if (lockPositionY)
            {
                for (int i = 0; i < hipPositionYCurve_in.keys.Length; ++i)
                {

                    hipPositionYCurveKFs_out[i] = hipPositionYCurve_in.keys[defaultKeyIndex];
                    hipPositionYCurveKFs_out[i].inTangent = 0.0f;
                    hipPositionYCurveKFs_out[i].outTangent = 0.0f;

                }
            }
            else
            {
                for (int i = 0; i < hipPositionYCurve_in.keys.Length; ++i)
                {

                    hipPositionYCurveKFs_out[i] = hipPositionYCurve_in.keys[i];

                }
            }
            if (lockPositionZ)
            {
                for (int i = 0; i < hipPositionZCurve_in.keys.Length; ++i)
                {

                    hipPositionZCurveKFs_out[i] = hipPositionZCurve_in.keys[defaultKeyIndex];
                    hipPositionZCurveKFs_out[i].inTangent = 0.0f;
                    hipPositionZCurveKFs_out[i].outTangent = 0.0f;

                }
            }
            else
            {
                for (int i = 0; i < hipPositionZCurve_in.keys.Length; ++i)
                {

                    hipPositionZCurveKFs_out[i] = hipPositionZCurve_in.keys[i];

                }
            }

            if (lockRotation)
            {

                for (int i = 0; i < hipRotationXCurve_in.keys.Length; ++i)
                {

                    hipRotationXCurveKFs_out[i] = hipRotationXCurve_in.keys[defaultKeyIndex];
                    hipRotationXCurveKFs_out[i].inTangent = 0.0f;
                    hipRotationXCurveKFs_out[i].outTangent = 0.0f;

                }
                for (int i = 0; i < hipRotationYCurve_in.keys.Length; ++i)
                {

                    hipRotationYCurveKFs_out[i] = hipRotationYCurve_in.keys[defaultKeyIndex];
                    hipRotationYCurveKFs_out[i].inTangent = 0.0f;
                    hipRotationYCurveKFs_out[i].outTangent = 0.0f;

                }
                for (int i = 0; i < hipRotationZCurve_in.keys.Length; ++i)
                {

                    hipRotationZCurveKFs_out[i] = hipRotationZCurve_in.keys[defaultKeyIndex];
                    hipRotationZCurveKFs_out[i].inTangent = 0.0f;
                    hipRotationZCurveKFs_out[i].outTangent = 0.0f;

                }
                for (int i = 0; i < hipRotationWCurve_in.keys.Length; ++i)
                {

                    hipRotationWCurveKFs_out[i] = hipRotationWCurve_in.keys[defaultKeyIndex];
                    hipRotationWCurveKFs_out[i].inTangent = 0.0f;
                    hipRotationWCurveKFs_out[i].outTangent = 0.0f;

                }

            }
            else
            {

                for (int i = 0; i < hipRotationXCurve_in.keys.Length; ++i)
                {

                    hipRotationXCurveKFs_out[i] = hipRotationXCurve_in.keys[i];

                }
                for (int i = 0; i < hipRotationYCurve_in.keys.Length; ++i)
                {

                    hipRotationYCurveKFs_out[i] = hipRotationYCurve_in.keys[i];

                }
                for (int i = 0; i < hipRotationZCurve_in.keys.Length; ++i)
                {

                    hipRotationZCurveKFs_out[i] = hipRotationZCurve_in.keys[i];

                }
                for (int i = 0; i < hipRotationWCurve_in.keys.Length; ++i)
                {

                    hipRotationWCurveKFs_out[i] = hipRotationWCurve_in.keys[i];

                }

            }



            AnimationCurve hipPositionXCurve_out = new AnimationCurve(hipPositionXCurveKFs_out);
            AnimationCurve hipPositionYCurve_out = new AnimationCurve(hipPositionYCurveKFs_out);
            AnimationCurve hipPositionZCurve_out = new AnimationCurve(hipPositionZCurveKFs_out);

            AnimationCurve hipRotationXCurve_out = new AnimationCurve(hipRotationXCurveKFs_out);
            AnimationCurve hipRotationYCurve_out = new AnimationCurve(hipRotationYCurveKFs_out);
            AnimationCurve hipRotationZCurve_out = new AnimationCurve(hipRotationZCurveKFs_out);
            AnimationCurve hipRotationWCurve_out = new AnimationCurve(hipRotationWCurveKFs_out);



            AnimationUtility.SetEditorCurves(
                outputClip,
                new EditorCurveBinding[] {

                    hipPositionXCurveBinding,
                    hipPositionYCurveBinding,
                    hipPositionZCurveBinding,

                    hipRotationXCurveBinding,
                    hipRotationYCurveBinding,
                    hipRotationZCurveBinding,
                    hipRotationWCurveBinding

                },
                new AnimationCurve[] {

                    hipPositionXCurve_out,
                    hipPositionYCurve_out,
                    hipPositionZCurve_out,

                    hipRotationXCurve_out,
                    hipRotationYCurve_out,
                    hipRotationZCurve_out,
                    hipRotationWCurve_out

                }
            );

        }
        #endregion

    }

}

#endif