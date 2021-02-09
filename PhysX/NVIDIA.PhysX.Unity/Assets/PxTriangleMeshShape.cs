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

using NVIDIA.PhysX.UnityExtensions;
using UnityEngine;

namespace NVIDIA.PhysX.Unity
{
    public class PxTriangleMeshShape : PxSimpleShape
    {
        #region Properties

        public PxTriangleMesh triangleMesh { get { return m_triangleMesh; } }

        public Vector3 scale { get { return m_scale; } set { m_scale = value; ValidateShape(); } }

        public Quaternion scaleRotation { get { return Quaternion.Euler(m_scaleRotation); } set { m_scaleRotation = value.eulerAngles; ValidateShape(); } }

        public bool doubleSided { get { return m_doubleSided; } set { m_doubleSided = value; ValidateShape(); } }

        #endregion

        #region Messsages

        #endregion

        #region Protected

        protected override IPxDependency[] GetDependencies()
        {
            return base.GetDependencies().Append(new[] { m_triangleMesh });
        }

        protected override PxGeometry CreateGeometry()
        {
            if (m_triangleMesh == null || !m_triangleMesh.valid) return null;
            var geometry = new PxTriangleMeshGeometry(m_triangleMesh.apiTriangleMesh, new PxMeshScale(m_scale.ToPxVec3(), Quaternion.Euler(m_scaleRotation).ToPxQuat()));
            if (m_doubleSided) geometry.meshFlags = PxMeshGeometryFlag.DOUBLE_SIDED;
            return geometry;
        }

        protected override void ApplyProperties()
        {
            base.ApplyProperties();

            if (valid)
            {
            }
        }

        protected override void ValidateShape()
        {
            float eps = 0.000001f;
            m_scale.x = Mathf.Max(m_scale.x, eps);
            m_scale.y = Mathf.Max(m_scale.y, eps);
            m_scale.z = Mathf.Max(m_scale.z, eps);

            base.ValidateShape();
        }

        #endregion

        #region Private

        [SerializeField]
        PxTriangleMesh m_triangleMesh = null;
        [SerializeField]
        Vector3 m_scale = Vector3.one;
        [SerializeField]
        Vector3 m_scaleRotation = Vector3.zero;
        [SerializeField]
        bool m_doubleSided = false;

        #endregion
    }
}
