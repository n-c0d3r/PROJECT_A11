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
        public struct ModifierSlot
        {

            public MonoScript Script;
            public AnimationModifier Object;

        }



        [SerializeField]
        private List<AnimationClip> m_Clips = new List<AnimationClip>();
        public List<AnimationClip> clips { get { return m_Clips; } }

        [SerializeField]
        private List<ModifierSlot> m_ModifierSlots = new List<ModifierSlot>();
        public List<ModifierSlot> modifierSlots { get { return m_ModifierSlots; } }

        [SerializeField]
        private SerializedObject m_SerializedObject;



        private void DrawModifiersSettings()
        {

            m_SerializedObject.Update();

            var modifierSlotsProperty = m_SerializedObject.FindProperty("m_ModifierSlots");

            EditorGUILayout.PropertyField(modifierSlotsProperty, new GUIContent("Modifier Slots"), true);

            m_SerializedObject.ApplyModifiedProperties();

        }

        private void ApplyModifierSlot(ModifierSlot slot, AnimationClip clip)
        {

            AnimationModifier modifier;

            if (slot.Object != null)
            {

                modifier = slot.Object;

            }
            else
            {

                modifier = (AnimationModifier)CreateInstance(slot.Script.GetClass());

            }

            modifier.Apply(clip);

        }

        private void ApplyModifiers(AnimationClip clip)
        {

            foreach (var slot in m_ModifierSlots)
            {

                ApplyModifierSlot(slot, clip);

            }

        }



        private void DrawClipSettings()
        {

            m_SerializedObject.Update();

            var clipsProperty = m_SerializedObject.FindProperty("m_Clips");

            EditorGUILayout.PropertyField(clipsProperty, new GUIContent("Clips"), true);

            m_SerializedObject.ApplyModifiedProperties();

        }



        private void ApplyAll()
        {

            foreach (var clip in m_Clips)
            {

                ApplyModifiers(clip);

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

            DrawClipSettings();
            DrawModifiersSettings();

            EditorGUILayout.Space(10);
            
            DrawFunctionalities();

        }

    }

}

#endif