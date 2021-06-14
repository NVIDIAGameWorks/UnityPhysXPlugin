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