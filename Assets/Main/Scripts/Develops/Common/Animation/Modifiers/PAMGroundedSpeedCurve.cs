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
    /// PAMGroundedSpeedCurve stands for Person Animation Modifier Grounded Speed Curve
    /// </summary>
    [System.Serializable]
    public class PAMGroundedSpeedCurve :
        Develops.Common.AnimationModifier
    {

        [SerializeField]
        public string speedCurvePath = "";
        [SerializeField]
        public string speedCurveProperty = "GroundedSpeed";
        [SerializeField]
        public Type speedCurveType = typeof(Animator);
        [SerializeField]
        public float framesPerSecond = 30.0f;
        [SerializeField]
        public Person personPrefab;



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
            var animator = testingPerson.GetComponent<Animator>();
            animator.runtimeAnimatorController = animatorController;
            var personAnimation = testingPerson.GetComponent<PersonAnimation>();



            int frameCount = (int)(inputClip.length * framesPerSecond);

            Keyframe[] keyframes = new Keyframe[frameCount];



            AnimationMode.StartAnimationMode();



            try
            {

                AnimationMode.SampleAnimationClip(testingPerson.gameObject, inputClip, 0);

                Vector3 oldHipPos = personAnimation.boneHip.localPosition;

                for (int i = 1; i < frameCount; ++i)
                {

                    float time = i * inputClip.length / (float)frameCount;
                    float dt = inputClip.length / (float)frameCount;

                    AnimationMode.SampleAnimationClip(testingPerson.gameObject, inputClip, time);

                    Vector3 hipPos = personAnimation.boneHip.localPosition;

                    float speed = (hipPos - oldHipPos).magnitude / dt;

                    oldHipPos = hipPos;

                    keyframes[i] = new Keyframe(time, speed);

                    if (i == 1)
                    {

                        keyframes[0] = new Keyframe(0, speed);

                    }

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



            AnimationCurve speedCurve = new AnimationCurve(keyframes);


            AnimationUtility.SetEditorCurve(
                outputClip,
                EditorCurveBinding.FloatCurve(speedCurvePath, speedCurveType, speedCurveProperty),
                speedCurve
            );
            
        }

    }

}

#endif