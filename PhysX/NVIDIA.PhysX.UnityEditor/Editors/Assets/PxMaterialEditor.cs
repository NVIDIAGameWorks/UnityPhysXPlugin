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