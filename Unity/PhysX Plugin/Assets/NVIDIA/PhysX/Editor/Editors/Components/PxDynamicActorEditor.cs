using NVIDIA.PhysX.Unity;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using PXU = NVIDIA.PhysX.Unity;

namespace NVIDIA.PhysX.UnityEditor
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(PxDynamicActor))]
    public class PxDynamicActorEditor : PxActorEditor
    {
        SerializedProperty m_sceneOverride;
        SerializedProperty m_collisionShape;
        SerializedProperty m_autoMass;
        SerializedProperty m_mass;
        SerializedProperty m_massPosition;
        SerializedProperty m_massRotation;
        SerializedProperty m_localInertia;
        SerializedProperty m_visualization;
        SerializedProperty m_linearVelocity;
        SerializedProperty m_angularVelocity;
        SerializedProperty m_disableGravity;
        SerializedProperty m_disableSimulation;
        //SerializedProperty m_collisionEvents;
        SerializedProperty m_positionIterations;
        SerializedProperty m_velocityIterations;
        SerializedProperty m_kinematic;
        SerializedProperty m_enableCCD;

        void OnEnable()
        {
            m_sceneOverride = serializedObject.FindProperty("m_sceneOverride");
            m_collisionShape = serializedObject.FindProperty("m_collisionShape");
            m_autoMass = serializedObject.FindProperty("m_autoMass");
            m_mass = serializedObject.FindProperty("m_mass");
            m_massPosition = serializedObject.FindProperty("m_massPosition");
            m_massRotation = serializedObject.FindProperty("m_massRotation");
            m_localInertia = serializedObject.FindProperty("m_localInertia");
            m_visualization = serializedObject.FindProperty("m_visualization");
            m_linearVelocity = serializedObject.FindProperty("m_linearVelocity");
            m_angularVelocity = serializedObject.FindProperty("m_angularVelocity");
            m_disableGravity = serializedObject.FindProperty("m_disableGravity");
            m_disableSimulation = serializedObject.FindProperty("m_disableSimulation");
            //m_collisionEvents = serializedObject.FindProperty("m_collisionEvents");
            m_positionIterations = serializedObject.FindProperty("m_positionIterations");
            m_velocityIterations = serializedObject.FindProperty("m_velocityIterations");
            m_kinematic = serializedObject.FindProperty("m_kinematic");
            m_enableCCD = serializedObject.FindProperty("m_enableCCD");

            InitProperties();
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            SceneOverrideUI();

            EditorGUILayout.Separator();

            EditorGUILayout.LabelField("Solver Iterations");
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(m_positionIterations, new GUIContent("Position"));
            EditorGUILayout.PropertyField(m_velocityIterations, new GUIContent("Velocity"));
            EditorGUI.indentLevel--;

            EditorGUILayout.Separator();

            CollisionLayerUI();

            EditorGUILayout.Separator();

            BeginRecreateCheck();
            EditorGUILayout.PropertyField(m_collisionShape);
            EndRecreateCheck();

            //EditorGUILayout.Separator();

            //EditorGUILayout.PropertyField(m_collisionEvents);

            EditorGUILayout.Separator();

            BeginRecreateCheck();
            EditorGUILayout.PropertyField(m_autoMass);
            GUI.enabled = !m_autoMass.boolValue && !m_autoMass.hasMultipleDifferentValues;
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(m_mass);
            //EditorGUILayout.LabelField("Mass Frame");
            //EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(m_massPosition/*, new GUIContent("Position")*/);
            EditorGUILayout.PropertyField(m_massRotation/*, new GUIContent("Rotation")*/);
            //EditorGUI.indentLevel--;
            EditorGUILayout.PropertyField(m_localInertia);
            EditorGUI.indentLevel--;
            GUI.enabled = true;
            EndRecreateCheck();

            EditorGUILayout.Separator();

            EditorGUILayout.PropertyField(m_linearVelocity);
            EditorGUILayout.PropertyField(m_angularVelocity);

            EditorGUILayout.Separator();

            //EditorGUILayout.PropertyField(m_visualization);
            EditorGUILayout.PropertyField(m_disableGravity);
            EditorGUILayout.PropertyField(m_kinematic);
            EditorGUILayout.PropertyField(m_enableCCD);
            //EditorGUILayout.PropertyField(m_disableSimulation);

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