using NVIDIA.PhysX;
using NVIDIA.PhysX.UnityExtensions;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SampleContactReport : SampleBase
{
    #region Messages

    private void FixedUpdate()
    {
        if (m_scene != null)
        {
            if (m_throwBall) ThrowBall();

            m_scene.simulate(Time.fixedDeltaTime);
            m_scene.fetchResults(true);

            UpdateContactMesh();
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
        Graphics.DrawMesh(m_contactMesh, Matrix4x4.identity, m_contactMaterial, 0, camera);
    }

    #endregion

    #region Private

    void CreateScene()
    {
        // Create CPU dispatcher
        m_cpuDispatcher = PxCpuDispatcher.createDefault((uint)SystemInfo.processorCount);
        // Create EventCallback instance
        m_eventCallback = new EventCallback();

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
        foreach (var a in m_dynamicActors) a?.release();
        m_dynamicActors.Clear();
        Destroy(m_boxMesh);
        m_boxMesh = null;
        Destroy(m_ballMesh);
        m_ballMesh = null;
        Destroy(m_contactMesh);
        m_contactMesh = null;
        Destroy(m_activeBoxMaterial);
        m_activeBoxMaterial = null;
        Destroy(m_inactiveBoxMaterial);
        m_inactiveBoxMaterial = null;
        Destroy(m_activeBallMaterial);
        m_activeBallMaterial = null;
        Destroy(m_inactiveBallMaterial);
        m_inactiveBallMaterial = null;
        Destroy(m_contactMaterial);
        m_contactMaterial = null;
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

        m_contactMaterial = new Material(Resources.Load<Material>("UnlitColor"));
        m_contactMaterial.color = COLOR5;

        m_boxMesh = CreateBoxMesh(1, 1, 1);
        m_ballMesh = CreateSphereMesh(0.5f, 0.01f);
        m_contactMesh = CreateContactMesh();

        for (int i = 0; i < 1; ++i)
            CreatePyramid(Vector3.forward * 2.0f * i, 20);
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

                m_dynamicActors.Add(body);
                AddRenderActor(body, m_boxMesh, m_activeBoxMaterial, m_inactiveBoxMaterial);
                PxDefaultSimulationFilterShader.setCallbacksEnabled(body, true);
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

    Mesh CreateContactMesh()
    {
        Mesh mesh = new Mesh();
        mesh.MarkDynamic();
        mesh.vertices = m_eventCallback.positions;
        mesh.SetIndices(m_eventCallback.indices, MeshTopology.Lines, 0);
        return mesh;
    }

    void UpdateContactMesh()
    {
        m_contactMesh.vertices = m_eventCallback.positions;
        m_contactMesh.RecalculateBounds();
        m_eventCallback.reset();
    }

    // Inherit from PxSimulationEventCallback to override onContact() method
    class EventCallback : PxSimulationEventCallback
    {
        const int MAX_COUNT = 10000;
        public Vector3[] positions = new Vector3[MAX_COUNT];
        public int[] indices = Enumerable.Range(0, MAX_COUNT).ToArray();
        public int count = 0;

        public override void onContact(PxContactPairHeader pairHeader, PxContactPairList pairs)
        {
            for (uint i = 0; i < pairs.count && count < MAX_COUNT - 1; ++i)
            {
                var pair = pairs.get(i);
                uint contactCount = (uint)Mathf.Min(pair.contactCount, points.Length);
                pair.extractContacts(points, contactCount);
                for (uint j = 0; j < contactCount && count < MAX_COUNT - 1; ++j)
                {
                    var point = points[j];
                    positions[count++] = point.position.ToVector3();
                    positions[count++] = (point.position + point.impulse * 0.01f).ToVector3();
                }
            }
        }

        public void reset()
        {
            Array.Clear(positions, 0, MAX_COUNT);
            count = 0;
        }

        PxContactPairPoint[] points = new PxContactPairPoint[10];
    }

    EventCallback m_eventCallback;
    PxDefaultCpuDispatcher m_cpuDispatcher;
    PxScene m_scene;
    PxMaterial m_physicsMaterial;
    PxRigidStatic m_groundActor;
    Mesh m_groundMesh, m_boxMesh, m_ballMesh, m_contactMesh;
    Material m_groundMaterial, m_activeBoxMaterial, m_inactiveBoxMaterial, m_activeBallMaterial, m_inactiveBallMaterial, m_contactMaterial;
    List<PxRigidDynamic> m_dynamicActors = new List<PxRigidDynamic>();
    Vector3 m_mousePosition;
    bool m_throwBall = false;

    #endregion
}
