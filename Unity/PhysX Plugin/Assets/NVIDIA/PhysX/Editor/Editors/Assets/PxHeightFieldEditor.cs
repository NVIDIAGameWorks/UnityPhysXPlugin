using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using PXU = NVIDIA.PhysX.Unity;

namespace NVIDIA.PhysX.UnityEditor
{
    [CustomEditor(typeof(PXU.PxHeightField))]
    public class PxHeightFieldEditor : PxAssetEditor
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
                var heightField = target as PXU.PxHeightField;
                if (heightField)
                {
                    GUI.enabled = false;
                    EditorGUILayout.IntField(new GUIContent("Row Count"), heightField.rowCount);
                    EditorGUILayout.IntField(new GUIContent("Column Count"), heightField.columnCount);
                    GUI.enabled = true;
                }
            }

            EditorGUILayout.Separator();

            if (GUILayout.Button("Build From TerrainData..."))
            {
                int controlID = EditorGUIUtility.GetControlID(FocusType.Passive);
                EditorGUIUtility.ShowObjectPicker<TerrainData>(null, true, "", controlID);
            }

            string commandName = Event.current.commandName;
            if (commandName == "ObjectSelectorClosed")
            {
                var terrainData = EditorGUIUtility.GetObjectPickerObject() as TerrainData;
                if (terrainData)
                {
                    var heightField = target as PXU.PxHeightField;
                    if (heightField)
                    {
                        heightField.BuildFromTerrainData(terrainData);
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
            var heightField = target as PXU.PxHeightField;
            if (heightField != null)
            {
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
            }
        }
    }
}