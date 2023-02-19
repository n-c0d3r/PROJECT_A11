using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;



namespace PROJECT_A11.Develops.Common
{

    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(CapsuleCollider))]
    [RequireComponent(typeof(PersonController))]
    [RequireComponent(typeof(PersonFSM))]
    [RequireComponent(typeof(PersonAnimation))]
    public class Person :
        Develops.Common.Pawn<
            Person,
            PersonController,
            PersonFSM,
            PersonBrain,
            PersonAnimation
        >
    {

        [System.Serializable]
        public struct ShapeSettings
        {

            public float radius;
            public float height;

        }

        [System.Serializable]
        public struct HeadSettings
        {

            public Transform headTransform;
            public Vector3 headOffset;

        }



        public ShapeSettings shapeSettings = new ShapeSettings
        {

            radius = 0.5f,
            height = 2.0f

        };
        public HeadSettings headSettings = new HeadSettings
        {

            headOffset = new Vector3(0.0f, 1.5f, 0.0f)

        };



        private Rigidbody m_Rigidbody;
        public Rigidbody rigidbody
        {
            get
            {

                if (m_Rigidbody == null)
                    m_Rigidbody = GetComponent<Rigidbody>();

                return m_Rigidbody;
            }
        }

        private CapsuleCollider m_CapsuleCollider;
        public CapsuleCollider capsuleCollider
        {
            get
            {

                if (m_CapsuleCollider == null)
                    m_CapsuleCollider = GetComponent<CapsuleCollider>();

                return m_CapsuleCollider;
            }
        }



        public void UpdateShape()
        {

            capsuleCollider.height = shapeSettings.height;
            capsuleCollider.radius = shapeSettings.radius;
            capsuleCollider.center = Vector3.up * shapeSettings.height * 0.5f;

        }
        public void UpdateHead()
        {

            headSettings.headTransform.localPosition = headSettings.headOffset;

        }



        protected override void Awake()
        {

            base.Awake();

            m_Rigidbody = GetComponent<Rigidbody>();
            m_CapsuleCollider = GetComponent<CapsuleCollider>();

        }
        protected virtual void Update()
        {

            UpdateHead();

        }
        protected virtual void FixedUpdate()
        {

            UpdateShape();

        }

    }

}