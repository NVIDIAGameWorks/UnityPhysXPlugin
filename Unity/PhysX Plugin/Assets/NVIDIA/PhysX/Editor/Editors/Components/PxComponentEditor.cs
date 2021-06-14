using UnityEditor;

namespace NVIDIA.PhysX.UnityEditor
{
    public class PxComponentEditor : Editor
    {
        protected virtual void InitProperties()
        {
        }
        protected void BeginRecreateCheck()
        {
            EditorGUI.BeginChangeCheck();
        }

        protected void EndRecreateCheck()
        {
            if (EditorGUI.EndChangeCheck())
                serializedObject.FindProperty("m_forceRecreate").boolValue = true;
        }
    }
}