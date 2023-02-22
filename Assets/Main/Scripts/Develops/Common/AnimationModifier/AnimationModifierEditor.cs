#if UNITY_EDITOR

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.PackageManager.UI;
using UnityEditorInternal.VR;
using UnityEngine;
using UnityEngine.Assertions;



namespace PROJECT_A11.Develops.Common
{

    [System.Serializable]
    public class AnimationModifierEditor :
        EditorWindow
    {

        [System.Serializable]
        public struct ClipSlot
        {

            public AnimationClip input;
            public AnimationClip output;

        }



        [SerializeField]
        private List<ClipSlot> m_Clips = new List<ClipSlot>();
        public List<ClipSlot> clipSlots { get { return m_Clips; } }

        [SerializeField]
        private List<MonoScript> m_ModifierScripts = new List<MonoScript>();
        public List<MonoScript> modifierScripts { get { return m_ModifierScripts; } }

        [SerializeField]
        private SerializedObject m_SerializedObject;

        private Vector2 m_CurrentScrollPos;



        private void DrawModifiersSettings()
        {

            m_SerializedObject.Update();

            var modifierScriptsProperty = m_SerializedObject.FindProperty("m_ModifierScripts");

            EditorGUILayout.PropertyField(modifierScriptsProperty, new GUIContent("Modifier Scripts"), true);

            m_SerializedObject.ApplyModifiedProperties();

        }

        private void ApplyModifiers(ClipSlot clipSlot)
        {

            AnimationClip previousOutput = clipSlot.input;
            ClipSlot cs = clipSlot;

            foreach (var script in m_ModifierScripts)
            {

                AnimationModifier modifier = (AnimationModifier)CreateInstance(script.GetClass());

                if (modifier.usePreviousOutputAsInput)
                    cs.input = previousOutput;
                else 
                    cs.input = clipSlot.input;

                modifier.Apply(clipSlot.input, (cs.output == null) ? cs.input : cs.output);

                previousOutput = cs.output;

            }

        }



        private void DrawClipSettings()
        {

            m_SerializedObject.Update();

            var clipSlotsProperty = m_SerializedObject.FindProperty("m_Clips");

            EditorGUILayout.PropertyField(clipSlotsProperty, new GUIContent("Clips"), true);

            m_SerializedObject.ApplyModifiedProperties();

        }



        private void ApplyAll()
        {

            foreach (var clipSlot in m_Clips)
            {

                ApplyModifiers(clipSlot);

            }

        }

        private void DrawFunctionalities()
        {

            if (GUILayout.Button("Apply All"))
            {

                ApplyAll();

            }

        }



        [MenuItem("PROJECT_A11/Animation/Modifier Tool")]
        static private void Init()
        {

            AnimationModifierEditor editor = (AnimationModifierEditor)EditorWindow.GetWindow(typeof(AnimationModifierEditor), false, "Animation Modifier Tool");

            editor.Show();

        }



        private void OnEnable()
        {

            m_SerializedObject = new SerializedObject(this);

        }

        private void OnDisable()
        {
                


        }

        private void OnGUI()
        {

            m_CurrentScrollPos = EditorGUILayout.BeginScrollView(m_CurrentScrollPos, GUILayout.Width(position.width), GUILayout.Height(position.height));

            DrawClipSettings();
            DrawModifiersSettings();

            EditorGUILayout.Space(10);
            
            DrawFunctionalities();

            EditorGUILayout.EndScrollView();

        }

    }

}

#endif