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