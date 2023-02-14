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
    public class Person :
        Develops.Common.Pawn<
            Person,
            PersonController,
            PersonFSM,
            PersonBrain
        >
    {

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



        protected override void Awake()
        {

            base.Awake();

            m_Rigidbody = GetComponent<Rigidbody>();
            m_CapsuleCollider = GetComponent<CapsuleCollider>();

        }
        protected virtual void FixedUpdate()
        {



        }

    }

}