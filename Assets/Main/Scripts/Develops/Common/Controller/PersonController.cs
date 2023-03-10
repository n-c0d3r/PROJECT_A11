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

    [AddComponentMenu("PROJECT_A11/Common/Controller/PersonController")]
    [RequireComponent(typeof(PersonGroundChecker))]
    public class PersonController :
        Develops.Common.PawnController<Person>
    {

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        #region Nested Types
        [System.Serializable]
        public struct MovementSettings
        {

            public float maxGroundedSpeed_Ordinary;
            public float maxGroundedSpeed_Sprinting;
            public float maxGroundedSpeed_Crouching;

            public float velocityUpdatingSpeed_Ordinary;
            public float velocityUpdatingSpeed_Sprinting;
            public float velocityUpdatingSpeed_Crouching;

            public float jumpStartUpVelocity;
            public float jumpStartPlanarVelocity_Ordinary;
            public float jumpStartPlanarVelocity_Sprinting;
            public float jumpStartPlanarVelocity_Crouching;

            public Vector3 headRotatingAxis;
            public Vector3 selfRotatingAxis;

            public float headLookMin;
            public float headLookMax;

            public float jumpMaxDelay;
            public float bhopMaxDelay;
            public float bhopPlanarPowerMin;

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
            public float bhopPlanarPower;

            public float groundHeight;

        }
        #endregion

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        #region Fields and Private Properties
        [Space(10)]
        public MovementSettings movementSettings = new MovementSettings {

            maxGroundedSpeed_Ordinary = 5.0f,
            maxGroundedSpeed_Sprinting = 6.5f,
            maxGroundedSpeed_Crouching = 3.5f,

            velocityUpdatingSpeed_Ordinary = 10.0f,
            velocityUpdatingSpeed_Sprinting = 10.0f,
            velocityUpdatingSpeed_Crouching = 10.0f,

            jumpStartUpVelocity = 9.0f,
            jumpStartPlanarVelocity_Ordinary = 5.0f,
            jumpStartPlanarVelocity_Sprinting = 6.0f,
            jumpStartPlanarVelocity_Crouching = 3.5f,

            headRotatingAxis = Vector3.right,
            selfRotatingAxis = Vector3.up,

            headLookMin = -80.0f,
            headLookMax = 80.0f,

            jumpMaxDelay = 0.3f,
            bhopMaxDelay = 0.2f,
            bhopPlanarPowerMin = 0.15f,

            gravity = Vector3.down * 20.0f

        };



        [Space(10)]
        [Header("Bag Settings")]
        public Item defaultItem;



#if UNITY_EDITOR
        [Space(10)]
        [Header("Debug Settings")]
        public Color groundBottomCircleColor = Color.gray;
        public float groundBottomCircleRadius = 0.55f;
        public float groundBottomCircleThickness = 2.0f;
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

            jumpTime = 0.0f,

            groundHeight = 0.0f

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

        [ReadOnly]
        [SerializeField]
        private Item m_CurrentItem;
        public Item currentItem { get { return m_CurrentItem; } }
        #endregion

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        #region Utility Getters
        public Vector3 groundUp
        {

            get
            {

                return groundChecker.checkedNormal;
            }

        }
        public Vector3 groundBottom
        {

            get
            {

                return -groundChecker.checkedNormal;
            }

        }

        public Vector3 groundForward
        {

            get
            {

                return Vector3.Cross(transform.right, groundUp);
            }

        }
        public Vector3 groundBack
        {

            get
            {

                return -Vector3.Cross(transform.right, groundUp);
            }

        }

        public Vector3 groundRight
        {

            get
            {

                return -Vector3.Cross(transform.forward, groundUp);
            }

        }
        public Vector3 groundLeft
        {

            get
            {

                return Vector3.Cross(transform.forward, groundUp);
            }

        }



        public Matrix4x4 movementSpaceToWorldSpaceMatrix
        {

            get
            {

                Vector3 u = groundUp;
                Vector3 f = groundForward;
                Vector3 r = groundRight;

                return new Matrix4x4(
                    new Vector4(r.x, r.y, r.z, 0.0f),
                    new Vector4(u.x, u.y, u.z, 0.0f),
                    new Vector4(f.x, f.y, f.z, 0.0f),
                    new Vector4(transform.position.x, transform.position.y, transform.position.z, 1.0f)
                );
            }

        }
        public Matrix4x4 worldSpaceToMovementSpaceMatrix
        {

            get
            {

                return movementSpaceToWorldSpaceMatrix.inverse;
            }

        }



        public float maxGroundedSpeed
        {

            get
            {

                if (input.targetBodyState == BodyState.Ordinary)
                    return movementSettings.maxGroundedSpeed_Ordinary;
                if (input.targetBodyState == BodyState.Crouching)
                    return movementSettings.maxGroundedSpeed_Crouching;

                return movementSettings.maxGroundedSpeed_Sprinting;

            }

        }
        public float maxGroundedVelUpdatingSpeed
        {

            get
            {

                if (input.targetBodyState == BodyState.Ordinary)
                    return movementSettings.velocityUpdatingSpeed_Ordinary;
                if (input.targetBodyState == BodyState.Crouching)
                    return movementSettings.velocityUpdatingSpeed_Crouching;

                return movementSettings.velocityUpdatingSpeed_Sprinting;

            }

        }
        public float jumpStartPlanarVelocity
        {

            get
            {

                if (input.targetBodyState == BodyState.Ordinary)
                    return movementSettings.jumpStartPlanarVelocity_Ordinary;
                if (input.targetBodyState == BodyState.Crouching)
                    return movementSettings.jumpStartPlanarVelocity_Crouching;

                return movementSettings.jumpStartPlanarVelocity_Sprinting;

            }

        }
        #endregion

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        #region Required Components
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
        #endregion

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        #region Utility Methods  
        public Vector3 PointToMovementSpace(Vector3 point)
        {

            return worldSpaceToMovementSpaceMatrix * new Vector4(point.x, point.y, point.z, 1.0f);
        }
        public Vector3 VectorToMovementSpace(Vector3 v)
        {

            return worldSpaceToMovementSpaceMatrix * new Vector4(v.x, v.y, v.z, 0.0f);
        }
        #endregion

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        #region Controller Methods
        private void UpdateInput(float deltaTime)
        {

            m_Input.targetMoveDirection = Vector3.Normalize(groundForward * m_Input.targetMoveInput.y + groundRight * m_Input.targetMoveInput.x);

        }

        private void UpdateRotation(float deltaTime)
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



        private void UpdateCurrentMovement(float deltaTime)
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



            /// Update Ground Height
            RaycastHit groundHit;

            if (m_CurrentMovement.environment == Environment.InAir) { 

                if (Physics.Raycast(transform.position + 0.1f * Vector3.up, Vector3.down, out groundHit, Mathf.Infinity, groundChecker.mask))
                {

                    float groundHeight = (transform.position - groundHit.point).y;

                    m_CurrentMovement.groundHeight = groundHeight;

                } 

            }
            else
            {

                m_CurrentMovement.groundHeight = 0.0f;

            }

        }

        private void ApplyMovement(float deltaTime)
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

                if (
                    m_Input.targetEnvironment == Environment.InAir
                    && m_CurrentMovement.environment == Environment.Grounded
                    && (
                        m_CurrentMovement.jumpTime <= movementSettings.jumpMaxDelay
                    )
                )
                {

                    ImmediatelyJump(deltaTime);

                    isJumped = true;

                }
                else
                if (m_Input.targetEnvironment == Environment.InAir)
                {

                    m_CurrentMovement.jumpTime += deltaTime;

                }

            }



            if(!isJumped)
            {

                /// Ordinary Moves
                if (m_CurrentMovement.environment == Environment.Grounded)
                {

                    Vector3 currVelocity = m_CurrentMovement.velocity;

                    Vector3 targetMoveDirection = input.targetMoveDirection;

                    float targetSpeed = maxGroundedSpeed;

                    float velocityUpdatingSpeed = maxGroundedVelUpdatingSpeed;

                    Vector3 targetVelocity = targetSpeed * targetMoveDirection;

                    Vector3 newVelocity = Vector3.Lerp(currVelocity, targetVelocity, Mathf.Clamp01(deltaTime * velocityUpdatingSpeed));

                    pawn.rigidbody.AddForce(pawn.rigidbody.mass * ((newVelocity - currVelocity) / deltaTime - movementSettings.gravity));



                    if (
                        input.targetMoveInput == Vector2.zero
                        && input.targetGroundedMovementMode == GroundedMovementMode.Ordinary
                    )
                    {

                        OnStopGroundedMoving();

                    }

                }



                /// Strafes
                if (
                    input.targetInAirMovementMode == InAirMovementMode.Strafing
                    && m_CurrentMovement.environment == Environment.InAir
                )
                {

                    Vector3 currVelocity = m_CurrentMovement.velocity;
                    currVelocity.y = 0.0f;

                    Vector3 targetVelocity = (Quaternion.Euler(movementSettings.selfRotatingAxis * input.targetDeltaLook_forFixedUpdate.x) * currVelocity);

                    pawn.rigidbody.AddForce(pawn.rigidbody.mass * (targetVelocity - currVelocity) / deltaTime);

                }

            }



            m_Input.targetDeltaLook_forFixedUpdate = Vector2.zero;


            m_CurrentMovement.groundedMovementMode = input.targetGroundedMovementMode;
            m_CurrentMovement.inAirMovementMode = input.targetInAirMovementMode;
            m_CurrentMovement.bodyState = input.targetBodyState;

        }

        private void ApplyGravity(float deltaTime)
        {

            pawn.rigidbody.AddForce(pawn.rigidbody.mass * (movementSettings.gravity - Physics.gravity));

        }

        protected virtual void ImmediatelyJump(float deltaTime)
        {

            m_CurrentMovement.bhopPlanarPower = (m_CurrentMovement.jumpTime == 0.0f || m_CurrentMovement.jumpTime > movementSettings.bhopMaxDelay) ? movementSettings.bhopPlanarPowerMin : (
            
                (1.0f - Mathf.Clamp01(m_CurrentMovement.jumpTime / movementSettings.bhopMaxDelay)) * (1.0f - movementSettings.bhopPlanarPowerMin) + movementSettings.bhopPlanarPowerMin

            );

            m_Input.targetEnvironment = Environment.Grounded;

            m_CurrentMovement.jumpTime = movementSettings.jumpMaxDelay * 2.0f;

            Vector3 currVelocity = pawn.rigidbody.velocity;

            Vector3 movementSpaceCurrVel = worldSpaceToMovementSpaceMatrix * currVelocity;
            movementSpaceCurrVel.y = 0.0f;
            Vector3 movementSpaceMoveVel = worldSpaceToMovementSpaceMatrix * (input.targetMoveInput.x * groundRight + input.targetMoveInput.y * groundForward) * jumpStartPlanarVelocity;
            movementSpaceMoveVel.y = 0.0f;
            movementSpaceMoveVel *= m_CurrentMovement.bhopPlanarPower;

            movementSpaceMoveVel += movementSpaceCurrVel;

            Vector3 planarMoveVel = movementSpaceToWorldSpaceMatrix * movementSpaceMoveVel;

            Vector3 jumpVel = planarMoveVel + Vector3.up * movementSettings.jumpStartUpVelocity;

            pawn.rigidbody.velocity = jumpVel;

        }
        #endregion

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        #region Controller Events
        protected virtual void OnStartGrounded()
        {

            m_CurrentMovement.environment = Environment.Grounded;

            m_Input.targetInAirMovementMode = InAirMovementMode.None;

            //m_CurrentMovement.jumpTime = 0.0f;

            if (m_Input.targetMoveInput != Vector2.zero)
            {

                OnGroundedMoving(m_Input.targetMoveInput);

            }

            brain.OnStartGrounded();

        }
        protected virtual void OnEndGrounded()
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
        protected virtual void OnGroundedMoving(Vector2 input)
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

        protected virtual void OnStopGroundedMoving()
        {

            if (m_Input.targetGroundedMovementMode != GroundedMovementMode.Ordinary) return;

            m_Input.targetGroundedMovementMode = GroundedMovementMode.None;

            m_Input.targetMoveInput = Vector2.zero;

            brain.OnStopGroundedMoving();

        }
        protected virtual void OnStartOrdinaryBody()
        {

            m_Input.targetBodyState = BodyState.Ordinary;

            brain.OnStartOrdinaryBody();

        }
        protected virtual void OnStopOrdinaryBody()
        {

            m_Input.targetBodyState = BodyState.Sprinting;

            brain.OnStopOrdinaryBody();

        }
        protected virtual void OnStartCrouching()
        {

            m_Input.targetBodyState = BodyState.Crouching;

            brain.OnStartCrouching();

        }
        protected virtual void OnStopCrouching()
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
        protected virtual void OnStrafing(Vector2 input)
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
        protected virtual void OnStopStrafing()
        {

            if (m_Input.targetInAirMovementMode != InAirMovementMode.Strafing) return;

            m_Input.targetInAirMovementMode = InAirMovementMode.None;
            m_Input.targetMoveInput = Vector2.zero;

            brain.OnStopStrafing();

        }
        protected virtual void OnStartJumping()
        {

            m_CurrentMovement.jumpTime = 0.0f;
            m_Input.targetEnvironment = Environment.InAir;

            brain.OnStartJumping();

        }

        protected virtual void OnLooking(Vector2 input)
        {

            m_Input.targetDeltaLook += input;
            m_Input.targetDeltaLook_forFixedUpdate += input;

            brain.OnLooking(input);

        }

        protected virtual void OnChangingItem(Item newItem)
        {

            if (newItem == null) return;



            if(m_CurrentItem != null)
            {

                m_CurrentItem.StopUsing();

            }

            newItem.Use();

            m_CurrentItem = newItem;

        }
        #endregion

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        #region Input Methods
        public void GroundedMove(Vector2 input)
        {

            OnGroundedMoving(input);
        }
        public void StopGroundedMoving()
        {

            OnStopGroundedMoving();
        }

        public void Strafe(Vector2 input)
        {

            OnStrafing(input);
        }
        public void StopStrafing()
        {

            OnStopStrafing();
        }

        public void StartOrdinaryBody()
        {

            OnStartOrdinaryBody();
        }
        public void StopOrdinaryBody()
        {

            OnStopOrdinaryBody();
        }

        public void StartCrouching()
        {

            OnStartCrouching();
        }
        public void StopCrouching()
        {

            OnStopCrouching();
        }

        public void Look(Vector2 input)
        {

            OnLooking(input);
        }

        public void Jump()
        {

            OnStartJumping();
        }

        public void UseItem(Item newItem)
        {

            Item previousItem = m_CurrentItem;

        }
        #endregion

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        #region MonoBehavior Events
        protected override void Awake()
        {

            base.Awake();



            m_Brain = GetComponent<PersonBrain>();



            m_DefaultHeadRotation = pawn.headSettings.headTransform.localRotation;
            m_DefaultSelfRotation = transform.rotation;

        }

        protected virtual void Start()
        {

            UseItem(defaultItem);

        }

        protected virtual void Update()
        {

            UpdateInput(Time.deltaTime);
            UpdateRotation(Time.deltaTime);

        }
        protected virtual void FixedUpdate()
        {

            UpdateCurrentMovement(Time.fixedDeltaTime);
            ApplyGravity(Time.fixedDeltaTime);
            ApplyMovement(Time.fixedDeltaTime);

        }
        #endregion

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {

            if(groundChecker.isGrounded)
            {

                Handles.color = groundBottomCircleColor;
                Handles.DrawWireDisc(transform.position, groundChecker.checkedNormal, groundBottomCircleRadius, groundBottomCircleThickness);

            }

        }
#endif

    }

}