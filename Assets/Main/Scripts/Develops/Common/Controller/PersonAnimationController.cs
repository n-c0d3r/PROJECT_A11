using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Animations.Rigging;
using UnityEngine.Assertions;
using UnityEngine.Playables;
using UnityEngine.UIElements;

namespace PROJECT_A11.Develops.Common
{

    [AddComponentMenu("PROJECT_A11/Common/Controller/PersonAnimationController")]
    public class PersonAnimationController :
        PawnAnimationController<Person, PersonController>
    {

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        #region Fields and Private Properties
        public float moveInputUpdatingSpeed = 5.0f;
        public float animationSpeedUpdatingSpeed = 8.0f;
        public float maxAnimatorSpeed = 1.5f;
        public float groundHeightUpdatingSpeed = 8.0f;
        public float groundHeightOfHighestFalling = 2.0f;
        public float groundHeightOfHighestFootPlacement = 0.1f;

        [Space(10)]
        [Header("Bone Settings")]
        public Transform boneHip;
        public Transform boneFootL;
        public Transform boneFootR;

        [Space(10)]
        [Header("Physical IK Settings")]
        public Transform physicalIKRoot;
        public Transform physicalIKFootL;
        public Transform physicalIKFootR;

        [Space(10)]
        [Header("Rig Settings")]
        public Transform handRRig;
        public Transform handLRig;
        public Transform footRRig;
        public Transform footLRig;

        public float footLPlacementMinPreIKUpdatingSpeed = 0.0f;
        public float footRPlacementMinPreIKUpdatingSpeed = 0.0f;
        public float footLPlacementMaxPreIKUpdatingSpeed = 100.0f;
        public float footRPlacementMaxPreIKUpdatingSpeed = 100.0f;
        public float footLPlacementMinIKUpdatingSpeed = 15.0f;
        public float footRPlacementMinIKUpdatingSpeed = 15.0f;
        public float footLPlacementMaxIKUpdatingSpeed = 100.0f;
        public float footRPlacementMaxIKUpdatingSpeed = 100.0f;
        public float footLPlacementMaxLegLengthCorrectionDistance = 0.36f;
        public float footRPlacementMaxLegLengthCorrectionDistance = 0.36f;



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
        [SyncSceneToStream]
        private float m_GroundHeight = 0.0f;

        private TwoBoneIKConstraint m_HandRRig2BoneIKConstraint;
        private MultiRotationConstraint m_HandRRigRotationConstraint;
        private TwoBoneIKConstraint m_HandLRig2BoneIKConstraint;
        private MultiRotationConstraint m_HandLRigRotationConstraint;

        private FootPlacement m_FootPlacementL;
        private FootPlacement m_FootPlacementR;
        private TwoBoneIKConstraint m_FootLRig2BoneIKConstraint;
        private TwoBoneIKConstraint m_FootRRig2BoneIKConstraint;
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
        private void UpdateGroundedMovement(float deltaTime)
        {

            Vector3 moveInput = controller.input.targetMoveInput;

            m_MoveInput = Vector3.Lerp(m_MoveInput, moveInput, Mathf.Clamp01(deltaTime * moveInputUpdatingSpeed));



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

            m_AnimationSpeed = Mathf.Clamp(Mathf.Lerp(m_AnimationSpeed, targetSpeed, Mathf.Clamp01(deltaTime * animationSpeedUpdatingSpeed)), 0.0f, maxAnimatorSpeed);

            animator.speed = m_AnimationSpeed;

        }

        private void UpdateInAirMovement(float deltaTime)
        {

            m_AnimationSpeed = Mathf.Clamp(Mathf.Lerp(m_AnimationSpeed, 1.0f, Mathf.Clamp01(deltaTime * animationSpeedUpdatingSpeed)), 0.0f, maxAnimatorSpeed);

            animator.speed = m_AnimationSpeed;
        }

        private void UpdateBodyState(float deltaTime)
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

        private void UpdateEnvironment(float deltaTime)
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

        private void UpdateGroundHeight(float deltaTime)
        {

            m_GroundHeight = Mathf.Lerp(m_GroundHeight, controller.currentMovement.groundHeight, Mathf.Clamp01(deltaTime * groundHeightUpdatingSpeed));
            animator.SetFloat("GroundHeight", m_GroundHeight);
            animator.SetFloat("FallingStrength", m_GroundHeight / groundHeightOfHighestFalling);

        }

        private void UpdateHandRigs()
        {

            float handR_LockToItemView = animator.GetFloat("HandR_LockToItemView");
            float handL_LockToItemView = animator.GetFloat("HandL_LockToItemView");

            if (handRRig != null)
            {

                m_HandRRig2BoneIKConstraint.weight = handR_LockToItemView;
                m_HandRRigRotationConstraint.weight = handR_LockToItemView;

            }
            if (handLRig != null)
            {

                m_HandLRig2BoneIKConstraint.weight = handL_LockToItemView;
                m_HandLRigRotationConstraint.weight = handL_LockToItemView;

            }

        }
        private void UpdateFootRigs()
        {

            //float heightBasedWeight = ((controller.currentMovement.environment == PersonController.Environment.Grounded) ? 1.0f : 0.0f);
            float heightBasedWeight = 1.0f - Mathf.Clamp01(m_GroundHeight / groundHeightOfHighestFootPlacement);

            if (m_FootPlacementL)
            {

                float lockWeight = m_Animator.GetFloat("LockFootL");
                lockWeight *= heightBasedWeight;

                m_FootPlacementL.weight = lockWeight;
                m_FootLRig2BoneIKConstraint.weight = lockWeight;

                m_FootPlacementL.data.preIKUpdatingSpeed = Mathf.Lerp(footLPlacementMinPreIKUpdatingSpeed, footLPlacementMaxPreIKUpdatingSpeed, (1.0f - lockWeight));
                m_FootPlacementL.data.ikUpdatingSpeed = Mathf.Lerp(footLPlacementMinIKUpdatingSpeed, footLPlacementMaxIKUpdatingSpeed, heightBasedWeight);
                m_FootPlacementL.data.maxLegLengthCorrectionDistance = Mathf.Lerp(0.0f, footLPlacementMaxLegLengthCorrectionDistance, heightBasedWeight);
                m_FootPlacementL.data.groundStrength = Mathf.Lerp(0.0f, 1.0f, heightBasedWeight);

                m_FootPlacementL.data.checkingDirection = -pawn.controller.groundChecker.checkedNormal;

            }
            if (m_FootPlacementR)
            {

                float lockWeight = m_Animator.GetFloat("LockFootR");
                lockWeight *= heightBasedWeight;

                m_FootPlacementR.weight = lockWeight;
                m_FootRRig2BoneIKConstraint.weight = lockWeight;

                m_FootPlacementR.data.preIKUpdatingSpeed = Mathf.Lerp(footRPlacementMinPreIKUpdatingSpeed, footRPlacementMaxPreIKUpdatingSpeed, (1.0f - lockWeight));
                m_FootPlacementR.data.ikUpdatingSpeed = Mathf.Lerp(footRPlacementMinIKUpdatingSpeed, footRPlacementMaxIKUpdatingSpeed, heightBasedWeight);
                m_FootPlacementR.data.maxLegLengthCorrectionDistance = Mathf.Lerp(0.0f, footRPlacementMaxLegLengthCorrectionDistance, heightBasedWeight);
                m_FootPlacementR.data.groundStrength = Mathf.Lerp(0.0f, 1.0f, heightBasedWeight);

                m_FootPlacementR.data.checkingDirection = -pawn.controller.groundChecker.checkedNormal;

            }

        }

        public void EnableRigs()
        {

            RigBuilder rigBuilder = GetComponent<RigBuilder>();
            foreach (var layer in rigBuilder.layers)
            {

                layer.rig.weight = 1.0f;

            }

        }
        public void DisableRigs()
        {

            RigBuilder rigBuilder = GetComponent<RigBuilder>();
            foreach (var layer in rigBuilder.layers)
            {

                layer.rig.weight = 0.0f;

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

            m_Animator = GetComponent<Animator>();



            if (handRRig != null)
            {

                m_HandRRig2BoneIKConstraint = handRRig.GetComponent<TwoBoneIKConstraint>();

                m_HandRRigRotationConstraint = handRRig.GetComponent<MultiRotationConstraint>();

            }
            if (handLRig != null)
            {

                m_HandLRig2BoneIKConstraint = handLRig.GetComponent<TwoBoneIKConstraint>();

                m_HandLRigRotationConstraint = handLRig.GetComponent<MultiRotationConstraint>();

            }

            if (footLRig != null)
            {

                m_FootPlacementL = footLRig.GetComponent<FootPlacement>();
                m_FootLRig2BoneIKConstraint = footLRig.GetComponent<TwoBoneIKConstraint>();

            }
            if (footRRig != null)
            {

                m_FootPlacementR = footRRig.GetComponent<FootPlacement>();
                m_FootRRig2BoneIKConstraint = footRRig.GetComponent<TwoBoneIKConstraint>();

            }



            UpdateHandRigs();
            UpdateFootRigs();

        }

        protected override void Update()
        {

            base.Update();



            UpdateEnvironment(Time.deltaTime);




            switch (controller.currentMovement.environment)
            {

                case PersonController.Environment.Grounded:
                    UpdateGroundedMovement(Time.deltaTime);
                    break;

                case PersonController.Environment.InAir:
                    UpdateInAirMovement(Time.deltaTime);
                    break;

            }



            UpdateGroundHeight(Time.deltaTime);



            UpdateBodyState(Time.deltaTime);



            UpdateHandRigs();
            UpdateFootRigs();

        }
        #endregion

    }

}