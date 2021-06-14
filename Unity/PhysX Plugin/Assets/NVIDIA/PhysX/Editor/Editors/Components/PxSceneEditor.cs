using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using PXU = NVIDIA.PhysX.Unity;

namespace NVIDIA.PhysX.UnityEditor
{
    //[CanEditMultipleObjects]
    [CustomEditor(typeof(PXU.PxScene))]
    public class PxSceneEditor : PxComponentEditor
    {
        SerializedProperty m_gravity;
        SerializedProperty m_solverType;
        SerializedProperty m_broadPhaseType;
        SerializedProperty m_gpuSimulation;
        SerializedProperty m_enableCCD;
        SerializedProperty m_collisionLayers;
        SerializedProperty m_collisionMatrix;

        void OnEnable()
        {
            m_gravity = serializedObject.FindProperty("m_gravity");
            m_solverType = serializedObject.FindProperty("m_solverType");
            m_broadPhaseType = serializedObject.FindProperty("m_broadPhaseType");
            m_gpuSimulation = serializedObject.FindProperty("m_gpuSimulation");
            m_enableCCD = serializedObject.FindProperty("m_enableCCD");
            m_collisionLayers = serializedObject.FindProperty("m_collisionLayers");
            m_collisionMatrix = serializedObject.FindProperty("m_collisionMatrix");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(m_gravity);

            EditorGUILayout.Separator();

            BeginRecreateCheck();

            if (m_gpuSimulation.hasMultipleDifferentValues || m_gpuSimulation.boolValue)
            {
                GUI.enabled = false;
                if (m_gpuSimulation.hasMultipleDifferentValues) EditorGUI.showMixedValue = true;
                EditorGUILayout.IntPopup("Broad Phase Type", 0, new[] { "GPU" }, new[] { 0 });
                EditorGUI.showMixedValue = false;
                GUI.enabled = true;
            }
            else
            {
                var names = new GUIContent[] { new GUIContent("SAP"), /*new GUIContent("MBP"),*/ new GUIContent("ABP") };
                var values = new int[] { 0, /*1,*/ 2 };
                if (m_broadPhaseType.hasMultipleDifferentValues) EditorGUI.showMixedValue = true;
                EditorGUILayout.IntPopup(m_broadPhaseType, names, values);
                EditorGUI.showMixedValue = false;
            }

            EditorGUILayout.PropertyField(m_solverType);
            EditorGUILayout.PropertyField(m_gpuSimulation, new GUIContent("GPU Simulation"));
            EditorGUILayout.PropertyField(m_enableCCD);

            EndRecreateCheck();

            EditorGUILayout.Separator();

            CollisionLayersUI();

            if (GUI.changed)
                serializedObject.ApplyModifiedProperties();
        }

        void CollisionLayersUI()
        {
            BeginRecreateCheck();

            int count = m_collisionLayers.arraySize;
            EditorGUILayout.PropertyField(m_collisionLayers, false);

            if (m_collisionLayers.isExpanded)
            {
                EditorGUI.indentLevel++;

                for (int i = 0; i < count; ++i)
                {
                    GUI.enabled = i > 0;
                    EditorGUILayout.PropertyField(m_collisionLayers.GetArrayElementAtIndex(i), new GUIContent("Layer " + i));
                }

                EditorGUI.indentLevel--;
            }

            EditorGUILayout.Separator();

            List<GUIContent> layerNames = new List<GUIContent>();
            List<int> layerIndices = new List<int>();
            GUIStyle lableStyle = EditorStyles.label;
            float maxLength = 0;

            for (int i = 0; i < count; ++i)
            {
                string layerName = m_collisionLayers.GetArrayElementAtIndex(i).stringValue;
                if (layerName != string.Empty)
                {
                    layerNames.Add(new GUIContent(layerName));
                    layerIndices.Add(i);
                    maxLength = Mathf.Max(maxLength, lableStyle.CalcSize(new GUIContent(layerName)).x * 1.3f);
                }
            }

            int num = layerNames.Count;
            EditorGUILayout.PropertyField(m_collisionMatrix, false);

            if (m_collisionMatrix.isExpanded)
            {
                Rect rect = GUILayoutUtility.GetRect(EditorGUIUtility.labelWidth, maxLength);

                for (int j = 0; j < num; j++)
                {
                    Vector3 pos = new Vector3(EditorGUIUtility.labelWidth + (num - j + 1) * 15f - 1f, rect.y - 9f, 0f);
                    GUI.matrix = Matrix4x4.identity;
                    GUIUtility.RotateAroundPivot(90.0f, pos);
                    if (SystemInfo.graphicsDeviceVersion.StartsWith("Direct3D 9.0"))
                        GUI.matrix *= Matrix4x4.TRS(new Vector3(-0.5f, -0.5f, 0f), Quaternion.identity, Vector3.one);
                    GUI.Label(new Rect(pos.x, pos.y, maxLength, 15f), layerNames[j], "RightLabel");
                }

                GUI.matrix = Matrix4x4.identity;

                for (int k = 0; k < num; k++)
                {
                    GUILayoutUtility.GetRect((float)(30 + 15 * num + 100), 15f);
                    GUI.Label(new Rect(15f, rect.y + maxLength + k * 15f - 10f, EditorGUIUtility.labelWidth, 15f), layerNames[k], "RightLabel");

                    for (int l = k; l < num; l++)
                    {
                        GUIContent content = new GUIContent(string.Empty, layerNames[k].text + "/" + layerNames[l].text);
                        int ki = layerIndices[k], li = layerIndices[l];
                        int index = ki * count - ki * (ki + 1) / 2 + li;
                        SerializedProperty collision = m_collisionMatrix.GetArrayElementAtIndex(index);
                        bool flag = collision.boolValue;
                        bool flag2 = GUI.Toggle(new Rect(EditorGUIUtility.labelWidth + (num - l) * 15f, rect.y + maxLength + k * 15f - 10f, 15f, 15f), flag, content);
                        if (flag2 != flag)
                            collision.boolValue = flag2;
                    }
                }
            }

            EndRecreateCheck();
        }
    }
}