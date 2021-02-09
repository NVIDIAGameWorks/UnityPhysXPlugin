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
using System;
using UnityEngine;
using PX = NVIDIA.PhysX;

namespace NVIDIA.PhysX.Unity
{
    [AddComponentMenu("NVIDIA/PhysX/Px Fixed Joint", 400)]
    public class PxFixedJoint : PxJoint
    {
        #region Properties

        public override bool valid { get { return m_fixedJoint != null; } }

        public override PX.PxJoint apiJoint { get { return m_fixedJoint; } }

        #endregion

        #region Protected

        protected override void CreateJoint()
        {
            base.CreateJoint();
            CreateFixedJoint();
        }

        protected override void DestroyJoint()
        {
            DestroyFixedJoint();
            base.DestroyJoint();
        }

        protected override void ValidateJoint()
        {
            base.ValidateJoint();
            Recreate();
        }

        #endregion

        #region Private

        void CreateFixedJoint()
        {
            if ((actor0 && actor0.valid) || (actor1 && actor1.valid))
            {
                var bodies0 = (actor0 && actor0.valid) ? actor0.apiActors : new PX.PxActor[] { null };
                var bodies1 = (actor1 && actor1.valid) ? actor1.apiActors : new PX.PxActor[] { null };
                var a0 = bodies0[Mathf.Clamp(body0Index, 0, bodies0.Length)] as PxRigidActor;
                var a1 = bodies1[Mathf.Clamp(body1Index, 0, bodies1.Length)] as PxRigidActor;
                var localFrame0 = new PxTransform(anchor0Position.ToPxVec3(), anchor0Rotation.ToPxQuat());
                var localFrame1 = new PxTransform(anchor1Position.ToPxVec3(), anchor1Rotation.ToPxQuat());
                m_fixedJoint = PxPhysics.apiPhysics.createFixedJoint(a0, localFrame0, a1, localFrame1);
                m_fixedJoint.userData = this;
            }

            //Debug.Log("PxFixedJoint '" + name + "' created");
        }

        void DestroyFixedJoint()
        {
            m_fixedJoint?.release();
            m_fixedJoint = null;

            //Debug.Log("PxFixedJoint '" + name + "' destroyed");
        }

        [NonSerialized]
        PX.PxFixedJoint m_fixedJoint;

        #endregion
    }
}