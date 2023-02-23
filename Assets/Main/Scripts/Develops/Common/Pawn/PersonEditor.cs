
#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.InputSystem;
using static UnityEngine.GraphicsBuffer;



namespace PROJECT_A11.Develops.Common
{

    [CustomEditor(typeof(Person))]
    public class PersonEditor :
        Editor
    {

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        #region Events
        public override void OnInspectorGUI()
        {

            DrawDefaultInspector();

        }

        public void OnSceneGUI()
        {

            if (!Application.isPlaying)
            {

                ((Person)target).UpdateShape();
                ((Person)target).UpdateHead();

            }

        }
        #endregion

    }

}

#endif