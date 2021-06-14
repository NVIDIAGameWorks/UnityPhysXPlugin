using NVIDIA.PhysX.Unity;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using PXU = NVIDIA.PhysX.Unity;

namespace NVIDIA.PhysX.UnityEditor
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(PxSphereShape))]
    public class PxSphereShapeEditor : PxConvexShapeEditor
    {
        SerializedProperty m_radius;

        void OnEnable()
        {
            CreatePreviewRender();
            InitProperties();

            m_radius = serializedObject.FindProperty("m_radius");
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
            var sphereShape = target as PXU.PxSphereShape;
            if (sphereShape != null)
            {
                var TRS = Matrix4x4.TRS(sphereShape.position, sphereShape.rotation, Vector3.one);
                int start = positions.Count;
                AddSphere(positions, colors, sphereShape.radius, color);
                for (int i = start; i < positions.Count; ++i) positions[i] = TRS.MultiplyPoint(positions[i]);
            }
        }
    }
}