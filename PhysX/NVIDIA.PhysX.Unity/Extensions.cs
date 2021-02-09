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

using UnityEngine;
using PX = NVIDIA.PhysX;

#region Extensions

namespace NVIDIA.PhysX.UnityExtensions
{
    public static class Extensions
    {
        public static Vector3 ToVector3(this PxVec3 v)
        {
            unsafe { return *(Vector3*)&v; }
            //return new Vector3(v.x, v.y, v.z);
        }

        public static Vector4 ToVector4(this PxVec4 v)
        {
            unsafe { return *(Vector4*)&v; }
            //return new Vector4(v.x, v.y, v.z, v.w);
        }

        public static Quaternion ToQuaternion(this PxQuat q)
        {
            unsafe { return *(Quaternion*)&q; }
            //return new Quaternion(q.x, q.y, q.z, q.w);
        }

        public static Matrix4x4 ToMatrix4x4(this PxTransform t)
        {
            return Matrix4x4.TRS(t.p.ToVector3(), t.q.ToQuaternion(), Vector3.one);
        }

        public static Matrix4x4 ToMatrix4x4(this PxMat44 m)
        {
            unsafe { return *(Matrix4x4*)&m; }
            //return new Matrix4x4(m.column0.ToVector4(), m.column1.ToVector4(), m.column2.ToVector4(), m.column3.ToVector4());
        }

        public static PxVec3 ToPxVec3(this Vector3 v)
        {
            unsafe { return *(PxVec3*)&v; }
            //return new PxVec3(v.x, v.y, v.z);
        }

        public static PxTransform ToPxTransform(this Vector3 v)
        {
            return new PxTransform(new PxVec3(v.x, v.y, v.z));
        }

        public static PxTransform ToPxTransform(this Transform t)
        {
            return new PxTransform(t.position.ToPxVec3(), t.rotation.ToPxQuat());
        }

        public static void ToTransform(this PxTransform t0, Transform t1)
        {
            t1.SetPositionAndRotation(t0.p.ToVector3(), t0.q.ToQuaternion());
        }

        public static PxQuat ToPxQuat(this Quaternion q)
        {
            unsafe { return *(PxQuat*)&q; }
            //return new PxQuat(q.x, q.y, q.z, q.w);
        }

        public static void getRigidActorMatrices(this PxRigidActorList list, Matrix4x4[] matrices, int start, int count)
        {
            list.getRigidActorMatrices(ref matrices[0].m00, (uint)start, (uint)count);
        }
    }
}

#endregion
