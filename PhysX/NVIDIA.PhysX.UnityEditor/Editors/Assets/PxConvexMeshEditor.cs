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

using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using PXU = NVIDIA.PhysX.Unity;

namespace NVIDIA.PhysX.UnityEditor
{
    [CustomEditor(typeof(PXU.PxConvexMesh))]
    public class PxConvexMeshEditor : PxAssetEditor
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

            {
                var convexMesh = target as PXU.PxConvexMesh;
                if (convexMesh)
                {
                    GUI.enabled = false;
                    EditorGUILayout.IntField(new GUIContent("Vertex Count"), convexMesh.vertexCount);
                    EditorGUILayout.IntField(new GUIContent("Polygon Count"), convexMesh.polygonCount);
                    GUI.enabled = true;
                }
            }

            EditorGUILayout.Separator();

            if (GUILayout.Button("Build Mesh Convex Hull..."))
            {
                int controlID = EditorGUIUtility.GetControlID(FocusType.Passive);
                EditorGUIUtility.ShowObjectPicker<Mesh>(null, true, "", controlID);
            }

            string commandName = Event.current.commandName;
            if (commandName == "ObjectSelectorClosed")
            {
                var mesh = EditorGUIUtility.GetObjectPickerObject() as Mesh;
                if (mesh)
                {
                    var convexMesh = target as PXU.PxConvexMesh;
                    if (convexMesh)
                    {
                        convexMesh.BuildFromPoints(mesh.vertices);
                        EditorUtility.SetDirty(target);
                        ResetPreview();
                    }
                }
            }

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
            var convexMesh = target as PXU.PxConvexMesh;
            if (convexMesh != null)
            {
                var vertices = convexMesh.vertices;
                int polyCount = convexMesh.polygonCount;
                for (int i = 0; i < polyCount; ++i)
                {
                    var polygon = convexMesh.GetPolygonIndices(i);
                    for (int j = 0; j < polygon.Length; ++j)
                        AddLine(positions, colors, vertices[polygon[j]], vertices[polygon[(j + 1) % polygon.Length]], color);
                }
            }
        }
    }
}