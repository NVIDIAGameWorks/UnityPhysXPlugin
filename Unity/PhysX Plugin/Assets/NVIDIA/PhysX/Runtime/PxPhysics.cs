using System;
using System.IO;
using UnityEngine;
using PX = NVIDIA.PhysX;

namespace NVIDIA.PhysX.Unity
{
    public static class PxPhysics
    {
        #region Properties

        public static PX.PxFoundation apiFoundation { get { return sm_foundation; } }

        public static bool valid { get { return sm_physics != null; } }

        public static PX.PxPhysics apiPhysics { get { return sm_physics; } }

        public static PxCudaContextManager cudaContextManager { get { return sm_cudaContextManager; } }

        public static PxCpuDispatcher cpuDispatcher { get { return sm_cpuDispatcher; } }

        public static PX.PxMaterial noMaterial { get { return sm_noMaterial; } }

        public static PxSettings settings { get { return sm_settings ?? (sm_settings = Resources.Load<PxSettings>("PhysX Settings")); } }

        public static PxCooking cooking { get { return sm_cooking; } }

        #endregion

        #region Methods

        internal static void Acquire()
        {
            if (sm_refCount++ == 0) Create();
        }

        internal static void Release()
        {
            if (--sm_refCount == 0) Destroy();
        }

        #endregion

        #region Private

        static void Create()
        {
            sm_settings = settings;

            sm_allocatorCallback = new PX.PxDefaultAllocator();
            sm_errorCallback = new ErrorCallback();
            sm_foundation = PX.PxFoundation.create(PX.PxVersion.PX_PHYSICS_VERSION, sm_allocatorCallback, sm_errorCallback);

            if (sm_settings.pvdConnect)
            {
                sm_pvd = sm_foundation.createPvd();
                sm_pvdTransport = PX.PxPvdTransport.createDefaultSocketTransport(sm_settings.pvdHost, sm_settings.pvdPort, (int)(sm_settings.pvdTimeout * 1000));
                if (!sm_pvd.connect(sm_pvdTransport, sm_settings.pvdFlags))
                {
                    sm_pvd.release();
                    sm_pvd = null;
                    sm_pvdTransport.release();
                    sm_pvdTransport = null;
                }
                else
                    Debug.Log("Connected to PVD.");
            }

            var scale = new PX.PxTolerancesScale { length = sm_settings.lengthScale, speed = sm_settings.speedScale };
            sm_physics = sm_foundation.createPhysics(PX.PxVersion.PX_PHYSICS_VERSION, scale, sm_pvd);
            sm_physics.initExtensions(sm_pvd);

            int numThreads = sm_settings.cpuThreads < 0 ? SystemInfo.processorCount + sm_settings.cpuThreads + 1
                                                        : sm_settings.cpuThreads;
            numThreads = Mathf.Clamp(numThreads, 0, SystemInfo.processorCount);
            //Debug.Log("PhysX: Creating default CPU dispatcher with " + numThreads + " threads");
            uint[] affinityMasks = sm_settings.cpuThreads > 0 && sm_settings.cpuThreads == sm_settings.cpuAffinities.Length ? sm_settings.cpuAffinities : null;
            sm_cpuDispatcher = PX.PxCpuDispatcher.createDefault((uint)numThreads, affinityMasks);

            if (sm_settings.gpuSimulation)
            {
                string dllPath;
                if (Application.isEditor) dllPath = Path.GetFullPath(".\\Assets\\NVIDIA\\PhysX\\Runtime\\Plugins\\x86_64\\PhysXGpu_64.dll");
                else dllPath = UnityEngine.Application.dataPath.Replace("\\", "/") + "/Plugins/PhysXGpu_64.dll";

                //Debug.Log("dllPath - " + dllPath);
                sm_cudaContextManager = sm_foundation.createCudaContextManager(dllPath);
            }

            sm_noMaterial = sm_physics.createMaterial(0.5f, 0.5f, 0);

            sm_cooking = sm_foundation.createCooking(PxVersion.PX_PHYSICS_VERSION, new PxCookingParams(sm_physics.getTolerancesScale()));

            //Debug.Log("PxPhysics created");
        }

        static void Destroy()
        {
            sm_cooking?.release();
            sm_cooking = null;

            sm_noMaterial?.release();
            sm_noMaterial = null;

            sm_cudaContextManager?.release();
            sm_cudaContextManager = null;

            sm_cpuDispatcher?.release();
            sm_cpuDispatcher = null;

            sm_physics?.closeExtensions();
            sm_physics?.release();
            sm_physics = null;

            sm_pvd?.release();
            sm_pvd = null;

            sm_pvdTransport?.release();
            sm_pvdTransport = null;

            sm_foundation?.release();
            sm_foundation = null;

            //if (sm_settings != null)
            //{
            //    sm_settings = null;
            //}

            sm_refCount = 0;

            //Debug.Log("PxPhysics destroyed");
        }

        static PxPhysics()
        {
            AppDomain.CurrentDomain.DomainUnload += DomainUnload;
        }

        static void DomainUnload(object sender, EventArgs e)
        {
            //Debug.Log("Domain unload");

            if (sm_refCount > 0)
            {
                Debug.LogError("Some objects leaked");
                Destroy();
            }
        }

        class ErrorCallback : PX.PxErrorCallback
        {
            public override void reportError(PxErrorCode code, string message, string file, int line)
            {
                string msg = message + " - " + file + "(" + line + ")";
                switch (code)
                {
                    case PxErrorCode.DEBUG_INFO:
                        Debug.Log("PhysX Info: " + msg);
                        break;
                    case PxErrorCode.DEBUG_WARNING:
                    case PxErrorCode.PERF_WARNING:
                        Debug.LogWarning("PhysX Warning: " + msg);
                        break;
                    case PxErrorCode.INVALID_PARAMETER:
                    case PxErrorCode.INVALID_OPERATION:
                    case PxErrorCode.OUT_OF_MEMORY:
                    case PxErrorCode.INTERNAL_ERROR:
                    case PxErrorCode.ABORT:
                        Debug.LogError("PhysX Error: " + msg);
                        break;
                }
            }
        }

        [NonSerialized]
        static PX.PxAllocatorCallback sm_allocatorCallback;
        [NonSerialized]
        static PX.PxErrorCallback sm_errorCallback;
        [NonSerialized]
        static PX.PxFoundation sm_foundation;
        [NonSerialized]
        static PX.PxPvdTransport sm_pvdTransport;
        [NonSerialized]
        static PX.PxPvd sm_pvd;
        [NonSerialized]
        static PX.PxPhysics sm_physics;
        [NonSerialized]
        static PX.PxCudaContextManager sm_cudaContextManager;
        [NonSerialized]
        static PX.PxDefaultCpuDispatcher sm_cpuDispatcher;
        [NonSerialized]
        static PX.PxMaterial sm_noMaterial;
        [NonSerialized]
        static PxSettings sm_settings;
        [NonSerialized]
        static int sm_refCount = 0;
        [NonSerialized]
        static PX.PxCooking sm_cooking;

        #endregion
    }
}