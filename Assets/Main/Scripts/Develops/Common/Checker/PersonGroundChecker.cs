using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.InputSystem;



namespace PROJECT_A11.Develops.Common
{

    public class PersonGroundChecker :
        MonoBehaviour
    {

        public float maxDistance;
        public float radius;
        public Vector3 offset;
        public Vector3 direction;
        public LayerMask mask;

#if UNITY_EDITOR
        [Space(10)]
        [Header("Debug Settings")]
        public Color beginningSphereColor = Color.red;
        public Color endSphereColor = Color.green;
        public Color checkedSphereColor = Color.blue;
        public float checkedSphereRadius = 0.2f;
        public float checkedSphereThickness = 2.0f;
#endif



        [Space(10)]
        [Header("Read-only Details")]
        [ReadOnly]
        [SerializeField]
        private Vector3 m_CheckedPosition;
        public Vector3 checkedPosition { get { return m_CheckedPosition; } }
        [ReadOnly]
        [SerializeField]
        private Vector3 m_CheckedNormal;
        public Vector3 checkedNormal { get { return m_CheckedNormal; } }

        [ReadOnly]
        [SerializeField]
        private bool m_IsGrounded = false;
        public bool isGrounded { get { return m_IsGrounded; } }



        private PersonController m_Controller;
        public PersonController controller { get { return m_Controller; } }



        private void Awake()
        {
            
            m_Controller = GetComponent<PersonController>();

        }

        private void FixedUpdate()
        {

            Check();

        }

        public void Check()
        {

            RaycastHit hit;

            if (Physics.SphereCast(transform.position + offset, radius, direction, out hit, maxDistance, mask))
            {

                m_CheckedPosition = hit.point;
                m_CheckedNormal = hit.normal;
                m_IsGrounded = true;

            }
            else
            {

                m_CheckedNormal = transform.up;
                m_IsGrounded = false;

            }

        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {

            Gizmos.color = beginningSphereColor;
            Gizmos.DrawWireSphere(transform.position + offset, radius);

            Gizmos.color = endSphereColor;
            Gizmos.DrawWireSphere(transform.position + offset + maxDistance * direction, radius);

            if (!m_IsGrounded) return;

            Handles.color = checkedSphereColor;
            Handles.DrawWireDisc(m_CheckedPosition, m_CheckedNormal, checkedSphereRadius, checkedSphereThickness);
            Handles.color = checkedSphereColor * 0.5f;
            Handles.DrawWireDisc(m_CheckedPosition, transform.right, checkedSphereRadius, checkedSphereThickness * 0.5f);
            Handles.DrawWireDisc(m_CheckedPosition, transform.forward, checkedSphereRadius, checkedSphereThickness * 0.5f);

        }
#endif

    }

}