using NVIDIA.PhysX;
using NVIDIA.PhysX.UnityExtensions;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class SampleHeightField : SampleBase
{
    #region Messages

    private void FixedUpdate()
    {
        if (m_scene != null)
        {
            if (m_throwBall) ThrowBall();

            m_scene.simulate(Time.fixedDeltaTime);
            m_scene.fetchResults(true);
        }
    }

    private void Update()
    {
        UpdateCamera();
        UpdateInput();
        UpdatePicker(m_scene);
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

    #endregion

    #region Private

    void CreateScene()
    {
        // Create CPU dispatcher
        m_cpuDispatcher = PxCpuDispatcher.createDefault((uint)SystemInfo.processorCount);

        // Create PxScene
        var sceneDesc = new PxSceneDesc(physics.getTolerancesScale());
        sceneDesc.cpuDispatcher = m_cpuDispatcher;
        sceneDesc.filterShader = PxDefaultSimulationFilterShader.function;
        sceneDesc.gravity = new PxVec3(0, -9.8f, 0);
        sceneDesc.broadPhaseType = PxBroadPhaseType.ABP;
        sceneDesc.flags |= PxSceneFlag.ENABLE_CCD; // Enable CCD
        m_scene = physics.createScene(sceneDesc);
        sceneDesc.destroy();

        // Set PVD flags
        var pvdClient = m_scene.getScenePvdClient();
        if (pvdClient != null)
            pvdClient.setScenePvdFlags(PxPvdSceneFlag.TRANSMIT_CONTACTS | PxPvdSceneFlag.TRANSMIT_CONSTRAINTS | PxPvdSceneFlag.TRANSMIT_SCENEQUERIES);

        m_physicsMaterial = physics.createMaterial(0.5f, 0.5f, 0.05f);

        // Create PxCooking
        m_cooking = foundation.createCooking(PxVersion.PX_PHYSICS_VERSION, new PxCookingParams(physics.getTolerancesScale()));

        CreateGround();
        CreateObjects();
    }

    void DestoryScene()
    {
        foreach (var a in m_dynamicActors) a?.release();
        m_dynamicActors.Clear();
        Destroy(m_gemMesh);
        Destroy(m_ballMesh);
        Destroy(m_activeGemMaterial);
        m_activeGemMaterial = null;
        Destroy(m_inactiveGemMaterial);
        m_inactiveGemMaterial = null;
        Destroy(m_activeBallMaterial);
        m_activeBallMaterial = null;
        Destroy(m_inactiveBallMaterial);
        m_inactiveBallMaterial = null;
        m_gemMesh = null;
        m_groundActor?.release();
        m_groundActor = null;
        Destroy(m_groundMesh);
        m_groundMesh = null;
        Destroy(m_groundMaterial);
        m_groundMaterial = null;
        m_heightField?.release();
        m_heightField = null;
        m_scene?.release();
        m_scene = null;
        m_physicsMaterial?.release();
        m_physicsMaterial = null;
        m_cpuDispatcher?.release();
        m_cpuDispatcher = null;
        Destroy(m_terrainObject);
        m_cooking?.release();
        m_cooking = null;
        m_gemConvexMesh?.release();
        m_gemConvexMesh = null;
    }

    void CreateGround()
    {
        // Create PxRigidStatic for ground
        m_groundActor = physics.createRigidStatic(new PxTransform(PxIDENTITY.PxIdentity));
        // Add plane shape
        m_groundActor.createExclusiveShape(new PxPlaneGeometry(), m_physicsMaterial);
        m_groundActor.getShape(0).setLocalPose(new PxTransform(Quaternion.Euler(0, 0, 90.0f).ToPxQuat()));

        m_groundMesh = new Mesh();
        m_groundMesh.vertices = new[] { new Vector3(1000, 0, 1000), new Vector3(1000, 0, -1000), new Vector3(-1000, 0, -1000), new Vector3(-1000, 0, 1000) };
        m_groundMesh.normals = new[] { Vector3.up, Vector3.up, Vector3.up, Vector3.up };
        m_groundMesh.uv = new[] { Vector2.zero, Vector2.zero, Vector2.zero, Vector2.zero };
        m_groundMesh.triangles = new[] { 0, 1, 2, 0, 2, 3 };
        m_groundMesh.RecalculateBounds();
        m_groundMaterial = new Material(Shader.Find("Standard"));
        m_groundMaterial.color = COLOR3;
        AddRenderActor(m_groundActor, m_groundMesh, m_groundMaterial);

        CreateTerrain();

        var terrain = m_terrainObject.GetComponent<Terrain>();
        var terrainSize = terrain.terrainData.size;
        var terrainResolution = terrain.terrainData.heightmapResolution;

        // Add height field shape
        m_groundActor.createExclusiveShape(new PxHeightFieldGeometry(m_heightField, terrainSize.y / 0x7fff, terrainSize.z / terrainResolution, terrainSize.x / terrainResolution), m_physicsMaterial);
        m_groundActor.getShape(1).setLocalPose(new PxTransform(-terrainSize.x * 0.5f, 0, -terrainSize.z * 0.5f));

        // Add ground actor to scene
        m_scene.addActor(m_groundActor);
    }

    void CreateTerrain()
    {
        const int RESOLUTION = 513;
        const float SIZE = 200.0f;

        // Create Unity game object
        m_terrainObject = new GameObject("Terrain");
        m_terrainObject.transform.parent = transform;
        m_terrainObject.transform.position = new Vector3(-SIZE * 0.5f, 0, -SIZE * 0.5f);
        var terrain = m_terrainObject.AddComponent<Terrain>();
        //terrain.materialType = Terrain.MaterialType.Custom;
        terrain.materialTemplate = m_groundMaterial;
        terrain.terrainData = new TerrainData();
        terrain.terrainData.heightmapResolution = RESOLUTION;
        terrain.terrainData.size = new Vector3(SIZE, 10.0f, SIZE);
        int res = RESOLUTION;
        var heights = new float[res, res];
        for (int y = 0; y < res; ++y)
        {
            for (int x = 0; x < res; ++x)
            {
                float t = Mathf.Clamp01(1.0f - new Vector2(2 * (float)x / (float)res - 1, 2 * (float)y / (float)res - 1).magnitude * 1.0f);
                heights[x, y] = t * t * (0.5f + 0.5f * Mathf.Sin(t * 25));
            }
        }
        terrain.terrainData.SetHeights(0, 0, heights);

        // Create PhysX geometry
        var samples = new PxHeightFieldSample[res * res];
        for (int y = 0; y < res; ++y)
        {
            for (int x = 0; x < res; ++x)
            {
                PxHeightFieldSample sample = new PxHeightFieldSample();
                sample.height = (short)(heights[x, y] * 0x7fff);
                sample.materialIndex0 = sample.materialIndex1 = 0;
                sample.clearTessFlag();
                samples[x + y * res] = sample;
            }
        }

        // Managed memory should be pinned before passing it to native function
        var pinSamples = GCHandle.Alloc(samples, GCHandleType.Pinned);

        // Allocate and initialize PxHeightFieldDesc
        var desc = new PxHeightFieldDesc();
        desc.nbRows = RESOLUTION;
        desc.nbColumns = RESOLUTION;
        desc.samples.stride = (uint)Marshal.SizeOf<PxHeightFieldSample>();
        desc.samples.data = Marshal.UnsafeAddrOfPinnedArrayElement(samples, 0);

        // Create PxHeightField
        m_heightField = m_cooking.createHeightField(desc, physics.getPhysicsInsertionCallback());

        // Unpin managed memory
        pinSamples.Free();
    }

    void CreateObjects()
    {
        m_activeGemMaterial = new Material(Shader.Find("Standard"));
        m_activeGemMaterial.color = COLOR4;
        m_activeGemMaterial.enableInstancing = true;
        m_inactiveGemMaterial = new Material(Shader.Find("Standard"));
        m_inactiveGemMaterial.color = COLOR3;
        m_inactiveGemMaterial.enableInstancing = true;

        m_activeBallMaterial = new Material(Shader.Find("Standard"));
        m_activeBallMaterial.color = COLOR5;
        m_activeBallMaterial.enableInstancing = true;
        m_inactiveBallMaterial = new Material(Shader.Find("Standard"));
        m_inactiveBallMaterial.color = COLOR6;
        m_inactiveBallMaterial.enableInstancing = true;

        m_ballMesh = CreateSphereMesh(0.5f, 0.01f);

        m_gemConvexMesh = CreateGemConvexMesh(m_cooking);
        m_gemMesh = CreateMeshFromConvexMesh(m_gemConvexMesh);

        CreateStack(10, 20, 10);
    }

    void CreateStack(int sizeX, int sizeY, int sizeZ)
    {
        for (int z = 0; z < sizeZ; ++z)
        {
            for (int y = 0; y < sizeY; ++y)
            {
                for (int x = 0; x < sizeX; ++x)
                {
                    // Create PxRigidDynamic
                    var body = physics.createRigidDynamic(new PxTransform(new PxVec3((x - sizeX * 0.5f + 0.5f) * 2, y * 1.5f + 10.0f, (z - sizeZ * 0.5f + 0.5f) * 2)));
                    // Add convex mesh shape
                    body.createExclusiveShape(new PxConvexMeshGeometry(m_gemConvexMesh), m_physicsMaterial);
                    // Compute rigid body mass and inertia from its shapes (default density is 1000)
                    body.updateMassAndInertia();
                    // Add body to scene
                    m_scene.addActor(body);

                    m_dynamicActors.Add(body);
                    AddRenderActor(body, m_gemMesh, m_activeGemMaterial, m_inactiveGemMaterial);
                }
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
        // Enable CCD. Scene flag ENABLE_CCD should be set too
        body.setRigidBodyFlag(PxRigidBodyFlag.ENABLE_CCD, true);
        // Add body to scene
        m_scene.addActor(body);

        m_dynamicActors.Add(body);
        AddRenderActor(body, m_ballMesh, m_activeBallMaterial, m_inactiveBallMaterial);
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

    PxDefaultCpuDispatcher m_cpuDispatcher;
    PxScene m_scene;
    PxCooking m_cooking;
    PxMaterial m_physicsMaterial;
    PxRigidStatic m_groundActor;
    GameObject m_terrainObject;
    PxHeightField m_heightField;
    Mesh m_groundMesh, m_gemMesh, m_ballMesh;
    Material m_groundMaterial, m_activeGemMaterial, m_inactiveGemMaterial, m_activeBallMaterial, m_inactiveBallMaterial;
    List<PxRigidDynamic> m_dynamicActors = new List<PxRigidDynamic>();
    Vector3 m_mousePosition;
    bool m_throwBall = false;
    PxConvexMesh m_gemConvexMesh;

    #endregion
}
