using NVIDIA.PhysX.Unity;
using UnityEditor;
using UnityEngine;
using PXU = NVIDIA.PhysX.Unity;

namespace NVIDIA.PhysX.UnityEditor
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(PXU.PxFixedJoint))]
    public class PxFixedJointEditor : PxJointEditor
    {
        void OnEnable()
        {
            InitFields();
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            ActorBodiesUI();

            EditorGUILayout.Separator();

            JointOptionsUI();

            if (GUI.changed)
                serializedObject.ApplyModifiedProperties();
        }
    }
}