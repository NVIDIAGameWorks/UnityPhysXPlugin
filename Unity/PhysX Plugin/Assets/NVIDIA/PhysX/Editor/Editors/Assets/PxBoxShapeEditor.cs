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