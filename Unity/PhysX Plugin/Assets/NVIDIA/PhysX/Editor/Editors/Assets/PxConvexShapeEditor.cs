using NVIDIA.PhysX.Unity;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

namespace NVIDIA.PhysX.UnityEditor
{
    public class PxConvexShapeEditor : PxSolidShapeEditor
    {
        SerializedProperty m_density;

        protected override void InitProperties()
        {
            base.InitProperties();
            m_density = serializedObject.FindProperty("m_density");
        }

        protected void ShapeDensityUI()
        {
            BeginRecreateCheck();
            EditorGUILayout.PropertyField(m_density);
            EndRecreateCheck();
        }
    }
}