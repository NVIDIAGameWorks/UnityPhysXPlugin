using System;
using UnityEngine;

namespace NVIDIA.PhysX.Unity
{
    public class PxSettings : PxAsset
    {
        #region Properties

        public override bool valid { get { return PxPhysics.valid; } }

        public float lengthScale { get { return m_lengthScale; } set { m_lengthScale = value; ValidateAndRecreate(); } }

        public float speedScale { get { return m_speedScale; } set { m_speedScale = value; ValidateAndRecreate(); } }

        public bool pvdConnect { get { return m_pvdConnect; } set { m_pvdConnect = value; ValidateAndRecreate(); } }

        public string pvdHost { get { return m_pvdHost; } set { m_pvdHost = value; ValidateAndRecreate(); } }

        public int pvdPort { get { return m_pvdPort; } set { m_pvdPort = value; ValidateAndRecreate(); } }

        public float pvdTimeout { get { return m_pvdTimeout; } set { m_pvdTimeout = value; ValidateAndRecreate(); } }

        public PxPvdInstrumentationFlag pvdFlags
        {
            get
            {
                return (m_pvdInstrumentation.debug ? PxPvdInstrumentationFlag.DEBUG : 0) | (m_pvdInstrumentation.profile ? PxPvdInstrumentationFlag.PROFILE : 0) | (m_pvdInstrumentation.memory ? PxPvdInstrumentationFlag.MEMORY : 0);
            }

            set
            {
                m_pvdInstrumentation.debug = (value & PxPvdInstrumentationFlag.DEBUG) != 0;
                m_pvdInstrumentation.profile = (value & PxPvdInstrumentationFlag.PROFILE) != 0;
                m_pvdInstrumentation.memory = (value & PxPvdInstrumentationFlag.MEMORY) != 0;
                ValidateAndRecreate();
            }
        }

        public int cpuThreads { get { return m_cpuThreads; } set { m_cpuThreads = value; ValidateAndRecreate(); } }

        public uint[] cpuAffinities { get { return m_cpuAffinities; } set { m_cpuAffinities = value; ValidateAndRecreate(); } }

        public bool gpuSimulation { get { return m_gpuSimulation; } set { m_gpuSimulation = value; ValidateAndRecreate(); } }

        #endregion

        #region Methods

        #endregion

        #region Protected

        protected override IPxDependency[] GetDependencies()
        {
            return new IPxDependency[0];
        }

        protected override void CreateAsset()
        {
            PxPhysics.Acquire();
        }

        protected override void DestroyAsset()
        {
            PxPhysics.Release();
        }

        protected override void ValidateAsset()
        {
            m_lengthScale = Mathf.Max(m_lengthScale, 0);
            m_speedScale = Mathf.Max(m_speedScale, 0);
            m_pvdTimeout = Mathf.Max(m_pvdTimeout, 0);
        }

        protected override void ResetAsset()
        {
            m_pvdInstrumentation = new PvdInstrumentation();
        }

        #endregion

        #region Private

        [Serializable]
        class PvdInstrumentation
        {
            public bool debug = false;
            public bool profile = true;
            public bool memory = false;
        }

        [SerializeField]
        float m_lengthScale = 1.0f;
        [SerializeField]
        float m_speedScale = 10.0f;
        [SerializeField]
        bool m_pvdConnect = false;
        [SerializeField]
        string m_pvdHost = "localhost";
        [SerializeField]
        int m_pvdPort = 5425;
        [SerializeField]
        float m_pvdTimeout = 1.0f;
        //[SerializeField, EnumFlag]
        //PxPvdInstrumentationFlag m_pvdFlags = PxPvdInstrumentationFlag.PROFILE;
        [SerializeField]
        PvdInstrumentation m_pvdInstrumentation = new PvdInstrumentation();
        [SerializeField]
        int m_cpuThreads = -1;
        [SerializeField]
        uint[] m_cpuAffinities = new uint[0];
        [SerializeField]
        bool m_gpuSimulation = false;

        #endregion
    }
}
