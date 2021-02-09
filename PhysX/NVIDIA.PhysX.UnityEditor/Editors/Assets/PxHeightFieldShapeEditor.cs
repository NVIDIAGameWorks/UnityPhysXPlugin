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
    [CustomEditor(typeof(PxHeightFieldShape))]
    public class PxHeightFieldShapeEditor : PxSimpleShapeEditor
    {
        SerializedProperty m_heightField;
        SerializedProperty m_heightScale;
        SerializedProperty m_rowScale;
        SerializedProperty m_columnScale;
        SerializedProperty m_doubleSided;

        void OnEnable()
        {
            CreatePreviewRender();
            InitProperties();

            m_heightField = serializedObject.FindProperty("m_heightField");
            m_heightScale = serializedObject.FindProperty("m_heightScale");
            m_rowScale = serializedObject.FindProperty("m_rowScale");
            m_columnScale = serializedObject.FindProperty("m_columnScale");
            m_doubleSided = serializedObject.FindProperty("m_doubleSided");
        }

        void OnDisable()
        {
            DestroyPreviewRender();
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            BeginRecreateCheck();
            EditorGUILayout.PropertyField(m_heightField);
            EditorGUILayout.PropertyField(m_heightScale);
            EditorGUILayout.PropertyField(m_rowScale);
            EditorGUILayout.PropertyField(m_columnScale);
            EndRecreateCheck();

            EditorGUILayout.Separator();

            ShapePositionUI();

            EditorGUILayout.Separator();

            MaterialArrayUI();

            EditorGUILayout.Separator();

            ShapeFlagsUI();

            BeginRecreateCheck();
            EditorGUILayout.PropertyField(m_doubleSided);
            EndRecreateCheck();

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
            var heightFieldShape = target as PXU.PxHeightFieldShape;
            if (heightFieldShape != null)
            {
                var TRS = Matrix4x4.TRS(heightFieldShape.position, heightFieldShape.rotation, new Vector3(heightFieldShape.columnScale, heightFieldShape.heightScale, heightFieldShape.rowScale));
                var heightField = heightFieldShape.heightField;
                if (heightField != null)
                {
                    int start = positions.Count;
                    int rows = heightField.rowCount;
                    int columns = heightField.columnCount;
                    for (int c = 0; c < columns; ++c)
                    {
                        for (int r = 0; r < rows; ++r)
                        {
                            float x0 = c * 1.0f / columns;
                            float y0 = heightField.GetHeight(c, r) / 0x7fff;
                            float z0 = r * 1.0f / rows;
                            var p0 = new Vector3(x0, y0, z0);
                            if (c < columns - 1)
                            {
                                float x1 = (c + 1) * 1.0f / columns;
                                float y1 = heightField.GetHeight(c + 1, r) / 0x7fff;
                                var p1 = new Vector3(x1, y1, z0);
                                AddLine(positions, colors, p0, p1, color);
                            }
                            if (r < rows - 1)
                            {
                                float y1 = heightField.GetHeight(c, r + 1) / 0x7fff;
                                float z1 = (r + 1) * 1.0f / rows;
                                var p1 = new Vector3(x0, y1, z1);
                                AddLine(positions, colors, p0, p1, color);
                            }
                        }
                    }
                    AddLine(positions, colors, new Vector3(0, 0, 0), new Vector3(1, 0, 0), color);
                    AddLine(positions, colors, new Vector3(1, 0, 0), new Vector3(1, 0, 1), color);
                    AddLine(positions, colors, new Vector3(1, 0, 1), new Vector3(0, 0, 1), color);
                    AddLine(positions, colors, new Vector3(0, 0, 1), new Vector3(0, 0, 0), color);

                    AddLine(positions, colors, new Vector3(0, 0, 0), new Vector3(0, heightField.GetHeight(0, 0) / 0x7fff, 0), color);
                    AddLine(positions, colors, new Vector3(1, 0, 0), new Vector3(1, heightField.GetHeight(columns - 1, 0) / 0x7fff, 0), color);
                    AddLine(positions, colors, new Vector3(1, 0, 1), new Vector3(1, heightField.GetHeight(columns - 1, rows - 1) / 0x7fff, 1), color);
                    AddLine(positions, colors, new Vector3(0, 0, 1), new Vector3(0, heightField.GetHeight(0, rows - 1) / 0x7fff, 1), color);

                    for (int i = start; i < positions.Count; ++i) positions[i] = TRS.MultiplyPoint(positions[i]);
                }
            }
        }
    }
}