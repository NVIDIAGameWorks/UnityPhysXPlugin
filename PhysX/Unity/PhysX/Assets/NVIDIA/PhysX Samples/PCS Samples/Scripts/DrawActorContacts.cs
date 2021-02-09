using UnityEngine;
using PXU = NVIDIA.PhysX.Unity;
using PX = NVIDIA.PhysX;
using NVIDIA.PhysX.UnityExtensions;
using System.Collections.Generic;
using System.Linq;
using System;

[RequireComponent(typeof(PXU.PxActor))]
public class DrawActorContacts : MonoBehaviour
{
    void Start()
    {
        m_actor = GetComponent<PXU.PxActor>();
        m_actor.onContact += OnContact;

        m_scene = PXU.PxScene.main;
        m_scene.onAfterSimulation += OnAfterSimulation;

        Camera.onPreCull += OnRender;

        m_contactMesh = new Mesh();
        m_contactMesh.vertices = m_vertices;
        m_contactMesh.SetIndices(Enumerable.Range(0, MAX_CONTACTS * 2).ToArray(), MeshTopology.Lines, 0);

        m_contactMaterial = new Material(Resources.Load<Material>("UnlitColor"));
        m_contactMaterial.color = Color.red;
    }

    void OnDisable()
    {
        Destroy(m_contactMaterial);
        Destroy(m_contactMesh);

        Camera.onPreCull -= OnRender;

        m_actor.onContact -= OnContact;

        m_scene.onAfterSimulation -= OnAfterSimulation;
    }

    void OnContact(PXU.PxActor thisActor, PXU.PxActor otherActor, PX.PxContactPairHeader pairHeader, PX.PxContactPairList pairs)
    {
        for (uint i = 0; i < pairs.count; ++i)
        {
            var pair = pairs.get(i);
            uint contactCount = pair.contactCount;
            pair.extractContacts(m_contactBuffer, contactCount);
            for (uint j = 0; j < contactCount && m_vertexCount < MAX_CONTACTS * 2; ++j)
            {
                var point = m_contactBuffer[j];
                m_vertices[m_vertexCount++] = point.position.ToVector3();
                m_vertices[m_vertexCount++] = (point.position + point.impulse * 0.01f).ToVector3();
            }
        }
    }

    void OnAfterSimulation()
    {
        m_contactMesh.vertices = m_vertices.ToArray();
        m_contactMesh.RecalculateBounds();
        Array.Clear(m_vertices, 0, MAX_CONTACTS * 2);
        m_vertexCount = 0;
    }

    void OnRender(Camera camera)
    {
        if (camera.cameraType != CameraType.Preview)
        {
            Graphics.DrawMesh(m_contactMesh, Matrix4x4.identity, m_contactMaterial, 0, camera);
        }
    }

    const int MAX_CONTACTS = 10;
    PX.PxContactPairPoint[] m_contactBuffer = new PX.PxContactPairPoint[MAX_CONTACTS];
    Vector3[] m_vertices = new Vector3[MAX_CONTACTS * 2];
    int m_vertexCount = 0;
    Mesh m_contactMesh;
    Material m_contactMaterial;
    PXU.PxActor m_actor;
    PXU.PxScene m_scene;
}
