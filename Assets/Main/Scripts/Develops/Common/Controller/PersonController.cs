using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.InputSystem;
using UnityEngine.Windows;

namespace PROJECT_A11.Develops.Common
{

    [RequireComponent(typeof(PersonGroundChecker))]
    public class PersonController :
        Develops.Common.PawnController<Person>
    {

        [System.Serializable]
        public struct MovementInputState
        {

            public bool isMoving;
            public bool isWalking;
            public bool isCrouching;
            public bool isJumping;

        }

        [System.Serializable]
        public enum MoveMode
        {

            Running,
            Walking,
            Crouching

        }

        [System.Serializable]
        public enum MovementState
        {

            Idle,
            StartMoving,
            Moving,
            StopMoving,
            StartJumping,
            Jumping,
            InAirUp,
            InAirDown,
            Landing

        }



        [Space(10)]
        [Header("Movement Settings")]
        /// <summary>
        /// Speed-Distance curve (y is speed, x is distance).
        /// </summary>
        public AnimationCurve speedCurve;

        public float maxRunningSpeed;
        public float maxWalkingSpeed;
        public float maxCrouchingSpeed;

        public float startRunningTime = 1.5f;
        public float startWalkingTime = 1.0f;
        public float startCrouchingTime = 0.5f;

        public float groundedMoveDirUpdatingSpeed = 5.0f;
        public float inAirMoveDirUpdatingSpeed = 5.0f;
        public float groundedSpeedUpdatingSpeed = 10.0f;
        public float inAirSpeedUpdatingSpeed = 10.0f;

        public float jumpStartVelocity = 4.0f;
        public float startJumpingTime = 0.5f;
        public float jumpingTime = 0.5f;

        public float bhopStrength = 1.5f;

        [Space(10)]
        [Header("Head Settings")]
        public float lookSensitivity = 0.5f;
        public Transform headTransform;

#if UNITY_EDITOR
        [Space(10)]
        [Header("Debug Settings")]
        public Color targetMoveDirectionLineColor = Color.magenta;
        public Color currentMoveDirectionLineColor = Color.yellow;
#endif


        [Space(10)]
        [Header("Read-only Details")]
        [ReadOnly]
        [SerializeField]
        private MovementInputState m_MovementInputState = new MovementInputState { isCrouching = false, isJumping = false, isMoving = false, isWalking = false };
        [ReadOnly]
        [SerializeField]
        private MoveMode m_MoveMode = MoveMode.Running;
        [ReadOnly]
        [SerializeField]
        private MovementState m_MovementState;
        private MovementState movementState { get { return m_MovementState; } }

        [ReadOnly]
        [SerializeField]
        private float m_TargetSpeed = 0;
        [ReadOnly]
        [SerializeField]
        private Vector3 m_TargetMoveDirection = new Vector3(0, 0, 0);
        [ReadOnly]
        [SerializeField]
        private Vector3 m_TargetVelocity;

        private Vector3 m_StartPosition;
        private Vector3 m_StopPosition;

        private float m_MovingTime = 0;
        private float m_JumpingTime = 0;
        private bool m_IsJumpingForced = false;

        private Vector2 m_MovingInput;
        private Vector2 m_RotatingInput;
        public Vector2 rotatingInput { set { m_RotatingInput = value; } get { return m_RotatingInput; } }

        private PersonGroundChecker m_GroundChecker;
        public PersonGroundChecker groundChecker { get { return m_GroundChecker; } }

        private PersonBrain m_Brain;
        public PersonBrain brain { get { return m_Brain; } }



        private void UpdateInput()
        {

            UpdateInputVelocityAndSpeed();
            UpdateMovementState();

        }
        private void UpdateInputVelocityAndSpeed()
        {

            if (m_MovementInputState.isMoving)
            {

                m_MovingTime += Time.deltaTime;

                float startMovingTime = 0;
                float maxSpeed = 0;

                switch (m_MoveMode)
                {

                    case MoveMode.Running:
                        startMovingTime = startRunningTime;
                        maxSpeed = maxRunningSpeed;
                        break;
                    case MoveMode.Walking:
                        startMovingTime = startWalkingTime;
                        maxSpeed = maxWalkingSpeed;
                        break;
                    case MoveMode.Crouching:
                        startMovingTime = startCrouchingTime;
                        maxSpeed = maxCrouchingSpeed;
                        break;

                }

                float speedRatio = speedCurve.Evaluate(Mathf.InverseLerp(0.0f, startMovingTime, m_MovingTime));

                m_TargetSpeed = maxSpeed * speedRatio;

                if (speedRatio >= 0.98f)
                {

                    if (
                        m_MovementState != MovementState.InAirUp
                        && m_MovementState != MovementState.InAirDown
                    )
                        m_MovementState = MovementState.Moving;

                }

                m_TargetVelocity = m_TargetSpeed * m_TargetMoveDirection;

            }
            else
            {

                m_TargetSpeed = 0.0f;
                m_TargetVelocity = new Vector3(0, 0, 0);
                m_TargetMoveDirection = new Vector3(0, 0, 0);

            }

        }
        private void UpdateMovementState()
        {

            if (
                m_MovementState == MovementState.StopMoving
                && pawn.rigidbody.velocity.magnitude <= 0.05f
            )
            {

                m_MovementState = MovementState.Idle;

            }

        }

        private void UpdatePawn()
        {

            UpdaetPawnRotation();

            UpdatePawnVelocity();

            UpdatePawnMoving();

            UpdatePawnJumping();

            UpdatePawnInAir();

        }

        private void UpdaetPawnRotation()
        {

            Vector2 processedRotatingInput = m_RotatingInput;

            processedRotatingInput.x /= (float)Screen.width;
            processedRotatingInput.y /= (float)Screen.height;

            processedRotatingInput.x /= ((float)Screen.width / (float)Screen.height);



            transform.Rotate(lookSensitivity * m_RotatingInput.x * Vector3.up);
            headTransform.Rotate(-lookSensitivity * m_RotatingInput.y * Vector3.right);

            m_TargetMoveDirection = transform.forward * m_MovingInput.y + transform.right * m_MovingInput.x;

        }

        private void UpdatePawnVelocity()
        {

            Vector3 currVel = pawn.rigidbody.velocity;
            currVel.y = 0;
            Vector3 targetVel = m_TargetVelocity;

            Vector3 currDir = currVel.normalized;
            Vector3 targetDir = targetVel.normalized;

            bool isBraking = ((m_TargetSpeed == 0.0f) || (Vector3.Dot(currDir, targetDir) < -0.25f)) && groundChecker.isGrounded;

            float currSpeed = currVel.magnitude;
            float targetSpeed = targetVel.magnitude;

            if (isBraking)
            {

                currDir = (Vector3.Cross(targetDir, Vector3.up) * Vector3.Dot(currDir, targetDir)).normalized;

            }

            float moveDirUpdatingSpeed = groundedMoveDirUpdatingSpeed;
            float speedUpdatingSpeed = groundedSpeedUpdatingSpeed;

            if (!groundChecker.isGrounded)
            {

                moveDirUpdatingSpeed = inAirMoveDirUpdatingSpeed;
                speedUpdatingSpeed = inAirSpeedUpdatingSpeed;

            }

            Vector3 nextDir = Vector3.Lerp(currDir, targetDir, 
                Time.fixedDeltaTime * moveDirUpdatingSpeed
            );

            float nextSpeed = Mathf.Lerp(currSpeed, targetSpeed, Time.fixedDeltaTime * speedUpdatingSpeed);

            Vector3 nextVel = nextDir * nextSpeed;

            Vector3 acceleration = (nextVel - currVel) / Time.fixedDeltaTime;

            pawn.rigidbody.AddForce(pawn.rigidbody.mass * acceleration);

        }
        private void UpdatePawnJumping()
        {

            if (!m_MovementInputState.isJumping) 
                return;



            m_JumpingTime += Time.fixedDeltaTime;

                  

            if (m_JumpingTime >= startJumpingTime)
            {

                if (!m_IsJumpingForced)
                {

                    m_IsJumpingForced = true;

                    Vector3 currVel = pawn.rigidbody.velocity;

                    float minusYDown = -Mathf.Clamp(-currVel.y, 0.0f, Mathf.Infinity);

                    currVel.y = 0.0f;

                    Vector3 jumpDir = ((currVel.magnitude + 0.05f) * (1.0f / bhopStrength) * Vector3.up + currVel).normalized;

                    Vector3 startVel = jumpStartVelocity * jumpDir + minusYDown * Vector3.up;

                    Vector3 acceleration = startVel / Time.fixedDeltaTime;

                    acceleration -= Physics.gravity;

                    pawn.rigidbody.AddForce(pawn.rigidbody.mass * acceleration);

                }

                m_MovementState = MovementState.Jumping;

            }



            if (m_JumpingTime >= startJumpingTime + jumpingTime)
            {

                m_MovementInputState.isJumping = false;

            }


        }
        private void UpdatePawnInAir()
        {

            if (m_MovementInputState.isJumping) return;
            if (m_GroundChecker.isGrounded) return;

            Vector3 currVel = pawn.rigidbody.velocity;

            if (currVel.y > 0.0f)
                m_MovementState = MovementState.InAirUp;
            else
                m_MovementState = MovementState.InAirDown;

        }

        private void UpdatePawnMoving()
        {

            if (
                m_MovementState != MovementState.StartJumping
                && m_GroundChecker.isGrounded
            )
            {

                Vector3 currVel = pawn.rigidbody.velocity;

                if (currVel.magnitude <= 0.1f)
                {

                    m_MovementState = MovementState.Idle;

                }
                else
                {

                    if (
                        m_MovementState != MovementState.StartMoving
                        && m_MovementState != MovementState.StartMoving
                    )
                    {
                        m_MovementState = MovementState.Moving;
                    }

                }

            }

        }



        protected virtual void OnStartGrounded()
        {
                 
            m_Brain.OnStartGrounded();

        }
        protected virtual void OnEndGrounded()
        {

            m_Brain.OnEndGrounded();

        }

        private bool m_LastIsGround = false;
        private void UpdateGrounded()
        {

            if (m_LastIsGround != m_GroundChecker.isGrounded)
            {

                if (m_LastIsGround)
                {

                    OnEndGrounded();

                }
                else
                {

                    OnStartGrounded();

                }

            }

            m_LastIsGround = m_GroundChecker.isGrounded;

        }

        protected virtual void OnStartMoving(Vector3 input)
        {

            m_StartPosition = transform.position;

            m_MovementInputState.isMoving = true;

            m_MovingInput = input;

            m_TargetMoveDirection = transform.forward * input.y + transform.right * input.x;

            if(
                m_MovementState != MovementState.InAirUp
                && m_MovementState != MovementState.InAirDown
            )
                m_MovementState = MovementState.StartMoving;

            m_Brain.OnStartMoving(input);

        }
        protected virtual void OnMoving(Vector3 input)
        {

            m_MovementInputState.isMoving = true;

            m_MovingInput = input;

            m_TargetMoveDirection = transform.forward * input.y + transform.right * input.x;

            m_Brain.OnMoving(input);

        }
        protected virtual void OnStopMoving(Vector3 input)
        {

            m_MovingTime = 0.0f;

            m_MovementInputState.isMoving = false;

            m_StartPosition = transform.position;

            m_MovingInput = input;

            m_TargetMoveDirection = Vector3.zero;

            if (
                m_MovementState != MovementState.InAirUp
                && m_MovementState != MovementState.InAirDown
            )
                m_MovementState = MovementState.StopMoving;

            m_Brain.OnStopMoving(input);

        }
        public void OnInputMovingUpdate(Vector2 input)
        {

            if (input == Vector2.zero)
            {

                OnStopMoving(input);

            }
            else
            {

                if (!m_MovementInputState.isMoving)
                    OnStartMoving(input);
                OnMoving(input);

            }

        }
        public virtual void OnStartWalking()
        {

            m_MovementInputState.isWalking = true;
            m_MoveMode = MoveMode.Walking;

            m_Brain.OnStartWalking();

        }
        public virtual void OnStopWalking()
        {

            m_MovementInputState.isWalking = false;
            if (m_MovementInputState.isCrouching)
            {

                m_MoveMode = MoveMode.Crouching;

            }
            else
            {

                m_MoveMode = MoveMode.Running;

            }

            m_Brain.OnStopWalking();

        }
        public virtual void OnStartCrouching()
        {

            m_MovementInputState.isCrouching = true;
            m_MoveMode = MoveMode.Crouching;

            m_Brain.OnStartCrouching();

        }
        public virtual void OnStopCrouching()
        {

            m_MovementInputState.isCrouching = false;
            if (m_MovementInputState.isWalking)
            {

                m_MoveMode = MoveMode.Walking;

            }
            else
            {

                m_MoveMode = MoveMode.Running;

            }

            m_Brain.OnStopCrouching();

        }

        public virtual void OnStartJumping()
        {

            if (
                !m_GroundChecker.isGrounded
                || (m_MovementInputState.isJumping && m_JumpingTime > 0.0f)
            )
                return;

            m_MovementInputState.isJumping = true;
            m_JumpingTime = 0.0f;
            m_IsJumpingForced = false;
            m_MovementState = MovementState.StartJumping;

            m_Brain.OnStartJumping();

        }

        public virtual void OnLooking(Vector2 input)
        {

            m_RotatingInput = input;

            m_Brain.OnLooking(input);

        }



        protected override void Awake()
        {

            base.Awake();



            m_GroundChecker = GetComponent<PersonGroundChecker>();
            m_Brain = GetComponent<PersonBrain>();



            /// Set start value for some properties
            m_StartPosition = transform.position;
            m_MovementState = MovementState.Idle;

        }

        protected virtual void Update()
        {

            UpdateInput();

        }

        protected virtual void FixedUpdate()
        {

            UpdateGrounded();
            UpdatePawn();

        }



#if UNITY_EDITOR
        protected virtual void OnDrawGizmos()
        {

            Debug.DrawLine(transform.position, transform.position + m_TargetMoveDirection * 2.0f, targetMoveDirectionLineColor);
            Debug.DrawLine(transform.position, transform.position + pawn.rigidbody.velocity.normalized * 2.0f, currentMoveDirectionLineColor);

        }
#endif

    }

}