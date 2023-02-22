using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;
using static UnityEditor.Profiling.HierarchyFrameDataView;

namespace PROJECT_A11.Develops.Common
{

    public class PersonViewController :
        MonoBehaviour
    {

        public enum Mode
        {

            TPV = 0,

            FPV = 1

        }



        public Camera camera;

        public LayerMask tpvMask;
        public LayerMask fpvMask;

        public bool modeChangable = false;



        [Space(10)]
        [Header("Read-only Details")]
        [ReadOnly]
        [SerializeField]
        private Mode m_Mode = Mode.FPV;
        public Mode mode { get { return m_Mode; } }



        public void ChangeViewMode(Mode newMode)
        {

            if (!modeChangable) return;

            m_Mode = newMode;

        }



        private void UpdateRenderersVisibility()
        {

            if (camera == null) return;

            if (mode == Mode.TPV)
            {

                camera.cullingMask = (camera.cullingMask & (~tpvMask)) & (~fpvMask) | tpvMask;

            }

            if (mode == Mode.FPV)
            {

                camera.cullingMask = (camera.cullingMask & (~fpvMask)) & (~tpvMask) | fpvMask;

            }

        }



        protected virtual void Awake()
        {



        }

        protected virtual void Update()
        {

            UpdateRenderersVisibility();

        }

    }

}