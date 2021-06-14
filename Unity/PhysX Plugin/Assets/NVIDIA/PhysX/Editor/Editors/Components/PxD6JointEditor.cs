using NVIDIA.PhysX.Unity;
using NVIDIA.PhysX.UnityExtensions;
using UnityEditor;
using UnityEngine;
using PXU = NVIDIA.PhysX.Unity;

namespace NVIDIA.PhysX.UnityEditor
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(PXU.PxD6Joint))]
    public class PxD6JointEditor : PxJointEditor
    {
        SerializedProperty m_axes;
        SerializedProperty m_linearConstraints;
        SerializedProperty m_distanceConstraint;
        SerializedProperty m_twistConstraint;
        SerializedProperty m_swingConstraint;
        SerializedProperty m_coneConstraint;
        SerializedProperty m_pyramidConstraint;
        SerializedProperty m_drive;

        void OnEnable()
        {
            m_axes = serializedObject.FindProperty("m_axes");
            m_linearConstraints = serializedObject.FindProperty("m_linearConstraints");
            m_distanceConstraint = serializedObject.FindProperty("m_distanceConstraint");
            m_twistConstraint = serializedObject.FindProperty("m_twistConstraint");
            m_swingConstraint = serializedObject.FindProperty("m_swingConstraint");
            m_coneConstraint = serializedObject.FindProperty("m_coneConstraint");
            m_pyramidConstraint = serializedObject.FindProperty("m_pyramidConstraint");
            m_drive = serializedObject.FindProperty("m_drive");

            InitFields();
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            ActorBodiesUI();

            EditorGUILayout.Separator();

            EditorGUILayout.PropertyField(m_axes, false);
            if (m_axes.isExpanded)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(m_axes.GetArrayElementAtIndex(0), new GUIContent("Linear X"));
                EditorGUILayout.PropertyField(m_axes.GetArrayElementAtIndex(1), new GUIContent("Linear Y"));
                EditorGUILayout.PropertyField(m_axes.GetArrayElementAtIndex(2), new GUIContent("Linear Z"));
                EditorGUILayout.PropertyField(m_axes.GetArrayElementAtIndex(3), new GUIContent("Angular Twist (X)"));
                EditorGUILayout.PropertyField(m_axes.GetArrayElementAtIndex(4), new GUIContent("Angular Swing (Y)"));
                EditorGUILayout.PropertyField(m_axes.GetArrayElementAtIndex(5), new GUIContent("Angular Swing (Z)"));
                EditorGUI.indentLevel--;
            }

            EditorGUILayout.Separator();

            EditorGUILayout.PropertyField(m_linearConstraints, new GUIContent("Linear Constraints"), false);
            if (m_linearConstraints.isExpanded)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(m_linearConstraints.GetArrayElementAtIndex(0), new GUIContent("Linear X Constraint"), true);
                EditorGUILayout.PropertyField(m_linearConstraints.GetArrayElementAtIndex(1), new GUIContent("Linear Y Constraint"), true);
                EditorGUILayout.PropertyField(m_linearConstraints.GetArrayElementAtIndex(2), new GUIContent("Linear Z Constraint"), true);
                EditorGUI.indentLevel--;
            }

            EditorGUILayout.Separator();

            EditorGUILayout.PropertyField(m_distanceConstraint, new GUIContent("Distance (XYZ) Constraint"), true);

            EditorGUILayout.Separator();

            EditorGUILayout.PropertyField(m_twistConstraint, new GUIContent("Twist (X) Constraint"), true);

            EditorGUILayout.Separator();

            EditorGUILayout.PropertyField(m_swingConstraint);
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(m_coneConstraint, new GUIContent("Cone (YZ) Constraint"), true);
            EditorGUILayout.PropertyField(m_pyramidConstraint, new GUIContent("Pyramid (YZ) Constraint"), true);
            EditorGUI.indentLevel--;

            EditorGUILayout.Separator();

            EditorGUILayout.PropertyField(m_drive, false);
            if (m_drive.isExpanded)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(m_drive.GetArrayElementAtIndex(0), new GUIContent("Linear"), true);
                EditorGUILayout.PropertyField(m_drive.GetArrayElementAtIndex(1), new GUIContent("Angular"), true);
                EditorGUI.indentLevel--;
            }

            EditorGUILayout.Separator();

            JointOptionsUI();

            if (GUI.changed)
                serializedObject.ApplyModifiedProperties();
        }

        void OnSceneGUI()
        {
            var jointComponent = target as PXU.PxD6Joint;
            if (jointComponent)
            {
                var joint = jointComponent.apiD6Joint;
                if (joint != null)
                {
                    var body0 = joint.getActor0();
                    var body1 = joint.getActor1();
                    var anchor0 = body0 != null ? body0.getGlobalPose().transform(joint.getLocalPose(PxJointActorIndex.ACTOR0)) : joint.getLocalPose(PxJointActorIndex.ACTOR0);
                    var anchor1 = body1 != null ? body1.getGlobalPose().transform(joint.getLocalPose(PxJointActorIndex.ACTOR1)) : joint.getLocalPose(PxJointActorIndex.ACTOR1);

                    var axes = new[] { Vector3.right, Vector3.up, Vector3.forward };
                    var hands = new[] { Vector3.up, Vector3.right, Vector3.right };
                    for (int i = 0; i < 3; ++i)
                    {
                        var motion = joint.getMotion(PxD6Axis.X + i);
                        switch (motion)
                        {
                            case PxD6Motion.FREE:
                                DrawLinearAxisFree(anchor0, axes[i]);
                                break;
                            case PxD6Motion.LIMITED:
                                var limit = joint.getLinearLimit(PxD6Axis.X + i);
                                DrawLinearAxisLimited(anchor0, anchor1, axes[i], limit.lower, limit.upper);
                                break;
                        }
                    }
                    //for (int i = 0; i < 3; ++i)
                    //{
                    //    var motion = joint.getMotion(PxD6Axis.TWIST + i);
                    //    switch (motion)
                    //    {
                    //        case PxD6Motion.FREE:
                    //            DrawAngularAxisFree(anchor0, axes[i]);
                    //            break;
                    //        case PxD6Motion.LIMITED:
                    //            var limit = joint.getTwistLimit();
                    //            DrawAngularAxisLimited(anchor0, anchor1, axes[i], hands[i], limit.lower * Mathf.Rad2Deg, limit.upper * Mathf.Rad2Deg);
                    //            break;
                    //    }
                    //}
                }
            }
        }

        void DrawLinearAxisFree(PxTransform anchor, Vector3 axis)
        {
            Handles.color = new Color(axis.x, axis.y, axis.z);
            Handles.DrawLine(anchor.ToMatrix4x4().MultiplyPoint(axis * -0.5f), anchor.ToMatrix4x4().MultiplyPoint(axis * 0.5f));
        }

        void DrawLinearAxisLimited(PxTransform anchor, PxTransform parentAnchor, Vector3 axis, float lower, float upper)
        {
            var pp = parentAnchor.p.ToVector3();
            var ax = anchor.ToMatrix4x4().MultiplyVector(axis);
            var p0 = axis * lower;
            var p1 = axis * upper;
            var size = (Vector3.one - axis) * 0.1f;
            Handles.color = new Color(axis.x, axis.y, axis.z);
            Handles.matrix = anchor.ToMatrix4x4();
            Handles.DrawLine(p0, p1);
            Handles.DrawWireCube(p0, size);
            Handles.DrawWireCube(p1, size);
            Handles.matrix = parentAnchor.ToMatrix4x4();
            Handles.DrawWireCube(Vector3.zero, size);
        }

        void DrawAngularAxisFree(PxTransform anchor, Vector3 axis)
        {
            Handles.color = new Color(axis.x, axis.y, axis.z);
            Handles.matrix = anchor.ToMatrix4x4();
            Handles.DrawWireDisc(Vector3.zero, axis, 0.1f);
        }

        void DrawAngularAxisLimited(PxTransform anchor, PxTransform parentAnchor, Vector3 axis, Vector3 hand, float lower, float upper)
        {
            Handles.color = new Color(axis.x, axis.y, axis.z);
            Handles.matrix = anchor.ToMatrix4x4();
            Handles.DrawWireArc(Vector3.zero, axis, Quaternion.AngleAxis(-lower, axis) * hand, -(upper - lower), 0.1f);
            Handles.color = new Color(hand.x, hand.y, hand.z);
            Handles.matrix = parentAnchor.ToMatrix4x4();
            Handles.DrawLine(Vector3.zero, hand * 0.11f);
        }
    }
}