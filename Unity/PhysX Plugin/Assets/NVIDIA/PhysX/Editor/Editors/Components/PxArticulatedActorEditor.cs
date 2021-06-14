using NVIDIA.PhysX.Unity;
using NVIDIA.PhysX.UnityExtensions;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using PXU = NVIDIA.PhysX.Unity;

namespace NVIDIA.PhysX.UnityEditor
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(PxArticulatedActor))]
    public class PxArticulatedActorEditor : PxActorEditor
    {
        SerializedProperty m_sceneOverride;
        //SerializedProperty m_collisionShape;
        //SerializedProperty m_autoMass;
        //SerializedProperty m_mass;
        //SerializedProperty m_massPosition;
        //SerializedProperty m_massRotation;
        //SerializedProperty m_localInertia;
        //SerializedProperty m_visualization;
        SerializedProperty m_velocity;
        //SerializedProperty m_disableGravity;
        //SerializedProperty m_disableSimulation;
        //SerializedProperty m_collisionEvents;
        SerializedProperty m_fixedRoot;
        SerializedProperty m_positionIterations;
        SerializedProperty m_velocityIterations;
        //SerializedProperty m_childLinks;
        SerializedProperty m_linkInfos;
        SerializedProperty m_jointPosition;
        SerializedProperty m_jointVelocity;
        SerializedProperty m_collisionLayers;
        SerializedProperty m_collisionMatrix;

        Vector2 m_listScroll = Vector2.zero;
        int m_listIndex = 0;

        GUIStyle sm_listSelection = null;

        void OnEnable()
        {
            m_sceneOverride = serializedObject.FindProperty("m_sceneOverride");
            //m_collisionShape = serializedObject.FindProperty("m_collisionShape");
            //m_autoMass = serializedObject.FindProperty("m_autoMass");
            //m_mass = serializedObject.FindProperty("m_mass");
            //m_massPosition = serializedObject.FindProperty("m_massPosition");
            //m_massRotation = serializedObject.FindProperty("m_massRotation");
            //m_localInertia = serializedObject.FindProperty("m_localInertia");
            //m_visualization = serializedObject.FindProperty("m_visualization");
            m_velocity = serializedObject.FindProperty("m_velocity");
            //m_disableGravity = serializedObject.FindProperty("m_disableGravity");
            //m_disableSimulation = serializedObject.FindProperty("m_disableSimulation");
            //m_collisionEvents = serializedObject.FindProperty("m_collisionEvents");
            m_fixedRoot = serializedObject.FindProperty("m_fixedRoot");
            m_positionIterations = serializedObject.FindProperty("m_positionIterations");
            m_velocityIterations = serializedObject.FindProperty("m_velocityIterations");
            //m_childLinks = serializedObject.FindProperty("m_childLinks");
            m_linkInfos = serializedObject.FindProperty("m_linkInfos");
            m_jointPosition = serializedObject.FindProperty("m_jointPosition");
            m_jointVelocity = serializedObject.FindProperty("m_jointVelocity");
            m_collisionLayers = serializedObject.FindProperty("m_collisionLayers");
            m_collisionMatrix = serializedObject.FindProperty("m_collisionMatrix");

            InitProperties();
        }

        public override void OnInspectorGUI()
        {
            if (sm_listSelection == null)
            {
                sm_listSelection = new GUIStyle(GUI.skin.label);
                var tex1x1 = new Texture2D(1, 1);
                tex1x1.SetPixel(0, 0, GUI.skin.settings.selectionColor);
                tex1x1.Apply();
                sm_listSelection.normal.background = tex1x1;
            }

            serializedObject.Update();

            SceneOverrideUI();

            EditorGUILayout.Separator();

            CollisionLayerUI();

            //EditorGUILayout.Separator();

            //EditorGUILayout.PropertyField(m_collisionEvents);

            EditorGUILayout.Separator();

            EditorGUILayout.LabelField("Solver Iterations");
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(m_positionIterations, new GUIContent("Position"));
            EditorGUILayout.PropertyField(m_velocityIterations, new GUIContent("Velocity"));
            EditorGUI.indentLevel--;

            EditorGUILayout.Separator();

            var rootInfo = m_linkInfos.GetArrayElementAtIndex(0);

            EditorGUILayout.PropertyField(rootInfo, new GUIContent("Root Link"), false);
            if (rootInfo.isExpanded)
            {
                BeginRecreateCheck();
                EditorGUI.indentLevel++;

                EditorGUILayout.PropertyField(rootInfo.FindPropertyRelative("collisionShape"), false);

                //EditorGUILayout.Separator();

                var massInfo = rootInfo.FindPropertyRelative("massInfo");
                EditorGUILayout.PropertyField(massInfo, new GUIContent("Mass"), false);
                if (massInfo.isExpanded)
                {
                    EditorGUI.indentLevel++;
                    var autoMass = massInfo.FindPropertyRelative("autoMass");
                    EditorGUILayout.PropertyField(autoMass);
                    GUI.enabled = !autoMass.boolValue && !autoMass.hasMultipleDifferentValues;
                    EditorGUI.indentLevel++;
                    EditorGUILayout.PropertyField(massInfo.FindPropertyRelative("mass"));
                    EditorGUILayout.PropertyField(massInfo.FindPropertyRelative("position"));
                    EditorGUILayout.PropertyField(massInfo.FindPropertyRelative("rotation"));
                    EditorGUILayout.PropertyField(massInfo.FindPropertyRelative("inertia"));
                    EditorGUI.indentLevel--;
                    EditorGUI.indentLevel--;
                    GUI.enabled = true;
                }

                var jointInfo = rootInfo.FindPropertyRelative("jointInfo");
                EditorGUILayout.PropertyField(jointInfo, new GUIContent("Joint"), false);
                if (jointInfo.isExpanded)
                {
                    EditorGUI.indentLevel++;
                    EditorGUILayout.PropertyField(jointInfo.FindPropertyRelative("position"), new GUIContent("Position"));
                    EditorGUILayout.PropertyField(jointInfo.FindPropertyRelative("rotation"), new GUIContent("Rotation"));
                    EditorGUI.indentLevel--;
                    EditorGUILayout.PropertyField(jointInfo.FindPropertyRelative("type"));
                    var axisInfos = jointInfo.FindPropertyRelative("axisInfos");
                    EditorGUILayout.PropertyField(axisInfos, new GUIContent("Axes"), false);
                    if (axisInfos.isExpanded)
                    {
                        EditorGUI.indentLevel++;
                        Func<int, string, int> drawAxis = (axisIndex, axisName) =>
                        {
                            var axisInfo = axisInfos.GetArrayElementAtIndex(axisIndex);
                            EditorGUILayout.PropertyField(axisInfo, new GUIContent(axisName), false);
                            if (axisInfo.isExpanded)
                            {
                                EditorGUI.indentLevel++;
                                EditorGUILayout.PropertyField(axisInfo.FindPropertyRelative("motion"));
                                EditorGUILayout.PropertyField(axisInfo.FindPropertyRelative("limits"), true);
                                EditorGUILayout.PropertyField(axisInfo.FindPropertyRelative("drive"), true);
                                EditorGUI.indentLevel--;
                            }
                            return 0;
                        };
                        drawAxis((int)PxArticulationAxis.X, "Linear X");
                        drawAxis((int)PxArticulationAxis.Y, "Linear Y");
                        drawAxis((int)PxArticulationAxis.Z, "Linear Z");
                        drawAxis((int)PxArticulationAxis.TWIST, "Angular X");
                        drawAxis((int)PxArticulationAxis.SWING1, "Angular Y");
                        drawAxis((int)PxArticulationAxis.SWING2, "Angular Z");
                        EditorGUI.indentLevel--;
                    }
                    EditorGUILayout.PropertyField(jointInfo.FindPropertyRelative("friction"));
                    EditorGUILayout.PropertyField(jointInfo.FindPropertyRelative("maxVelocity"));
                    EditorGUI.indentLevel--;
                }

                //EditorGUILayout.Separator();

                EditorGUILayout.PropertyField(m_velocity, true);

                //EditorGUILayout.Separator();

                //EditorGUILayout.PropertyField(m_visualization);
                //EditorGUILayout.PropertyField(m_disableGravity);
                //EditorGUILayout.PropertyField(m_disableSimulation);
                EditorGUILayout.PropertyField(m_fixedRoot, new GUIContent("Fixed"));

                EditorGUI.indentLevel--;
                EndRecreateCheck();
            }

            EditorGUILayout.Separator();

            EditorGUILayout.PropertyField(m_linkInfos, new GUIContent("Child Links"), false);
            if (m_linkInfos.isExpanded)
            {
                EditorGUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();

                if (GUILayout.Button("Populate"))
                {
                    Undo.RecordObject(target, "Populate Articulation From Children");
                    (target as PxArticulatedActor).PopulateWithChildren();
                }

                if (GUILayout.Button("Add"))
                {
                    Undo.RecordObject(target, "Add Child Link To Articulation");
                    (target as PxArticulatedActor).AddLink(m_listIndex);
                    m_listIndex = (target as PxArticulatedActor).linkCount - 1;
                }

                if (GUILayout.Button("Move Up"))
                {
                    Undo.RecordObject(target, "Move Child Link Up In Hierarchy");
                    (target as PxArticulatedActor).MoveLinkUp(m_listIndex);
                }

                if (GUILayout.Button("Remove"))
                {
                    Undo.RecordObject(target, "Remove Child Link From Articulation");
                    (target as PxArticulatedActor).RemoveLink(m_listIndex);
                    m_listIndex = 0;
                }

                if (GUILayout.Button("Clear"))
                {
                    Undo.RecordObject(target, "Remove All Child Links From Articulation");
                    (target as PxArticulatedActor).ClearLinks();
                    m_listIndex = 0;
                }

                EditorGUILayout.EndHorizontal();
                m_listScroll = EditorGUILayout.BeginScrollView(m_listScroll, EditorStyles.textArea, GUILayout.Height(150));
                Func<int, int> drawListItem = null;
                drawListItem = linkIndex =>
                {
                    EditorGUI.indentLevel++;
                    var linkInfo = m_linkInfos.GetArrayElementAtIndex(linkIndex);
                    var rect = EditorGUILayout.GetControlRect();
                    if (GUI.Button(rect, "", m_listIndex == linkIndex ? sm_listSelection : EditorStyles.label)) m_listIndex = m_listIndex == linkIndex ? 0 : linkIndex;
                    string linkName = "Link " + linkIndex;
                    var linkTransform = linkInfo.FindPropertyRelative("transform").objectReferenceValue as Transform;
                    if (linkTransform) linkName += " (" + linkTransform.name + ")";
                    EditorGUI.LabelField(rect, linkName, m_listIndex == linkIndex ? EditorStyles.whiteLabel : EditorStyles.label);
                    var childLinks = linkInfo.FindPropertyRelative("childLinks");
                    for (int i = 0; i < childLinks.arraySize; ++i)
                        drawListItem(childLinks.GetArrayElementAtIndex(i).intValue);
                    EditorGUI.indentLevel--;
                    return 0;
                };
                var rootChildLinks = rootInfo.FindPropertyRelative("childLinks");
                for (int i = 0; i < rootChildLinks.arraySize; ++i)
                    drawListItem(rootChildLinks.GetArrayElementAtIndex(i).intValue);
                EditorGUILayout.EndScrollView();
                if (m_listIndex > 0 && m_listIndex < m_linkInfos.arraySize)
                {
                    BeginRecreateCheck();
                    EditorGUI.indentLevel++;
                    var linkInfo = m_linkInfos.GetArrayElementAtIndex(m_listIndex);
                    EditorGUILayout.PropertyField(linkInfo.FindPropertyRelative("transform"));
                    EditorGUILayout.PropertyField(linkInfo.FindPropertyRelative("collisionShape"));
                    var massInfo = linkInfo.FindPropertyRelative("massInfo");
                    EditorGUILayout.PropertyField(massInfo, new GUIContent("Mass"), false);
                    if (massInfo.isExpanded)
                    {
                        EditorGUI.indentLevel++;
                        var autoMass = massInfo.FindPropertyRelative("autoMass");
                        EditorGUILayout.PropertyField(autoMass);
                        GUI.enabled = !autoMass.boolValue && !autoMass.hasMultipleDifferentValues;
                        EditorGUI.indentLevel++;
                        EditorGUILayout.PropertyField(massInfo.FindPropertyRelative("mass"));
                        EditorGUILayout.PropertyField(massInfo.FindPropertyRelative("position"));
                        EditorGUILayout.PropertyField(massInfo.FindPropertyRelative("rotation"));
                        EditorGUILayout.PropertyField(massInfo.FindPropertyRelative("inertia"));
                        EditorGUI.indentLevel--;
                        EditorGUI.indentLevel--;
                        GUI.enabled = true;
                    }
                    var jointInfo = linkInfo.FindPropertyRelative("jointInfo");
                    EditorGUILayout.PropertyField(jointInfo, new GUIContent("Joint"), false);
                    if (jointInfo.isExpanded)
                    {
                        EditorGUI.indentLevel++;
                        EditorGUILayout.PropertyField(jointInfo.FindPropertyRelative("position"), new GUIContent("Position"));
                        EditorGUILayout.PropertyField(jointInfo.FindPropertyRelative("rotation"), new GUIContent("Rotation"));
                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField("On Parent");
                        GUILayout.FlexibleSpace();
                        if (GUILayout.Button("Compute"))
                        {
                            Undo.RecordObject(target, "Compute Anchor On Parent");
                            (target as PxArticulatedActor).SetLinkJointPoses(m_listIndex, jointInfo.FindPropertyRelative("position").vector3Value, Quaternion.Euler(jointInfo.FindPropertyRelative("rotation").vector3Value), true);
                            serializedObject.Update();
                        }
                        EditorGUILayout.EndHorizontal();
                        EditorGUI.indentLevel++;
                        EditorGUILayout.PropertyField(jointInfo.FindPropertyRelative("parentPosition"), new GUIContent("Position"));
                        EditorGUILayout.PropertyField(jointInfo.FindPropertyRelative("parentRotation"), new GUIContent("Rotation"));
                        EditorGUI.indentLevel--;
                        EditorGUILayout.PropertyField(jointInfo.FindPropertyRelative("type"));
                        var axisInfos = jointInfo.FindPropertyRelative("axisInfos");
                        EditorGUILayout.PropertyField(axisInfos, new GUIContent("Axes"), false);
                        if (axisInfos.isExpanded)
                        {
                            EditorGUI.indentLevel++;
                            Func<int, string, int> drawAxis = (axisIndex, axisName) =>
                            {
                                var axisInfo = axisInfos.GetArrayElementAtIndex(axisIndex);
                                EditorGUILayout.PropertyField(axisInfo, new GUIContent(axisName), false);
                                if (axisInfo.isExpanded)
                                {
                                    EditorGUI.indentLevel++;
                                    EditorGUILayout.PropertyField(axisInfo.FindPropertyRelative("motion"));
                                    EditorGUILayout.PropertyField(axisInfo.FindPropertyRelative("limits"), true);
                                    EditorGUILayout.PropertyField(axisInfo.FindPropertyRelative("drive"), true);
                                    //int dofIndex = axisInfo.FindPropertyRelative("dofIndex").intValue;
                                    //if (dofIndex > -1 && dofIndex < m_jointPosition.arraySize)
                                    //{
                                    //    EditorGUILayout.PropertyField(m_jointPosition.GetArrayElementAtIndex(dofIndex), new GUIContent("Position"));
                                    //    EditorGUILayout.PropertyField(m_jointVelocity.GetArrayElementAtIndex(dofIndex), new GUIContent("Velocity"));
                                    //}
                                    //else
                                    //{
                                    //    GUI.enabled = false;
                                    //    EditorGUILayout.TextField("Position", "");
                                    //    EditorGUILayout.TextField("Velocity", "");
                                    //    GUI.enabled = true;
                                    //}
                                    EditorGUI.indentLevel--;
                                }
                                return 0;
                            };
                            drawAxis((int)PxArticulationAxis.X, "Linear X");
                            drawAxis((int)PxArticulationAxis.Y, "Linear Y");
                            drawAxis((int)PxArticulationAxis.Z, "Linear Z");
                            drawAxis((int)PxArticulationAxis.TWIST, "Angular X");
                            drawAxis((int)PxArticulationAxis.SWING1, "Angular Y");
                            drawAxis((int)PxArticulationAxis.SWING2, "Angular Z");
                            EditorGUI.indentLevel--;
                        }
                        EditorGUILayout.PropertyField(jointInfo.FindPropertyRelative("friction"));
                        EditorGUILayout.PropertyField(jointInfo.FindPropertyRelative("maxVelocity"));
                        EditorGUI.indentLevel--;
                    }
                    EditorGUI.indentLevel--;
                    EndRecreateCheck();
                }
            }

            EditorGUILayout.Separator();

            CollisionLayersUI();

            if (GUI.changed)
                serializedObject.ApplyModifiedProperties();
        }

        void CollisionLayersUI()
        {
            int count = m_collisionLayers.arraySize;
            EditorGUILayout.PropertyField(m_collisionLayers, false);
            if (m_collisionLayers.isExpanded)
            {
                EditorGUI.indentLevel++;
                for (int i = 0; i < count; ++i)
                {
                    GUI.enabled = i > 0;
                    EditorGUILayout.PropertyField(m_collisionLayers.GetArrayElementAtIndex(i), new GUIContent("Layer " + i/*.ToString("X")*/));
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
        }

        void OnSceneGUI()
        {
            var actor = target as PXU.PxActor;
            if (actor)
            {
                DrawBodies(actor);
                Handles.matrix = Matrix4x4.identity;
                var bodies = actor.apiActors;
                foreach (var body in bodies)
                {
                    var link = body as PxArticulationLink;
                    if (link != null)
                    {
                        var linkPose = link.getGlobalPose();
                        var joint = link.getInboundJointReducedCoordinate();
                        if (joint != null)
                        {
                            var parentLink = joint.getParentArticulationLink();
                            var parentPose = parentLink != null ? parentLink.getGlobalPose() : PxTransform.identity;
                            var linkAnchor = linkPose.transform(joint.getChildPose());
                            var parentAnchor = parentPose.transform(joint.getParentPose());

                            var axes = new[] { Vector3.right, Vector3.up, Vector3.forward };
                            var hands = new[] { Vector3.up, Vector3.right, Vector3.right };
                            for (int i = 0; i < 3; ++i)
                            {
                                var motion = joint.getMotion(PxArticulationAxis.X + i);
                                switch (motion)
                                {
                                    case PxArticulationMotion.FREE:
                                        DrawLinearAxisFree(linkAnchor, axes[i]);
                                        break;
                                    case PxArticulationMotion.LIMITED:
                                        float lower, upper; joint.getLimit(PxArticulationAxis.X + i, out lower, out upper);
                                        DrawLinearAxisLimited(linkAnchor, parentAnchor, axes[i], lower, upper);
                                        break;
                                }
                            }
                            for (int i = 0; i < 3; ++i)
                            {
                                var motion = joint.getMotion(PxArticulationAxis.TWIST + i);
                                switch (motion)
                                {
                                    case PxArticulationMotion.FREE:
                                        DrawAngularAxisFree(linkAnchor, axes[i]);
                                        break;
                                    case PxArticulationMotion.LIMITED:
                                        float lower, upper; joint.getLimit(PxArticulationAxis.TWIST + i, out lower, out upper);
                                        DrawAngularAxisLimited(linkAnchor, parentAnchor, axes[i], hands[i], lower * Mathf.Rad2Deg, upper * Mathf.Rad2Deg);
                                        break;
                                }
                            }
                        }
                    }
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