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
    [CustomEditor(typeof(PxConvexMeshShape))]
    public class PxConvexMeshShapeEditor : PxConvexShapeEditor
    {
        SerializedProperty m_convexMesh;
        SerializedProperty m_scale;
        SerializedProperty m_scaleRotation;

        void OnEnable()
        {
            CreatePreviewRender();
            InitProperties();

            m_convexMesh = serializedObject.FindProperty("m_convexMesh");
            m_scale = serializedObject.FindProperty("m_scale");
            m_scaleRotation = serializedObject.FindProperty("m_scaleRotation");
        }

        void OnDisable()
        {
            DestroyPreviewRender();
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            BeginRecreateCheck();
            EditorGUILayout.PropertyField(m_convexMesh);
            EditorGUILayout.PropertyField(m_scale);
            EditorGUILayout.PropertyField(m_scaleRotation);
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
            var convexMeshShape = target as PXU.PxConvexMeshShape;
            if (convexMeshShape != null)
            {
                var TRS = Matrix4x4.TRS(convexMeshShape.position, convexMeshShape.rotation, Vector3.one) *
                          Matrix4x4.Rotate(Quaternion.Inverse(convexMeshShape.scaleRotation)) *
                          Matrix4x4.Scale(convexMeshShape.scale) *
                          Matrix4x4.Rotate(convexMeshShape.scaleRotation);
                var convexMesh = convexMeshShape.convexMesh;
                if (convexMesh)
                {
                    var vertices = convexMesh.vertices;
                    int polyCount = convexMesh.polygonCount;
                    for (int i = 0; i < polyCount; ++i)
                    {
                        var polygon = convexMesh.GetPolygonIndices(i);
                        for (int j = 0; j < polygon.Length; ++j)
                        {
                            positions.Add(TRS.MultiplyPoint(vertices[polygon[j]])); colors.Add(color);
                            positions.Add(TRS.MultiplyPoint(vertices[polygon[(j + 1) % polygon.Length]])); colors.Add(color);
                        }
                    }
                }
            }
        }
    }
}