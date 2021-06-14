using NVIDIA.PhysX.UnityExtensions;
using System;
using System.Collections.Generic;
using UnityEngine;
using PX = NVIDIA.PhysX;

namespace NVIDIA.PhysX.Unity
{
    [DisallowMultipleComponent]
    [AddComponentMenu("NVIDIA/PhysX/Px Scene", 100)]
    public class PxScene : PxComponent
    {
        #region Properties

        public override bool valid { get { return m_scene != null; } }

        public PX.PxScene apiScene { get { return m_scene; } }

        public int collisionLayerCount { get { return m_collisionLayers.Length; } }

        public string[] collisionLayerNames { get { return (string[])m_collisionLayers.Clone(); } }

        public Vector3 gravity { get { return m_gravity; } set { m_gravity = value; ValidateAndApply(); } }

        public PxSolverType solverType { get { return m_solverType; } set { m_solverType = value; ValidateAndRecreate(); } }

        public PxBroadPhaseType broadPhaseType { get { return m_broadPhaseType; } set { m_broadPhaseType = value; ValidateAndRecreate(); } }

        public bool gpuSimulation { get { return m_gpuSimulation; } set { m_gpuSimulation = value; ValidateAndRecreate(); } }

        public static PxScene main { get { return GameObject.FindWithTag("MainPxScene")?.GetComponent<PxScene>(); } }

        #endregion

        #region Events

        public event Action onBeforeSimulation;

        public event Action onAfterSimulation;

        #endregion

        #region Methods

        public string CollisionLayerName(int layerIndex)
        {
            if (layerIndex >= collisionLayerCount)
            {
                Debug.LogError("Bad layer index");
                return "";
            }

            return m_collisionLayers[layerIndex];
        }

        public void SetCollisionLayerName(int layerIndex, string name)
        {
            if (layerIndex >= collisionLayerCount)
            {
                Debug.LogError("Bad layer index");
                return;
            }

            m_collisionLayers[layerIndex] = name;
        }

        public bool LayersCollide(int layerIndex0, int layerIndex1)
        {
            if (layerIndex0 >= collisionLayerCount || layerIndex0 >= collisionLayerCount)
            {
                Debug.LogError("Bad layer index");
                return false;
            }

            int layerCount = collisionLayerCount;
            int i = layerIndex0 > layerIndex1 ? layerIndex0 : layerIndex1;
            int j = layerIndex0 > layerIndex1 ? layerIndex1 : layerIndex0;
            return m_collisionMatrix[j * layerCount - j * (j + 1) / 2 + i];
        }

        public void SetLayersCollide(int layerIndex0, int layerIndex1, bool yes)
        {
            if (layerIndex0 >= collisionLayerCount || layerIndex0 >= collisionLayerCount)
            {
                Debug.LogError("Bad layer index");
                return;
            }

            int layerCount = collisionLayerCount;
            int i = layerIndex0 > layerIndex1 ? layerIndex0 : layerIndex1;
            int j = layerIndex0 > layerIndex1 ? layerIndex1 : layerIndex0;
            m_collisionMatrix[j * layerCount - j * (j + 1) / 2 + i] = yes;

            ValidateAndRecreate();
        }

        #endregion

        #region Messages

        void FixedUpdate()
        {
            UpdateScene();
        }

        #endregion

        #region Protected

        protected override void CreateComponent()
        {
            CreateScene();
        }

        protected override void DestroyComponent()
        {
            DestroyScene();
        }

        protected override void ValidateComponent()
        {
            ValidateScene();
        }

        protected override void ResetComponent()
        {
            ResetScene();
        }

        protected override void ApplyProperties()
        {
            base.ApplyProperties();

            if (valid)
            {
                m_scene.setGravity(m_gravity.ToPxVec3());

                int layerCount = m_collisionLayers.Length;
                for (int i = 0; i < layerCount; ++i) for (int j = 0; j <= i; ++j)
                        m_sceneCollision.setCanCollide((uint)i, (uint)j, m_collisionMatrix[j * layerCount - j * (j + 1) / 2 + i]);
            }
        }

        #endregion

        #region Private

        void CreateScene()
        {
            var scale = PxPhysics.apiPhysics.getTolerancesScale();
            var sceneDesc = new PxSceneDesc(scale);
            sceneDesc.cpuDispatcher = PxPhysics.cpuDispatcher;
            sceneDesc.filterShader = PxUnityCollisionFiltering.function;
            sceneDesc.filterCallback = PxUnityCollisionFiltering.instance;
            sceneDesc.gravity = m_gravity.ToPxVec3();
            sceneDesc.solverType = m_solverType;
            sceneDesc.broadPhaseType = m_broadPhaseType;
            sceneDesc.flags |= m_enableCCD ? PxSceneFlag.ENABLE_CCD : 0;
            if (m_gpuSimulation)
            {
                var cudaContextManager = PxPhysics.cudaContextManager;
                if (cudaContextManager != null && cudaContextManager.contextIsValid())
                {
                    sceneDesc.cudaContextManager = cudaContextManager;
                    sceneDesc.flags |= PxSceneFlag.ENABLE_GPU_DYNAMICS;
                    sceneDesc.broadPhaseType = PxBroadPhaseType.GPU;
                }
                else
                    Debug.LogWarning("GPU simulation is not enabled or is not available on this hardware. Falling back to CPU.");
            }
            m_eventCallback = new EventCallback();
            sceneDesc.simulationEventCallback = m_eventCallback;
            m_scene = PxPhysics.apiPhysics.createScene(sceneDesc);

            m_sceneCollision = new PxSceneCollision();
            int layerCount = m_collisionLayers.Length;
            for (int i = 0; i < layerCount; ++i) for (int j = 0; j <= i; ++j)
                    m_sceneCollision.setCanCollide((uint)i, (uint)j, m_collisionMatrix[j * layerCount - j * (j + 1) / 2 + i]);
            PxUnityCollisionFiltering.setCollision(m_scene, m_sceneCollision);

            ApplyProperties();

            //Debug.Log("PxScene '" + name + "' created");
        }

        void DestroyScene()
        {
            m_eventCallback?.destroy();
            m_eventCallback = null;

            m_scene?.release();
            m_scene = null;

            //Debug.Log("PxScene '" + name + "' destroyed");
        }

        void ValidateScene()
        {
            if (string.IsNullOrEmpty(m_collisionLayers[0]))
            {
                m_collisionLayers[0] = "Default";
                m_collisionMatrix[0] = true;
            }
        }

        void ResetScene()
        {
            m_collisionLayers = new[] { "Default", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "" };
            m_collisionMatrix[0] = true;
            for (int i = 1; i < m_collisionMatrix.Length; ++i) m_collisionMatrix[i] = false;
        }

        void UpdateScene()
        {
            if (m_scene != null)
            {
                onBeforeSimulation?.Invoke();

                m_scene.simulate(Time.fixedDeltaTime);
                m_scene.fetchResults(true);

                onAfterSimulation?.Invoke();
            }
        }

        class EventCallback : PxSimulationEventCallback
        {
            public override void onContact(PxContactPairHeader pairHeader, PxContactPairList pairs)
            {
                var actor0 = pairHeader.actor0.userData as PxActor;
                var actor1 = pairHeader.actor1.userData as PxActor;
                actor0?.onContact?.Invoke(actor0, actor1, pairHeader, pairs);
                actor1?.onContact?.Invoke(actor1, actor0, pairHeader, pairs);
            }
            public override void onConstraintBreak(PxConstraintInfoList constraints)
            {
                for (uint i = 0; i < constraints.count; ++i)
                {
                    var constraintInfo = constraints.get(i);
                    var joint = constraintInfo.joint?.userData as PxJoint;
                    joint?.onBreak?.Invoke(joint);
                }
            }
        }

        [NonSerialized]
        PX.PxScene m_scene;
        [NonSerialized]
        EventCallback m_eventCallback;
        [NonSerialized]
        PxSceneCollision m_sceneCollision = null;

        [SerializeField]
        Vector3 m_gravity = Vector3.up * -9.8f;
        [SerializeField]
        PxSolverType m_solverType = PxSolverType.PGS;
        [SerializeField]
        PxBroadPhaseType m_broadPhaseType = PxBroadPhaseType.ABP;
        [SerializeField]
        bool m_gpuSimulation = false;
        [SerializeField]
        bool m_enableCCD = false;
        [SerializeField]
        string[] m_collisionLayers = new string[16] { "Default", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "" };
        [SerializeField]
        bool[] m_collisionMatrix = new bool[16 * 17 / 2] { true,
                                                           false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false,
                                                           false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false,
                                                           false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false,
                                                           false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false,
                                                           false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false,
                                                           false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false,
                                                           false, false, false, false, false, false, false, false, false, false, false, false, false, false, false };

        #endregion
    }
}