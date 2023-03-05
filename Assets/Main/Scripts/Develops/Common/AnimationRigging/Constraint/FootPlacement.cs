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

        private void FindFootPlacement(Vector3 preIKFootPosition, Vector3 preIKLegUpPosition, float maxLegLength, float offsetHeight, float checkingDistance, LayerMask groundMask)
        {

            RaycastHit hit = new RaycastHit();

            if (Physics.Raycast(preIKFootPosition + Vector3.up * offsetHeight, Vector3.down, out hit, checkingDistance, groundMask))
            {



            }
            else
            {

                hit.point = preIKFootPosition;
                hit.normal = Vector3.up;

            }

            if (Vector3.Dot(Vector3.up, hit.normal) < m_Data.minGroundSlope)
            {

                hit.normal = Vector3.up;

            }



            Vector3 nextPosition = hit.point + hit.normal * m_Data.placementOffsetHeight;
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
            if (Physics.Raycast(nextPosition - nextNormal * m_Data.placementOffsetHeight + (preIKLegUpPosition - nextPosition).normalized * offsetHeight, (nextPosition - preIKLegUpPosition).normalized, out hit, checkingDistance, groundMask))
            {



            }
            else
            {

                hit.point = nextPosition - nextNormal * m_Data.placementOffsetHeight;
                hit.normal = nextNormal;

            }

            nextPosition = hit.point + hit.normal * m_Data.placementOffsetHeight;
            nextNormal = hit.normal;

            if (Vector3.Dot(Vector3.up, nextNormal) < m_Data.minGroundSlope)
            {

                nextNormal = Vector3.up;

            }



            m_Data.nextPosition = nextPosition;
            m_Data.nextNormal = nextNormal;

        }



        private void Awake()
        {

            m_Data.checkedNormal = Vector3.up;
            m_Data.nextNormal = Vector3.up;

            if (m_Data.footBone != null)
            {

                m_Data.preIKFootPosition = m_Data.footBone.position;
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
                FindFootPlacement(m_Data.preIKFootPosition - Vector3.up * m_Data.placementOffsetHeight, m_Data.preIKLegUpPosition, m_Data.maxLegLength, m_Data.offsetHeight, m_Data.checkingDistance, m_Data.groundMask);

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
        private void OnDrawGizmos()
        {
            if (m_Data.footBone == null) return;
            if (m_Data.legUpBone == null) return;

            if (!Application.isPlaying)
            {

                m_Data.preIKFootPosition = m_Data.footBone.position;
                m_Data.preIKLegUpPosition = m_Data.legUpBone.position;
                FindFootPlacement(m_Data.preIKFootPosition - Vector3.up * m_Data.placementOffsetHeight, m_Data.preIKLegUpPosition, m_Data.maxLegLength, m_Data.offsetHeight, m_Data.checkingDistance, m_Data.groundMask);

            }

            Debug.DrawLine(
                m_Data.preIKFootPosition - Vector3.up * m_Data.placementOffsetHeight + Vector3.up * m_Data.offsetHeight,
                m_Data.preIKFootPosition - Vector3.up * m_Data.placementOffsetHeight,
                m_Data.checkingRayColorTop
            );
            Debug.DrawLine(
                m_Data.preIKFootPosition - Vector3.up * m_Data.placementOffsetHeight,
                m_Data.checkedPosition,
                m_Data.checkingRayColorMiddle
            );
            Debug.DrawLine(
                m_Data.checkedPosition,
                m_Data.preIKFootPosition - Vector3.up * m_Data.placementOffsetHeight + Vector3.up * m_Data.offsetHeight + Vector3.down * m_Data.checkingDistance,
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
        public float maxLegLengthCorrectionDistance;

        public float offsetHeight;
        public float checkingDistance;
        public float placementOffsetHeight;
        public LayerMask groundMask;

        [Range(0.0f, 1.0f)]
        public float minGroundSlope;

        public Quaternion rotationOffset;



        [SyncSceneToStream]
        public float updatingSpeed;



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

            offsetHeight = 0.1f;
            checkingDistance = 0.1f;
            placementOffsetHeight = 0.0f;

            minGroundSlope = 0.4f;



            updatingSpeed = 1.0f;



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
        public Vector3Property preIKLegUpPositionProperty;

        public Vector3Property nextPositionProperty;
        public Vector3Property nextNormalProperty;

        public Vector4Property rotationV4OffsetProperty;



        public FloatProperty updatingSpeedProperty;



        public void ProcessRootMotion(AnimationStream stream)
        { }

        public void ProcessAnimation(AnimationStream stream)
        {

            AnimationRuntimeUtils.PassThrough(stream, this.footBone);
            AnimationRuntimeUtils.PassThrough(stream, this.legUpBone);
            AnimationRuntimeUtils.PassThrough(stream, this.ikFoot);



            Vector3 preIKFootPosition = footBone.GetPosition(stream);
            preIKFootPositionProperty.Set(stream, preIKFootPosition);
            Vector3 preIKLegUpPosition = legUpBone.GetPosition(stream);
            preIKLegUpPositionProperty.Set(stream, preIKLegUpPosition);

            Quaternion preIKFootRotation = footBone.GetRotation(stream);



            Vector4 rotationV4Offset = rotationV4OffsetProperty.Get(stream);
            Quaternion rotationOffset = new Quaternion(rotationV4Offset.x, rotationV4Offset.y, rotationV4Offset.z, rotationV4Offset.w);
            Quaternion rotationOffsetInv = Quaternion.Inverse(rotationOffset);

            Quaternion preIKGroundRotation = preIKFootRotation * rotationOffset;

            Vector3 preIKFootGroundTangent = preIKGroundRotation * Vector3.forward;
            Vector3 preIKFootGroundBiTangent = preIKGroundRotation * Vector3.right;
            Vector3 preIKGroundNormal = preIKGroundRotation * Vector3.up;


            Vector3 nextPosition = nextPositionProperty.Get(stream);
            Vector3 nextNormal = nextNormalProperty.Get(stream);

            Vector3 nextTangent = Vector3.Cross(preIKFootGroundBiTangent, nextNormal);
            Vector3 nextBiTangent = Vector3.Cross(nextNormal, preIKFootGroundTangent);

            Quaternion nextRotation = (
                Quaternion.FromToRotation(preIKFootGroundTangent, nextTangent) *
                Quaternion.FromToRotation(preIKFootGroundBiTangent, nextBiTangent)
            ) * preIKFootRotation;



            Vector3 targetIKFootPosition = Vector3.Lerp(preIKFootPosition, nextPosition, jobWeight.Get(stream));
            Quaternion targetIKFootRotation = Quaternion.Lerp(preIKFootRotation, nextRotation, jobWeight.Get(stream));

            Vector3 currIKFootPosition = ikFoot.GetPosition(stream);
            Quaternion currIKFootRotation = ikFoot.GetRotation(stream);

            float updatingSpeed = updatingSpeedProperty.Get(stream);

            ikFoot.SetPosition(stream, Vector3.Lerp(currIKFootPosition, targetIKFootPosition, stream.deltaTime * updatingSpeed));
            ikFoot.SetRotation(stream, Quaternion.Lerp(currIKFootRotation, targetIKFootRotation, stream.deltaTime * updatingSpeed));

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
                preIKLegUpPositionProperty = Vector3Property.Bind(animator, component, "m_Data." + nameof(data.preIKLegUpPosition)),

                nextPositionProperty = Vector3Property.Bind(animator, component, "m_Data." + nameof(data.nextPosition)),
                nextNormalProperty = Vector3Property.Bind(animator, component, "m_Data." + nameof(data.nextNormal)),

                rotationV4OffsetProperty = Vector4Property.Bind(animator, component, "m_Data." + nameof(data.rotationV4Offset)),



                updatingSpeedProperty = FloatProperty.Bind(animator, component, "m_Data." + nameof(data.updatingSpeed)),

            };
        }

        public override void Destroy(FootPlacementJob job)
        {



        }
    }

}
