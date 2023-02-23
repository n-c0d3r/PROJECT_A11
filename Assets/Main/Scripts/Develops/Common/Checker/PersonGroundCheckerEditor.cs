
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

    [CustomEditor(typeof(PersonGroundChecker))]
    public class PersonGroundCheckerEditor :
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

                ((PersonGroundChecker)target).Check();

            }

        }
        #endregion

    }

}

#endif