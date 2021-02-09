// This code contains NVIDIA Confidential Information and is disclosed to you
// under a form of NVIDIA software license agreement provided separately to you.
//
// Notice
// NVIDIA Corporation and its licensors retain all intellectual property and
// proprietary rights in and to this software and related documentation and
// any modifications thereto. Any use, reproduction, disclosure, or
// distribution of this software and related documentation without an express
// license agreement from NVIDIA Corporation is strictly prohibited.
//
// ALL NVIDIA DESIGN SPECIFICATIONS, CODE ARE PROVIDED "AS IS.". NVIDIA MAKES
// NO WARRANTIES, EXPRESSED, IMPLIED, STATUTORY, OR OTHERWISE WITH RESPECT TO
// THE MATERIALS, AND EXPRESSLY DISCLAIMS ALL IMPLIED WARRANTIES OF NONINFRINGEMENT,
// MERCHANTABILITY, AND FITNESS FOR A PARTICULAR PURPOSE.
//
// Information and code furnished is believed to be accurate and reliable.
// However, NVIDIA Corporation assumes no responsibility for the consequences of use of such
// information or for any infringement of patents or other rights of third parties that may
// result from its use. No license is granted by implication or otherwise under any patent
// or patent rights of NVIDIA Corporation. Details are subject to change without notice.
// This code supersedes and replaces all information previously supplied.
// NVIDIA Corporation products are not authorized for use as critical
// components in life support devices or systems without express written approval of
// NVIDIA Corporation.
//
// Copyright (c) 2019 NVIDIA Corporation. All rights reserved.

using NVIDIA.PhysX.Unity;
using NVIDIA.PhysX.UnityExtensions;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using PX = NVIDIA.PhysX;
using PXU = NVIDIA.PhysX.Unity;

namespace NVIDIA.PhysX.UnityEditor
{
    public class PxActorEditor : PxComponentEditor
    {
        SerializedProperty m_sceneOverride;
        SerializedProperty m_collisionLayer;
        SerializedProperty m_collisionEvents;

        protected override void InitProperties()
        {
            base.InitProperties();
            m_sceneOverride = serializedObject.FindProperty("m_sceneOverride");
            m_collisionLayer = serializedObject.FindProperty("m_collisionLayer");
            m_collisionEvents = serializedObject.FindProperty("m_collisionEvents");
        }

        protected void SceneOverrideUI()
        {
            BeginRecreateCheck();
            EditorGUILayout.PropertyField(m_sceneOverride);
            EndRecreateCheck();
        }

        protected void CollisionLayerUI()
        {
            string[] groupLayers = CollisionLayerNames();
            if (groupLayers.Length > 0)
            {
                List<GUIContent> layerNames = new List<GUIContent>();
                List<int> layerIndices = new List<int>();
                for (int i = 0; i < groupLayers.Length; ++i)
                {
                    string layerName = groupLayers[i];
                    if (layerName != string.Empty)
                    {
                        layerNames.Add(new GUIContent("" + i + ". " + layerName));
                        layerIndices.Add(i);
                    }
                }
                EditorGUILayout.IntPopup(m_collisionLayer, layerNames.ToArray(), layerIndices.ToArray());
            }
            else
            {
                GUI.enabled = false;
                EditorGUILayout.PropertyField(m_collisionLayer);
                GUI.enabled = true;
            }
            EditorGUILayout.PropertyField(m_collisionEvents);
        }
        string[] CollisionLayerNames()
        {
            var actor = target as PXU.PxActor;
            if (actor)
            {
                var scene = actor.sceneOverride;
                if (!scene)
                    scene = GameObject.FindGameObjectWithTag("MainPxScene")?.GetComponent<PXU.PxScene>();

                if (scene)
                {
                    return scene.collisionLayerNames;
                }
            }
            return new string[0];
        }

        protected void DrawBodies(PXU.PxActor actor)
        {
            var bodies = actor.apiActors;
            foreach (var body in bodies)
            {
                var rb = body as PxRigidBody;
                if (rb != null)
                {
                    Handles.color = Color.magenta * Color.gray * 0.7f;
                    var massPose = rb.getGlobalPose().transform(rb.getCMassLocalPose());
                    Handles.matrix = Matrix4x4.TRS(massPose.p.ToVector3(), massPose.q.ToQuaternion(), InertiaToEllipsoid(rb.getMassSpaceInertiaTensor().ToVector3() / rb.getMass()));
                    Handles.DrawWireDisc(Vector3.zero, Vector3.up, 1.0f);
                    Handles.DrawWireDisc(Vector3.zero, Vector3.right, 1.0f);
                    Handles.DrawWireDisc(Vector3.zero, Vector3.forward, 1.0f);
                }
                var ra = body as PxRigidActor;
                if (ra != null)
                {
                    Handles.color = Color.green * Color.gray;
                    var raPose = ra.getGlobalPose();
                    uint shapeCount = ra.getNbShapes();
                    for (uint i = 0; i < shapeCount; ++i)
                    {
                        var shape = ra.getShape(i);
                        var shapePose = raPose.transform(shape.getLocalPose());
                        if (shape.getGeometryType() == PxGeometryType.SPHERE)
                            DrawSphereShape(shapePose, shape.getGeometry().sphere().radius);
                        else if (shape.getGeometryType() == PxGeometryType.CAPSULE)
                            DrawCapsuleShape(shapePose, shape.getGeometry().capsule().radius, shape.getGeometry().capsule().halfHeight);
                        else if (shape.getGeometryType() == PxGeometryType.BOX)
                            DrawBoxShape(shapePose, shape.getGeometry().box().halfExtents.ToVector3());
                        else if (shape.getGeometryType() == PxGeometryType.CONVEXMESH)
                            DrawConvexMeshShape(shapePose, shape.getGeometry().convexMesh());
                        else if (shape.getGeometryType() == PxGeometryType.PLANE)
                            DrawPlaneShape(shapePose, shape.getGeometry().plane());
                    }
                }
            }
        }

        static Vector3 InertiaToEllipsoid(Vector3 inertia)
        {
            Vector3 ellipsoid = Vector3.zero;
            ellipsoid.x = Mathf.Sqrt(2.5f * (inertia.y + inertia.z - inertia.x));
            ellipsoid.y = Mathf.Sqrt(2.5f * (inertia.x + inertia.z - inertia.y));
            ellipsoid.z = Mathf.Sqrt(2.5f * (inertia.x + inertia.y - inertia.z));
            return ellipsoid;
        }

        static void DrawSphereShape(PxTransform pose, float radius)
        {
            Handles.matrix = Matrix4x4.TRS(pose.p.ToVector3(), pose.q.ToQuaternion(), Vector3.one);
            Handles.DrawWireDisc(Vector3.zero, Vector3.up, radius);
            Handles.DrawWireDisc(Vector3.zero, Vector3.right, radius);
            Handles.DrawWireDisc(Vector3.zero, Vector3.forward, radius);
        }

        static void DrawCapsuleShape(PxTransform pose, float radius, float halfHeight)
        {
            Handles.matrix = Matrix4x4.TRS(pose.p.ToVector3(), pose.q.ToQuaternion(), Vector3.one);
            Handles.DrawWireArc(Vector3.right * halfHeight, Vector3.up, Vector3.forward, 180.0f, radius);
            Handles.DrawWireArc(Vector3.right * halfHeight, Vector3.back, Vector3.up, 180.0f, radius);
            Handles.DrawWireDisc(Vector3.right * halfHeight, Vector3.right, radius);
            Handles.DrawLines(new[] { Vector3.up * radius + Vector3.right * halfHeight, Vector3.up * radius + Vector3.left * halfHeight,
                                      Vector3.down * radius + Vector3.right * halfHeight, Vector3.down * radius + Vector3.left * halfHeight,
                                      Vector3.forward * radius + Vector3.right * halfHeight, Vector3.forward * radius + Vector3.left * halfHeight,
                                      Vector3.back * radius + Vector3.right * halfHeight, Vector3.back * radius + Vector3.left * halfHeight });
            Handles.DrawWireDisc(Vector3.left * halfHeight, Vector3.right, radius);
            Handles.DrawWireArc(Vector3.left * halfHeight, Vector3.down, Vector3.forward, 180.0f, radius);
            Handles.DrawWireArc(Vector3.left * halfHeight, Vector3.forward, Vector3.up, 180.0f, radius);
        }

        static void DrawBoxShape(PxTransform pose, Vector3 halfExtents)
        {
            Handles.matrix = Matrix4x4.TRS(pose.p.ToVector3(), pose.q.ToQuaternion(), Vector3.one);
            Handles.DrawWireCube(Vector3.zero, halfExtents * 2.0f);
        }

        static void DrawConvexMeshShape(PxTransform pose, PxConvexMeshGeometry geometry)
        {
            Handles.matrix = Matrix4x4.TRS(pose.p.ToVector3(), pose.q.ToQuaternion(), Vector3.one) *
                             Matrix4x4.Rotate(Quaternion.Inverse(geometry.scale.rotation.ToQuaternion())) *
                             Matrix4x4.Scale(geometry.scale.scale.ToVector3()) *
                             Matrix4x4.Rotate(geometry.scale.rotation.ToQuaternion());
            var convexMesh = geometry.convexMesh;
            if (convexMesh != null)
            {
                uint vertexCount = convexMesh.getNbVertices();
                var vertices = new Vector3[vertexCount];
                for (uint i = 0; i < vertexCount; ++i)
                {
                    vertices[i] = convexMesh.getVertex(i).ToVector3();
                }
                List<int> edges = new List<int>();
                uint polyCount = convexMesh.getNbPolygons();
                for (uint i = 0; i < polyCount; ++i)
                {
                    PxHullPolygon p;
                    if (convexMesh.getPolygonData(i, out p))
                    {
                        int index0 = convexMesh.getIndex((uint)p.mIndexBase);
                        for (uint j = 1; j < p.mNbVerts; ++j)
                        {
                            int index1 = convexMesh.getIndex((uint)(p.mIndexBase + j));
                            edges.AddRange(new[] { index0, index1 });
                            index0 = index1;
                        }
                    }
                }
                Handles.DrawLines(vertices, edges.ToArray());
            }
        }

        static void DrawPlaneShape(PxTransform pose, PxPlaneGeometry geometry)
        {
            Handles.matrix = Matrix4x4.TRS(pose.p.ToVector3(), pose.q.ToQuaternion(), Vector3.one);
            Handles.DrawLines(new[] { new Vector3(0, 1, 0.5f), new Vector3(0, -1, 0.5f),
                                      new Vector3(0, 1, -0.5f), new Vector3(0, -1, -0.5f),
                                      new Vector3(0, 0.5f, 1), new Vector3(0 , 0.5f, -1),
                                      new Vector3(0, -0.5f, 1), new Vector3(0, -0.5f, -1),
                                      new Vector3(0, 0.5f, 0.5f), new Vector3(-0.5f, 0.5f, 0.5f),
                                      new Vector3(0, -0.5f, 0.5f), new Vector3(-0.5f, -0.5f, 0.5f),
                                      new Vector3(0, -0.5f, -0.5f), new Vector3(-0.5f, -0.5f, -0.5f),
                                      new Vector3(0, 0.5f, -0.5f), new Vector3(-0.5f, 0.5f, -0.5f) });
        }
    }
}