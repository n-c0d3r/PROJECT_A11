#if UNITY_EDITOR

using PROJECT_A11.Develops.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.InputSystem;

namespace PROJECT_A11.Researches.SpeedPrediction
{

    public class SpeedPrediction :
        MonoBehaviour
    {

        public Transform boneFootL;
        public Transform boneFootR;
        public float velUpdatingSpeed = 5.0f;

        public Vector3 moveDir = Vector3.zero;



        private Animator m_Animator;

        bool isFirstPrediction = true;
        Vector3 oldFootL = Vector3.zero;
        Vector3 oldFootR = Vector3.zero;

        [ReadOnly]
        [SerializeField]
        float vL = 0;
        [ReadOnly]
        [SerializeField]
        float vR = 0;



        private void Awake()
        {
            
            m_Animator = GetComponent<Animator>();

        }



        private void Update()
        {

            Matrix4x4 globalToLocal = transform.worldToLocalMatrix;

            Vector3 footL = globalToLocal * boneFootL.position;
            Vector3 footR = globalToLocal * boneFootR.position;

            if (isFirstPrediction)
            {

                isFirstPrediction = false;

            }
            else
            {

                Vector3 dL = -(footL - oldFootL);
                Vector3 dR = -(footR - oldFootR);
                float sL = dL.magnitude;
                float sR = dR.magnitude;

                float s = 0.0f;

                if (Vector3.Dot(dL, transform.forward) > 0.7f)
                {

                    s += sL;

                    transform.position += dL;

                }
                if (Vector3.Dot(dR, transform.forward) > 0.7f)
                {

                    s += sR;

                    transform.position += dR;

                }

            }

            oldFootL = globalToLocal * boneFootL.position;
            oldFootR = globalToLocal * boneFootR.position;

        }

    }

}

#endif