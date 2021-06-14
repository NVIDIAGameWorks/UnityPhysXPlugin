using NVIDIA.PhysX.Unity;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

namespace NVIDIA.PhysX.UnityEditor
{
    public class PxSimpleShapeEditor : PxShapeEditor
    {
        SerializedProperty m_position;
        SerializedProperty m_rotation;
        SerializedProperty m_materials;
        SerializedProperty m_simulationShape;
        SerializedProperty m_sceneQueryShape;
        SerializedProperty m_collisionLayer;
        SerializedProperty m_solveCollision;

        protected override void InitProperties()
        {
            base.InitProperties();
            m_position = serializedObject.FindProperty("m_position");
            m_rotation = serializedObject.FindProperty("m_rotation");
            m_materials = serializedObject.FindProperty("m_materials");
            m_simulationShape = serializedObject.FindProperty("m_simulationShape");
            m_sceneQueryShape = serializedObject.FindProperty("m_sceneQueryShape");
            m_collisionLayer = serializedObject.FindProperty("m_collisionLayer");
            m_solveCollision = serializedObject.FindProperty("m_solveCollision");
        }

        protected void ShapePositionUI()
        {
            BeginRecreateCheck();
            EditorGUILayout.PropertyField(m_position);
            EditorGUILayout.PropertyField(m_rotation);
            EndRecreateCheck();
        }

        protected void SingleMaterialUI()
        {
            BeginRecreateCheck();
            EditorGUILayout.PropertyField(m_materials.GetArrayElementAtIndex(0), new GUIContent("Material"));
            EndRecreateCheck();
        }

        protected void MaterialArrayUI()
        {
            BeginRecreateCheck();
            EditorGUILayout.PropertyField(m_materials);
            if (m_materials.isExpanded && !m_materials.hasMultipleDifferentValues)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(serializedObject.FindProperty("m_materials.Array.size"));
                for (int i = 0; i < m_materials.arraySize; ++i)
                    EditorGUILayout.PropertyField(m_materials.GetArrayElementAtIndex(i), new GUIContent("Material " + i));
                EditorGUI.indentLevel--;
            }
            EndRecreateCheck();
        }

        protected void ShapeFlagsUI()
        {
            BeginRecreateCheck();
            EditorGUILayout.PropertyField(m_collisionLayer);
            EditorGUILayout.PropertyField(m_simulationShape);
            EditorGUILayout.PropertyField(m_sceneQueryShape);
            EditorGUILayout.PropertyField(m_solveCollision);
            EndRecreateCheck();
        }
    }
}