//using NVIDIA.PhysX.Unity;
//using UnityEngine;
//using PX = NVIDIA.PhysX;
//using NVIDIA.PhysX.UnityExtensions;

//public class StackOfDolls : MonoBehaviour
//{
//    public PxScene m_scene = null;
//    public Vector3Int m_size = Vector3Int.one;
//    public Material m_material = null;

//    private void OnEnable()
//    {
//        if (m_scene != null)
//            m_scene.onSceneCreated += OnSceneCreated;

//        if (m_scene != null && m_scene.valid)
//            OnSceneCreated();

//        Camera.onPreCull += DrawMeshInstanced;
//    }

//    private void OnDisable()
//    {
//        if (m_meshes != null)
//        {
//            foreach (var m in m_meshes)
//                Destroy(m);
//        }

//        foreach (var l in m_lists)
//        {
//            l.releaseRigidActors();
//            l.destroy();
//        }

//        Camera.onPreCull -= DrawMeshInstanced;
//    }

//    void DrawMeshInstanced(Camera camera)
//    {
//        //return;
//        if (m_material != null)
//        {
//            for (int listIndex = 0; listIndex < m_lists.Length; ++listIndex)
//            {
//                var list = m_lists[listIndex];
//                int actorCount = (int)list.getNbRigidActors();
//                for (int i = 0; i < actorCount; i += m_batch.Length)
//                {
//                    int count = Mathf.Min(actorCount - i, m_batch.Length);
//                    list.getRigidActorMatrices(m_batch, i, count);
//                    Graphics.DrawMeshInstanced(m_meshes[listIndex], 0, m_material, m_batch, count,
//                                               null, UnityEngine.Rendering.ShadowCastingMode.On, true, 0, camera);
//                }
//            }
//        }
//    }

//    void OnSceneCreated()
//    {
//        m_scene.onSceneCreated -= OnSceneCreated;

//        CreateMeshesAndLists();

//        for (int z = 0; z < m_size.z; ++z)
//        {
//            for (int y = 0; y < m_size.y; ++y)
//            {
//                for (int x = 0; x < m_size.x; ++x)
//                {
//                    CreateRagdoll(new Vector3(x * 1.8f, y * 2.1f + 2, z));
//                }
//            }
//        }
//    }

//    void CreateRagdoll(Vector3 pos)
//    {
//        var pelvis = CreateBodyPart(pos + Vector3.up * 1.0f, BodyParts.PELVIS);
//        var torso = CreateBodyPart(pos + Vector3.up * 1.4f, BodyParts.TORSO);
//        var head = CreateBodyPart(pos + Vector3.up * 1.82f, BodyParts.HEAD);
//        var leg0r = CreateBodyPart(pos + Vector3.right * 0.125f + Vector3.up * 0.675f, BodyParts.LEG0);
//        var leg1r = CreateBodyPart(pos + Vector3.right * 0.125f + Vector3.up * 0.225f, BodyParts.LEG1);
//        var leg0l = CreateBodyPart(pos + Vector3.left * 0.125f + Vector3.up * 0.675f, BodyParts.LEG0);
//        var leg1l = CreateBodyPart(pos + Vector3.left * 0.125f + Vector3.up * 0.225f, BodyParts.LEG1);
//        var arm0r = CreateBodyPart(pos + Vector3.right * 0.35f + Vector3.up * 1.625f, BodyParts.ARM0);
//        var arm1r = CreateBodyPart(pos + Vector3.right * 0.65f + Vector3.up * 1.625f, BodyParts.ARM1);
//        var arm0l = CreateBodyPart(pos + Vector3.left * 0.35f + Vector3.up * 1.625f, BodyParts.ARM0);
//        var arm1l = CreateBodyPart(pos + Vector3.left * 0.65f + Vector3.up * 1.625f, BodyParts.ARM1);

//        var torso_pelvis = CreateBodyJoint(torso, pelvis, Vector3.down * 0.3f, Quaternion.identity, -10, 60, 0, 0);
//        var head_torso = CreateBodyJoint(head, torso, Vector3.down * 0.125f, Quaternion.identity, -10, 60, 0, 0);
//        var leg0r_pelvis = CreateBodyJoint(leg0r, pelvis, Vector3.up * 0.225f, Quaternion.identity, -90, 10, 0, 0);
//        var leg1r_leg0r = CreateBodyJoint(leg1r, leg0r, Vector3.up * 0.225f, Quaternion.identity, -10, 90, 0, 0);
//        var leg0l_pelvis = CreateBodyJoint(leg0l, pelvis, Vector3.up * 0.225f, Quaternion.identity, -90, 10, 0, 0);
//        var leg1l_leg0l = CreateBodyJoint(leg1l, leg0l, Vector3.up * 0.225f, Quaternion.identity, -10, 90, 0, 0);
//        var arm0r_torso = CreateBodyJoint(arm0r, torso, Vector3.left * 0.15f, Quaternion.Euler(0, 90, 0), -90, 90, 0, 0);
//        var arm1r_arm0r = CreateBodyJoint(arm1r, arm0r, Vector3.left * 0.15f, Quaternion.Euler(0, 0, 90), -90, 0, 0, 0);
//        var arm0l_torso = CreateBodyJoint(arm0l, torso, Vector3.right * 0.15f, Quaternion.Euler(0, 90, 0), -90, 90, 0, 0);
//        var arm1l_arm0l = CreateBodyJoint(arm1l, arm0l, Vector3.right * 0.15f, Quaternion.Euler(0, 0, 90), -0, 90, 0, 0);
//    }

//    PX.PxD6Joint CreateBodyJoint(PX.PxRigidActor body0, PX.PxRigidActor body1, Vector3 position, Quaternion rotation, float xMin, float xMax, float yLimit, float zLimit)
//    {
//        var p0 = body0.getGlobalPose().ToMatrix4x4().MultiplyPoint(position);
//        var p1 = body1.getGlobalPose().ToMatrix4x4().inverse.MultiplyPoint(p0);
//        var r0 = body0.getGlobalPose().q.ToQuaternion() * rotation;
//        var r1 = Quaternion.Inverse(body1.getGlobalPose().q.ToQuaternion()) * r0;
//        var joint = PxPhysics.apiPhysics.createD6Joint(body0, new PX.PxTransform(position.ToPxVec3(), rotation.ToPxQuat()), body1, new PX.PxTransform(p1.ToPxVec3(), r1.ToPxQuat()));
//        joint.setMotion(PX.PxD6Axis.X, PX.PxD6Motion.LOCKED);
//        joint.setMotion(PX.PxD6Axis.Y, PX.PxD6Motion.LOCKED);
//        joint.setMotion(PX.PxD6Axis.Z, PX.PxD6Motion.LOCKED);
//        //joint.setMotion(PX.PxD6Axis.TWIST, PX.PxD6Motion.LOCKED);
//        //joint.setMotion(PX.PxD6Axis.SWING1, PX.PxD6Motion.FREE);
//        //joint.setMotion(PX.PxD6Axis.SWING2, PX.PxD6Motion.FREE);
//        float stiffness = 3.0f;
//        float damping = 1.0f;
//        if (xMin == 0 && xMax == 0) joint.setMotion(PX.PxD6Axis.TWIST, PX.PxD6Motion.LOCKED);
//        else
//        {
//            joint.setMotion(PX.PxD6Axis.TWIST, PX.PxD6Motion.LIMITED);
//            var limit = joint.getTwistLimit();
//            limit.lower = xMin * Mathf.Deg2Rad; limit.upper = xMax * Mathf.Deg2Rad;
//            joint.setTwistLimit(limit);
//            var drive = joint.getDrive(PX.PxD6Drive.TWIST);
//            drive.stiffness = stiffness;
//            drive.damping = damping;
//            joint.setDrive(PX.PxD6Drive.TWIST, drive);
//        }
//        if (yLimit == 0) joint.setMotion(PX.PxD6Axis.SWING1, PX.PxD6Motion.LOCKED);
//        else
//        {
//            joint.setMotion(PX.PxD6Axis.SWING1, PX.PxD6Motion.LIMITED);
//            var limit = joint.getSwingLimit();
//            limit.yAngle = yLimit * Mathf.Deg2Rad;
//            joint.setSwingLimit(limit);
//            var drive = joint.getDrive(PX.PxD6Drive.SWING);
//            drive.stiffness = stiffness;
//            drive.damping = damping;
//            joint.setDrive(PX.PxD6Drive.SWING, drive);
//        }
//        if (zLimit == 0) joint.setMotion(PX.PxD6Axis.SWING2, PX.PxD6Motion.LOCKED);
//        else
//        {
//            joint.setMotion(PX.PxD6Axis.SWING2, PX.PxD6Motion.LIMITED);
//            var limit = joint.getSwingLimit();
//            limit.zAngle = zLimit * Mathf.Deg2Rad;
//            joint.setSwingLimit(limit);
//            var drive = joint.getDrive(PX.PxD6Drive.SWING);
//            drive.stiffness = stiffness;
//            drive.damping = damping;
//            joint.setDrive(PX.PxD6Drive.SWING, drive);
//        }
//        return joint;
//    }

//    PX.PxRigidActor CreateBodyPart(Vector3 pos, BodyParts part)
//    {
//        var body = PxPhysics.apiPhysics.createRigidDynamic(pos.ToPxTransform());
//        //var shape = PxPhysics.apiPhysics.createShape(new PX.PxBoxGeometry((BODY_PART_SIZE[(int)part] * 0.5f).ToPxVec3()), PxPhysics.defaultMaterial);
//        body.createExclusiveShape(new PX.PxBoxGeometry((BODY_PART_SIZE[(int)part] * 0.5f).ToPxVec3()), PxPhysics.defaultMaterial);
//        //body.attachShape(shape);
//        //shape.release();
//        //body.updateMassAndInertia();
//        m_lists[(int)part].addRigidActor(body);
//        m_scene.apiScene.addActor(body);
//        return body;
//    }

//    void CreateMeshesAndLists()
//    {
//        m_meshes = new Mesh[(int)BodyParts.COUNT];
//        m_lists = new PX.PxRigidActorList[(int)BodyParts.COUNT];
//        for (int i = 0; i < (int)BodyParts.COUNT; ++i)
//        {
//            m_meshes[i] = CreateBoxMesh(BODY_PART_SIZE[i]);
//            m_lists[i] = new PX.PxRigidActorList();
//        }
//    }

//    Mesh CreateBoxMesh(Vector3 s)
//    {
//        return CreateBoxMesh(s.x, s.y, s.z);
//    }
//    Mesh CreateBoxMesh(float sx, float sy, float sz)
//    {
//        Vector3[] positions = new[] { new Vector3(0.5f * sx, -0.5f * sy, 0.5f * sz), new Vector3(-0.5f * sx, -0.5f * sy, 0.5f * sz), new Vector3(0.5f * sx, 0.5f * sy, 0.5f * sz), new Vector3(-0.5f * sx, 0.5f * sy, 0.5f * sz), new Vector3(0.5f * sx, 0.5f * sy, -0.5f * sz), new Vector3(-0.5f * sx, 0.5f * sy, -0.5f * sz), new Vector3(0.5f * sx, -0.5f * sy, -0.5f * sz), new Vector3(-0.5f * sx, -0.5f * sy, -0.5f * sz), new Vector3(0.5f * sx, 0.5f * sy, 0.5f * sz), new Vector3(-0.5f * sx, 0.5f * sy, 0.5f * sz), new Vector3(0.5f * sx, 0.5f * sy, -0.5f * sz), new Vector3(-0.5f * sx, 0.5f * sy, -0.5f * sz), new Vector3(0.5f * sx, -0.5f * sy, -0.5f * sz), new Vector3(0.5f * sx, -0.5f * sy, 0.5f * sz), new Vector3(-0.5f * sx, -0.5f * sy, 0.5f * sz), new Vector3(-0.5f * sx, -0.5f * sy, -0.5f * sz), new Vector3(-0.5f * sx, -0.5f * sy, 0.5f * sz), new Vector3(-0.5f * sx, 0.5f * sy, 0.5f * sz), new Vector3(-0.5f * sx, 0.5f * sy, -0.5f * sz), new Vector3(-0.5f * sx, -0.5f * sy, -0.5f * sz), new Vector3(0.5f * sx, -0.5f * sy, -0.5f * sz), new Vector3(0.5f * sx, 0.5f * sy, -0.5f * sz), new Vector3(0.5f * sx, 0.5f * sy, 0.5f * sz), new Vector3(0.5f * sx, -0.5f * sy, 0.5f * sz), };
//        Vector3[] normals = new[] { new Vector3(0f, 0f, 1f), new Vector3(0f, 0f, 1f), new Vector3(0f, 0f, 1f), new Vector3(0f, 0f, 1f), new Vector3(0f, 1f, 0f), new Vector3(0f, 1f, 0f), new Vector3(0f, 0f, -1f), new Vector3(0f, 0f, -1f), new Vector3(0f, 1f, 0f), new Vector3(0f, 1f, 0f), new Vector3(0f, 0f, -1f), new Vector3(0f, 0f, -1f), new Vector3(0f, -1f, 0f), new Vector3(0f, -1f, 0f), new Vector3(0f, -1f, 0f), new Vector3(0f, -1f, 0f), new Vector3(-1f, 0f, 0f), new Vector3(-1f, 0f, 0f), new Vector3(-1f, 0f, 0f), new Vector3(-1f, 0f, 0f), new Vector3(1f, 0f, 0f), new Vector3(1f, 0f, 0f), new Vector3(1f, 0f, 0f), new Vector3(1f, 0f, 0f), };
//        Vector2[] uvs = new[] { new Vector2(0f, 0f), new Vector2(1f, 0f), new Vector2(0f, 1f), new Vector2(1f, 1f), new Vector2(0f, 1f), new Vector2(1f, 1f), new Vector2(0f, 1f), new Vector2(1f, 1f), new Vector2(0f, 0f), new Vector2(1f, 0f), new Vector2(0f, 0f), new Vector2(1f, 0f), new Vector2(0f, 0f), new Vector2(0f, 1f), new Vector2(1f, 1f), new Vector2(1f, 0f), new Vector2(0f, 0f), new Vector2(0f, 1f), new Vector2(1f, 1f), new Vector2(1f, 0f), new Vector2(0f, 0f), new Vector2(0f, 1f), new Vector2(1f, 1f), new Vector2(1f, 0f), };
//        int[] indices = new[] { 0, 2, 3, 0, 3, 1, 8, 4, 5, 8, 5, 9, 10, 6, 7, 10, 7, 11, 12, 13, 14, 12, 14, 15, 16, 17, 18, 16, 18, 19, 20, 21, 22, 20, 22, 23, };
//        var mesh = new Mesh();
//        mesh.vertices = positions;
//        mesh.normals = normals;
//        mesh.uv = uvs;
//        mesh.triangles = indices;
//        mesh.RecalculateBounds();
//        return mesh;
//    }

//    enum BodyParts
//    {
//        PELVIS, TORSO, HEAD, LEG0, LEG1, ARM0, ARM1,
//        COUNT
//    }

//    Vector3[] BODY_PART_SIZE = new[]
//    {
//        new Vector3(0.4f, 0.2f, 0.175f), // PELVIS
//        new Vector3(0.4f, 0.6f, 0.2f), // TORSO
//        new Vector3(0.25f, 0.25f, 0.25f), // HEAD
//        new Vector3(0.15f, 0.45f, 0.15f), // LEG0
//        new Vector3(0.125f, 0.45f, 0.125f), // LEG1
//        new Vector3(0.3f, 0.12f, 0.12f), // ARM0
//        new Vector3(0.3f, 0.1f, 0.1f), // ARM1
//    };

//    Mesh[] m_meshes = null;
//    PX.PxRigidActorList[] m_lists;
//    Matrix4x4[] m_batch = new Matrix4x4[1023];
//}
