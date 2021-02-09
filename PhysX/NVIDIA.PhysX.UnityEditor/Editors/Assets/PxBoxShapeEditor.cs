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
    [CustomEditor(typeof(PxBoxShape))]
    public class PxBoxShapeEditor : PxConvexShapeEditor
    {
        SerializedProperty m_halfExtents;

        void OnEnable()
        {
            CreatePreviewRender();
            InitProperties();

            m_halfExtents = serializedObject.FindProperty("m_halfExtents");
        }

        void OnDisable()
        {
            DestroyPreviewRender();
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            BeginRecreateCheck();
            EditorGUILayout.PropertyField(m_halfExtents);
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
            var boxShape = target as PXU.PxBoxShape;
            if (boxShape != null)
            {
                var TRS = Matrix4x4.TRS(boxShape.position, boxShape.rotation, boxShape.halfExtents);
                Vector3[] vertices = new[] { new Vector3(1, 1,  1), new Vector3(-1, 1,  1), new Vector3(-1, -1,  1), new Vector3(1, -1,  1),
                                             new Vector3(1, 1, -1), new Vector3(-1, 1, -1), new Vector3(-1, -1, -1), new Vector3(1, -1, -1) };
                for (int i = 0; i < 8; ++i) vertices[i] = TRS.MultiplyPoint(vertices[i]);
                positions.AddRange(new[] { vertices[0], vertices[1], vertices[1], vertices[2], vertices[2], vertices[3], vertices[3], vertices[0] });
                positions.AddRange(new[] { vertices[4], vertices[5], vertices[5], vertices[6], vertices[6], vertices[7], vertices[7], vertices[4] });
                positions.AddRange(new[] { vertices[0], vertices[4], vertices[1], vertices[5], vertices[2], vertices[6], vertices[3], vertices[7] });
                for (int i = 0; i < 24; ++i) colors.Add(color);
            }
        }
    }
}