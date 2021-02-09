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
            int rows = terrainData.heightmapHeight;
            int columns = terrainData.heightmapWidth;
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
