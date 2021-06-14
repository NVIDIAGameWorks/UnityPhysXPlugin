using NVIDIA.PhysX;
using NVIDIA.PhysX.UnityExtensions;
using System;
using System.Globalization;
using System.IO;
using UnityEngine;

public class SampleHelloGRB : SampleBase
{
    #region Messages

    private void FixedUpdate()
    {
        if (m_scene != null)
        {
            if (m_throwBall) ThrowBall();

            m_simulationTime = Time.realtimeSinceStartup;

            m_scene.simulate(Time.fixedDeltaTime);
            m_scene.fetchResults(true);

            m_simulationTime = Time.realtimeSinceStartup - m_simulationTime;
        }
    }

    private void Update()
    {
        UpdateCamera();
        UpdateInput();
        UpdatePicker(m_scene);
    }

    CultureInfo m_culture = new CultureInfo("en-US");

    private void OnGUI()
    {
        int y = 0;

        GUI.Label(new Rect(160, y++ * 20 + 5, 300, 50), "CPU: " + SystemInfo.processorType);

        GUI.Label(new Rect(160, y++ * 20 + 5, 300, 50), "GPU: " + SystemInfo.graphicsDeviceName);

        GUI.Label(new Rect(160, y++ * 20 + 5, 300, 50), "Rigid body count: " + (m_boxList.getNbRigidActors() + m_ballList.getNbRigidActors()));

        GUI.Label(new Rect(160, y++ * 20 + 5, 300, 50), "Simulation time: " + string.Format(m_culture, "{0:0.0} ms", m_simulationTime * 1000));

        GUI.contentColor = m_enableGRB ? Color.green : Color.red;
        m_enableGRB = GUI.Toggle(new Rect(160, y++ * 20 + 5, 300, 50), m_enableGRB, m_enableGRB ? "GPU Rigid Bodies enabled" : "GPU Rigid Bodies disabled");
        if (GUI.changed)
        {
            enabled = false;
            enabled = true;
        }
    }

    #endregion

    #region Protected

    protected override void CreateSample()
    {
        base.CreateSample();
        CreateScene();
    }

    protected override void DestroySample()
    {
        DestoryScene();
        base.DestroySample();
    }

    protected override void OnRender(Camera camera)
    {
        RenderActorList(camera, m_boxList, m_boxMesh, m_boxMaterial);
        RenderActorList(camera, m_ballList, m_ballMesh, m_ballMaterial);
        base.OnRender(camera);
    }

    #endregion

    #region Private

    void CreateScene()
    {
        // Create CPU dispatcher
        m_cpuDispatcher = PxCpuDispatcher.createDefault((uint)SystemInfo.processorCount);

        // Allocate and initialize PxSceneDesc
        var sceneDesc = new PxSceneDesc(physics.getTolerancesScale());
        sceneDesc.cpuDispatcher = m_cpuDispatcher;
        sceneDesc.filterShader = PxDefaultSimulationFilterShader.function;
        sceneDesc.gravity = new PxVec3(0, -9.8f, 0);

        sceneDesc.broadPhaseType = PxBroadPhaseType.ABP;
        if (m_enableGRB)
        {
            // Trying to enable GRB
#if UNITY_EDITOR
            string dllPath = Path.GetFullPath("./Assets/NVIDIA/PhysX/Runtime/Plugins/x86_64/PhysXGpu_64.dll");
#else
            string dllPath = UnityEngine.Application.dataPath.Replace("\\", "/") + "/Plugins/PhysXGpu_64.dll";
#endif
            try { m_cudaContextManager = foundation.createCudaContextManager(dllPath); }
            catch (Exception e) { Debug.LogException(e); }

            var cudaContextManager = m_cudaContextManager;
            if (cudaContextManager != null)
            {
                if (cudaContextManager.contextIsValid())
                {
                    sceneDesc.cudaContextManager = cudaContextManager;
                    sceneDesc.flags |= PxSceneFlag.ENABLE_GPU_DYNAMICS;
                    sceneDesc.broadPhaseType = PxBroadPhaseType.GPU;
                }
                else
                    Debug.LogWarning("GPU dynamics is not available on this hardware. Falling back to CPU.");
            }
            else
                Debug.LogWarning("Can't create CUDA context manager. Falling back to CPU.");
        }

        // Create PxScene
        m_scene = physics.createScene(sceneDesc);
        sceneDesc.destroy();

        // Set PVD flags
        var pvdClient = m_scene.getScenePvdClient();
        if (pvdClient != null)
            pvdClient.setScenePvdFlags(PxPvdSceneFlag.TRANSMIT_CONTACTS | PxPvdSceneFlag.TRANSMIT_CONSTRAINTS | PxPvdSceneFlag.TRANSMIT_SCENEQUERIES);

        m_physicsMaterial = physics.createMaterial(0.5f, 0.5f, 0.05f);

        m_boxList = new PxRigidActorList();
        m_ballList = new PxRigidActorList();

        CreateGround();
        CreateObjects();
    }

    void DestoryScene()
    {
        m_boxList?.releaseRigidActors();
        m_boxList?.destroy();
        m_boxList = null;
        m_ballList?.releaseRigidActors();
        m_ballList?.destroy();
        m_ballList = null;
        Destroy(m_boxMesh);
        Destroy(m_ballMesh);
        Destroy(m_boxMaterial);
        m_boxMaterial = null;
        Destroy(m_ballMaterial);
        m_ballMaterial = null;
        m_boxMesh = null;
        m_groundActor?.release();
        m_groundActor = null;
        Destroy(m_groundMesh);
        m_groundMesh = null;
        Destroy(m_groundMaterial);
        m_groundMaterial = null;
        m_scene?.release();
        m_scene = null;
        m_physicsMaterial?.release();
        m_physicsMaterial = null;
        m_cpuDispatcher?.release();
        m_cpuDispatcher = null;
        m_cudaContextManager?.release();
        m_cudaContextManager = null;
    }

    void CreateGround()
    {
        // Create PxRigidStatic for ground
        m_groundActor = physics.createRigidStatic(new PxTransform(Quaternion.Euler(0, 0, 90.0f).ToPxQuat()));
        // Add plane shape
        m_groundActor.createExclusiveShape(new PxPlaneGeometry(), m_physicsMaterial);
        // Add ground actor to scene
        m_scene.addActor(m_groundActor);

        m_groundMesh = new Mesh();
        m_groundMesh.vertices = new[] { new Vector3(0, 1000, 1000), new Vector3(0, -1000, 1000), new Vector3(0, -1000, -1000), new Vector3(0, 1000, -1000) };
        m_groundMesh.normals = new[] { Vector3.right, Vector3.right, Vector3.right, Vector3.right };
        m_groundMesh.uv = new[] { Vector2.zero, Vector2.zero, Vector2.zero, Vector2.zero };
        m_groundMesh.triangles = new[] { 0, 1, 2, 0, 2, 3 };
        m_groundMesh.RecalculateBounds();
        m_groundMaterial = new Material(Shader.Find("Standard"));
        m_groundMaterial.color = COLOR3;
        AddRenderActor(m_groundActor, m_groundMesh, m_groundMaterial);
    }

    void CreateObjects()
    {
        var instaceableMaterial = Resources.Load<Material>("Instanceable");

        m_boxMaterial = new Material(instaceableMaterial);
        m_boxMaterial.color = COLOR4;

        m_ballMaterial = new Material(instaceableMaterial);
        m_ballMaterial.color = COLOR5;

        m_boxMesh = CreateBoxMesh(1, 1, 1);
        m_ballMesh = CreateSphereMesh(0.5f, 0.01f);

        for (int i = 0; i < 50; ++i)
            CreatePyramid(Vector3.forward * 2.0f * i, 25);
    }

    void CreatePyramid(Vector3 pos, int size)
    {
        for (int y = 0; y < size; ++y)
        {
            var start = pos - Vector3.right * 0.5f * (size - 1 - y) + Vector3.up * (y + 0.5f);
            for (int i = 0; i < size - y; ++i)
            {
                // Create PxRigidDynamic
                var body = physics.createRigidDynamic(new PxTransform((start + Vector3.right * i).ToPxVec3()));
                // Add box shape
                body.createExclusiveShape(new PxBoxGeometry(0.5f, 0.5f, 0.5f), m_physicsMaterial);
                // Compute rigid body mass and inertia from its shapes (default density is 1000)
                body.updateMassAndInertia();
                // Add body to scene
                m_scene.addActor(body);

                m_boxList.addRigidActor(body);
            }
        }
    }

    void ThrowBall()
    {
        var pos = Camera.main.transform.position + Camera.main.transform.forward * 2 + Camera.main.transform.up * -2;
        var rot = Camera.main.transform.rotation;

        // Create PxRigidDynamic
        var body = physics.createRigidDynamic(new PxTransform(pos.ToPxVec3(), rot.ToPxQuat()));
        // Add sphere shape
        body.createExclusiveShape(new PxSphereGeometry(0.5f), m_physicsMaterial);
        // Set body mass and compute inertia
        body.setMassAndUpdateInertia(5000);
        // Set body velocity
        body.setLinearVelocity((Camera.main.transform.forward * 80).ToPxVec3());
        // Add body to scene
        m_scene.addActor(body);

        m_ballList.addRigidActor(body);
    }

    void UpdateInput()
    {
        m_throwBall = Input.GetKey(KeyCode.Space);
    }

    void UpdateCamera()
    {
        const float MOTION_SPEED = 20.0f;
        const float ROTATION_SPEED = 0.3f;
        float forward = 0, right = 0;
        if (Input.GetKey(KeyCode.W)) forward += 1;
        if (Input.GetKey(KeyCode.S)) forward -= 1;
        if (Input.GetKey(KeyCode.D)) right += 1;
        if (Input.GetKey(KeyCode.A)) right -= 1;
        float dt = Time.deltaTime;
        var cameraTransform = Camera.main.transform;
        cameraTransform.Translate(Vector3.forward * forward * MOTION_SPEED * dt + Vector3.right * right * MOTION_SPEED * dt, Space.Self);

        if (Input.GetMouseButtonDown(1))
        {
            m_mousePosition = Input.mousePosition;
        }
        if (Input.GetMouseButton(1))
        {
            var mouseDelta = Input.mousePosition - m_mousePosition;
            cameraTransform.Rotate(Vector3.up, mouseDelta.x * ROTATION_SPEED, Space.World);
            cameraTransform.Rotate(Vector3.left, mouseDelta.y * ROTATION_SPEED, Space.Self);
            m_mousePosition = Input.mousePosition;
        }
    }

    void RenderActorList(Camera camera, PxRigidActorList list, Mesh mesh, Material material)
    {
        int actorCount = (int)list.getNbRigidActors();
        for (int i = 0; i < actorCount; i += m_batch.Length)
        {
            int count = Mathf.Min(actorCount - i, m_batch.Length);
            list.getRigidActorMatrices(m_batch, i, count);
            Graphics.DrawMeshInstanced(mesh, 0, material, m_batch, count,
                                       null, UnityEngine.Rendering.ShadowCastingMode.On, true, 0, camera);
        }
    }

    PxDefaultCpuDispatcher m_cpuDispatcher;
    PxCudaContextManager m_cudaContextManager;
    PxScene m_scene;
    PxMaterial m_physicsMaterial;
    PxRigidStatic m_groundActor;
    Mesh m_groundMesh, m_boxMesh, m_ballMesh;
    Material m_groundMaterial, m_boxMaterial, m_ballMaterial;
    Vector3 m_mousePosition;
    bool m_throwBall = false;
    bool m_enableGRB = true;
    PxRigidActorList m_boxList, m_ballList;
    Matrix4x4[] m_batch = new Matrix4x4[1023];
    float m_simulationTime = 0;

    #endregion
}
