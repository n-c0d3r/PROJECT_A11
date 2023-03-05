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
        [SerializeField]
        public string curvePath = "";
        [SerializeField]
        public string curveLProperty = "LockFootL";
        public string curveRProperty = "LockFootR";
        [SerializeField]
        public Type curveType = typeof(Animator);
        [SerializeField]
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
            var animator = testingPerson.GetComponent<Animator>();
            animator.runtimeAnimatorController = animatorController;
            var personAnimationController = testingPerson.GetComponent<PersonAnimationController>();



            int frameCount = (int)(inputClip.length * framesPerSecond);

            Keyframe[] keyframesL = new Keyframe[frameCount];
            Keyframe[] keyframesR = new Keyframe[frameCount];

            FootPlacement footPlacementL = personAnimationController.footLRig.GetComponent<FootPlacement>();
            FootPlacement footPlacementR = personAnimationController.footRRig.GetComponent<FootPlacement>();



            personAnimationController.DisableRigs();



            AnimationMode.StartAnimationMode();



            try
            {

                AnimationMode.SampleAnimationClip(testingPerson.gameObject, inputClip, 0);

                float maxheightL = 0.0f;
                float maxheightR = 0.0f;

                for (int i = 0; i < frameCount; ++i)
                {

                    float time = i * inputClip.length / (float)frameCount;
                    float dt = inputClip.length / (float)frameCount;

                    AnimationMode.SampleAnimationClip(testingPerson.gameObject, inputClip, time);

                    Vector3 footLPos = personAnimationController.boneFootL.position - Vector3.up * footPlacementL.data.placementOffsetHeight;
                    Vector3 footRPos = personAnimationController.boneFootR.position - Vector3.up * footPlacementL.data.placementOffsetHeight;

                    float valueL = footLPos.y;
                    float valueR = footRPos.y;

                    if (maxheightL < valueL)
                        maxheightL = valueL;
                    if (maxheightR < valueR)
                        maxheightR = valueR;

                    keyframesL[i] = new Keyframe(time, valueL);
                    keyframesR[i] = new Keyframe(time, valueR);

                }

                if(maxheightL >= 0.1f)
                    for (int i = 0; i < frameCount; ++i)
                    {

                        keyframesL[i].value = 1.0f - keyframesL[i].value / maxheightL;

                    }
                else
                    for (int i = 0; i < frameCount; ++i)
                    {

                        keyframesL[i].value = 1.0f;

                    }

                if (maxheightL >= 0.1f)
                    for (int i = 0; i < frameCount; ++i)
                    {

                        keyframesR[i].value = 1.0f - keyframesR[i].value / maxheightR;

                    }
                else
                    for (int i = 0; i < frameCount; ++i)
                    {

                        keyframesR[i].value = 1.0f;

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



            AnimationCurve curveL = new AnimationCurve(keyframesL);
            AnimationCurve curveR = new AnimationCurve(keyframesR);


            AnimationUtility.SetEditorCurve(
                outputClip,
                EditorCurveBinding.FloatCurve(curvePath, curveType, curveLProperty),
                curveL
            );
            AnimationUtility.SetEditorCurve(
                outputClip,
                EditorCurveBinding.FloatCurve(curvePath, curveType, curveRProperty),
                curveR
            );

        }
        #endregion

    }

}

#endif