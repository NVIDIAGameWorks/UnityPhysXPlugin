using NVIDIA.PhysX;
using NVIDIA.PhysX.UnityExtensions;
using System;
using System.Globalization;
using System.IO;
using UnityEngine;

public class SampleCarouselGRB : SampleBase
{
    #region Messages

    //private void OnEnable()
    //{
    //    //initializePhysics();
    //    createScene();
    //    m_time = 0;
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

            // Rotate the barrel
            m_time += Time.fixedDeltaTime;
            m_barrelActor.setKinematicTarget(new PxTransform(m_barrelActor.getGlobalPose().p, Quaternion.Euler(0, 0, -45 * m_time).ToPxQuat()));

            m_simulationTime = Time.realtimeSinceStartup;

            m_scene.simulate(Time.fixedDeltaTime);
            m_scene.fetchResults(true);

            m_simulationTime = Time.realtimeSinceStartup - m_simulationTime;

            // Update barrel position
            var pos = m_barrelActor.getGlobalPose();
            m_barrelObject.transform.position = pos.p.ToVector3();
            m_barrelObject.transform.rotation = pos.q.ToQuaternion();
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

        int partCount = 0;
        foreach (var l in m_lists) partCount += (int)l.getNbRigidActors();
        GUI.Label(new Rect(160, y++ * 20 + 5, 300, 50), "Rigid body count: " + (m_boxList.getNbRigidActors() + m_ballList.getNbRigidActors() + partCount));

        GUI.Label(new Rect(160, y++ * 20 + 5, 300, 50), "Simulation time: " + string.Format(m_culture, "{0:0.0} ms", m_simulationTime * 1000));

        GUI.contentColor = m_enableGRB ? Color.green : Color.red;
        m_enableGRB = GUI.Toggle(new Rect(160, y++ * 20 + 5, 300, 50), m_enableGRB, m_enableGRB ? "GPU Rigid Bodies ON" : "GPU Rigid Bodies OFF");
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
        RenderActorList(camera, m_boxList, m_boxMesh, m_skinMaterial);
        RenderActorList(camera, m_ballList, m_ballMesh, m_ballMaterial);
        for (int i = 0; i < m_lists.Length; ++i)
        {
            switch ((BodyParts)i)
            {
                case BodyParts.PELVIS: RenderActorList(camera, m_lists[i], m_meshes[i], m_pantsMaterial); break;
                case BodyParts.TORSO: RenderActorList(camera, m_lists[i], m_meshes[i], m_shirtMaterial); break;
                default: RenderActorList(camera, m_lists[i], m_meshes[i], m_skinMaterial); break;
            }
        }
        base.OnRender(camera);
    }

    #endregion

    #region Private

    void CreateScene()
    {
        m_cpuDispatcher = PxCpuDispatcher.createDefault((uint)SystemInfo.processorCount);

        var sceneDesc = new PxSceneDesc(physics.getTolerancesScale());
        sceneDesc.cpuDispatcher = m_cpuDispatcher;
        sceneDesc.filterShader = PxDefaultSimulationFilterShader.function;
        sceneDesc.gravity = new PxVec3(0, -9.8f, 0);
        sceneDesc.flags |= PxSceneFlag.ENABLE_CCD;

        sceneDesc.broadPhaseType = PxBroadPhaseType.ABP;
        if (m_enableGRB)
        {
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

        m_scene = physics.createScene(sceneDesc);
        sceneDesc.destroy();

        var pvdClient = m_scene.getScenePvdClient();
        if (pvdClient != null)
            pvdClient.setScenePvdFlags(PxPvdSceneFlag.TRANSMIT_CONTACTS | PxPvdSceneFlag.TRANSMIT_CONSTRAINTS | PxPvdSceneFlag.TRANSMIT_SCENEQUERIES);

        m_physicsMaterial = physics.createMaterial(0.5f, 0.5f, 0.05f);

        m_boxList = new PxRigidActorList();
        m_ballList = new PxRigidActorList();

        CreateGround();
        CreateBarrel();
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
        Destroy(m_skinMaterial);
        m_skinMaterial = null;
        Destroy(m_shirtMaterial);
        m_shirtMaterial = null;
        Destroy(m_pantsMaterial);
        m_pantsMaterial = null;
        Destroy(m_ballMaterial);
        m_ballMaterial = null;
        m_boxMesh = null;
        if (m_meshes != null) foreach (var m in m_meshes) Destroy(m);
        m_meshes = null;
        foreach (var l in m_lists) { l.releaseRigidActors(); l.destroy(); }
        m_barrelActor?.release();
        m_barrelActor = null;
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
        m_groundActor = physics.createRigidStatic(new PxTransform(Quaternion.Euler(0, 0, 90.0f).ToPxQuat()));
        m_groundActor.createExclusiveShape(new PxPlaneGeometry(), m_physicsMaterial);
        m_scene.addActor(m_groundActor);
        m_groundMesh = new Mesh();
        m_groundMesh.vertices = new[] { new Vector3(0, 1000, 1000), new Vector3(0, -1000, 1000), new Vector3(0, -1000, -1000), new Vector3(0, 1000, -1000) };
        m_groundMesh.normals = new[] { Vector3.right, Vector3.right, Vector3.right, Vector3.right };
        m_groundMesh.uv = new[] { Vector2.zero, Vector2.zero, Vector2.zero, Vector2.zero };
        m_groundMesh.triangles = new[] { 0, 1, 2, 0, 2, 3 };
        m_groundMesh.RecalculateBounds();
        m_groundMaterial = new Material(Shader.Find("Standard"));
        m_groundMaterial.color = new Color(0.9f, 0.8f, 0.7f) * 0.5f;// new Color(0.3f, 0.2f, 0.0f);// COLOR3;
        AddRenderActor(m_groundActor, m_groundMesh, m_groundMaterial);
    }

    void CreateBarrel()
    {
        m_barrelObject = GameObject.Find("Barrel");
        if (m_barrelObject)
        {
            m_barrelObject.transform.rotation = Quaternion.identity;
            m_barrelActor = physics.createRigidDynamic(m_barrelObject.transform.ToPxTransform());
            var shapeObjects = m_barrelObject.GetComponentsInChildren<MeshRenderer>();
            foreach (var s in shapeObjects)
            {
                //s.material = m_groundMaterial;
                var shape = m_barrelActor.createExclusiveShape(new PxBoxGeometry((s.transform.localScale * 0.5f).ToPxVec3()), m_physicsMaterial);
                shape.setLocalPose(new PxTransform(m_barrelObject.transform.InverseTransformPoint(s.transform.position).ToPxVec3(),
                                                   (Quaternion.Inverse(m_barrelObject.transform.rotation) * s.transform.rotation).ToPxQuat()));
            }
            m_barrelActor.setRigidBodyFlag(PxRigidBodyFlag.KINEMATIC, true);
            m_scene.addActor(m_barrelActor);
        }
    }

    void CreateObjects()
    {
        var instaceableMaterial = Resources.Load<Material>("Instanceable");

        m_skinMaterial = new Material(instaceableMaterial);
        m_skinMaterial.color = new Color(1.0f, 0.8f, 0.8f);// COLOR4;

        m_shirtMaterial = new Material(instaceableMaterial);
        m_shirtMaterial.color = Color.white;

        m_pantsMaterial = new Material(instaceableMaterial);
        m_pantsMaterial.color = new Color(0.2f, 0.2f, 1.0f);// COLOR4;

        m_ballMaterial = new Material(instaceableMaterial);
        m_ballMaterial.color = COLOR5;

        m_boxMesh = CreateBoxMesh(1, 1, 1);
        m_ballMesh = CreateSphereMesh(0.5f, 0.01f);

        CreateMeshesAndLists();

        var pos = m_barrelObject.transform.position;
        int sizeX = 6, sizeY = 6, sizeZ = 24;
        for (int z = 0; z < sizeZ; ++z)
            for (int y = 0; y < sizeY; ++y)
                for (int x = 0; x < sizeX; ++x)
                    CreateRagdoll(Vector3.right * (x - (sizeX - 1) * 0.5f) * 1.8f + Vector3.up * ((y - (sizeY - 1) * 0.5f) * 2.1f + pos.y - 1) + Vector3.forward * ((z - (sizeZ - 1) * 0.5f) * 0.7f - 0));
    }

    void CreateRagdoll(Vector3 pos)
    {
        var pelvis = CreateBodyPart(pos + Vector3.up * 1.0f, BodyParts.PELVIS);
        var torso = CreateBodyPart(pos + Vector3.up * 1.4f, BodyParts.TORSO);
        var head = CreateBodyPart(pos + Vector3.up * 1.82f, BodyParts.HEAD);
        var leg0r = CreateBodyPart(pos + Vector3.right * 0.125f + Vector3.up * 0.675f, BodyParts.LEG0);
        var leg1r = CreateBodyPart(pos + Vector3.right * 0.125f + Vector3.up * 0.225f, BodyParts.LEG1);
        var leg0l = CreateBodyPart(pos + Vector3.left * 0.125f + Vector3.up * 0.675f, BodyParts.LEG0);
        var leg1l = CreateBodyPart(pos + Vector3.left * 0.125f + Vector3.up * 0.225f, BodyParts.LEG1);
        var arm0r = CreateBodyPart(pos + Vector3.right * 0.35f + Vector3.up * 1.625f, BodyParts.ARM0);
        var arm1r = CreateBodyPart(pos + Vector3.right * 0.65f + Vector3.up * 1.625f, BodyParts.ARM1);
        var arm0l = CreateBodyPart(pos + Vector3.left * 0.35f + Vector3.up * 1.625f, BodyParts.ARM0);
        var arm1l = CreateBodyPart(pos + Vector3.left * 0.65f + Vector3.up * 1.625f, BodyParts.ARM1);

        var torso_pelvis = CreateBodyJoint(torso, pelvis, Vector3.down * 0.3f, Quaternion.identity, -10, 60, 0, 0);
        var head_torso = CreateBodyJoint(head, torso, Vector3.down * 0.125f, Quaternion.identity, -10, 60, 0, 0);
        var leg0r_pelvis = CreateBodyJoint(leg0r, pelvis, Vector3.up * 0.225f, Quaternion.identity, -90, 10, 0, 0);
        var leg1r_leg0r = CreateBodyJoint(leg1r, leg0r, Vector3.up * 0.225f, Quaternion.identity, -10, 90, 0, 0);
        var leg0l_pelvis = CreateBodyJoint(leg0l, pelvis, Vector3.up * 0.225f, Quaternion.identity, -90, 10, 0, 0);
        var leg1l_leg0l = CreateBodyJoint(leg1l, leg0l, Vector3.up * 0.225f, Quaternion.identity, -10, 90, 0, 0);
        var arm0r_torso = CreateBodyJoint(arm0r, torso, Vector3.left * 0.15f, Quaternion.Euler(0, 90, 0), -90, 90, 0, 0);
        var arm1r_arm0r = CreateBodyJoint(arm1r, arm0r, Vector3.left * 0.15f, Quaternion.Euler(0, 0, 90), -90, 0, 0, 0);
        var arm0l_torso = CreateBodyJoint(arm0l, torso, Vector3.right * 0.15f, Quaternion.Euler(0, 90, 0), -90, 90, 0, 0);
        var arm1l_arm0l = CreateBodyJoint(arm1l, arm0l, Vector3.right * 0.15f, Quaternion.Euler(0, 0, 90), -0, 90, 0, 0);
    }


    PxD6Joint CreateBodyJoint(PxRigidActor body0, PxRigidActor body1, Vector3 position, Quaternion rotation, float xMin, float xMax, float yLimit, float zLimit)
    {
        var p0 = body0.getGlobalPose().ToMatrix4x4().MultiplyPoint(position);
        var p1 = body1.getGlobalPose().ToMatrix4x4().inverse.MultiplyPoint(p0);
        var r0 = body0.getGlobalPose().q.ToQuaternion() * rotation;
        var r1 = Quaternion.Inverse(body1.getGlobalPose().q.ToQuaternion()) * r0;
        var joint = physics.createD6Joint(body0, new PxTransform(position.ToPxVec3(), rotation.ToPxQuat()), body1, new PxTransform(p1.ToPxVec3(), r1.ToPxQuat()));
        joint.setMotion(PxD6Axis.X, PxD6Motion.LOCKED);
        joint.setMotion(PxD6Axis.Y, PxD6Motion.LOCKED);
        joint.setMotion(PxD6Axis.Z, PxD6Motion.LOCKED);
        //joint.setMotion(PxD6Axis.TWIST, PxD6Motion.LOCKED);
        //joint.setMotion(PxD6Axis.SWING1, PxD6Motion.FREE);
        //joint.setMotion(PxD6Axis.SWING2, PxD6Motion.FREE);
        float stiffness = 3.0f;
        float damping = 1.0f;
        if (xMin == 0 && xMax == 0) joint.setMotion(PxD6Axis.TWIST, PxD6Motion.LOCKED);
        else
        {
            joint.setMotion(PxD6Axis.TWIST, PxD6Motion.LIMITED);
            var limit = joint.getTwistLimit();
            limit.lower = xMin * Mathf.Deg2Rad; limit.upper = xMax * Mathf.Deg2Rad;
            joint.setTwistLimit(limit);
            var drive = joint.getDrive(PxD6Drive.TWIST);
            drive.stiffness = stiffness;
            drive.damping = damping;
            joint.setDrive(PxD6Drive.TWIST, drive);
        }
        if (yLimit == 0) joint.setMotion(PxD6Axis.SWING1, PxD6Motion.LOCKED);
        else
        {
            joint.setMotion(PxD6Axis.SWING1, PxD6Motion.LIMITED);
            var limit = joint.getSwingLimit();
            limit.yAngle = yLimit * Mathf.Deg2Rad;
            joint.setSwingLimit(limit);
            var drive = joint.getDrive(PxD6Drive.SWING);
            drive.stiffness = stiffness;
            drive.damping = damping;
            joint.setDrive(PxD6Drive.SWING, drive);
        }
        if (zLimit == 0) joint.setMotion(PxD6Axis.SWING2, PxD6Motion.LOCKED);
        else
        {
            joint.setMotion(PxD6Axis.SWING2, PxD6Motion.LIMITED);
            var limit = joint.getSwingLimit();
            limit.zAngle = zLimit * Mathf.Deg2Rad;
            joint.setSwingLimit(limit);
            var drive = joint.getDrive(PxD6Drive.SWING);
            drive.stiffness = stiffness;
            drive.damping = damping;
            joint.setDrive(PxD6Drive.SWING, drive);
        }
        return joint;
    }

    PxRigidActor CreateBodyPart(Vector3 pos, BodyParts part)
    {
        var body = physics.createRigidDynamic(pos.ToPxTransform());
        body.createExclusiveShape(new PxBoxGeometry((BODY_PART_SIZE[(int)part] * 0.5f).ToPxVec3()), m_physicsMaterial);
        //body.updateMassAndInertia();
        m_lists[(int)part].addRigidActor(body);
        m_scene.addActor(body);
        return body;
    }

    void CreateMeshesAndLists()
    {
        m_meshes = new Mesh[(int)BodyParts.COUNT];
        m_lists = new PxRigidActorList[(int)BodyParts.COUNT];
        for (int i = 0; i < (int)BodyParts.COUNT; ++i)
        {
            m_meshes[i] = CreateBoxMesh(BODY_PART_SIZE[i]);
            m_lists[i] = new PxRigidActorList();
        }
    }

    void CreatePyramid(Vector3 pos, int size)
    {
        for (int y = 0; y < size; ++y)
        {
            var start = pos - Vector3.right * 0.5f * (size - 1 - y) + Vector3.up * (y + 0.5f);
            for (int i = 0; i < size - y; ++i)
            {
                var body = physics.createRigidDynamic(new PxTransform((start + Vector3.right * i).ToPxVec3()));
                body.createExclusiveShape(new PxBoxGeometry(0.5f, 0.5f, 0.5f), m_physicsMaterial);
                body.updateMassAndInertia();
                m_scene.addActor(body);
                m_boxList.addRigidActor(body);
            }
        }
    }

    void ThrowBall()
    {
        var pos = Camera.main.transform.position + Camera.main.transform.forward * 2 + Camera.main.transform.up * -2;
        var rot = Camera.main.transform.rotation;
        var body = physics.createRigidDynamic(new PxTransform(pos.ToPxVec3(), rot.ToPxQuat()));
        body.createExclusiveShape(new PxSphereGeometry(0.5f), m_physicsMaterial);
        body.setLinearVelocity((Camera.main.transform.forward * 80).ToPxVec3());
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

    enum BodyParts
    {
        PELVIS, TORSO, HEAD, LEG0, LEG1, ARM0, ARM1,
        COUNT
    }

    Vector3[] BODY_PART_SIZE = new[]
    {
        new Vector3(0.4f, 0.2f, 0.175f), // PELVIS
        new Vector3(0.4f, 0.6f, 0.2f), // TORSO
        new Vector3(0.25f, 0.25f, 0.25f), // HEAD
        new Vector3(0.15f, 0.45f, 0.15f), // LEG0
        new Vector3(0.125f, 0.45f, 0.125f), // LEG1
        new Vector3(0.3f, 0.12f, 0.12f), // ARM0
        new Vector3(0.3f, 0.1f, 0.1f), // ARM1
    };

    PxDefaultCpuDispatcher m_cpuDispatcher;
    PxCudaContextManager m_cudaContextManager;
    PxScene m_scene;
    PxMaterial m_physicsMaterial;
    PxRigidStatic m_groundActor;
    Mesh m_groundMesh, m_boxMesh, m_ballMesh;
    Material m_groundMaterial, m_skinMaterial, m_shirtMaterial, m_pantsMaterial, m_ballMaterial;
    Vector3 m_mousePosition;
    bool m_throwBall = false;
    bool m_enableGRB = true;
    PxRigidActorList m_boxList, m_ballList;
    Matrix4x4[] m_batch = new Matrix4x4[1023];
    GameObject m_barrelObject;
    PxRigidDynamic m_barrelActor;
    Mesh[] m_meshes = null;
    PxRigidActorList[] m_lists;
    float m_time = 0;
    float m_simulationTime = 0;

    #endregion
}
