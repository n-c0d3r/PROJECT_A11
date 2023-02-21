#if UNITY_EDITOR

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;



namespace PROJECT_A11.Develops.Common
{
    public class LockRotation :
        MonoBehaviour
    {

        public bool lockRotationX = false;
        public bool lockRotationY = false;
        public bool lockRotationZ = true;



        private void LateUpdate()
        {

            Vector3 euler = transform.rotation.eulerAngles;

            euler.x *= lockRotationX ? 0.0f : 1.0f;
            euler.y *= lockRotationY ? 0.0f : 1.0f;
            euler.z *= lockRotationZ ? 0.0f : 1.0f;

            transform.rotation = Quaternion.Euler(euler);

        }

    }

}

#endif