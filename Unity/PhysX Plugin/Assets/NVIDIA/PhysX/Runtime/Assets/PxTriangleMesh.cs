using NVIDIA.PhysX.UnityExtensions;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using PX = NVIDIA.PhysX;

namespace NVIDIA.PhysX.Unity
{
    public class PxTriangleMesh : PxAsset
    {
        #region Properties

        public override bool valid { get { return m_triangleMesh != null; } }

        public PX.PxTriangleMesh apiTriangleMesh { get { return m_triangleMesh; } }

        public int vertexCount { get { return valid ? (int)m_triangleMesh.getNbVertices() : 0; } }

        public Vector3[] vertices
        {
            get
            {
                if (!valid) return new Vector3[0];
                var verts = new Vector3[vertexCount];
                for (int i = 0; i < verts.Length; ++i)
                    verts[i] = m_triangleMesh.getVertex((uint)i).ToVector3();
                return verts;
            }
        }

        public int triangleCount { get { return valid ? (int)m_triangleMesh.getNbTriangles() : 0; } }

        public int[] triangles
        {
            get
            {
                if (!valid) return new int[0];
                var inds = new int[triangleCount * 3];
                for (int i = 0; i < inds.Length / 3; ++i)
                    for (int j = 0; j < 3; ++j)
                        inds[i * 3 + j] = m_triangleMesh.getTriangleVertex((uint)i, (uint)j);
                return inds;
            }
        }

        #endregion

        #region Methods

        public void BuildFromMesh(Mesh mesh)
        {
            var vertices = mesh.vertices;
            var triangleList = new List<int>();
            var materialList = new List<short>();
            for (int i = 0; i < mesh.subMeshCount; ++i)
            {
                if (mesh.GetTopology(i) != MeshTopology.Triangles) continue;
                triangleList.AddRange(mesh.GetTriangles(i));
                int count = (int)mesh.GetIndexCount(i) / 3;
                for (int j = 0; j < count; ++j) materialList.Add((short)i);
            }
            BuildFromMesh(vertices, triangleList.ToArray(), materialList.ToArray());
        }

        public void BuildFromMesh(Vector3[] vertices, int[] triangles, short[] materials)
        {
            var pinPoints = GCHandle.Alloc(vertices, GCHandleType.Pinned);
            var pinTriangles = GCHandle.Alloc(triangles, GCHandleType.Pinned);
            var pinMaterials = GCHandle.Alloc(materials, GCHandleType.Pinned);
            var desc = new PxTriangleMeshDesc();
            desc.points.count = (uint)vertices.Length;
            desc.points.stride = sizeof(float) * 3;
            desc.points.data = Marshal.UnsafeAddrOfPinnedArrayElement(vertices, 0);
            desc.triangles.count = (uint)triangles.Length / 3;
            desc.triangles.stride = sizeof(int) * 3;
            desc.triangles.data = Marshal.UnsafeAddrOfPinnedArrayElement(triangles, 0);
            desc.materialIndices.stride = sizeof(short);
            desc.materialIndices.data = Marshal.UnsafeAddrOfPinnedArrayElement(materials, 0);
            var outputStream = new PxDefaultMemoryOutputStream();
            if (PxPhysics.cooking.cookTriangleMesh(desc, outputStream))
            {
                m_triangleMeshData = new byte[outputStream.getSize()];
                Marshal.Copy(outputStream.getData(), m_triangleMeshData, 0, m_triangleMeshData.Length);
                Recreate();
            }
            else
            {
                Debug.LogError("PhysX Cooking: Failed to create triangle mesh.");
            }
            outputStream.destroy();
            pinPoints.Free();
            pinTriangles.Free();
            pinMaterials.Free();
        }

        #endregion

        #region Protected

        protected override void CreateAsset()
        {
            CreateTriangleMesh();
        }

        protected override void DestroyAsset()
        {
            DestroyTriangleMesh();
        }

        protected override void ValidateAsset()
        {
            Recreate();
        }

        #endregion

        #region Private

        void CreateTriangleMesh()
        {
            if (m_triangleMeshData.Length > 0)
            {
                var pinData = GCHandle.Alloc(m_triangleMeshData, GCHandleType.Pinned);
                var inputData = new PxDefaultMemoryInputData(Marshal.UnsafeAddrOfPinnedArrayElement(m_triangleMeshData, 0), (uint)m_triangleMeshData.Length);
                m_triangleMesh = PxPhysics.apiPhysics.createTriangleMesh(inputData);
                inputData.destroy();
                pinData.Free();
                ApplyProperties();
            }

            //Debug.Log("PxConvexMesh '" + name + "' created");
        }

        void DestroyTriangleMesh()
        {
            m_triangleMesh?.release();
            m_triangleMesh = null;

            //Debug.Log("PxConvexMesh '" + name + "' destroyed");
        }

        protected override void ApplyProperties()
        {
            base.ApplyProperties();

            if (valid)
            {
            }
        }

        [NonSerialized]
        PX.PxTriangleMesh m_triangleMesh;

        [SerializeField]
        byte[] m_triangleMeshData = new byte[0];

        #endregion
    }
}
