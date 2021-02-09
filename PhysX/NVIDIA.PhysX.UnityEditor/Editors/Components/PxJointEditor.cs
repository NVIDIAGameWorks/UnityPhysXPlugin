// This code contains NVIDIA Confidential Information and is disclosed to you
// under a form of NVIDIA software license agreement provided separately to you.
//
// Notice
// NVIDIA Corporation and its licensors retain all intellectual property and
// proprietary rights in and to this software and related documentation and
// any modifications thereto. Any use, reproduction, disclosure, or
// distribution of this software and related documentation without an express
// license agreement from NVIDIA Corporation is strictly prohibited.
//
// ALL NVIDIA DESIGN SPECIFICATIONS, CODE ARE PROVIDED "AS IS.". NVIDIA MAKES
// NO WARRANTIES, EXPRESSED, IMPLIED, STATUTORY, OR OTHERWISE WITH RESPECT TO
// THE MATERIALS, AND EXPRESSLY DISCLAIMS ALL IMPLIED WARRANTIES OF NONINFRINGEMENT,
// MERCHANTABILITY, AND FITNESS FOR A PARTICULAR PURPOSE.
//
// Information and code furnished is believed to be accurate and reliable.
// However, NVIDIA Corporation assumes no responsibility for the consequences of use of such
// information or for any infringement of patents or other rights of third parties that may
// result from its use. No license is granted by implication or otherwise under any patent
// or patent rights of NVIDIA Corporation. Details are subject to change without notice.
// This code supersedes and replaces all information previously supplied.
// NVIDIA Corporation products are not authorized for use as critical
// components in life support devices or systems without express written approval of
// NVIDIA Corporation.
//
// Copyright (c) 2019 NVIDIA Corporation. All rights reserved.

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