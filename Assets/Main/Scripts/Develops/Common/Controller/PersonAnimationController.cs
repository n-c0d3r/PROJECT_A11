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
        public float groundHeightUpdatingSpeed = 3.0f;
        public float groundHeightOfHighestFalling = 4.0f;


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
        private Vector3 m_MoveInput = Vector3.zero;
        [ReadOnly]
        [SerializeField]
        private float m_AnimationSpeed = 1.0f;
        [ReadOnly]
        [SerializeField]
        private float m_GroundHeight = 0.0f;
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

            Vector3 moveInput = controller.input.targetMoveInput;

            m_MoveInput = Vector3.Lerp(m_MoveInput, moveInput, Mathf.Clamp01(Time.deltaTime * moveInputUpdatingSpeed));



            animator.SetFloat("MoveInputX", m_MoveInput.x);
            animator.SetFloat("MoveInputY", m_MoveInput.y);



            Vector3 currentPlanarVel = pawn.rigidbody.velocity;
            currentPlanarVel.y = 0.0f;

            float currentSpeed = currentPlanarVel.magnitude;



            bool isMoving = (currentSpeed >= 1.0f) || (controller.input.targetGroundedMovementMode != PersonController.GroundedMovementMode.None);

            animator.SetBool(
                "IsMoving",
                isMoving
            );


            
            float groundedSpeed = animator.GetFloat("GroundedSpeed");

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

        private void UpdateInAirMovement()
        {

            m_AnimationSpeed = Mathf.Clamp(Mathf.Lerp(m_AnimationSpeed, 1.0f, Mathf.Clamp01(Time.deltaTime * animationSpeedUpdatingSpeed)), 0.0f, maxAnimatorSpeed);

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

        private void UpdateEnvironment()
        {

            animator.SetBool(
                "IsGrounded",
                controller.currentMovement.environment == PersonController.Environment.Grounded
            );

            animator.SetBool(
                "IsInAir",
                controller.currentMovement.environment == PersonController.Environment.InAir
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

            UpdateEnvironment();


            
            switch(controller.currentMovement.environment)
            {

                case PersonController.Environment.Grounded:
                    UpdateGroundedMovement();
                    break;

                case PersonController.Environment.InAir:
                    UpdateInAirMovement();
                    break;

            }



            m_GroundHeight = Mathf.Lerp(m_GroundHeight, controller.currentMovement.groundHeight, Mathf.Clamp01(Time.deltaTime * groundHeightUpdatingSpeed));
            animator.SetFloat("GroundHeight", m_GroundHeight);
            animator.SetFloat("FallingStrength", m_GroundHeight / groundHeightOfHighestFalling);



            UpdateBodyState();

        }

        #endregion

    }

}