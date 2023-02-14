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
        public bool isMouseLocking = true;
        public bool isMouseVisible = false;
        public float MouseSensitivity = 1.0f;



        private PlayerInput m_PlayerInput;

        private bool m_LastIsGroundedOrdinaryMoving = false;
        private bool m_LastIsStrafing = false;



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



        protected override void Awake()
        {

            base.Awake();



            m_PlayerInput = GetComponent<PlayerInput>();



            m_PlayerInput.actions["GroundedOrdinaryMove"].performed += ctx => {

                if (
                    controller.input.targetInAirMovementMode == PersonController.InAirMovementMode.Strafing
                    || controller.currentMovement.environment != PersonController.Environment.Grounded
                ) return;

                if (!m_LastIsGroundedOrdinaryMoving)
                {

                    controller.OnStartGroundedMoving(ctx.ReadValue<Vector2>());

                }

                m_LastIsGroundedOrdinaryMoving = true;

            };
            m_PlayerInput.actions["GroundedOrdinaryMove"].canceled += ctx => {

                if (
                    controller.input.targetGroundedMovementMode != PersonController.GroundedMovementMode.Ordinary
                ) return;

                m_LastIsGroundedOrdinaryMoving = false;

                controller.OnStopGroundedMoving();

            };

            m_PlayerInput.actions["Strafe"].performed += ctx => {

                if (
                    controller.currentMovement.environment != PersonController.Environment.InAir
                    && controller.input.targetInAirMovementMode != PersonController.InAirMovementMode.Strafing
                ) 
                    return;

                if (!m_LastIsStrafing)
                {

                    controller.OnStartStrafing(ctx.ReadValue<Vector2>());

                }

                controller.OnStrafing(ctx.ReadValue<Vector2>());

                m_LastIsStrafing = true;

            };
            m_PlayerInput.actions["Strafe"].canceled += ctx => {

                if (controller.input.targetInAirMovementMode != PersonController.InAirMovementMode.Strafing) return;

                m_LastIsStrafing = false;

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

        }

        protected virtual void Update()
        {



        }

    }

}