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
using System.Collections.Generic;
using UnityEngine;
using PX = NVIDIA.PhysX;

namespace NVIDIA.PhysX.Unity
{
    public class PxCompoundShape : PxShape
    {
        #region Properties

        public override bool valid { get { return Array.Exists(m_shapes, x => x && x.valid); } }

        public override PX.PxShape[] apiShapes
        {
            get
            {
                var shapeList = new List<PX.PxShape>();
                foreach (var s in m_shapes)
                    if (s.valid) shapeList.AddRange(s.apiShapes);
                return shapeList.ToArray();
            }
        }

        public override float[] densities
        {
            get
            {
                var densityList = new List<float>();
                foreach (var s in m_shapes)
                    if (s.valid) densityList.AddRange(s.densities);
                return densityList.ToArray();
            }
        }

        public PxShape[] shapes { get { return m_shapes; } set { m_shapes = value; ValidateShape(); } }

        #endregion

        #region Protected

        protected override void ValidateShape()
        {
            CheckLoops();
            base.ValidateShape();
        }

        #endregion

        #region Private

        void CheckLoops()
        {
            for (int i = 0; i < m_shapes.Length; ++i)
                if (LoopDependency(m_shapes[i]))
                    m_shapes[i] = null;
        }

        bool LoopDependency(PxShape shape)
        {
            var queue = new Queue<PxShape>();
            queue.Enqueue(shape);
            while (queue.Count > 0)
            {
                var item = queue.Dequeue() as PxCompoundShape;

                if (item == this) return true;

                if (item != null)
                    foreach (var s in item.shapes)
                        queue.Enqueue(s);

            }
            return false;
        }

        [SerializeField]
        PxShape[] m_shapes = new PxShape[0];

        #endregion
    }
}
