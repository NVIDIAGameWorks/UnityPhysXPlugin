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
    [CanEditMultipleObjects]
    [CustomEditor(typeof(PxSphereShape))]
    public class PxSphereShapeEditor : PxConvexShapeEditor
    {
        SerializedProperty m_radius;

        void OnEnable()
        {
            CreatePreviewRender();
            InitProperties();

            m_radius = serializedObject.FindProperty("m_radius");
        }

        void OnDisable()
        {
            DestroyPreviewRender();
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            BeginRecreateCheck();
            EditorGUILayout.PropertyField(m_radius);
            EndRecreateCheck();

            EditorGUILayout.Separator();

            ShapePositionUI();

            EditorGUILayout.Separator();

            ShapeDensityUI();

            EditorGUILayout.Separator();

            SingleMaterialUI();

            EditorGUILayout.Separator();

            ShapeFlagsUI();

            if (GUI.changed)
            {
                serializedObject.ApplyModifiedProperties();
                ResetPreview();
            }
        }

        public override bool HasPreviewGUI()
        {
            return targets.Length > 0;
        }
        protected override void GetPreviewLines(List<Vector3> positions, List<Color> colors)
        {
            base.GetPreviewLines(positions, colors);

            var color = Color.green * Color.gray;
            var sphereShape = target as PXU.PxSphereShape;
            if (sphereShape != null)
            {
                var TRS = Matrix4x4.TRS(sphereShape.position, sphereShape.rotation, Vector3.one);
                int start = positions.Count;
                AddSphere(positions, colors, sphereShape.radius, color);
                for (int i = start; i < positions.Count; ++i) positions[i] = TRS.MultiplyPoint(positions[i]);
            }
        }
    }
}