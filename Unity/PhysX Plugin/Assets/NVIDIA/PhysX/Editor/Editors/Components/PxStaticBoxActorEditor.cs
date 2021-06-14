using NVIDIA.PhysX.Unity;
using UnityEditor;
using UnityEngine;
using PXU = NVIDIA.PhysX.Unity;

namespace NVIDIA.PhysX.UnityEditor
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(PxStaticBoxActor))]
    public class PxStaticBoxActorEditor : PxActorEditor
    {
        SerializedProperty m_simulationShape;
        SerializedProperty m_sceneQueryShape;
        SerializedProperty m_solveCollision;

        void OnEnable()
        {
            m_simulationShape = serializedObject.FindProperty("m_simulationShape");
            m_sceneQueryShape = serializedObject.FindProperty("m_sceneQueryShape");
            m_solveCollision = serializedObject.FindProperty("m_solveCollision");

            InitProperties();
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            SceneOverrideUI();

            EditorGUILayout.Separator();

            CollisionLayerUI();

            EditorGUILayout.Separator();

            BeginRecreateCheck();
            EditorGUILayout.PropertyField(m_simulationShape);
            EditorGUILayout.PropertyField(m_sceneQueryShape);
            EditorGUILayout.PropertyField(m_solveCollision);
            EndRecreateCheck();

            if (GUI.changed)
                serializedObject.ApplyModifiedProperties();
        }

        void OnSceneGUI()
        {
            var actor = target as PXU.PxActor;
            if (actor) DrawBodies(actor);
        }
    }
}