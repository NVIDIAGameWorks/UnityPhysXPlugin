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
using PXU = NVIDIA.PhysX.Unity;

namespace NVIDIA.PhysX.UnityEditor
{
    [CustomEditor(typeof(PXU.PxTriangleMesh))]
    public class PxTriangleMeshEditor : PxAssetEditor
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
                var triangleMesh = target as PXU.PxTriangleMesh;
                if (triangleMesh)
                {
                    GUI.enabled = false;
                    EditorGUILayout.IntField(new GUIContent("Vertex Count"), triangleMesh.vertexCount);
                    EditorGUILayout.IntField(new GUIContent("Triangle Count"), triangleMesh.triangleCount);
                    GUI.enabled = true;
                }
            }

            EditorGUILayout.Separator();

            if (GUILayout.Button("Build From Mesh..."))
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
                    var triangleMesh = target as PXU.PxTriangleMesh;
                    if (triangleMesh)
                    {
                        triangleMesh.BuildFromMesh(mesh);
                        EditorUtility.SetDirty(target);
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
            var triangleMesh = target as PXU.PxTriangleMesh;
            if (triangleMesh != null)
            {
                var vertices = triangleMesh.vertices;
                var triangles = triangleMesh.triangles;
                for (int i = 0; i < triangles.Length / 3; ++i)
                {
                    for (int j = 0; j < 3; ++j)
                    {
                        int index0 = triangles[i * 3 + j];
                        int index1 = triangles[i * 3 + (j + 1) % 3];
                        //if (index0 > index1)
                            AddLine(positions, colors, vertices[index0], vertices[index1], color);
                    }
                }
            }
        }
    }
}