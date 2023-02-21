using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XR;
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

            public float walkingVelocityUpdatingSpeed;
            public float sprintingVelocityUpdatingSpeed;
            public float crouchingVelocityUpdatingSpeed;

            public float jumpStartVelocity;

            public Vector3 headRotatingAxis;
            public Vector3 selfRotatingAxis;

            public float headLookMin;
            public float headLookMax;

            public float jumpMaxDelay;
            public float bhopMaxDelay;

            public Vector3 gravity;

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
            Ordinary

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
            public Vector2 targetMoveInput;
            public Vector2 targetDeltaLook;
            public Vector2 targetDeltaLook_forFixedUpdate;

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

            public float jumpTime;

        }



        [Space(10)]
        public MovementSettings movementSettings = new MovementSettings {

            maxWalkingSpeed = 5.5f,
            maxSprintingSpeed = 7.0f,
            maxCrouchingSpeed = 4.0f,

            walkingVelocityUpdatingSpeed = 10.0f,
            sprintingVelocityUpdatingSpeed = 20.0f,
            crouchingVelocityUpdatingSpeed = 10.0f,

            jumpStartVelocity = 11.0f,

            headRotatingAxis = Vector3.right,
            selfRotatingAxis = Vector3.up,

            headLookMin = -90.0f,
            headLookMax = 90.0f,

            jumpMaxDelay = 0.3f,
            bhopMaxDelay = 0.2f,

            gravity = Vector3.down * 20.0f

        };



#if UNITY_EDITOR
        [Space(10)]
        [Header("Debug Settings")]
        public Color bottomCircleColor = Color.gray;
        public float bottomCircleRadius = 1.0f;
        public float bottomCircleThickness = 2.0f;
#endif



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
            targetDeltaLook = Vector2.zero,
            targetDeltaLook_forFixedUpdate = Vector2.zero
        
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
            look = Vector2.zero,

            jumpTime = 0.0f

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



        public Vector3 up
        {

            get
            {

                return groundChecker.checkedNormal;
            }

        }
        public Vector3 bottom
        {

            get
            {

                return -groundChecker.checkedNormal;
            }

        }

        public Vector3 forward
        {

            get
            {

                return Vector3.Cross(transform.right, up);
            }

        }
        public Vector3 back
        {

            get
            {

                return -Vector3.Cross(transform.right, up);
            }

        }

        public Vector3 right
        {

            get
            {

                return -Vector3.Cross(transform.forward, up);
            }

        }
        public Vector3 left
        {

            get
            {

                return Vector3.Cross(transform.forward, up);
            }

        }



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



        private void UpdateInput()
        {

            m_Input.targetMoveDirection = forward * m_Input.targetMoveInput.y + right * m_Input.targetMoveInput.x;

        }



        private void Rotate()
        {

            /// Rotates
            {

                m_CurrentMovement.look.x += input.targetDeltaLook.x;
                m_CurrentMovement.look.y = Mathf.Clamp(m_CurrentMovement.look.y + input.targetDeltaLook.y, movementSettings.headLookMin, movementSettings.headLookMax);

                transform.rotation = m_DefaultSelfRotation * Quaternion.Euler(movementSettings.selfRotatingAxis * m_CurrentMovement.look.x);
                pawn.headSettings.headTransform.localRotation = m_DefaultHeadRotation * Quaternion.Euler(-movementSettings.headRotatingAxis * m_CurrentMovement.look.y);

            }



            pawn.aimSettings.aimTargetTransform.position = pawn.headSettings.headTransform.position + pawn.headSettings.headTransform.forward;



            m_Input.targetDeltaLook = Vector2.zero;

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



            /// Update velocity,...
            m_CurrentMovement.velocity = pawn.rigidbody.velocity;

        }



        private void ApplyInput()
        {
            
            switch (m_CurrentMovement.environment)
            {

                case Environment.Grounded:



                    break;
                case Environment.InAir:



                    break;
            }



            bool isJumped = false;

            /// Jumps
            {

                if(m_Input.targetEnvironment == Environment.InAir)
                    m_CurrentMovement.jumpTime += Time.fixedDeltaTime;

                if (
                    m_Input.targetEnvironment == Environment.InAir
                    && m_CurrentMovement.environment == Environment.Grounded
                    && (
                        m_CurrentMovement.jumpTime <= movementSettings.jumpMaxDelay
                    )
                )
                {

                    ImmediatelyJump();

                    isJumped = true;

                }

            }



            if(!isJumped)
            {

                /// Ordinary Moves
                if (m_CurrentMovement.environment == Environment.Grounded)
                {

                    Vector3 currVelocity = m_CurrentMovement.velocity;

                    Vector3 targetMoveDirection = input.targetMoveDirection;

                    float targetSpeed = movementSettings.maxSprintingSpeed;
                    if (input.targetBodyState == BodyState.Ordinary)
                        targetSpeed = movementSettings.maxWalkingSpeed;
                    if (input.targetBodyState == BodyState.Crouching)
                        targetSpeed = movementSettings.maxCrouchingSpeed;

                    float velocityUpdatingSpeed = movementSettings.sprintingVelocityUpdatingSpeed;
                    if (input.targetBodyState == BodyState.Ordinary)
                        velocityUpdatingSpeed = movementSettings.walkingVelocityUpdatingSpeed;
                    if (input.targetBodyState == BodyState.Crouching)
                        velocityUpdatingSpeed = movementSettings.crouchingVelocityUpdatingSpeed;

                    Vector3 targetVelocity = targetSpeed * targetMoveDirection;

                    Vector3 newVelocity = Vector3.Lerp(currVelocity, targetVelocity, Mathf.Clamp01(Time.fixedDeltaTime * velocityUpdatingSpeed));

                    pawn.rigidbody.AddForce(pawn.rigidbody.mass * (newVelocity - currVelocity) / Time.fixedDeltaTime);



                    if (
                        input.targetMoveInput == Vector2.zero
                        && input.targetGroundedMovementMode == GroundedMovementMode.Ordinary
                    )
                    {

                        OnStopGroundedMoving();

                    }

                }



                /// Strafes
                if (input.targetInAirMovementMode == InAirMovementMode.Strafing)
                {

                    Vector3 currVelocity = m_CurrentMovement.velocity;
                    currVelocity.y = 0.0f;

                    Vector3 targetVelocity = (Quaternion.Euler(movementSettings.selfRotatingAxis * input.targetDeltaLook_forFixedUpdate.x) * currVelocity);

                    pawn.rigidbody.AddForce(pawn.rigidbody.mass * (targetVelocity - currVelocity) / Time.fixedDeltaTime);

                }

            }



            m_Input.targetDeltaLook_forFixedUpdate = Vector2.zero;


            m_CurrentMovement.groundedMovementMode = input.targetGroundedMovementMode;
            m_CurrentMovement.inAirMovementMode = input.targetInAirMovementMode;
            m_CurrentMovement.bodyState = input.targetBodyState;

        }

        private void ApplyGravity()
        {

            pawn.rigidbody.AddForce(pawn.rigidbody.mass * (movementSettings.gravity - Physics.gravity));

        }



        protected virtual void ImmediatelyJump()
        {

            m_Input.targetEnvironment = Environment.Grounded;

            m_CurrentMovement.jumpTime = movementSettings.jumpMaxDelay * 2.0f;

            Vector3 currVelocity = pawn.rigidbody.velocity;
            Vector3 targetVelocity = currVelocity;

            targetVelocity.y = Mathf.Max(currVelocity.y, movementSettings.jumpStartVelocity);

            pawn.rigidbody.velocity = targetVelocity;

            //Debug.Log(targetVelocity);

            //pawn.rigidbody.AddForce(pawn.rigidbody.mass * (targetVelocity - currVelocity) / Time.fixedDeltaTime);

        }



        public virtual void OnStartGrounded()
        {

            m_CurrentMovement.environment = Environment.Grounded;

            m_Input.targetInAirMovementMode = InAirMovementMode.None;

            m_CurrentMovement.jumpTime = 0.0f;

            if (m_Input.targetMoveInput != Vector2.zero)
            {

                OnGroundedMoving(m_Input.targetMoveInput);

            }

            brain.OnStartGrounded();

        }
        public virtual void OnEndGrounded()
        {

            m_CurrentMovement.environment = Environment.InAir;

            m_Input.targetGroundedMovementMode = GroundedMovementMode.None;

            if (m_Input.targetMoveInput != Vector2.zero)
            {

                OnStrafing(m_Input.targetMoveInput);

            }

            brain.OnEndGrounded();

        }

        protected virtual void OnStartGroundedMoving(Vector2 input)
        {

            m_Input.targetMoveInput = input;

            if (currentMovement.environment != PersonController.Environment.Grounded)
                return;

            m_Input.targetGroundedMovementMode = GroundedMovementMode.Ordinary;

            brain.OnStartGroundedMoving(input);

        }
        public virtual void OnGroundedMoving(Vector2 input)
        {

            m_Input.targetMoveInput = input;

            if (currentMovement.environment != PersonController.Environment.Grounded)
                return;

            if (m_Input.targetGroundedMovementMode != GroundedMovementMode.Ordinary)
            {

                OnStartGroundedMoving(input);

            }

            m_Input.targetGroundedMovementMode = GroundedMovementMode.Ordinary;

            brain.OnStartGroundedMoving(input);

        }

        public virtual void OnStopGroundedMoving()
        {

            if (m_Input.targetGroundedMovementMode != GroundedMovementMode.Ordinary) return;

            m_Input.targetGroundedMovementMode = GroundedMovementMode.None;

            m_Input.targetMoveInput = Vector2.zero;

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

        protected virtual void OnStartStrafing(Vector2 input)
        {

            if (currentMovement.environment != PersonController.Environment.InAir)
                return;

            m_Input.targetInAirMovementMode = InAirMovementMode.Strafing;
            m_Input.targetMoveInput = input;

            brain.OnStartStrafing(input);

        }
        public virtual void OnStrafing(Vector2 input)
        {

            if (currentMovement.environment != PersonController.Environment.InAir)
                return;

            if (m_Input.targetInAirMovementMode != InAirMovementMode.Strafing)
            {

                OnStartStrafing(input);

            }

            m_Input.targetInAirMovementMode = InAirMovementMode.Strafing;
            m_Input.targetMoveInput = input;

            brain.OnStrafing(input);

        }
        public virtual void OnStopStrafing()
        {

            if (m_Input.targetInAirMovementMode != InAirMovementMode.Strafing) return;

            m_Input.targetInAirMovementMode = InAirMovementMode.None;
            m_Input.targetMoveInput = Vector2.zero;

            brain.OnStopStrafing();

        }
        public virtual void OnStartJumping()
        {

            m_Input.targetEnvironment = Environment.InAir;
            m_CurrentMovement.jumpTime = 0.0f;

            brain.OnStartJumping();

        }

        public virtual void OnLooking(Vector2 input)
        {

            m_Input.targetDeltaLook += input;
            m_Input.targetDeltaLook_forFixedUpdate += input;

            brain.OnLooking(input);

        }



        protected override void Awake()
        {

            base.Awake();



            m_Brain = GetComponent<PersonBrain>();



            m_DefaultHeadRotation = pawn.headSettings.headTransform.localRotation;
            m_DefaultSelfRotation = transform.rotation;

        }
        protected virtual void Update()
        {

            UpdateInput();
            Rotate();

        }
        protected virtual void FixedUpdate()
        {

            UpdateCurrentMovement();
            ApplyGravity();
            ApplyInput();

        }



#if UNITY_EDITOR
        private void OnDrawGizmos()
        {

            if(groundChecker.isGrounded)
            {

                Handles.color = bottomCircleColor;
                Handles.DrawWireDisc(transform.position, groundChecker.checkedNormal, bottomCircleRadius, bottomCircleThickness);

            }

        }
#endif

    }

}