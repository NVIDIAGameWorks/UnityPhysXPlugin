using System;

namespace NVIDIA.PhysX.Unity
{
    public delegate void DependencyAction(IPxDependency dependency);

    public interface IPxDependency
    {
        #region Properties

        bool created { get; }

        #endregion

        #region Events

        event DependencyAction onAfterCreate;
        event DependencyAction onBeforeDestroy;
        event DependencyAction onBeforeRecreate;
        event DependencyAction onAfterRecreate;

        #endregion
    }

    public static class PxDependencyArrayExtension
    {
        public static IPxDependency[] Append(this IPxDependency[] a, IPxDependency[] b)
        {
            var c = new IPxDependency[a.Length + b.Length];
            Array.Copy(a, c, a.Length);
            Array.Copy(b, 0, c, a.Length, b.Length);
            return c;
        }
    }

}