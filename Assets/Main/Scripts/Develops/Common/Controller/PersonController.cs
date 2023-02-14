using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.InputSystem;
using UnityEngine.Windows;
using static PROJECT_A11.Develops.Common.PersonController;

namespace PROJECT_A11.Develops.Common
{

    [RequireComponent(typeof(PersonGroundChecker))]
    public class PersonController :
        Develops.Common.PawnController<Person>
    {

        [System.Serializable]
        public struct MovementSettings
        {

            public float maxWalkingSpeed;
            public float maxSprintingSpeed;
            public float maxCrouchingSpeed;

            public float walkingAcceleration;
            public float sprintingAcceleration;
            public float crouchingAcceleration;

            public float groundedStrafingAcceleration;
            public float inAirStrafingAcceleration;

            public float jumpStartVelocity;

            public Vector3 headRotatingAxis;
            public Vector3 selfRotatingAxis;

            public float headLookMin;
            public float headLookMax;

        }

        [System.Serializable]
        public struct HeadSettings
        {

            public Transform headTransform;

        }

        [System.Serializable]
        public enum Environment
        {

            Grounded,
            InAir,
            UnderWater,
            OnWater

        }

        [System.Serializable]
        public enum BodyState
        {

            Ordinary,
            Sprinting,
            Crouching,
            Swimming

        }

        [System.Serializable]
        public enum GroundedMovementMode
        {

            None, 
            Ordinary,
            Strafing

        }

        [System.Serializable]
        public enum InAirMovementMode
        {

            None,
            Strafing,
            Flying

        }

        [System.Serializable]
        public struct Input
        {

            public Environment targetEnvironment;
            public GroundedMovementMode targetGroundedMovementMode;
            public InAirMovementMode targetInAirMovementMode;
            public BodyState targetBodyState;

            public Vector3 targetMoveDirection;
            public Vector2 targetDeltaLook;

        }

        [System.Serializable]
        public struct Movement
        {

            public Environment environment;
            public GroundedMovementMode groundedMovementMode;
            public InAirMovementMode inAirMovementMode;
            public BodyState bodyState;

            public Vector3 velocity;
            public Vector2 look;

        }



        [Space(10)]
        public MovementSettings movementSettings = new MovementSettings {

            maxWalkingSpeed = 4.0f,
            maxSprintingSpeed = 6.0f,
            maxCrouchingSpeed = 2.7f,

            walkingAcceleration = 10.0f,
            sprintingAcceleration = 10.0f,
            crouchingAcceleration = 10.0f,

            groundedStrafingAcceleration = 10.0f,
            inAirStrafingAcceleration = 10.0f,

            jumpStartVelocity = 5.0f,

            headRotatingAxis = Vector3.right,
            selfRotatingAxis = Vector3.up,

            headLookMin = -Mathf.PI * 0.5f,
            headLookMax = Mathf.PI * 0.5f,
        
        };
        public HeadSettings headSettings = new HeadSettings { 
        

        
        };



        [Space(10)]
        [Header("Read-only Details")]
        [ReadOnly]
        [SerializeField]
        private Input m_Input = new Input {
        
            targetEnvironment = Environment.Grounded,
            targetGroundedMovementMode = GroundedMovementMode.None,
            targetInAirMovementMode = InAirMovementMode.None,
            targetBodyState = BodyState.Sprinting,

            targetMoveDirection = Vector3.zero,
            targetDeltaLook = Vector2.zero
        
        };
        public Input input { get { return m_Input; } }
        [ReadOnly]
        [SerializeField]
        private Movement m_CurrentMovement = new Movement { 
        
            environment = Environment.InAir,
            groundedMovementMode = GroundedMovementMode.None,
            inAirMovementMode = InAirMovementMode.None,
            bodyState = BodyState.Sprinting,

            velocity = Vector3.zero,
            look = Vector2.zero

        };
        public Movement currentMovement { get { return m_CurrentMovement; } }

        [ReadOnly]
        [SerializeField]
        private Quaternion m_DefaultHeadRotation;
        public Quaternion defaultHeadRotation { get { return m_DefaultHeadRotation; } }
        [ReadOnly]
        [SerializeField]
        private Quaternion m_DefaultSelfRotation;
        public Quaternion defaultSelfRotation { get { return m_DefaultSelfRotation; } }



        private PersonBrain m_Brain;
        public PersonBrain brain
        {

            get
            {

                if(m_Brain == null)
                    m_Brain = GetComponent<PersonBrain>();

                return m_Brain;
            }

        }

        private PersonGroundChecker m_GroundChecker;
        public PersonGroundChecker groundChecker
        {

            get
            {

                if(m_GroundChecker == null)
                    m_GroundChecker = GetComponent<PersonGroundChecker>();

                return m_GroundChecker;
            }

        }



        private void UpdateCurrentMovement()
        {

            /// Update Environment
            /// Grounded
            if (groundChecker.isGrounded && m_CurrentMovement.environment != Environment.Grounded)
            {

                OnStartGrounded();

            }
            if (!groundChecker.isGrounded && m_CurrentMovement.environment == Environment.Grounded)
            {

                OnEndGrounded();

            }
            m_CurrentMovement.environment = groundChecker.isGrounded ? Environment.Grounded : Environment.InAir;

        }
        private void ApplyInput()
        {



        }



        public virtual void OnStartGrounded()
        {

            m_CurrentMovement.environment = Environment.Grounded;

            if (
                m_CurrentMovement.inAirMovementMode == InAirMovementMode.Strafing
                || m_Input.targetInAirMovementMode == InAirMovementMode.Strafing
            )
            {

                m_CurrentMovement.groundedMovementMode = GroundedMovementMode.Strafing;
                m_Input.targetGroundedMovementMode = GroundedMovementMode.Strafing;

            }

            brain.OnStartGrounded();

        }
        public virtual void OnEndGrounded()
        {

            m_CurrentMovement.environment = Environment.InAir;

            brain.OnEndGrounded();

        }

        public virtual void OnStartGroundedMoving(Vector2 input)
        {
            if (
                m_CurrentMovement.inAirMovementMode != InAirMovementMode.Strafing
                && m_Input.targetInAirMovementMode != InAirMovementMode.Strafing
            )
            {

                m_Input.targetGroundedMovementMode = GroundedMovementMode.Ordinary;

            }

            brain.OnStartGroundedMoving(input);

        }
        public virtual void OnStopGroundedMoving()
        {

            m_Input.targetGroundedMovementMode = GroundedMovementMode.None;

            brain.OnStopGroundedMoving();

        }
        public virtual void OnStartOrdinaryBody()
        {

            m_Input.targetBodyState = BodyState.Ordinary;

            brain.OnStartOrdinaryBody();

        }
        public virtual void OnStopOrdinaryBody()
        {

            m_Input.targetBodyState = BodyState.Sprinting;

            brain.OnStopOrdinaryBody();

        }
        public virtual void OnStartCrouching()
        {

            m_Input.targetBodyState = BodyState.Crouching;

            brain.OnStartCrouching();

        }
        public virtual void OnStopCrouching()
        {

            m_Input.targetBodyState = BodyState.Sprinting;

            brain.OnStopCrouching();

        }

        public virtual void OnStartStrafing(Vector2 input)
        {

            m_Input.targetInAirMovementMode = InAirMovementMode.Strafing;

            brain.OnStartStrafing(input);

        }
        public virtual void OnStrafing(Vector2 input)
        {

            m_Input.targetInAirMovementMode = InAirMovementMode.Strafing;

            if (m_Input.targetGroundedMovementMode == GroundedMovementMode.Strafing)
            {

                m_Input.targetGroundedMovementMode = GroundedMovementMode.None;

            }

            brain.OnStrafing(input);

        }
        public virtual void OnStopStrafing()
        {

            m_Input.targetInAirMovementMode = InAirMovementMode.None;

            brain.OnStopStrafing();

        }
        public virtual void OnStartJumping()
        {

            m_Input.targetEnvironment = Environment.InAir;

            brain.OnStartJumping();

        }

        public virtual void OnLooking(Vector2 input)
        {

            m_Input.targetDeltaLook += input;

            brain.OnLooking(input);

        }



        protected override void Awake()
        {

            base.Awake();



            m_Brain = GetComponent<PersonBrain>();



            m_DefaultHeadRotation = headSettings.headTransform.rotation;
            m_DefaultSelfRotation = transform.rotation;

        }
        protected virtual void Update()
        {



        }
        protected virtual void FixedUpdate()
        {

            UpdateCurrentMovement();
            ApplyInput();

        }

    }

}