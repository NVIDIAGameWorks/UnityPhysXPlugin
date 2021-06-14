using NVIDIA.PhysX.Unity;
using UnityEditor;
using UnityEngine;

namespace NVIDIA.PhysX.UnityEditor
{
    [CustomEditor(typeof(PxSettings))]
    public class PxSettingsEditor : PxAssetEditor
    {
        SerializedProperty m_lengthScale;
        SerializedProperty m_speedScale;
        SerializedProperty m_pvdConnect;
        SerializedProperty m_pvdHost;
        SerializedProperty m_pvdPort;
        SerializedProperty m_pvdTimeout;
        SerializedProperty m_pvdInstrumentation;
        SerializedProperty m_cpuThreads;
        SerializedProperty m_cpuAffinities;
        SerializedProperty m_gpuSimulation;

        void OnEnable()
        {
            m_lengthScale = serializedObject.FindProperty("m_lengthScale");
            m_speedScale = serializedObject.FindProperty("m_speedScale");
            m_pvdConnect = serializedObject.FindProperty("m_pvdConnect");
            m_pvdHost = serializedObject.FindProperty("m_pvdHost");
            m_pvdPort = serializedObject.FindProperty("m_pvdPort");
            m_pvdTimeout = serializedObject.FindProperty("m_pvdTimeout");
            m_pvdInstrumentation = serializedObject.FindProperty("m_pvdInstrumentation");
            m_cpuThreads = serializedObject.FindProperty("m_cpuThreads");
            m_cpuAffinities = serializedObject.FindProperty("m_cpuAffinities");
            m_gpuSimulation = serializedObject.FindProperty("m_gpuSimulation");
        }

        void OnDisable()
        {
            if (serializedObject.hasModifiedProperties)
            {
                var message = string.Format("Unapplied changes for '{0}'", target.name);
                if (EditorUtility.DisplayDialog("Unapplied changes", message, "Apply", "Revert"))
                {
                    serializedObject.ApplyModifiedPropertiesWithoutUndo();
                    serializedObject.SetIsDifferentCacheDirty();
                    serializedObject.Update();
                }
            }
        }

        public override void OnInspectorGUI()
        {
            BeginRecreateCheck();

            EditorGUILayout.LabelField("Tolerances Scale");
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(m_lengthScale, new GUIContent("Length"));
            EditorGUILayout.PropertyField(m_speedScale, new GUIContent("Speed"));
            EditorGUI.indentLevel--;

            EditorGUILayout.Separator();

            EditorGUILayout.LabelField("CPU Dispatcher");
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(m_cpuThreads, new GUIContent("Threads"));
            EditorGUILayout.PropertyField(m_cpuAffinities, new GUIContent("Affinity Masks"), true);
            EditorGUI.indentLevel--;

            EditorGUILayout.Separator();

            m_gpuSimulation.boolValue = EditorGUILayout.Toggle/*Left*/("GPU Simulation", m_gpuSimulation.boolValue);

            EditorGUILayout.Separator();

            m_pvdConnect.boolValue = EditorGUILayout.Toggle/*Left*/("Visual Debugger", m_pvdConnect.boolValue);
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(m_pvdHost, new GUIContent("Host"));
            EditorGUILayout.PropertyField(m_pvdPort, new GUIContent("Port"));
            EditorGUILayout.PropertyField(m_pvdTimeout, new GUIContent("Timeout"));
            EditorGUILayout.LabelField("Instrumentation");
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(m_pvdInstrumentation.FindPropertyRelative("debug"));
            EditorGUILayout.PropertyField(m_pvdInstrumentation.FindPropertyRelative("profile"));
            EditorGUILayout.PropertyField(m_pvdInstrumentation.FindPropertyRelative("memory"));
            EditorGUI.indentLevel--;
            EditorGUI.indentLevel--;

            EditorGUILayout.Separator();

            EndRecreateCheck();

            using (new EditorGUI.DisabledScope(!serializedObject.hasModifiedProperties))
            {
                using (new EditorGUILayout.HorizontalScope())
                {
                    GUILayout.FlexibleSpace();
                    if (GUILayout.Button("Apply", GUILayout.Width(80), GUILayout.Height(25)))
                    {
                        serializedObject.ApplyModifiedPropertiesWithoutUndo();
                        serializedObject.SetIsDifferentCacheDirty();
                        serializedObject.Update();
                    }
                    if (GUILayout.Button("Revert", GUILayout.MaxWidth(80), GUILayout.Height(25)))
                    {
                        serializedObject.SetIsDifferentCacheDirty();
                        serializedObject.Update();
                        if (serializedObject.hasModifiedProperties)
                            Debug.LogError("Asset reports modified values after reverting.");
                    }
                }
            }
        }
    }
}