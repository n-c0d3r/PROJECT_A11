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

        public bool isMouseLocking = true;
        public bool isMouseVisible = false;



        private PlayerInput m_PlayerInput;

        private Vector2 m_RotatingInput;



        protected override void Awake()
        {

            base.Awake();



            /// Bind actions
            m_PlayerInput = GetComponent<PlayerInput>();

            m_PlayerInput.actions["Move"].performed += ctx => controller.OnInputMovingUpdate(ctx.ReadValue<Vector2>());
            m_PlayerInput.actions["Move"].canceled += ctx => controller.OnInputMovingUpdate(ctx.ReadValue<Vector2>());

            m_PlayerInput.actions["Walk"].performed += ctx => controller.OnStartWalking();
            m_PlayerInput.actions["Walk"].canceled += ctx => controller.OnStopWalking();

            m_PlayerInput.actions["Crouch"].performed += ctx => controller.OnStartCrouching();
            m_PlayerInput.actions["Crouch"].canceled += ctx => controller.OnStopCrouching();

            m_PlayerInput.actions["Jump"].performed += ctx => controller.OnStartJumping();

            m_PlayerInput.actions["Look"].performed += ctx => controller.OnLooking(ctx.ReadValue<Vector2>());

        }

        private void Update()
        {

            UpdateCirsor();
            UpdateRotationInput();

        }

        private void UpdateRotationInput()
        {

            controller.rotatingInput = m_PlayerInput.actions["Look"].ReadValue<Vector2>();

        }

        private void UpdateCirsor()
        {

            if (isMouseLocking)
            {

                Cursor.lockState = CursorLockMode.Locked;

            }
            else
            {

                Cursor.lockState = CursorLockMode.None;

            }

            Cursor.visible = isMouseVisible;

        }

    }

}