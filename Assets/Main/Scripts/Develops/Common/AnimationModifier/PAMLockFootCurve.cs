#if UNITY_EDITOR

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;



namespace PROJECT_A11.Develops.Common
{
    /// <summary>
    /// PAMLockFootCurve stands for Person Animation Modifier Lock Foot Curve
    /// </summary>
    [System.Serializable]
    public class PAMLockFootCurve :
        Develops.Common.AnimationModifier
    {

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        #region Fields
        public string curvePath = "";

        public string lockCurveLProperty = "LockFootL";
        public string lockCurveRProperty = "LockFootR";

        public string footHeightLProperty = "FootHeightL";
        public string footHeightRProperty = "FootHeightR";

        public string footRotOffsetLXProperty = "FootRotationOffsetL.x";
        public string footRotOffsetLYProperty = "FootRotationOffsetL.y";
        public string footRotOffsetLZProperty = "FootRotationOffsetL.z";
        public string footRotOffsetLWProperty = "FootRotationOffsetL.w";

        public string footRotOffsetRXProperty = "FootRotationOffsetR.x";
        public string footRotOffsetRYProperty = "FootRotationOffsetR.y";
        public string footRotOffsetRZProperty = "FootRotationOffsetR.z";
        public string footRotOffsetRWProperty = "FootRotationOffsetR.w";

        public Type curveType = typeof(Animator);
        public float framesPerSecond = 30.0f;
        [SerializeField]
        public Person personPrefab;
        #endregion

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        #region Methods
        public override void Apply(AnimationClip inputClip, AnimationClip outputClip)
        {

            base.Apply(inputClip, outputClip);


           
            UnityEditor.Animations.AnimatorController animatorController = new UnityEditor.Animations.AnimatorController();
            animatorController.AddLayer("Main");
            var stateMachine = animatorController.layers[0].stateMachine;
            var mainState = stateMachine.AddState("Main");
            stateMachine.defaultState = mainState;
            mainState.motion = inputClip;



            Person testingPerson = Instantiate(personPrefab);
            testingPerson.transform.position = Vector3.zero;
            testingPerson.transform.rotation = Quaternion.identity;
            var animator = testingPerson.GetComponent<Animator>();
            animator.runtimeAnimatorController = animatorController;
            var personAnimationController = testingPerson.GetComponent<PersonAnimationController>();



            int frameCount = (int)(inputClip.length * framesPerSecond);

            Keyframe[] keyframesLockCurveL = new Keyframe[frameCount];
            Keyframe[] keyframesLockCurveR = new Keyframe[frameCount];

            Keyframe[] keyframesFootHeightL = new Keyframe[frameCount];
            Keyframe[] keyframesFootHeightR = new Keyframe[frameCount];

            Keyframe[] keyframesfootRotOffsetLX = new Keyframe[frameCount];
            Keyframe[] keyframesfootRotOffsetLY = new Keyframe[frameCount];
            Keyframe[] keyframesfootRotOffsetLZ = new Keyframe[frameCount];
            Keyframe[] keyframesfootRotOffsetLW = new Keyframe[frameCount];

            Keyframe[] keyframesfootRotOffsetRX = new Keyframe[frameCount];
            Keyframe[] keyframesfootRotOffsetRY = new Keyframe[frameCount];
            Keyframe[] keyframesfootRotOffsetRZ = new Keyframe[frameCount];
            Keyframe[] keyframesfootRotOffsetRW = new Keyframe[frameCount];

            FootPlacement footPlacementL = personAnimationController.footLRig.GetComponent<FootPlacement>();
            FootPlacement footPlacementR = personAnimationController.footRRig.GetComponent<FootPlacement>();



            personAnimationController.DisableRigs();



            AnimationMode.StartAnimationMode();



            try
            {

                AnimationMode.SampleAnimationClip(testingPerson.gameObject, inputClip, 0);



                float maxSpeedL = 0.0f;
                float maxSpeedR = 0.0f;

                float toeBaseHeight = Mathf.Infinity;



                for (int i = 0; i < frameCount; ++i)
                {

                    float time = i * inputClip.length / (float)frameCount;
                    float dt = inputClip.length / (float)frameCount;



                    AnimationMode.SampleAnimationClip(testingPerson.gameObject, inputClip, time);

                    Vector3 footLPos1 = personAnimationController.boneFootL.position;
                    Vector3 footRPos1 = personAnimationController.boneFootR.position;

                    toeBaseHeight = Mathf.Min(toeBaseHeight, personAnimationController.boneToeBaseL.position.y);
                    toeBaseHeight = Mathf.Min(toeBaseHeight, personAnimationController.boneToeBaseR.position.y);



                    float footHeightL = Mathf.Clamp(personAnimationController.boneFootL.position.y - personAnimationController.boneToeBaseL.position.y, 0.0f, Mathf.Infinity);
                    keyframesFootHeightL[i] = new Keyframe(time, footHeightL);

                    float footHeightR = Mathf.Clamp(personAnimationController.boneFootR.position.y - personAnimationController.boneToeBaseR.position.y, 0.0f, Mathf.Infinity);
                    keyframesFootHeightR[i] = new Keyframe(time, footHeightR);



                    keyframesfootRotOffsetLX[i] = new Keyframe(time, personAnimationController.boneFootL.rotation.x);
                    keyframesfootRotOffsetLY[i] = new Keyframe(time, personAnimationController.boneFootL.rotation.y);
                    keyframesfootRotOffsetLZ[i] = new Keyframe(time, personAnimationController.boneFootL.rotation.z);
                    keyframesfootRotOffsetLW[i] = new Keyframe(time, personAnimationController.boneFootL.rotation.w);

                    keyframesfootRotOffsetRX[i] = new Keyframe(time, personAnimationController.boneFootR.rotation.x);
                    keyframesfootRotOffsetRY[i] = new Keyframe(time, personAnimationController.boneFootR.rotation.y);
                    keyframesfootRotOffsetRZ[i] = new Keyframe(time, personAnimationController.boneFootR.rotation.z);
                    keyframesfootRotOffsetRW[i] = new Keyframe(time, personAnimationController.boneFootR.rotation.w);



                    AnimationMode.SampleAnimationClip(testingPerson.gameObject, inputClip, time + dt);

                    Vector3 footLPos2 = personAnimationController.boneFootL.position;
                    Vector3 footRPos2 = personAnimationController.boneFootR.position;



                    float speedL = (footLPos2 - footLPos1).magnitude / dt;
                    float speedR = (footRPos2 - footRPos1).magnitude / dt;

                    if(maxSpeedL < speedL)
                    {

                        maxSpeedL = speedL;

                    }
                    if (maxSpeedR < speedR)
                    {

                        maxSpeedR = speedR;

                    }

                    keyframesLockCurveL[i] = new Keyframe(time, speedL);
                    keyframesLockCurveR[i] = new Keyframe(time, speedR);

                }



                if (maxSpeedL >= 0.1f)
                {

                    for (int i = 0; i < frameCount; ++i)
                    {

                        keyframesLockCurveL[i].value = 1.0f - Mathf.Clamp01(keyframesLockCurveL[i].value / maxSpeedL);

                    }

                }
                else
                {

                    for (int i = 0; i < frameCount; ++i)
                    {

                        keyframesLockCurveL[i].value = 1.0f + 0.001f * (i % 2);

                    }

                }

                if (maxSpeedR >= 0.1f)
                {

                    for (int i = 0; i < frameCount; ++i)
                    {

                        keyframesLockCurveR[i].value = 1.0f - Mathf.Clamp01(keyframesLockCurveR[i].value / maxSpeedR);

                    }

                }
                else
                {

                    for (int i = 0; i < frameCount; ++i)
                    {

                        keyframesLockCurveR[i].value = 1.0f + 0.001f * (i % 2);

                    }

                }



                for (int i = 0; i < frameCount; ++i)
                {

                    keyframesFootHeightL[i].value += toeBaseHeight;

                }
                for (int i = 0; i < frameCount; ++i)
                {

                    keyframesFootHeightR[i].value += toeBaseHeight;

                }

            }
            catch
            {

                AnimationMode.StopAnimationMode();

            }
            finally
            {

                AnimationMode.StopAnimationMode();

            }



            DestroyImmediate(testingPerson.gameObject);



            AnimationCurve lockCurveL = new AnimationCurve(keyframesLockCurveL);
            AnimationCurve lockCurveR = new AnimationCurve(keyframesLockCurveR);

            AnimationUtility.SetEditorCurve(
                outputClip,
                EditorCurveBinding.FloatCurve(curvePath, curveType, lockCurveLProperty),
                lockCurveL
            );
            AnimationUtility.SetEditorCurve(
                outputClip,
                EditorCurveBinding.FloatCurve(curvePath, curveType, lockCurveRProperty),
                lockCurveR
            );



            AnimationCurve footHeightCurveL = new AnimationCurve(keyframesFootHeightL);
            AnimationCurve footHeightCurveR = new AnimationCurve(keyframesFootHeightR);

            AnimationUtility.SetEditorCurve(
                outputClip,
                EditorCurveBinding.FloatCurve(curvePath, curveType, footHeightLProperty),
                footHeightCurveL
            );
            AnimationUtility.SetEditorCurve(
                outputClip,
                EditorCurveBinding.FloatCurve(curvePath, curveType, footHeightRProperty),
                footHeightCurveR
            );



            AnimationCurve footRotOffsetCurveLX = new AnimationCurve(keyframesfootRotOffsetLX);
            AnimationCurve footRotOffsetCurveLY = new AnimationCurve(keyframesfootRotOffsetLY);
            AnimationCurve footRotOffsetCurveLZ = new AnimationCurve(keyframesfootRotOffsetLZ);
            AnimationCurve footRotOffsetCurveLW = new AnimationCurve(keyframesfootRotOffsetLW);

            AnimationCurve footRotOffsetCurveRX = new AnimationCurve(keyframesfootRotOffsetRX);
            AnimationCurve footRotOffsetCurveRY = new AnimationCurve(keyframesfootRotOffsetRY);
            AnimationCurve footRotOffsetCurveRZ = new AnimationCurve(keyframesfootRotOffsetRZ);
            AnimationCurve footRotOffsetCurveRW = new AnimationCurve(keyframesfootRotOffsetRW);

            AnimationUtility.SetEditorCurve(
                outputClip,
                EditorCurveBinding.FloatCurve(curvePath, curveType, footRotOffsetLXProperty),
                footRotOffsetCurveLX
            );
            AnimationUtility.SetEditorCurve(
                outputClip,
                EditorCurveBinding.FloatCurve(curvePath, curveType, footRotOffsetLYProperty),
                footRotOffsetCurveLY
            );
            AnimationUtility.SetEditorCurve(
                outputClip,
                EditorCurveBinding.FloatCurve(curvePath, curveType, footRotOffsetLZProperty),
                footRotOffsetCurveLZ
            );
            AnimationUtility.SetEditorCurve(
                outputClip,
                EditorCurveBinding.FloatCurve(curvePath, curveType, footRotOffsetLWProperty),
                footRotOffsetCurveLW
            );

            AnimationUtility.SetEditorCurve(
                outputClip,
                EditorCurveBinding.FloatCurve(curvePath, curveType, footRotOffsetRXProperty),
                footRotOffsetCurveRX
            );
            AnimationUtility.SetEditorCurve(
                outputClip,
                EditorCurveBinding.FloatCurve(curvePath, curveType, footRotOffsetRYProperty),
                footRotOffsetCurveRY
            );
            AnimationUtility.SetEditorCurve(
                outputClip,
                EditorCurveBinding.FloatCurve(curvePath, curveType, footRotOffsetRZProperty),
                footRotOffsetCurveRZ
            );
            AnimationUtility.SetEditorCurve(
                outputClip,
                EditorCurveBinding.FloatCurve(curvePath, curveType, footRotOffsetRWProperty),
                footRotOffsetCurveRW
            );

        }
        #endregion

    }

}

#endif