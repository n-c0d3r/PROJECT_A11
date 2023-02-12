using System;
using System.Collections;
using System.Collections.Generic;
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
#endif



        [Space(10)]
        [Header("Read-only Details")]
        [ReadOnly]
        [SerializeField]
        private Vector3 m_CheckedPosition;

        [ReadOnly]
        [SerializeField]
        private bool m_IsGrounding = false;
        public bool isGrounding { get { return m_IsGrounding; } }



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
                m_IsGrounding = true;

            }
            else
            {

                m_CheckedPosition = transform.position + offset;
                m_IsGrounding = false;

            }

        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {

            Gizmos.color = beginningSphereColor;
            Gizmos.DrawWireSphere(transform.position + offset, radius);

            Gizmos.color = endSphereColor;
            Gizmos.DrawWireSphere(transform.position + offset + maxDistance * direction, radius);

            if (!m_IsGrounding) return;

            Gizmos.color = checkedSphereColor;
            Gizmos.DrawWireSphere(m_CheckedPosition, checkedSphereRadius);

        }
#endif

    }

}