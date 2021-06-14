using System;
using System.Collections.Generic;

namespace NVIDIA.PhysX
{
    public partial struct PxVec3
    {
        public static PxVec3 operator -(PxVec3 a) { return new PxVec3(-a.x, -a.y, -a.z); }
        public static PxVec3 operator +(PxVec3 a, PxVec3 b) { return new PxVec3(a.x + b.x, a.y + b.y, a.z + b.z); }
        public static PxVec3 operator -(PxVec3 a, PxVec3 b) { return new PxVec3(a.x - b.x, a.y - b.y, a.z - b.z); }
        public static PxVec3 operator *(PxVec3 a, float b) { return new PxVec3(a.x * b, a.y * b, a.z * b); }
        public static PxVec3 operator *(float a, PxVec3 b) { return new PxVec3(a * b.x, a * b.y, a * b.z); }
        public static PxVec3 operator /(PxVec3 a, float b) { return new PxVec3(a.x / b, a.y / b, a.z / b); }
        public static bool operator ==(PxVec3 a, PxVec3 b) { return a.x == b.x && a.y == b.y && a.z == b.z; }
        public static bool operator !=(PxVec3 a, PxVec3 b) { return a.x != b.x || a.y != b.y || a.z != b.z; }
        public override bool Equals(object other) { return other is PxVec3 && (PxVec3)other == this; }
        public override int GetHashCode() { return x.GetHashCode() ^ y.GetHashCode() ^ z.GetHashCode(); }
        public float this[int i] {
            get
            {
                switch (i) {
                    case 0: return x;
                    case 1: return y;
                    case 2: return z;
                }
                return 0;
            }
            set
            {
                switch (i)
                {
                    case 0: x = value; break;
                    case 1: y = value; break;
                    case 2: z = value; break;
                }
            }
        }
    }

    public partial struct PxVec4
    {
        public static PxVec4 operator -(PxVec4 a) { return new PxVec4(-a.x, -a.y, -a.z, -a.w); }
        public static PxVec4 operator +(PxVec4 a, PxVec4 b) { return new PxVec4(a.x + b.x, a.y + b.y, a.z + b.z, a.w + b.w); }
        public static PxVec4 operator -(PxVec4 a, PxVec4 b) { return new PxVec4(a.x - b.x, a.y - b.y, a.z - b.z, a.w - b.w); }
        public static PxVec4 operator *(PxVec4 a, float b) { return new PxVec4(a.x * b, a.y * b, a.z * b, a.w * b); }
        public static PxVec4 operator *(float a, PxVec4 b) { return new PxVec4(a * b.x, a * b.y, a * b.z, a * b.w); }
        public static PxVec4 operator /(PxVec4 a, float b) { return new PxVec4(a.x / b, a.y / b, a.z / b, a.w / b); }
        public static bool operator ==(PxVec4 a, PxVec4 b) { return a.x == b.x && a.y == b.y && a.z == b.z && a.w == b.w; }
        public static bool operator !=(PxVec4 a, PxVec4 b) { return a.x != b.x || a.y != b.y || a.z != b.z || a.w != b.w; }
        public override bool Equals(object other) { return other is PxVec4 && (PxVec4)other == this; }
        public override int GetHashCode() { return x.GetHashCode() ^ y.GetHashCode() ^ z.GetHashCode() ^ w.GetHashCode(); }
        public float this[int i] {
            get
            {
                switch (i) {
                    case 0: return x;
                    case 1: return y;
                    case 2: return z;
                    case 3: return w;
                }
                return 0;
            }
            set
            {
                switch (i)
                {
                    case 0: x = value; break;
                    case 1: y = value; break;
                    case 2: z = value; break;
                    case 3: w = value; break;
                }
            }
        }
    }

    public partial struct PxContactPair
    {
        public uint extractContacts(PxContactPairPoint[] userBuffer, uint bufferSize)
        {
            return extractContacts(ref userBuffer[0], bufferSize);
        }
    }

    public partial struct PxTransform
    {
        public static PxTransform identity { get { return new PxTransform(PxIDENTITY.PxIdentity); } }
    }

    internal static class WrapperCache
    {
        public static object find(IntPtr ptr)
        {
            lock (sm_wrappers)
            {
                if (sm_wrappers.ContainsKey(ptr))
                    return sm_wrappers[ptr].Target;
            }
            return null;
        }

        public static void add(IntPtr ptr, object wrapper)
        {
            lock (sm_wrappers)
            {
                if (sm_wrappers.ContainsKey(ptr))
                    sm_wrappers[ptr] = new WeakReference(wrapper);
                else
                    sm_wrappers.Add(ptr, new WeakReference(wrapper));
            }
        }

        public static void remove(IntPtr ptr, object wrapper)
        {
            lock (sm_wrappers)
            {
                if (sm_wrappers.ContainsKey(ptr) && ReferenceEquals(sm_wrappers[ptr], wrapper))
                    sm_wrappers.Remove(ptr);
            }
        }

        static Dictionary<IntPtr, WeakReference> sm_wrappers = new Dictionary<IntPtr, WeakReference>();
    }
}