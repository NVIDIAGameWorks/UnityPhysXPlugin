using UnityEditor;
using UnityEngine;
using PXU = NVIDIA.PhysX.Unity;

namespace NVIDIA.PhysX.UnityEditor
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(PXU.PxMaterial))]
    public class PxMaterialEditor : Editor
    {
        SerializedProperty m_staticFriction;
        SerializedProperty m_dynamicFriction;
        SerializedProperty m_frictionCombine;
        SerializedProperty m_restitution;
        SerializedProperty m_restitutionCombine;
        SerializedProperty m_disableFriction;
        SerializedProperty m_disableStrongFriction;
        SerializedProperty m_improvedPatchFriction;

        void OnEnable()
        {
            m_staticFriction = serializedObject.FindProperty("m_staticFriction");
            m_dynamicFriction = serializedObject.FindProperty("m_dynamicFriction");
            m_frictionCombine = serializedObject.FindProperty("m_frictionCombine");
            m_restitution = serializedObject.FindProperty("m_restitution");
            m_restitutionCombine = serializedObject.FindProperty("m_restitutionCombine");
            m_disableFriction = serializedObject.FindProperty("m_disableFriction");
            m_disableStrongFriction = serializedObject.FindProperty("m_disableStrongFriction");
            m_improvedPatchFriction = serializedObject.FindProperty("m_improvedPatchFriction");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(m_staticFriction);
            EditorGUILayout.PropertyField(m_dynamicFriction);
            EditorGUILayout.PropertyField(m_restitution);
            EditorGUILayout.PropertyField(m_frictionCombine);
            EditorGUILayout.PropertyField(m_restitutionCombine);
            EditorGUILayout.PropertyField(m_disableFriction);
            EditorGUILayout.PropertyField(m_disableStrongFriction);
            EditorGUILayout.PropertyField(m_improvedPatchFriction);

            if (GUI.changed)
                serializedObject.ApplyModifiedProperties();
        }
    }
}