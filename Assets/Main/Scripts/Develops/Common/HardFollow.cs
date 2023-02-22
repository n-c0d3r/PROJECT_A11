using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.Assertions;



namespace PROJECT_A11.Develops.Common
{

    public class HardFollow :
        MonoBehaviour
    {

        public Transform targetTransform;
        public Vector3 offset;

        public Vector3 followMask = new Vector3(1.0f, 1.0f, 1.0f);



        private Vector3 V3Bool2V3F(Vector3Bool v3b)
        {

            return new Vector3(v3b.x ? 1.0f : 0.0f, v3b.y ? 1.0f : 0.0f, v3b.z ? 1.0f : 0.0f);
        }
        private Vector3 V3Mask(Vector3 v1, Vector3 v2, Vector3 mask)
        {

            return new Vector3(
                Mathf.Lerp(v1.x, v2.x, mask.x),
                Mathf.Lerp(v1.y, v2.y, mask.y),
                Mathf.Lerp(v1.z, v2.z, mask.z)
            );
        }



        private void Update()
        {

            Matrix4x4 w2lMatrix = transform.parent.worldToLocalMatrix;
            Matrix4x4 l2wMatrix = transform.localToWorldMatrix;

            Vector3 lTPos = w2lMatrix * (new Vector4(targetTransform.position.x, targetTransform.position.y, targetTransform.position.z, 1.0f) + l2wMatrix * offset);

            transform.localPosition = V3Mask(transform.localPosition, lTPos, followMask);

        }

    }

}