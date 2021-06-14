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
