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
    [CustomEditor(typeof(PxPlaneShape))]
    public class PxPlaneShapeEditor : PxSolidShapeEditor
    {
        void OnEnable()
        {
            CreatePreviewRender();
            InitProperties();
        }

        void OnDisable()
        {
            DestroyPreviewRender();
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            ShapePositionUI();

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
            var color2 = Color.blue * Color.gray;
            var planeShape = target as PXU.PxPlaneShape;
            if (planeShape != null)
            {
                var TRS = Matrix4x4.TRS(planeShape.position, planeShape.rotation, Vector3.one);
                int start = positions.Count;
                AddLine(positions, colors, new Vector3(0, 1, 0.5f), new Vector3(0, -1, 0.5f), color);
                AddLine(positions, colors, new Vector3(0, 1, -0.5f), new Vector3(0, -1, -0.5f), color);
                AddLine(positions, colors, new Vector3(0, 0.5f, 1), new Vector3(0 , 0.5f, -1), color);
                AddLine(positions, colors, new Vector3(0, -0.5f, 1), new Vector3(0, -0.5f, -1), color);

                AddLine(positions, colors, new Vector3(0, 0.5f, 0.5f), new Vector3(-0.5f, 0.5f, 0.5f), color);
                AddLine(positions, colors, new Vector3(0, -0.5f, 0.5f), new Vector3(-0.5f, -0.5f, 0.5f), color);
                AddLine(positions, colors, new Vector3(0, -0.5f, -0.5f), new Vector3(-0.5f, -0.5f, -0.5f), color);
                AddLine(positions, colors, new Vector3(0, 0.5f, -0.5f), new Vector3(-0.5f, 0.5f, -0.5f), color);
                //AddLine(positions, colors, new Vector3(0, 0.5f, 0.5f), new Vector3(0.2f, 0.5f, 0.5f), color2);
                //AddLine(positions, colors, new Vector3(0, -0.5f, 0.5f), new Vector3(0.2f, -0.5f, 0.5f), color2);
                //AddLine(positions, colors, new Vector3(0, -0.5f, -0.5f), new Vector3(0.2f, -0.5f, -0.5f), color2);
                //AddLine(positions, colors, new Vector3(0, 0.5f, -0.5f), new Vector3(0.2f, 0.5f, -0.5f), color2);

                for (int i = start; i < positions.Count; ++i) positions[i] = TRS.MultiplyPoint(positions[i]);
            }
        }
    }
}