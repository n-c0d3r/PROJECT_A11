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
    [RequireComponent(typeof(PersonViewController))]
    public class PlayablePersonBrain:
        PersonBrain
    {

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        #region Fields
        [Space(10)]
        [Header("Mouse Settings")]
        public bool lockMouse = true;
        public float mouseSensitivity = 0.045f;
        #endregion

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        #region Required Components
        private PlayerInput m_PlayerInput;
        public PlayerInput playerInput
        {
            get
            {

                if (m_PlayerInput == null)
                {

                    m_PlayerInput = GetComponent<PlayerInput>();

                }
                return m_PlayerInput;
            }
        }



        private PersonViewController m_ViewController;
        public PersonViewController viewController
        {
            get
            {

                if (m_ViewController == null)
                {

                    m_ViewController = GetComponent<PersonViewController>();

                }
                return m_ViewController;
            }
        }
        #endregion

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        #region Utility Methods
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
        #endregion

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        #region Methods
        private void BindInputActions()
        {

            m_PlayerInput.actions["GroundedOrdinaryMove"].performed += ctx => {

                controller.GroundedMove(ctx.ReadValue<Vector2>());

            };
            m_PlayerInput.actions["GroundedOrdinaryMove"].canceled += ctx => {

                controller.StopGroundedMoving();

            };

            m_PlayerInput.actions["Strafe"].performed += ctx => {

                controller.Strafe(ctx.ReadValue<Vector2>());

            };
            m_PlayerInput.actions["Strafe"].canceled += ctx => {

                controller.StopStrafing();

            };

            m_PlayerInput.actions["OrdinaryBody"].performed += ctx => {

                controller.StartOrdinaryBody();

            };
            m_PlayerInput.actions["OrdinaryBody"].canceled += ctx => {

                controller.StopOrdinaryBody();

            };

            m_PlayerInput.actions["Crouch"].performed += ctx => {

                controller.StartCrouching();

            };
            m_PlayerInput.actions["Crouch"].canceled += ctx => {

                controller.StopCrouching();

            };

            m_PlayerInput.actions["Look"].performed += ctx => {

                controller.Look(ProcessLookInput(ctx.ReadValue<Vector2>()));

            };
            m_PlayerInput.actions["Look"].canceled += ctx => {

                controller.Look(ProcessLookInput(ctx.ReadValue<Vector2>()));

            };

            m_PlayerInput.actions["Jump"].performed += ctx => {

                controller.Jump();

            };

            m_PlayerInput.actions["ChangeViewMode"].performed += ctx => {

                if(viewController.mode == PersonViewController.Mode.FPV)
                    viewController.ChangeViewMode(PersonViewController.Mode.TPV);
                else
                    viewController.ChangeViewMode(PersonViewController.Mode.FPV);

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
        #endregion

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        #region MonoBehavior Events
        protected override void Awake()
        {

            base.Awake();



            m_PlayerInput = GetComponent<PlayerInput>();
            m_ViewController = GetComponent<PersonViewController>();



            BindInputActions();

        }

        protected virtual void Update()
        {

            UpdateCursorState();

        }
        #endregion

    }

}