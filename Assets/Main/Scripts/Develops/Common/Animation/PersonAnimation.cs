using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;



namespace PROJECT_A11.Develops.Common
{
    public class PersonAnimation :
        PawnAnimation<Person, PersonController>
    {

        public float moveInputUpdatingSpeed = 5.0f;
        public float moveSpeedUpdatingSpeed = 5.0f;


        [Space(10)]
        [Header("Bone Settings")]
        public Transform bonePelvis;
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
        private float m_MoveSpeed = 0.0f;



        private Animator m_Animator;
        public Animator animator { get { return m_Animator; } }



        private void UpdateGroundedMovementProperties()
        {

            Vector3 localMoveInput = controller.input.targetMoveInput;

            m_LocalMoveInput = Vector3.Lerp(m_LocalMoveInput, localMoveInput, Mathf.Clamp01(Time.deltaTime * moveInputUpdatingSpeed));



            animator.SetFloat("GroundedMoveDirectionX", m_LocalMoveInput.x);
            animator.SetFloat("GroundedMoveDirectionZ", m_LocalMoveInput.y);



            animator.SetBool(
                "IsGroundedMoving",
                controller.currentMovement.environment == PersonController.Environment.Grounded
                &&  controller.currentMovement.groundedMovementMode == PersonController.GroundedMovementMode.Ordinary
            );

        }



        protected override void Awake()
        {

            base.Awake();

            m_Animator = GetComponent<Animator>();

        }

        protected override void Update()
        {

            base.Update();

            UpdateGroundedMovementProperties();

        }

    }

}