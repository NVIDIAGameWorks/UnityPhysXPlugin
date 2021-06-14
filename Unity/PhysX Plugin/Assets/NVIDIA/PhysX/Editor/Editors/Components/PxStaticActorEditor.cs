using NVIDIA.PhysX.Unity;
using UnityEditor;
using UnityEngine;
using PXU = NVIDIA.PhysX.Unity;

namespace NVIDIA.PhysX.UnityEditor
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(PxStaticActor))]
    public class PxStaticActorEditor : PxActorEditor
    {
        SerializedProperty m_collisionShape;

        void OnEnable()
        {
            m_collisionShape = serializedObject.FindProperty("m_collisionShape");

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
            EditorGUILayout.PropertyField(m_collisionShape);
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