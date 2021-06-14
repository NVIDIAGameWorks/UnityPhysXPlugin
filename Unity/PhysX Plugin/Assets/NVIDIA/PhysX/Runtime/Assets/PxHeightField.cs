using System;
using System.Runtime.InteropServices;
using UnityEngine;
using PX = NVIDIA.PhysX;

namespace NVIDIA.PhysX.Unity
{
    public class PxHeightField : PxAsset
    {
        #region Properties

        public override bool valid { get { return m_heightField != null; } }

        public PX.PxHeightField apiHeightField { get { return m_heightField; } }

        public int columnCount { get { return valid ? (int)m_heightField.getNbColumns() : 0; } }

        public int rowCount { get { return valid ? (int)m_heightField.getNbRows() : 0; } }

        #endregion

        #region Methods

        public float GetHeight(int x, int z)
        {
            if (!valid) return 0;
            return m_heightField.getHeight(x, z);
        }

        public void BuildFromTerrainData(TerrainData terrainData)
        {
            int rows = terrainData.heightmapResolution;
            int columns = terrainData.heightmapResolution;
            var heights = terrainData.GetHeights(0, 0, columns, rows);
            var samples = new PxHeightFieldSample[rows * columns];
            PxHeightFieldSample sample = new PxHeightFieldSample();
            sample.materialIndex0 = sample.materialIndex1 = 0;
            sample.clearTessFlag();
            for (int y = 0; y < rows; ++y)
            {
                for (int x = 0; x < columns; ++x)
                {
                    sample.height = (short)(heights[x, y] * 0x7fff);
                    samples[x + y * columns] = sample;
                }
            }
            var pinSamples = GCHandle.Alloc(samples, GCHandleType.Pinned);
            var desc = new PxHeightFieldDesc();
            desc.nbRows = (uint)rows;
            desc.nbColumns = (uint)columns;
            desc.samples.stride = (uint)Marshal.SizeOf<PxHeightFieldSample>();
            desc.samples.data = Marshal.UnsafeAddrOfPinnedArrayElement(samples, 0);
            var outputStream = new PxDefaultMemoryOutputStream();
            if (PxPhysics.cooking.cookHeightField(desc, outputStream))
            {
                m_heightFieldData = new byte[outputStream.getSize()];
                Marshal.Copy(outputStream.getData(), m_heightFieldData, 0, m_heightFieldData.Length);
                Recreate();
            }
            else
            {
                Debug.LogError("PhysX Cooking: Failed to create height field.");
            }
            outputStream.destroy();
            pinSamples.Free();
        }

        #endregion

        #region Protected

        protected override void CreateAsset()
        {
            CreateHeightField();
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

        void CreateHeightField()
        {
            if (m_heightFieldData.Length > 0)
            {
                var pinData = GCHandle.Alloc(m_heightFieldData, GCHandleType.Pinned);
                var inputData = new PxDefaultMemoryInputData(Marshal.UnsafeAddrOfPinnedArrayElement(m_heightFieldData, 0), (uint)m_heightFieldData.Length);
                m_heightField = PxPhysics.apiPhysics.createHeightField(inputData);
                inputData.destroy();
                pinData.Free();
                ApplyProperties();
            }

            //Debug.Log("PxHeightField '" + name + "' created");
        }

        void DestroyTriangleMesh()
        {
            m_heightField?.release();
            m_heightField = null;

            //Debug.Log("PxHeightField '" + name + "' destroyed");
        }

        protected override void ApplyProperties()
        {
            base.ApplyProperties();

            if (valid)
            {
            }
        }

        [NonSerialized]
        PX.PxHeightField m_heightField;

        [SerializeField]
        byte[] m_heightFieldData = new byte[0];

        #endregion
    }
}
