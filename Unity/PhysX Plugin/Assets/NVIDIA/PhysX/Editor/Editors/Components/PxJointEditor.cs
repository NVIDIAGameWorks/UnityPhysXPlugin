using NVIDIA.PhysX.Unity;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using PXU = NVIDIA.PhysX.Unity;

namespace NVIDIA.PhysX.UnityEditor
{
    public class PxJointEditor : PxComponentEditor
    {
        SerializedProperty m_actor0;
        SerializedProperty m_body0Index;
        SerializedProperty m_position0;
        SerializedProperty m_rotation0;
        SerializedProperty m_actor1;
        SerializedProperty m_body1Index;
        SerializedProperty m_position1;
        SerializedProperty m_rotation1;
        SerializedProperty m_breakForce;
        SerializedProperty m_breakTorque;
        SerializedProperty m_enableCollision;
        SerializedProperty m_disablePreprocessing;
        SerializedProperty m_invMassScale0;
        SerializedProperty m_invInertiaScale0;
        SerializedProperty m_invMassScale1;
        SerializedProperty m_invInertiaScale1;

        protected void InitFields()
        {
            m_actor0 = serializedObject.FindProperty("m_actor0");
            m_body0Index = serializedObject.FindProperty("m_body0Index");
            m_position0 = serializedObject.FindProperty("m_position0");
            m_rotation0 = serializedObject.FindProperty("m_rotation0");
            m_actor1 = serializedObject.FindProperty("m_actor1");
            m_body1Index = serializedObject.FindProperty("m_body1Index");
            m_position1 = serializedObject.FindProperty("m_position1");
            m_rotation1 = serializedObject.FindProperty("m_rotation1");
            m_breakForce = serializedObject.FindProperty("m_breakForce");
            m_breakTorque = serializedObject.FindProperty("m_breakTorque");
            m_enableCollision = serializedObject.FindProperty("m_enableCollision");
            m_disablePreprocessing = serializedObject.FindProperty("m_disablePreprocessing");
            m_invMassScale0 = serializedObject.FindProperty("m_invMassScale0");
            m_invInertiaScale0 = serializedObject.FindProperty("m_invInertiaScale0");
            m_invMassScale1 = serializedObject.FindProperty("m_invMassScale1");
            m_invInertiaScale1 = serializedObject.FindProperty("m_invInertiaScale1");
        }

        protected void JointOptionsUI()
        {
            EditorGUILayout.PropertyField(m_breakForce);
            EditorGUILayout.PropertyField(m_breakTorque);
            EditorGUILayout.PropertyField(m_enableCollision);
            EditorGUILayout.PropertyField(m_disablePreprocessing);
            EditorGUILayout.PropertyField(m_invMassScale0);
            EditorGUILayout.PropertyField(m_invInertiaScale0);
            EditorGUILayout.PropertyField(m_invMassScale1);
            EditorGUILayout.PropertyField(m_invInertiaScale1);
        }

        protected void ActorBodiesUI()
        {
            BeginRecreateCheck();
            ActorAndBodyIndexFields(m_actor0, m_body0Index, "Body");
            ActorAndBodyIndexFields(m_actor1, m_body1Index, "Body");
            EndRecreateCheck();

            EditorGUILayout.Separator();

            EditorGUILayout.LabelField("Anchor 0");
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(m_position0, new GUIContent("Position"));
            EditorGUILayout.PropertyField(m_rotation0, new GUIContent("Rotation"));
            EditorGUI.indentLevel--;
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Anchor 1");
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Compute"))
            {
                Undo.RecordObjects(targets, "Compute Anchor On Body 1");
                foreach (var t in targets)
                {
                    var joint = t as PXU.PxJoint;
                    if (joint)
                    {
                        joint.SetJointAnchors(joint.anchor0Position, joint.anchor0Rotation, true);
                    }
                }
            }
            EditorGUILayout.EndHorizontal();
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(m_position1, new GUIContent("Position"));
            EditorGUILayout.PropertyField(m_rotation1, new GUIContent("Rotation"));
            EditorGUI.indentLevel--;
        }

        void ActorAndBodyIndexFields(SerializedProperty actorField, SerializedProperty bodyIndexField, string bodyLabel)
        {
            EditorGUILayout.PropertyField(actorField);
            EditorGUI.indentLevel++;
            var actor = actorField.objectReferenceValue as PXU.PxActor;
            if (actor && targets.Length == 1)
            {
                var bodyNames = actor.apiActorNames;
                var bodyNamesGC = new GUIContent[bodyNames.Length];
                var bodyIndices = new int[bodyNames.Length];
                for (int i = 0; i < bodyNames.Length; ++i)
                {
                    bodyNamesGC[i] = new GUIContent("" + i + ". " + bodyNames[i]);
                    bodyIndices[i] = i;
                }
                EditorGUILayout.IntPopup(bodyIndexField, bodyNamesGC, bodyIndices, new GUIContent(bodyLabel));
            }
            else
            {
                EditorGUILayout.PropertyField(bodyIndexField, new GUIContent(bodyLabel));
            }
            EditorGUI.indentLevel--;
        }
    }
}