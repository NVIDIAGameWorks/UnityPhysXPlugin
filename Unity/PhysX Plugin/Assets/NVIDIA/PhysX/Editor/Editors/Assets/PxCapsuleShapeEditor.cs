using NVIDIA.PhysX.Unity;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using PXU = NVIDIA.PhysX.Unity;

namespace NVIDIA.PhysX.UnityEditor
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(PxCapsuleShape))]
    public class PxCapsuleShapeEditor : PxConvexShapeEditor
    {
        SerializedProperty m_radius;
        SerializedProperty m_halfHeight;

        void OnEnable()
        {
            CreatePreviewRender();
            InitProperties();

            m_radius = serializedObject.FindProperty("m_radius");
            m_halfHeight = serializedObject.FindProperty("m_halfHeight");
        }

        void OnDisable()
        {
            DestroyPreviewRender();
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            BeginRecreateCheck();
            EditorGUILayout.PropertyField(m_radius);
            EditorGUILayout.PropertyField(m_halfHeight);
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
            var capsuleShape = target as PXU.PxCapsuleShape;
            if (capsuleShape != null)
            {
                var TRS = Matrix4x4.TRS(capsuleShape.position, capsuleShape.rotation, Vector3.one);
                int start = positions.Count;
                AddCapsule(positions, colors, capsuleShape.radius, capsuleShape.halfHeight, color);
                for (int i = start; i < positions.Count; ++i) positions[i] = TRS.MultiplyPoint(positions[i]);
            }
        }
    }
}