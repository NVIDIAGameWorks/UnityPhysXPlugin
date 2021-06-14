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