using NVIDIA.PhysX.Unity;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using PXU = NVIDIA.PhysX.Unity;

namespace NVIDIA.PhysX.UnityEditor
{
    [CustomEditor(typeof(PxCompoundShape))]
    public class PxCompoundShapeEditor : PxShapeEditor
    {
        SerializedProperty m_shapes;

        void OnEnable()
        {
            CreatePreviewRender();
            InitProperties();

            m_shapes = serializedObject.FindProperty("m_shapes");
        }

        void OnDisable()
        {
            DestroyPreviewRender();
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            BeginRecreateCheck();
            EditorGUILayout.PropertyField(m_shapes);
            if (m_shapes.isExpanded && !m_shapes.hasMultipleDifferentValues)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(serializedObject.FindProperty("m_shapes.Array.size"));
                for (int i = 0; i < m_shapes.arraySize; ++i)
                    EditorGUILayout.PropertyField(m_shapes.GetArrayElementAtIndex(i), new GUIContent("Shape " + i));
                EditorGUI.indentLevel--;
            }
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
            var compoundShape = target as PXU.PxCompoundShape;
            if (compoundShape != null)
            {
                List<PXU.PxShape> queue = new List<PXU.PxShape>(compoundShape.shapes);
                while (queue.Count > 0)
                {
                    var shape = queue[0]; queue.RemoveAt(0);
                    if (shape is PXU.PxCompoundShape) queue.AddRange((shape as PXU.PxCompoundShape).shapes);
                    else
                    {
                        var simpleShape = shape as PXU.PxSimpleShape;
                        if (simpleShape)
                        {
                            if (shape is PXU.PxSphereShape || shape is PXU.PxCapsuleShape || shape is PXU.PxBoxShape)
                            {
                                var TRS = Matrix4x4.TRS(simpleShape.position, simpleShape.rotation, Vector3.one);
                                int start = positions.Count;
                                if (shape is PXU.PxSphereShape) AddSphere(positions, colors, (shape as PXU.PxSphereShape).radius, color);
                                else if (shape is PXU.PxCapsuleShape) AddCapsule(positions, colors, (shape as PXU.PxCapsuleShape).radius, (shape as PXU.PxCapsuleShape).halfHeight, color);
                                else if (shape is PXU.PxBoxShape) AddBox(positions, colors, (shape as PXU.PxBoxShape).halfExtents, color);
                                for (int i = start; i < positions.Count; ++i) positions[i] = TRS.MultiplyPoint(positions[i]);
                            }
                            else if (shape is PXU.PxConvexMeshShape)
                            {
                                var convexMeshShape = shape as PXU.PxConvexMeshShape;
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
                            else if (shape is PXU.PxTriangleMeshShape)
                            {
                                var triangleMeshShape = shape as PXU.PxTriangleMeshShape;
                                var TRS = Matrix4x4.TRS(triangleMeshShape.position, triangleMeshShape.rotation, Vector3.one) *
                                          Matrix4x4.Rotate(Quaternion.Inverse(triangleMeshShape.scaleRotation)) *
                                          Matrix4x4.Scale(triangleMeshShape.scale) *
                                          Matrix4x4.Rotate(triangleMeshShape.scaleRotation);
                                var triangleMesh = triangleMeshShape.triangleMesh;
                                if (triangleMesh)
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
                                            AddLine(positions, colors, TRS.MultiplyPoint(vertices[index0]), TRS.MultiplyPoint(vertices[index1]), color);
                                        }
                                    }
                                }
                            }
                            else if (shape is PXU.PxHeightFieldShape)
                            {
                                var heightFieldShape = shape as PXU.PxHeightFieldShape;
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
            }
        }
    }
}