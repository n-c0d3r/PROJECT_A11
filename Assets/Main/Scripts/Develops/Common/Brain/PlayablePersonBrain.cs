using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.InputSystem;



namespace PROJECT_A11.Develops.Common
{

    /// <summary>
    /// PlayablePersonBrain means that this pawn brain is controllable by player.
    /// </summary>
    [RequireComponent(typeof(PlayerInput))]
    public class PlayablePersonBrain:
        PersonBrain
    {

        [Space(10)]
        [Header("Mouse Settings")]
        public bool lockMouse = true;
        public float mouseSensitivity = 0.045f;



        private PlayerInput m_PlayerInput;



        private bool isCurrentDeviceMouse
        {
            get
            {
#if ENABLE_INPUT_SYSTEM
                return m_PlayerInput.currentControlScheme == "Keyboard&Mouse";
#else
				return false;
#endif
            }
        }



        public Vector2 ProcessLookInput(Vector2 raw)
        {

            return raw * mouseSensitivity * (isCurrentDeviceMouse ? 1.0f : Time.deltaTime);
        }



        private void BindInputActions()
        {

            m_PlayerInput.actions["GroundedOrdinaryMove"].performed += ctx => {

                controller.OnGroundedMoving(ctx.ReadValue<Vector2>());

            };
            m_PlayerInput.actions["GroundedOrdinaryMove"].canceled += ctx => {

                controller.OnStopGroundedMoving();

            };

            m_PlayerInput.actions["Strafe"].performed += ctx => {

                controller.OnStrafing(ctx.ReadValue<Vector2>());

            };
            m_PlayerInput.actions["Strafe"].canceled += ctx => {

                controller.OnStopStrafing();

            };

            m_PlayerInput.actions["OrdinaryBody"].performed += ctx => {

                controller.OnStartOrdinaryBody();

            };
            m_PlayerInput.actions["OrdinaryBody"].canceled += ctx => {

                controller.OnStopOrdinaryBody();

            };

            m_PlayerInput.actions["Crouch"].performed += ctx => {

                controller.OnStartCrouching();

            };
            m_PlayerInput.actions["Crouch"].canceled += ctx => {

                controller.OnStopCrouching();

            };

            m_PlayerInput.actions["Look"].performed += ctx => {

                controller.OnLooking(ProcessLookInput(ctx.ReadValue<Vector2>()));

            };
            m_PlayerInput.actions["Look"].canceled += ctx => {

                controller.OnLooking(ProcessLookInput(ctx.ReadValue<Vector2>()));

            };

            m_PlayerInput.actions["Jump"].performed += ctx => {

                controller.OnStartJumping();

            };

        }

        private void UpdateCursorState()
        {

            if (lockMouse)
            {

                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;

            }
            else
            {

                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;

            }

        }



        protected override void Awake()
        {

            base.Awake();



            m_PlayerInput = GetComponent<PlayerInput>();



            BindInputActions();

        }

        protected virtual void Update()
        {

            UpdateCursorState();

        }

    }

}