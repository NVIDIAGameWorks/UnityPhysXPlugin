using NVIDIA.PhysX;
using NVIDIA.PhysX.UnityExtensions;
using System.Collections.Generic;
using UnityEngine;

public class SampleArticulation : SampleBase
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

    protected override void OnRender(Camera camera)
    {
        base.OnRender(camera);
        RenderLinks(camera);
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
        //sceneDesc.solverType = PxSolverType.TGS;
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
        m_articulationLinks.Clear();
        m_articulationLinks = null;
        foreach (var a in m_dynamicActors) a?.release();
        m_dynamicActors.Clear();
        Destroy(m_capsuleMesh);
        Destroy(m_ballMesh);
        Destroy(m_activeCapsuleMaterial);
        m_activeCapsuleMaterial = null;
        Destroy(m_inactiveCapsuleMaterial);
        m_inactiveCapsuleMaterial = null;
        Destroy(m_activeBallMaterial);
        m_activeBallMaterial = null;
        Destroy(m_inactiveBallMaterial);
        m_inactiveBallMaterial = null;
        m_capsuleMesh = null;
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
        m_activeCapsuleMaterial = new Material(Shader.Find("Standard"));
        m_activeCapsuleMaterial.color = COLOR4;
        m_activeCapsuleMaterial.enableInstancing = true;
        m_inactiveCapsuleMaterial = new Material(Shader.Find("Standard"));
        m_inactiveCapsuleMaterial.color = COLOR3;
        m_inactiveCapsuleMaterial.enableInstancing = true;

        m_activeBallMaterial = new Material(Shader.Find("Standard"));
        m_activeBallMaterial.color = COLOR5;
        m_activeBallMaterial.enableInstancing = true;
        m_inactiveBallMaterial = new Material(Shader.Find("Standard"));
        m_inactiveBallMaterial.color = COLOR6;
        m_inactiveBallMaterial.enableInstancing = true;

        float capsuleRadius = 0.4f, capsuleLength = 1.8f;
        m_capsuleMesh = CreateCapsuleMesh(capsuleRadius, capsuleLength);
        m_ballMesh = CreateSphereMesh(0.5f);

        m_articulationLinks = new List<PxArticulationLink>();

        const int SIZE_X = 10, SIZE_Y = 10;
        for (int y = 0; y < SIZE_Y; ++y)
            for (int x = 0; x < SIZE_X; ++x)
                CreateCapsuleChain(Vector3.right * 2.0f * (x - (SIZE_X - 1) * 0.5f) + Vector3.forward * 2.0f * (y - (SIZE_Y - 1) * 0.5f) + Vector3.up * 15, 10, capsuleRadius, capsuleLength);
    }

    void CreateCapsuleChain(Vector3 pos, int length, float capsuleRadius, float capsuleLength)
    {
        // Create PxArticulationReducedCoordinate
        var articulation = physics.createArticulationReducedCoordinate();
        // Make the root link fixed
        articulation.setArticulationFlag(PxArticulationFlag.FIX_BASE, true);
        // Create the root link
        var rootLink = articulation.createLink(null, new PxTransform(PxIDENTITY.PxIdentity));
        var parentLink = rootLink;
        for (int i = 0; i < length; ++i)
        {
            var parentTransform = parentLink.getGlobalPose();
            var linkTransform = new PxTransform((pos + Vector3.up * (length - i)).ToPxVec3(), Quaternion.Euler(0, 0, 90).ToPxQuat());
            // Create a link
            var link = articulation.createLink(parentLink, linkTransform);
            // Add link shape
            var shape = link.createExclusiveShape(new PxCapsuleGeometry(capsuleRadius, Mathf.Max(0, capsuleLength * 0.5f - capsuleRadius)), m_physicsMaterial);
            // Compute link mass and inertia from its shapes (default density is 1000)
            link.updateMassAndInertia();
            // Get link joint
            var joint = link.getInboundJoint().getArticulationJointReducedCoordinate();
            // Set joint properties
            joint.setJointType(PxArticulationJointType.SPHERICAL);
            joint.setMotion(PxArticulationAxis.X, PxArticulationMotion.LOCKED);
            joint.setMotion(PxArticulationAxis.Y, PxArticulationMotion.LOCKED);
            joint.setMotion(PxArticulationAxis.Z, PxArticulationMotion.LOCKED);
            joint.setMotion(PxArticulationAxis.TWIST, PxArticulationMotion.LOCKED);
            joint.setMotion(PxArticulationAxis.SWING1, PxArticulationMotion.FREE);
            joint.setMotion(PxArticulationAxis.SWING2, PxArticulationMotion.FREE);
            joint.setDrive(PxArticulationAxis.TWIST, 0, 0, 0);
            joint.setDrive(PxArticulationAxis.SWING1, 0, 0, 0);
            joint.setDrive(PxArticulationAxis.SWING2, 0, 0, 0);
            joint.setMaxJointVelocity(10);
            // Set joint child and parent poses
            var anchor = (pos + Vector3.up * (length - i + 0.5f)).ToPxVec3();
            var transform0 = linkTransform.transformInv(new PxTransform(anchor));
            joint.setChildPose(transform0);
            var transform1 = parentTransform.transformInv(new PxTransform(anchor));
            joint.setParentPose(transform1);

            m_articulationLinks.Add(link);
            parentLink = link;
        }
        m_scene.addArticulation(articulation);
    }

    void RenderLinks(Camera camera)
    {
        foreach (var l in m_articulationLinks)
            Graphics.DrawMesh(m_capsuleMesh, l.getGlobalMatrix().ToMatrix4x4(), m_activeCapsuleMaterial,
                              0, camera, 0, null, UnityEngine.Rendering.ShadowCastingMode.On, true);
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

    PxDefaultCpuDispatcher m_cpuDispatcher;
    PxScene m_scene;
    PxMaterial m_physicsMaterial;
    PxRigidStatic m_groundActor;
    Mesh m_groundMesh, m_capsuleMesh, m_ballMesh;
    Material m_groundMaterial, m_activeCapsuleMaterial, m_inactiveCapsuleMaterial, m_activeBallMaterial, m_inactiveBallMaterial;
    List<PxRigidDynamic> m_dynamicActors = new List<PxRigidDynamic>();
    Vector3 m_mousePosition;
    bool m_throwBall = false;
    List<PxArticulationLink> m_articulationLinks;

    #endregion
}
