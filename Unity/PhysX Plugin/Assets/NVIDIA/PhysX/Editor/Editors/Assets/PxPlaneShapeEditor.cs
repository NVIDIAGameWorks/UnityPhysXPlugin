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