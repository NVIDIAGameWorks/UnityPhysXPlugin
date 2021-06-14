using UnityEngine;
using NVIDIA.PhysX;
using NVIDIA.PhysX.UnityExtensions;

public class TestPhysX : MonoBehaviour
{
    #region Fields

    public bool m_draw = true;

    #endregion

    #region Messages

    private void OnEnable()
    {
        m_allocatorCallback = new PxDefaultAllocator();
        m_errorCallback = new PxDefaultErrorCallback();
        m_foundation = PxFoundation.create(PxVersion.PX_PHYSICS_VERSION, m_allocatorCallback, m_errorCallback);
        m_physics = m_foundation.createPhysics(PxVersion.PX_PHYSICS_VERSION);
        m_cpuDispatcher = PxCpuDispatcher.createDefault((uint)SystemInfo.processorCount);
        m_eventCallback = new EventCallback();
        if (m_physics != null)
        {
            var sceneDesc = new PxSceneDesc(m_physics.getTolerancesScale());
            sceneDesc.cpuDispatcher = m_cpuDispatcher;
            sceneDesc.gravity = new PxVec3(0, -9.8f, 0);
            sceneDesc.solverType = PxSolverType.TGS;
            sceneDesc.frictionType = PxFrictionType.TWO_DIRECTIONAL;
            sceneDesc.filterShader = PxDefaultSimulationFilterShader.function;
            sceneDesc.simulationEventCallback = m_eventCallback;
            m_scene = m_physics.createScene(sceneDesc);
            if (m_scene != null)
            {
                var defaultMaterial = m_physics.createMaterial(0.5f, 0.5f, 0.05f);
                var ground = m_physics.createRigidStatic(new PxTransform(Quaternion.Euler(0, 0, 90.0f).ToPxQuat()));
                ground.createExclusiveShape(new PxPlaneGeometry(), defaultMaterial);
                m_scene.addActor(ground);
                PxDefaultSimulationFilterShader.setCallbacksEnabled(ground, true);
                //for (int i = 0; i < 20; ++i)
                //{
                //    var body = m_physics.createRigidDynamic(new PxTransform(new PxVec3(0, 2 + i, 0)));
                //    body.createExclusiveShape(new PxSphereGeometry(0.5f), defaultMaterial);
                //    m_scene.addActor(body);
                //}
                //for (int i = 0; i < 20; ++i)
                //{
                //    var body = m_physics.createRigidDynamic(new PxTransform(new PxVec3(4, 2 + i, 0), Quaternion.Euler(0, i % 2 == 0 ? 90 : 0, 0).ToPxQuat()));
                //    body.createExclusiveShape(new PxCapsuleGeometry(0.5f, 0.5f), defaultMaterial);
                //    m_scene.addActor(body);
                //}
                //for (int i = 0; i < 1000; ++i)
                //{
                //    var body = m_physics.createRigidDynamic(new PxTransform(new PxVec3(2, 2 + i, 0)));
                //    body.createExclusiveShape(new PxBoxGeometry(i % 2 == 0 ? 0.25f : 1.0f, 0.5f, i % 2 == 0 ? 1.0f : 0.25f), defaultMaterial);
                //    m_scene.addActor(body);
                //}
                for (int i = 0; i < 30; ++i)
                {
                    var body0 = m_physics.createRigidDynamic(new PxTransform(new PxVec3(4, 2 + i, 0), Quaternion.Euler(0.01f, 0, 0).ToPxQuat()));
                    body0.createExclusiveShape(new PxCapsuleGeometry(0.5f, 0.5f), defaultMaterial);
                    body0.updateMassAndInertia();
                    m_scene.addActor(body0);
                    var body1 = m_physics.createRigidDynamic(new PxTransform(new PxVec3(5, 2 + i, 0), Quaternion.Euler(0.0001f, 0, 0).ToPxQuat()));
                    body1.createExclusiveShape(new PxCapsuleGeometry(0.25f, 0.5f), defaultMaterial);
                    body1.createExclusiveShape(new PxSphereGeometry(0.4f), defaultMaterial);
                    body1.getShape(1).setLocalPose(new PxTransform(new PxVec3(0.5f, 0, 0)));
                    body1.updateMassAndInertia();
                    m_scene.addActor(body1);
                    var joint = m_physics.createD6Joint(body0, new PxTransform(new PxVec3(0.5f, 0, 0)), body1, new PxTransform(new PxVec3(-0.5f, 0, 0)));
                    joint.setMotion(PxD6Axis.X, PxD6Motion.LOCKED);
                    joint.setMotion(PxD6Axis.Y, PxD6Motion.LOCKED);
                    joint.setMotion(PxD6Axis.Z, PxD6Motion.LOCKED);
                    joint.setMotion(PxD6Axis.TWIST, PxD6Motion.LOCKED);
                    joint.setMotion(PxD6Axis.SWING1, PxD6Motion.LIMITED);
                    joint.setMotion(PxD6Axis.SWING2, PxD6Motion.LIMITED);
                    var limit = joint.getSwingLimit();
                    limit.yAngle = 90.0f * Mathf.Deg2Rad;
                    limit.zAngle = 90.0f * Mathf.Deg2Rad;
                    joint.setSwingLimit(limit);
                }
            }
        }
    }

    private void OnDisable()
    {
        if (m_scene != null) m_scene.release();
        if (m_eventCallback != null) m_eventCallback.destroy();
        if (m_physics != null) m_physics.release();
        if (m_cpuDispatcher != null) m_cpuDispatcher.release();
        if (m_foundation != null) m_foundation.release();
        if (m_errorCallback != null) m_errorCallback.destroy();
        if (m_allocatorCallback != null) m_allocatorCallback.destroy();
    }

    private void FixedUpdate()
    {
        if (m_scene != null)
        {
            m_scene.simulate(Time.fixedDeltaTime);
            m_scene.fetchResults(true);
        }
    }

    private void OnDrawGizmos()
    {
        TestRaycast();

        if (m_scene != null && m_draw) Draw(m_scene);
    }

    #endregion

    #region Private
    void TestRaycast()
    {
        if (m_scene != null)
        {
            var origin = Vector3.right * 10 + Vector3.up * 0.5f;
            var unitDir = Quaternion.Euler(0, 30.0f * Time.time, 0) * Vector3.left;
            var distance = 20.0f;
            var buffer = new PxRaycastBuffer1();
            if (m_scene.raycast(origin.ToPxVec3(), unitDir.ToPxVec3(), distance, buffer))
            {
                var hit = buffer.getAnyHit(0);
                //Debug.Log("Hit: " + hit.distance);
                distance = hit.distance;
            }

            Gizmos.color = Color.red;
            Gizmos.DrawLine(origin, origin + unitDir * distance);
        }
    }

    void Draw(PxScene _scene)
    {
        var actorCount = _scene.getNbActors();
        for (uint i = 0; i < actorCount; ++i) Draw(_scene.getActor(i));
    }

    void Draw(PxActor _actor)
    {
        var rigidActor = _actor.getRigidActor();
        if (rigidActor != null)
        {
            Gizmos.matrix = rigidActor.getGlobalPose().ToMatrix4x4();
            if (rigidActor.getRigidBody() != null) Draw(rigidActor.getRigidBody());
            if (rigidActor.getRigidStatic() != null) Gizmos.color = Color.gray;
            else if (rigidActor.getRigidDynamic() != null)
            {
                if (rigidActor.getRigidDynamic().isSleeping()) Gizmos.color = Color.green * Color.gray;
                else Gizmos.color = Color.green;
            }
            for (uint i = 0; i < rigidActor.getNbShapes(); ++i)
                Draw(rigidActor.getShape(i));
        }
    }

    void Draw(PxRigidBody _rigidBody)
    {
        var oldMatrix = Gizmos.matrix;
        Gizmos.matrix = oldMatrix * _rigidBody.getCMassLocalPose().ToMatrix4x4();
        float s = 0.1f;
        Gizmos.color = Color.red;
        Gizmos.DrawLine(Vector3.left * s, Vector3.right * s);
        Gizmos.color = Color.green;
        Gizmos.DrawLine(Vector3.down * s, Vector3.up * s);
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(Vector3.back * s, Vector3.forward * s);
        Gizmos.matrix = oldMatrix;
    }

    void Draw(PxShape _shape)
    {
        var oldMatrix = Gizmos.matrix;
        Gizmos.matrix = oldMatrix * _shape.getLocalPose().ToMatrix4x4();
        switch (_shape.getGeometryType())
        {
            case PxGeometryType.SPHERE: Draw(_shape.getGeometry().sphere()); break;
            case PxGeometryType.CAPSULE: Draw(_shape.getGeometry().capsule()); break;
            case PxGeometryType.BOX: Draw(_shape.getGeometry().box()); break;
        }
        Gizmos.matrix = oldMatrix;
    }

    void Draw(PxSphereGeometry _geometry)
    {
        Gizmos.DrawWireSphere(Vector3.zero, _geometry.radius);
    }

    void Draw(PxBoxGeometry _geometry)
    {
        Gizmos.DrawWireCube(Vector3.zero, _geometry.halfExtents.ToVector3() * 2);
    }

    void Draw(PxCapsuleGeometry _geometry)
    {
        DrawCapsule(_geometry.radius, (_geometry.halfHeight + _geometry.radius) * 2);
    }

    void DrawArc(Vector3 _centre, Vector3 _axis, Vector3 _start, float _radius, float _angle, float _error = 0.01f)
    {
        float maxStep = 2.0f * Mathf.Acos((_radius - _error) / _radius);
        int steps = (int)(_angle / maxStep);
        float step = _angle / steps;
        for (int i = 0; i < steps; ++i)
            Gizmos.DrawLine(_centre + Quaternion.AngleAxis(step * i, _axis) * _start * _radius,
                            _centre + Quaternion.AngleAxis(step * (i + 1), _axis) * _start * _radius);
    }

    void DrawCapsule(float _radius, float _height)
    {
        Vector3 p0 = Vector3.right * (_height * 0.5f - _radius);
        Vector3 p1 = Vector3.left * (_height * 0.5f - _radius);

        DrawArc(p0, Vector3.right, Vector3.up, _radius, 360.0f);
        DrawArc(p0, Vector3.up, Vector3.forward, _radius, 180.0f);
        DrawArc(p0, Vector3.back, Vector3.up, _radius, 180.0f);

        DrawArc(p1, Vector3.left, Vector3.up, _radius, 360.0f);
        DrawArc(p1, Vector3.down, Vector3.forward, _radius, 180.0f);
        DrawArc(p1, Vector3.forward, Vector3.up, _radius, 180.0f);

        Gizmos.DrawLine(p0 + Vector3.up * _radius, p1 + Vector3.up * _radius);
        Gizmos.DrawLine(p0 + Vector3.forward * _radius, p1 + Vector3.forward * _radius);
        Gizmos.DrawLine(p0 + Vector3.down * _radius, p1 + Vector3.down * _radius);
        Gizmos.DrawLine(p0 + Vector3.back * _radius, p1 + Vector3.back * _radius);
    }

    class EventCallback : PxSimulationEventCallback
    {
        public override void onContact(PxContactPairHeader pairHeader, PxContactPairList pairs)
        {
            for (uint i = 0; i < pairs.count; ++i)
            {
                var pair = pairs.get(i);
                uint contactCount = (uint)Mathf.Min(pair.contactCount, points.Length);
                pair.extractContacts(points, contactCount);
                for (uint j = 0; j < contactCount; ++j)
                {
                    var point = points[j];
                    Debug.DrawLine(point.position.ToVector3(), (point.position + point.impulse * 0.01f).ToVector3());
                }
            }
        }

        PxContactPairPoint[] points = new PxContactPairPoint[10];
    }

    PxSimulationEventCallback m_eventCallback;
    PxDefaultCpuDispatcher m_cpuDispatcher;
    PxAllocatorCallback m_allocatorCallback;
    PxErrorCallback m_errorCallback;
    PxFoundation m_foundation;
    PxPhysics m_physics;
    PxScene m_scene;

    #endregion
}

//#region Extensions

//static class xx
//{
//    public static Vector3 ToVector3(this PxVec3 v)
//    {
//        return new Vector3(v.x, v.y, v.z);
//    }

//    public static Quaternion ToQuaternion(this PxQuat q)
//    {
//        return new Quaternion(q.x, q.y, q.z, q.w);
//    }

//    public static Matrix4x4 ToMatrix4x4(this PxTransform t)
//    {
//        return Matrix4x4.TRS(t.p.ToVector3(), t.q.ToQuaternion(), Vector3.one);
//    }

//    public static PxVec3 ToPxVec3(this Vector3 v)
//    {
//        return new PxVec3(v.x, v.y, v.z);
//    }

//    public static PxQuat ToPxQuat(this Quaternion q)
//    {
//        return new PxQuat(q.x, q.y, q.z, q.w);
//    }
//}

//#endregion