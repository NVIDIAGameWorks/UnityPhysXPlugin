using NVIDIA.PhysX.UnityExtensions;
using System;
using System.Runtime.InteropServices;
using UnityEngine;
using PX = NVIDIA.PhysX;

namespace NVIDIA.PhysX.Unity
{
    public class PxConvexMesh : PxAsset
    {
        #region Properties

        public override bool valid { get { return m_convexMesh != null; } }

        public PX.PxConvexMesh apiConvexMesh { get { return m_convexMesh; } }

        public int vertexCount { get { return valid ? (int)m_convexMesh.getNbVertices() : 0; } }

        public Vector3[] vertices
        {
            get
            {
                if (!valid) return new Vector3[0];
                var verts = new Vector3[vertexCount];
                for (int i = 0; i < verts.Length; ++i) verts[i] = m_convexMesh.getVertex((uint)i).ToVector3();
                return verts;
            }
        }

        public int polygonCount { get { return valid ? (int)m_convexMesh.getNbPolygons() : 0; } }

        #endregion

        #region Methods

        public int[] GetPolygonIndices(int polygon)
        {
            if (!valid) return new int[0];
            PxHullPolygon data;
            m_convexMesh.getPolygonData((uint)polygon, out data);
            var inds = new int[data.mNbVerts];
            for (int i = 0; i < inds.Length; ++i) inds[i] = m_convexMesh.getIndex((uint)(data.mIndexBase + i));
            return inds;
        }

        public void BuildFromPoints(Vector3[] points)
        {
            var pinPoints = GCHandle.Alloc(points, GCHandleType.Pinned);
            var desc = new PxConvexMeshDesc();
            desc.points.count = (uint)points.Length;
            desc.points.stride = sizeof(float) * 3;
            desc.points.data = Marshal.UnsafeAddrOfPinnedArrayElement(points, 0);
            desc.flags = PxConvexFlag.COMPUTE_CONVEX;
            var outputStream = new PxDefaultMemoryOutputStream();
            PxConvexMeshCookingResult result;
            var convexMesh = PxPhysics.cooking.cookConvexMesh(desc, outputStream, out result);
            if (result == PxConvexMeshCookingResult.SUCCESS)
            {
                m_convexMeshData = new byte[outputStream.getSize()];
                Marshal.Copy(outputStream.getData(), m_convexMeshData, 0, m_convexMeshData.Length);
                Recreate();
            }
            else
            {
                Debug.LogError("PhysX Cooking: Failed to create convex mesh.");
            }
            outputStream.destroy();
            pinPoints.Free();
        }

        #endregion

        #region Protected

        protected override void CreateAsset()
        {
            CreateConvexMesh();
        }

        protected override void DestroyAsset()
        {
            DestroyConvexMesh();
        }

        protected override void ValidateAsset()
        {
            Recreate();
        }

        #endregion

        #region Private

        void CreateConvexMesh()
        {
            if (m_convexMeshData.Length > 0)
            {
                var pinData = GCHandle.Alloc(m_convexMeshData, GCHandleType.Pinned);
                var inputData = new PxDefaultMemoryInputData(Marshal.UnsafeAddrOfPinnedArrayElement(m_convexMeshData, 0), (uint)m_convexMeshData.Length);
                m_convexMesh = PxPhysics.apiPhysics.createConvexMesh(inputData);
                inputData.destroy();
                pinData.Free();
                ApplyProperties();
            }

            //Debug.Log("PxConvexMesh '" + name + "' created");
        }

        void DestroyConvexMesh()
        {
            m_convexMesh?.release();
            m_convexMesh = null;

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
        PX.PxConvexMesh m_convexMesh;

        [SerializeField]
        byte[] m_convexMeshData = new byte[0];

        #endregion
    }
}
