using UnityEngine;
using UnityEngine.Assertions;

namespace PROJECT_A11.Develops.Common
{

    /// <summary>
    /// a person is a human-like character that has basic abilities of human such as walking, crouching, jumping,...
    /// </summary>
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(CapsuleCollider))]
    [RequireComponent(typeof(PersonController))]
    [RequireComponent(typeof(PersonFSM))]
    [RequireComponent(typeof(PersonAnimationController))]
    public class Person :
        Develops.Common.Pawn<
            Person,
            PersonController,
            PersonFSM,
            PersonBrain,
            PersonAnimationController
        >
    {

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        #region Nested Types
        [System.Serializable]
        public struct ShapeSettings
        {

            public float radius;

            public Transform centerTransform;
            public Transform[] topTransforms;
            public Transform[] bottomTransforms;

        }

        [System.Serializable]
        public struct HeadSettings
        {

            public Transform headTransform;
            public Vector3 headOffset;

        }

        [System.Serializable]
        public struct AimSettings
        {

            public Transform aimTargetTransform;

        }
        #endregion

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        #region Fields
        public ShapeSettings shapeSettings = new ShapeSettings
        {

            radius = 0.35f

        };
        public HeadSettings headSettings = new HeadSettings
        {

            headOffset = new Vector3(0.0f, 1.68f, 0.0f)

        };
        public AimSettings aimSettings = new AimSettings
        {



        };
        #endregion

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        
        #region Required Components
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
        #endregion

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        #region Utility Getters
        public float bottomDistance
        {

            get
            {

#if UNITY_EDITOR
                if (shapeSettings.bottomTransforms == null) return 0.0f;
                if (shapeSettings.bottomTransforms.Length == 0) return 0.0f;
                if (shapeSettings.centerTransform == null) return 0.0f;
#endif

                float bottom = 0.0f;

                foreach (Transform bottomTransform in shapeSettings.bottomTransforms)
                {

                    bottom = Mathf.Max(bottom, Vector3.Dot((bottomTransform.position - shapeSettings.centerTransform.position), -controller.groundUp));

                }

                return bottom;

            }

        }
        public float topDistance
        {

            get
            {

#if UNITY_EDITOR
                if (shapeSettings.topTransforms == null) return 0.0f;
                if (shapeSettings.topTransforms.Length == 0) return 0.0f;
                if (shapeSettings.centerTransform == null) return 0.0f;
#endif

                float top = 0.0f;

                foreach (Transform topTransform in shapeSettings.topTransforms)
                {

                    top = Mathf.Max(top, Vector3.Dot((topTransform.position - shapeSettings.centerTransform.position), controller.groundUp));

                }

                return top;

            }

        }

        public Vector3 bottomPoint
        {

            get
            {

#if UNITY_EDITOR
                if (shapeSettings.centerTransform == null) return transform.position;
#endif

                return shapeSettings.centerTransform.position - controller.groundUp * bottomDistance;
            }

        }
        public Vector3 topPoint
        {

            get
            {

#if UNITY_EDITOR
                if (shapeSettings.centerTransform == null) return transform.position;
#endif

                return shapeSettings.centerTransform.position + controller.groundUp * topDistance;
            }

        }
        #endregion

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        #region Utility Methods
        public float BottomDistance(Transform[] bottomTransforms)
        {

#if UNITY_EDITOR
            if (bottomTransforms == null) return 0.0f;
            if (bottomTransforms.Length == 0) return 0.0f;
            if (shapeSettings.centerTransform == null) return 0.0f;
#endif

            float bottom = 0.0f;

            foreach (Transform bottomTransform in bottomTransforms)
            {

                bottom = Mathf.Max(bottom, Vector3.Dot((bottomTransform.position - shapeSettings.centerTransform.position), -controller.groundUp));

            }

            return bottom;
        }
        public float TopDistance(Transform[] topTransforms)
        {

#if UNITY_EDITOR
            if (topTransforms == null) return 0.0f;
            if (topTransforms.Length == 0) return 0.0f;
            if (shapeSettings.centerTransform == null) return 0.0f;
#endif

            float top = 0.0f;

            foreach (Transform topTransform in topTransforms)
            {

                top = Mathf.Max(top, Vector3.Dot((topTransform.position - shapeSettings.centerTransform.position), controller.groundUp));

            }

            return top;
        }

        public Vector3 BottomPoint(Transform[] bottomTransforms)
        {

#if UNITY_EDITOR
            if (shapeSettings.centerTransform == null) return transform.position;
#endif

            return shapeSettings.centerTransform.position - controller.groundUp * BottomDistance(bottomTransforms);

        }
        public Vector3 TopPoint(Transform[] topTransforms)
        {

#if UNITY_EDITOR
            if (shapeSettings.centerTransform == null) return transform.position;
#endif

            return shapeSettings.centerTransform.position + controller.groundUp * TopDistance(topTransforms);
        }
        #endregion

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        #region Methods
        public void UpdateShape()
        {

#if UNITY_EDITOR
            if (shapeSettings.centerTransform == null) return;
#endif

            float top = topDistance;
            float bottom = bottomDistance;
            float center = Vector3.Dot((shapeSettings.centerTransform.position - transform.position), controller.groundUp);

            float height = top + bottom;

            capsuleCollider.height = height;
            capsuleCollider.radius = shapeSettings.radius;

            capsuleCollider.center = Vector3.up * (center - bottom + height * 0.5f);

        }
        public void UpdateHead()
        {

            Matrix4x4 l2w = transform.localToWorldMatrix;

            headSettings.headTransform.position = l2w * new Vector4(headSettings.headOffset.x, headSettings.headOffset.y, headSettings.headOffset.z, 1.0f);

        }
        #endregion

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        #region MonoBehavior Events
        protected override void Awake()
        {

            base.Awake();

            m_Rigidbody = GetComponent<Rigidbody>();
            m_CapsuleCollider = GetComponent<CapsuleCollider>();

        }
        protected virtual void Update()
        {



        }

        protected virtual void FixedUpdate()
        {

            UpdateShape();

        }
        #endregion

    }

}