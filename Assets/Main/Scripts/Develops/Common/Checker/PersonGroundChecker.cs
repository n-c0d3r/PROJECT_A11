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

        public float radius = 0.5f;
        public Vector3 offset = Vector3.up * 0.5f;
        public Vector3 direction = Vector3.down;
        public LayerMask mask;

#if UNITY_EDITOR
        [Space(10)]
        [Header("Debug Settings")]
        public Color checkingSphereColor = Color.green;
        public float checkingSphereThickness = 2.0f;
        public Color checkedSphereColor = Color.magenta;
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

            if (Physics.SphereCast(transform.position + offset - direction.normalized * 0.001f, radius, direction, out hit, 0.05f, mask))
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

            Handles.color = checkingSphereColor;
            Handles.DrawWireDisc(transform.position + offset, m_CheckedNormal, radius, checkingSphereThickness);
            Handles.color = checkingSphereColor * 0.75f;
            Handles.DrawWireDisc(transform.position + offset, transform.right, radius, checkingSphereThickness * 0.5f);
            Handles.DrawWireDisc(transform.position + offset, transform.forward, radius, checkingSphereThickness * 0.5f);

            if (!m_IsGrounded) return;

            Handles.color = checkedSphereColor;
            Handles.DrawWireDisc(m_CheckedPosition, m_CheckedNormal, checkedSphereRadius, checkedSphereThickness);
            Handles.color = checkedSphereColor * 0.5f;
            Handles.DrawWireDisc(m_CheckedPosition, transform.right, checkedSphereRadius, checkedSphereThickness * 0.75f);
            Handles.DrawWireDisc(m_CheckedPosition, transform.forward, checkedSphereRadius, checkedSphereThickness * 0.75f);

        }
#endif

    }

}