using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Animations.Rigging;
using UnityEngine.Assertions;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

namespace PROJECT_A11.Develops.Common
{

    [AddComponentMenu("PROJECT_A11/Common/AnimationRigging/FootPlacement")]
    public class FootPlacement :
        RigConstraint<
            FootPlacementJob,
            FootPlacementData,
            FootPlacementJobBinder
        >
    {

        private void FindFootPlacement(Vector3 preIKFootPosition, Vector3 preIKLegUpPosition, float maxLegLength, float checkingHeightOffset, float checkingDistance, Vector3 checkingDirection, LayerMask groundMask)
        {

            RaycastHit hit = new RaycastHit();

            if (Physics.Raycast(preIKFootPosition + (-m_Data.checkingDirection) * checkingHeightOffset, m_Data.checkingDirection, out hit, checkingDistance, groundMask))
            {



            }
            else
            {

                hit.point = preIKFootPosition;
                hit.normal = (-m_Data.checkingDirection);

            }

            if (Vector3.Dot((-m_Data.checkingDirection), hit.normal) < m_Data.minGroundSlope)
            {

                hit.normal = (-m_Data.checkingDirection);

            }

            hit.point = Vector3.Lerp(preIKFootPosition, hit.point, m_Data.groundStrength);



            Vector3 nextPosition = hit.point + hit.normal * m_Data.footHeight;
            Vector3 nextNormal = hit.normal;

            m_Data.checkedPosition = nextPosition;
            m_Data.checkedNormal = nextNormal;



            /// Finds new foot position that guarantee max leg length
            Vector3 C = preIKLegUpPosition - nextPosition;
            float legLength = C.magnitude;
            if (legLength > maxLegLength)
            {

                Vector3 T = Vector3.Cross(nextNormal, C).normalized;
                Vector3 B = Vector3.Cross(T, nextNormal).normalized;

                float BDotC = Vector3.Dot(B, C);

                Vector3 H = B * BDotC - C;



                float h = H.magnitude;
                float dbSquare = Mathf.Clamp(maxLegLength * maxLegLength - h * h, 0.0f, Mathf.Infinity);
                float db = Mathf.Sqrt(dbSquare);

                nextPosition = nextPosition + B * Mathf.Clamp(BDotC - db, 0.0f, m_Data.maxLegLengthCorrectionDistance);

            }



            /// second layer ground checking
            if (Physics.Raycast(nextPosition - nextNormal * m_Data.footHeight + (preIKLegUpPosition - nextPosition).normalized * checkingHeightOffset, (nextPosition - preIKLegUpPosition).normalized, out hit, checkingDistance, groundMask))
            {



            }
            else
            {

                hit.point = nextPosition - nextNormal * m_Data.footHeight;
                hit.normal = nextNormal;

            }

            nextPosition = hit.point + hit.normal * m_Data.footHeight;
            nextNormal = hit.normal;

            if (Vector3.Dot((-m_Data.checkingDirection), nextNormal) < m_Data.minGroundSlope)
            {

                nextNormal = (-m_Data.checkingDirection);

            }



            m_Data.nextPosition = Vector3.Lerp(m_Data.checkedPosition, nextPosition, Mathf.Clamp01(m_Data.groundStrength));
            m_Data.nextNormal = Vector3.Lerp(m_Data.checkedNormal, nextNormal, Mathf.Clamp01(m_Data.groundStrength));

        }



        private void Awake()
        {

            m_Data.checkedNormal = (-m_Data.checkingDirection);
            m_Data.nextNormal = (-m_Data.checkingDirection);

            if (m_Data.footBone != null)
            {

                m_Data.preIKFootPosition = m_Data.footBone.position;
                m_Data.preIKFootRotationV4 = new Vector4(m_Data.footBone.rotation.x, m_Data.footBone.rotation.y, m_Data.footBone.rotation.z, m_Data.footBone.rotation.w);
                m_Data.checkedPosition = m_Data.preIKFootPosition;
                m_Data.nextPosition = m_Data.preIKFootPosition;

            }

            if (m_Data.legUpBone != null)
            {

                m_Data.preIKLegUpPosition = m_Data.legUpBone.position;

            }

        }

        private void FixedUpdate()
        {

            if (m_Data.footBone != null)
                FindFootPlacement(m_Data.preIKFootPosition - (-m_Data.checkingDirection) * m_Data.footHeight, m_Data.preIKLegUpPosition, m_Data.maxLegLength, m_Data.checkingHeightOffset, m_Data.checkingDistance, m_Data.checkingDirection, m_Data.groundMask);

        }

        private void Update()
        {

            m_Data.rotationV4Offset = new Vector4(m_Data.rotationOffset.x, m_Data.rotationOffset.y, m_Data.rotationOffset.z, m_Data.rotationOffset.w);

        }

        protected override void OnValidate()
        {
            base.OnValidate();

            if (
                m_Data.rotationOffset.x == 0.0f
                && m_Data.rotationOffset.y == 0.0f
                && m_Data.rotationOffset.z == 0.0f
            )
            {

                m_Data.rotationOffset.w = 1.0f;

            }

            m_Data.rotationV4Offset = new Vector4(m_Data.rotationOffset.x, m_Data.rotationOffset.y, m_Data.rotationOffset.z, m_Data.rotationOffset.w);

        }



#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            if (m_Data.footBone == null) return;
            if (m_Data.legUpBone == null) return;

            if (!Application.isPlaying)
            {

                m_Data.preIKFootPosition = m_Data.footBone.position;
                m_Data.preIKFootRotationV4 = new Vector4(m_Data.footBone.rotation.x, m_Data.footBone.rotation.y, m_Data.footBone.rotation.z, m_Data.footBone.rotation.w);
                m_Data.preIKLegUpPosition = m_Data.legUpBone.position;
                FindFootPlacement(m_Data.preIKFootPosition - (-m_Data.checkingDirection) * m_Data.footHeight, m_Data.preIKLegUpPosition, m_Data.maxLegLength, m_Data.checkingHeightOffset, m_Data.checkingDistance, m_Data.checkingDirection, m_Data.groundMask);

            }

            Debug.DrawLine(
                m_Data.preIKFootPosition - (-m_Data.checkingDirection) * m_Data.footHeight + (-m_Data.checkingDirection) * m_Data.checkingHeightOffset,
                m_Data.preIKFootPosition - (-m_Data.checkingDirection) * m_Data.footHeight,
                m_Data.checkingRayColorTop
            );
            Debug.DrawLine(
                m_Data.preIKFootPosition - (-m_Data.checkingDirection) * m_Data.footHeight,
                m_Data.checkedPosition,
                m_Data.checkingRayColorMiddle
            );
            Debug.DrawLine(
                m_Data.checkedPosition,
                m_Data.preIKFootPosition - (-m_Data.checkingDirection) * m_Data.footHeight + (-m_Data.checkingDirection) * m_Data.checkingHeightOffset + (m_Data.checkingDirection) * m_Data.checkingDistance,
                m_Data.checkingRayColorBottom
            );

            Handles.color = m_Data.checkedSphereColor;
            Handles.DrawWireDisc(m_Data.checkedPosition, m_Data.checkedNormal, m_Data.checkedSphereRadius, m_Data.checkedSphereThickness);

            Handles.color = m_Data.nextSphereColor;
            Handles.DrawWireDisc(m_Data.nextPosition, m_Data.nextNormal, m_Data.nextSphereRadius, m_Data.nextSphereThickness);

        }
#endif

    }



    [Serializable]
    public struct FootPlacementData : IAnimationJobData
    {

        [SyncSceneToStream]
        public Transform footBone;
        [SyncSceneToStream]
        public Transform legUpBone;
        [SyncSceneToStream]
        public Transform ikFoot;



        public float maxLegLength;

        public float checkingHeightOffset;
        public float checkingDistance;
        public LayerMask groundMask;

        [Range(0.0f, 1.0f)]
        public float minGroundSlope;



#if UNITY_EDITOR
        [Space(10)]
        [Header("Debug Settings")]
        public Color checkingRayColorTop;
        public Color checkingRayColorMiddle;
        public Color checkingRayColorBottom;

        public Color checkedSphereColor;
        public float checkedSphereRadius;
        public float checkedSphereThickness;

        public Color nextSphereColor;
        public float nextSphereRadius;
        public float nextSphereThickness;
#endif



        [Space(10)]
        [Header("Read-only Details")]
        [ReadOnly]
        [SyncSceneToStream]
        public Vector3 preIKFootPosition;
        [ReadOnly]
        [SyncSceneToStream]
        public Vector4 preIKFootRotationV4;
        [ReadOnly]
        [SyncSceneToStream]
        public Vector3 preIKLegUpPosition;

        [ReadOnly]
        public Vector3 checkedPosition;
        [ReadOnly]
        public Vector3 checkedNormal;

        [ReadOnly]
        [SyncSceneToStream]
        public Vector3 nextPosition;
        [ReadOnly]
        [SyncSceneToStream]
        public Vector3 nextNormal;

        [ReadOnly]
        [SyncSceneToStream]
        public Vector4 rotationV4Offset;

        [ReadOnly]
        [Range(0.0f, 1.0f)]
        public float groundStrength;

        [ReadOnly]
        public float maxLegLengthCorrectionDistance;
        [ReadOnly]
        [SyncSceneToStream]
        public float preIKUpdatingSpeed;
        [ReadOnly]
        [SyncSceneToStream]
        public float ikUpdatingSpeed;
        [ReadOnly]
        [SyncSceneToStream]
        public Vector3 checkingDirection;
        [ReadOnly]
        [SyncSceneToStream]
        public float footHeight;
        [ReadOnly]
        [SyncSceneToStream]
        public Quaternion rotationOffset;



        public bool IsValid()
        {
            return footBone != null;
        }

        public void SetDefaultValues()
        {

            footBone = null;
            ikFoot = null;



            maxLegLength = 0.8f;
            maxLegLengthCorrectionDistance = 0.1f;

            checkingHeightOffset = 0.1f;
            checkingDistance = 0.1f;
            checkingDirection = Vector3.down;
            footHeight = 0.0f;

            groundStrength = 1.0f;

            minGroundSlope = 0.4f;



            preIKUpdatingSpeed = 1.0f;
            ikUpdatingSpeed = 1.0f;



#if UNITY_EDITOR
            checkingRayColorTop = Color.blue;
            checkingRayColorMiddle = Color.yellow;
            checkingRayColorBottom = Color.red;

            checkedSphereColor = Color.grey;
            checkedSphereRadius = 0.03f;
            checkedSphereThickness = 2.0f;

            nextSphereColor = Color.magenta;
            nextSphereRadius = 0.045f;
            nextSphereThickness = 2.0f;
#endif

        }

    }


    public struct FootPlacementJob : IWeightedAnimationJob
    {
        public FloatProperty jobWeight { get; set; }



        public ReadWriteTransformHandle footBone;
        public ReadWriteTransformHandle legUpBone;
        public ReadWriteTransformHandle ikFoot;



        public Vector3Property preIKFootPositionProperty;
        public Vector4Property preIKFootRotationV4Property;
        public Vector3Property preIKLegUpPositionProperty;

        public Vector3Property nextPositionProperty;
        public Vector3Property nextNormalProperty;

        public Vector4Property rotationV4OffsetProperty;



        public FloatProperty preIKUpdatingSpeedProperty;
        public FloatProperty ikUpdatingSpeedProperty;



        public void ProcessRootMotion(AnimationStream stream)
        { }

        public void ProcessAnimation(AnimationStream stream)
        {

            AnimationRuntimeUtils.PassThrough(stream, this.footBone);
            AnimationRuntimeUtils.PassThrough(stream, this.legUpBone);
            AnimationRuntimeUtils.PassThrough(stream, this.ikFoot);



            float preIKUpdatingSpeed = preIKUpdatingSpeedProperty.Get(stream);
            float ikUpdatingSpeed = ikUpdatingSpeedProperty.Get(stream);



            Vector3 preIKFootPosition = footBone.GetPosition(stream);
            preIKFootPosition = Vector3.Lerp(preIKFootPositionProperty.Get(stream), preIKFootPosition, Mathf.Clamp01(stream.deltaTime * preIKUpdatingSpeed));
            preIKFootPositionProperty.Set(stream, preIKFootPosition);
            Vector3 preIKLegUpPosition = legUpBone.GetPosition(stream);
            preIKLegUpPositionProperty.Set(stream, preIKLegUpPosition);

            Vector4 lastPreIKFootRotationV4 = preIKFootRotationV4Property.Get(stream);
            Quaternion preIKFootRotation = footBone.GetRotation(stream);
            preIKFootRotation = Quaternion.Lerp(new Quaternion(lastPreIKFootRotationV4.x, lastPreIKFootRotationV4.y, lastPreIKFootRotationV4.z, lastPreIKFootRotationV4.w), preIKFootRotation, Mathf.Clamp01(stream.deltaTime * preIKUpdatingSpeed));
            preIKFootRotationV4Property.Set(stream, new Vector4(preIKFootRotation.x, preIKFootRotation.y, preIKFootRotation.z, preIKFootRotation.w));



            Vector4 rotationV4Offset = rotationV4OffsetProperty.Get(stream);
            Quaternion rotationOffset = new Quaternion(rotationV4Offset.x, rotationV4Offset.y, rotationV4Offset.z, rotationV4Offset.w);
            Quaternion rotationOffsetInv = Quaternion.Inverse(rotationOffset);

            Quaternion preIKGroundRotation = preIKFootRotation * rotationOffsetInv;

            Vector3 preIKGroundNormal = preIKGroundRotation * Vector3.up;


            Vector3 nextPosition = nextPositionProperty.Get(stream);
            Vector3 nextNormal = nextNormalProperty.Get(stream);

            Quaternion nextRotation = Quaternion.FromToRotation(preIKGroundNormal, nextNormal) * preIKFootRotation;



            Vector3 targetIKFootPosition = Vector3.Lerp(preIKFootPosition, nextPosition, jobWeight.Get(stream));
            Quaternion targetIKFootRotation = Quaternion.Lerp(preIKFootRotation, nextRotation, jobWeight.Get(stream));

            Vector3 currIKFootPosition = ikFoot.GetPosition(stream);
            Quaternion currIKFootRotation = ikFoot.GetRotation(stream);

            ikFoot.SetPosition(stream, Vector3.Lerp(currIKFootPosition, targetIKFootPosition, Mathf.Clamp01(stream.deltaTime * ikUpdatingSpeed)));
            ikFoot.SetRotation(stream, Quaternion.Lerp(currIKFootRotation, targetIKFootRotation, Mathf.Clamp01(stream.deltaTime * ikUpdatingSpeed)));

        }

    }



    public class FootPlacementJobBinder :
        AnimationJobBinder<
            FootPlacementJob,
            FootPlacementData
        >
    {
        public override FootPlacementJob Create(
            Animator animator,
            ref FootPlacementData data,
            Component component
        )
        {

            return new FootPlacementJob
            {

                footBone = ReadWriteTransformHandle.Bind(animator, data.footBone),
                legUpBone = ReadWriteTransformHandle.Bind(animator, data.legUpBone),
                ikFoot = ReadWriteTransformHandle.Bind(animator, data.ikFoot),



                preIKFootPositionProperty = Vector3Property.Bind(animator, component, "m_Data." + nameof(data.preIKFootPosition)),
                preIKFootRotationV4Property = Vector4Property.Bind(animator, component, "m_Data." + nameof(data.preIKFootRotationV4)),
                preIKLegUpPositionProperty = Vector3Property.Bind(animator, component, "m_Data." + nameof(data.preIKLegUpPosition)),

                nextPositionProperty = Vector3Property.Bind(animator, component, "m_Data." + nameof(data.nextPosition)),
                nextNormalProperty = Vector3Property.Bind(animator, component, "m_Data." + nameof(data.nextNormal)),

                rotationV4OffsetProperty = Vector4Property.Bind(animator, component, "m_Data." + nameof(data.rotationV4Offset)),



                preIKUpdatingSpeedProperty = FloatProperty.Bind(animator, component, "m_Data." + nameof(data.preIKUpdatingSpeed)),
                ikUpdatingSpeedProperty = FloatProperty.Bind(animator, component, "m_Data." + nameof(data.ikUpdatingSpeed)),

            };

        }

        public override void Destroy(FootPlacementJob job)
        {



        }
    }

}
