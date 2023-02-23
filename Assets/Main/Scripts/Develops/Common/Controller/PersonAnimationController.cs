using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;



namespace PROJECT_A11.Develops.Common
{
    public class PersonAnimationController :
        PawnAnimationController<Person, PersonController>
    {

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        #region Fields and Private Properties
        public float moveInputUpdatingSpeed = 5.0f;
        public float animationSpeedUpdatingSpeed = 5.0f;
        public float maxAnimatorSpeed = 3.0f;


        [Space(10)]
        [Header("Bone Settings")]
        public Transform boneHip;
        public Transform boneFootL;
        public Transform boneFootR;


        [Space(10)]
        [Header("IK Settings")]
        public Transform ikFootL;
        public Transform ikFootR;



        [Space(10)]
        [Header("Read-only Details")]
        [ReadOnly]
        [SerializeField]
        private Vector3 m_LocalMoveInput = Vector3.zero;
        [ReadOnly]
        [SerializeField]
        private float m_AnimationSpeed = 1.0f;
        #endregion

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        #region Required Components
        private Animator m_Animator;
        public Animator animator { get { return m_Animator; } }
        #endregion

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        #region Methods
        private void UpdateGroundedMovement()
        {

            Vector3 localMoveInput = controller.input.targetMoveInput;

            m_LocalMoveInput = Vector3.Lerp(m_LocalMoveInput, localMoveInput, Mathf.Clamp01(Time.deltaTime * moveInputUpdatingSpeed));



            animator.SetFloat("GroundedMoveDirectionX", m_LocalMoveInput.x);
            animator.SetFloat("GroundedMoveDirectionZ", m_LocalMoveInput.y);



            animator.SetBool(
                "IsGroundedMoving",
                pawn.rigidbody.velocity.magnitude >= 0.02f
            );


            
            float groundedSpeed = animator.GetFloat("GroundedSpeed");

            float currentSpeed = pawn.rigidbody.velocity.magnitude;

            float targetSpeed = animator.speed;

            if (currentSpeed <= 0.5f || groundedSpeed <= 0.5f)
            {

                targetSpeed = 1.0f;

            }
            else
            {

                targetSpeed = currentSpeed / groundedSpeed;

            }

            m_AnimationSpeed = Mathf.Clamp(Mathf.Lerp(m_AnimationSpeed, targetSpeed, Mathf.Clamp01(Time.deltaTime * animationSpeedUpdatingSpeed)), 0.0f, maxAnimatorSpeed);

            animator.speed = m_AnimationSpeed;

        }

        private void UpdateBodyState()
        {

            animator.SetBool(
                "IsSprintingBody",
                controller.currentMovement.bodyState == PersonController.BodyState.Sprinting
            );
            animator.SetBool(
                "IsCrouchingBody",
                controller.currentMovement.bodyState == PersonController.BodyState.Crouching
            );
            animator.SetBool(
                "IsOrdinaryBody",
                controller.currentMovement.bodyState == PersonController.BodyState.Ordinary
            );

        }
        #endregion

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        #region MonoBehavior Events
        protected override void Awake()
        {

            base.Awake();

            m_Animator = GetComponent<Animator>();

        }

        protected override void Update()
        {

            base.Update();

            UpdateGroundedMovement();
            UpdateBodyState();

        }
        #endregion

    }

}