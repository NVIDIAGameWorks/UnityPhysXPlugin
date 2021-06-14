using NVIDIA.PhysX;
using NVIDIA.PhysX.UnityExtensions;
using System.Collections.Generic;
using UnityEngine;

public class SampleTriggerReport : SampleBase
{
    #region Messages

    //private void OnEnable()
    //{
    //    //initializePhysics();
    //    createScene();
    //}

    //private void OnDisable()
    //{
    //    destoryScene();
    //    //finalizePhysics();
    //}

    private void FixedUpdate()
    {
        if (m_scene != null)
        {
            if (m_throwBall) ThrowBall();

            m_scene.simulate(Time.fixedDeltaTime);
            m_scene.fetchResults(true);

            ProcessHitActors();
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

    protected override void OnRender(Camera camera)
    {
        base.OnRender(camera);
        RenderTriggers(camera);
    }

    #endregion

    #region Private

    void CreateScene()
    {
        // Create CPU dispatcher
        m_cpuDispatcher = PxCpuDispatcher.createDefault((uint)SystemInfo.processorCount);
        // Create EventCallback instance
        m_eventCallback = new EventCallback(this);

        // Create PxScene
        var sceneDesc = new PxSceneDesc(physics.getTolerancesScale());
        sceneDesc.cpuDispatcher = m_cpuDispatcher;
        sceneDesc.filterShader = PxDefaultSimulationFilterShader.function;
        sceneDesc.simulationEventCallback = m_eventCallback;
        sceneDesc.gravity = new PxVec3(0, -9.8f, 0);
        m_scene = physics.createScene(sceneDesc);
        sceneDesc.destroy();

        // Set PVD flags
        var pvdClient = m_scene.getScenePvdClient();
        if (pvdClient != null)
            pvdClient.setScenePvdFlags(PxPvdSceneFlag.TRANSMIT_CONTACTS | PxPvdSceneFlag.TRANSMIT_CONSTRAINTS | PxPvdSceneFlag.TRANSMIT_SCENEQUERIES);

        m_physicsMaterial = physics.createMaterial(0.5f, 0.5f, 0.05f);

        CreateGround();
        CreateObjects();
    }

    void DestoryScene()
    {
        foreach (var t in m_triggers) t.release();
        m_triggers.Clear();
        foreach (var a in m_dynamicActors) a?.release();
        m_dynamicActors.Clear();
        Destroy(m_boxMesh);
        m_boxMesh = null;
        Destroy(m_ballMesh);
        m_ballMesh = null;
        Destroy(m_activeBoxMaterial);
        m_activeBoxMaterial = null;
        Destroy(m_inactiveBoxMaterial);
        m_inactiveBoxMaterial = null;
        Destroy(m_activeBallMaterial);
        m_activeBallMaterial = null;
        Destroy(m_inactiveBallMaterial);
        m_inactiveBallMaterial = null;
        m_groundActor?.release();
        m_groundActor = null;
        Destroy(m_groundMesh);
        m_groundMesh = null;
        Destroy(m_groundMaterial);
        m_groundMaterial = null;
        m_scene?.release();
        m_scene = null;
        m_eventCallback?.destroy();
        m_eventCallback = null;
        m_physicsMaterial?.release();
        m_physicsMaterial = null;
        m_cpuDispatcher?.release();
        m_cpuDispatcher = null;
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
        m_activeBoxMaterial = new Material(Shader.Find("Standard"));
        m_activeBoxMaterial.color = COLOR4;
        m_inactiveBoxMaterial = new Material(Shader.Find("Standard"));
        m_inactiveBoxMaterial.color = COLOR3;

        m_activeBallMaterial = new Material(Shader.Find("Standard"));
        m_activeBallMaterial.color = COLOR5;
        m_inactiveBallMaterial = new Material(Shader.Find("Standard"));
        m_inactiveBallMaterial.color = COLOR6;

        m_boxMesh = CreateBoxMesh(1, 1, 1);
        m_ballMesh = CreateSphereMesh(0.5f, 0.01f);

        CreateTriggers(31);
        CreateBalls(31);
    }

    void CreateTriggers(int size)
    {
        for (int y = 0; y < size; ++y)
        {
            for (int x = 0; x < size; ++x)
            {
                if ((x + y) % 2 == 0)
                {
                    // Create PxRigidStatic (can be PxRigidDynamic as well)
                    var trigger = physics.createRigidStatic(new PxTransform(new PxVec3(x - size * 0.5f, 0.5f, y - size * 0.5f)));
                    // Add a shape with TRIGGER_SHAPE flag
                    trigger.createExclusiveShape(new PxBoxGeometry(0.5f, 0.5f, 0.5f), m_physicsMaterial, PxShapeFlag.TRIGGER_SHAPE);
                    // Add trigger to scene
                    m_scene.addActor(trigger);
                    // Custom data can be assigned to PxBase.userData field
                    trigger.userData = new TriggerData();
                    // Need to hold a reference to trigger to prevent GC from destroing it.
                    // (GC destroys only C# wrapper object. To destroy native object PxBase.release() should be called)
                    m_triggers.Add(trigger);
                }
            }
        }
    }

    void CreateBalls(int size)
    {
        for (int y = 0; y < size; ++y)
        {
            for (int x = 0; x < size; ++x)
            {
                if ((x + y) % 2 == 0)
                {
                    // Create PxRigidDynamic
                    var body = physics.createRigidDynamic(new PxTransform(new PxVec3(x - size * 0.5f, 2 + Random.Range(0.0f, 5.0f), y - size * 0.5f)));
                    // Add sphere shape
                    body.createExclusiveShape(new PxSphereGeometry(0.5f), m_physicsMaterial);
                    // Set body mass and compute inertia
                    body.setMassAndUpdateInertia(5000);
                    // Add body to scene
                    m_scene.addActor(body);

                    m_dynamicActors.Add(body);
                    AddRenderActor(body, m_ballMesh, m_activeBoxMaterial, m_inactiveBoxMaterial);
                }
            }
        }
    }

    void RenderTriggers(Camera camera)
    {
        if (m_eventCallback != null)
        {
            foreach (var t in m_triggers)
            {
                Graphics.DrawMesh(m_boxMesh, t.getGlobalMatrix().ToMatrix4x4(), (t.userData as TriggerData).hits > 0 ? m_activeBoxMaterial : m_inactiveBoxMaterial,
                                  0, camera, 0, null, UnityEngine.Rendering.ShadowCastingMode.On, true);
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

    void OnTrigger(PxTriggerPairList pairs)
    {
        for (uint i = 0; i < pairs.count; ++i)
        {
            var pair = pairs.get(i);
            var trigger = pair.triggerActor;
            if (trigger.userData is TriggerData)
            {
                var data = trigger.userData as TriggerData;
                if ((pair.status & PxPairFlag.NOTIFY_TOUCH_FOUND) != 0) data.hits++;
                if ((pair.status & PxPairFlag.NOTIFY_TOUCH_LOST) != 0) data.hits--;
            }

            // PhysX doesn't allow to change simulation state
            // from a callback. So we have to defer this.
            var otherActor = pair.otherActor as PxRigidDynamic;
            if (otherActor != null) m_hitActors.Add(otherActor);
        }
    }

    void ProcessHitActors()
    {
        foreach (var b in m_hitActors)
            b.setLinearVelocity((Vector3.up * 10).ToPxVec3() + b.getLinearVelocity() * 0.1f, true);

        m_hitActors.Clear();
    }

    class TriggerData
    {
        public int hits = 0;
    }

    // Inherit from PxSimulationEventCallback to override onTrigger() method
    class EventCallback : PxSimulationEventCallback
    {
        SampleTriggerReport sample;

        public EventCallback(SampleTriggerReport s)
        {
            sample = s;
        }

        public override void onTrigger(PxTriggerPairList pairs)
        {
            sample.OnTrigger(pairs);
        }
    }

    EventCallback m_eventCallback;
    PxDefaultCpuDispatcher m_cpuDispatcher;
    PxScene m_scene;
    PxMaterial m_physicsMaterial;
    PxRigidStatic m_groundActor;
    Mesh m_groundMesh, m_boxMesh, m_ballMesh;
    Material m_groundMaterial, m_activeBoxMaterial, m_inactiveBoxMaterial, m_activeBallMaterial, m_inactiveBallMaterial;
    List<PxRigidDynamic> m_dynamicActors = new List<PxRigidDynamic>();
    Vector3 m_mousePosition;
    bool m_throwBall = false;
    List<PxRigidStatic> m_triggers = new List<PxRigidStatic>();
    List<PxRigidDynamic> m_hitActors = new List<PxRigidDynamic>();

    #endregion
}
