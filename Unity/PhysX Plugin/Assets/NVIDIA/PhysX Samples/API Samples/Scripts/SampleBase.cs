using NVIDIA.PhysX;
using NVIDIA.PhysX.UnityExtensions;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using PXU = NVIDIA.PhysX.Unity;

public class SampleBase : PXU.PxComponent
{
    #region Protected

    protected PxFoundation foundation { get { return PXU.PxPhysics.apiFoundation; } }
    protected PxPhysics physics { get { return PXU.PxPhysics.apiPhysics; } }

    protected override void CreateComponent()
    {
        base.CreateComponent();
        CreateSample();
    }

    protected override void DestroyComponent()
    {
        DestroySample();
        base.DestroyComponent();
    }

    protected virtual void CreateSample()
    {
        Camera.onPreCull += OnCameraPreCull;
    }

    protected virtual void DestroySample()
    {
        Camera.onPreCull -= OnCameraPreCull;

        m_pickerJoint?.release();
        m_pickerJoint = null;

        m_staticActors.Clear();
        m_dynamicActors.Clear();
    }

    protected virtual void OnRender(Camera camera)
    {
        RenderStatics(camera);
        RenderDynamics(camera);
    }

    protected void AddRenderActor(PxRigidStatic actor, Mesh mesh, Material material)
    {
        m_staticActors.Add(actor, new RenderRigidStatic { mesh = mesh, material = material });
    }

    protected void RemoveRenderActor(PxRigidStatic actor)
    {
        m_staticActors.Remove(actor);
    }

    protected void AddRenderActor(PxRigidDynamic actor, Mesh mesh, Material activeMaterial, Material inactiveMaterial)
    {
        m_dynamicActors.Add(actor, new RenderRigidDynamic { mesh = mesh, activeMaterial = activeMaterial, inactiveMaterial = inactiveMaterial });
    }

    protected void AddRenderActor(PxRigidDynamic actor, Mesh mesh, Material material)
    {
        AddRenderActor(actor, mesh, material, material);
    }

    protected void RemoveRenderActor(PxRigidDynamic actor)
    {
        m_dynamicActors.Remove(actor);
    }

    protected static Mesh CreateBoxMesh(Vector3 s)
    {
        return CreateBoxMesh(s.x, s.y, s.z);
    }

    protected static Mesh CreateBoxMesh(float sx, float sy, float sz)
    {
        Vector3[] positions = new[] { new Vector3(0.5f * sx, -0.5f * sy, 0.5f * sz), new Vector3(-0.5f * sx, -0.5f * sy, 0.5f * sz), new Vector3(0.5f * sx, 0.5f * sy, 0.5f * sz), new Vector3(-0.5f * sx, 0.5f * sy, 0.5f * sz), new Vector3(0.5f * sx, 0.5f * sy, -0.5f * sz), new Vector3(-0.5f * sx, 0.5f * sy, -0.5f * sz), new Vector3(0.5f * sx, -0.5f * sy, -0.5f * sz), new Vector3(-0.5f * sx, -0.5f * sy, -0.5f * sz), new Vector3(0.5f * sx, 0.5f * sy, 0.5f * sz), new Vector3(-0.5f * sx, 0.5f * sy, 0.5f * sz), new Vector3(0.5f * sx, 0.5f * sy, -0.5f * sz), new Vector3(-0.5f * sx, 0.5f * sy, -0.5f * sz), new Vector3(0.5f * sx, -0.5f * sy, -0.5f * sz), new Vector3(0.5f * sx, -0.5f * sy, 0.5f * sz), new Vector3(-0.5f * sx, -0.5f * sy, 0.5f * sz), new Vector3(-0.5f * sx, -0.5f * sy, -0.5f * sz), new Vector3(-0.5f * sx, -0.5f * sy, 0.5f * sz), new Vector3(-0.5f * sx, 0.5f * sy, 0.5f * sz), new Vector3(-0.5f * sx, 0.5f * sy, -0.5f * sz), new Vector3(-0.5f * sx, -0.5f * sy, -0.5f * sz), new Vector3(0.5f * sx, -0.5f * sy, -0.5f * sz), new Vector3(0.5f * sx, 0.5f * sy, -0.5f * sz), new Vector3(0.5f * sx, 0.5f * sy, 0.5f * sz), new Vector3(0.5f * sx, -0.5f * sy, 0.5f * sz), };
        Vector3[] normals = new[] { new Vector3(0f, 0f, 1f), new Vector3(0f, 0f, 1f), new Vector3(0f, 0f, 1f), new Vector3(0f, 0f, 1f), new Vector3(0f, 1f, 0f), new Vector3(0f, 1f, 0f), new Vector3(0f, 0f, -1f), new Vector3(0f, 0f, -1f), new Vector3(0f, 1f, 0f), new Vector3(0f, 1f, 0f), new Vector3(0f, 0f, -1f), new Vector3(0f, 0f, -1f), new Vector3(0f, -1f, 0f), new Vector3(0f, -1f, 0f), new Vector3(0f, -1f, 0f), new Vector3(0f, -1f, 0f), new Vector3(-1f, 0f, 0f), new Vector3(-1f, 0f, 0f), new Vector3(-1f, 0f, 0f), new Vector3(-1f, 0f, 0f), new Vector3(1f, 0f, 0f), new Vector3(1f, 0f, 0f), new Vector3(1f, 0f, 0f), new Vector3(1f, 0f, 0f), };
        Vector2[] uvs = new[] { new Vector2(0f, 0f), new Vector2(1f, 0f), new Vector2(0f, 1f), new Vector2(1f, 1f), new Vector2(0f, 1f), new Vector2(1f, 1f), new Vector2(0f, 1f), new Vector2(1f, 1f), new Vector2(0f, 0f), new Vector2(1f, 0f), new Vector2(0f, 0f), new Vector2(1f, 0f), new Vector2(0f, 0f), new Vector2(0f, 1f), new Vector2(1f, 1f), new Vector2(1f, 0f), new Vector2(0f, 0f), new Vector2(0f, 1f), new Vector2(1f, 1f), new Vector2(1f, 0f), new Vector2(0f, 0f), new Vector2(0f, 1f), new Vector2(1f, 1f), new Vector2(1f, 0f), };
        int[] indices = new[] { 0, 2, 3, 0, 3, 1, 8, 4, 5, 8, 5, 9, 10, 6, 7, 10, 7, 11, 12, 13, 14, 12, 14, 15, 16, 17, 18, 16, 18, 19, 20, 21, 22, 20, 22, 23, };
        var mesh = new Mesh();
        mesh.vertices = positions;
        mesh.normals = normals;
        mesh.uv = uvs;
        mesh.triangles = indices;
        mesh.RecalculateBounds();
        return mesh;
    }

    protected static Mesh CreateSphereMesh(float r, float eps = 0.01f)
    {
        return CreateCapsuleMesh(r, r * 2, eps);
    }

    protected static Mesh CreateCapsuleMesh(float r, float l, float eps = 0.01f)
    {
        float maxStepSize = 2.0f * Mathf.Acos((r - eps) / r) * Mathf.Rad2Deg;
        int quarterSteps = (int)(90.0f / maxStepSize + 0.5f);
        float stepSize = 90.0f / quarterSteps;
        int ringSteps = quarterSteps * 4;

        List<Vector3> positions = new List<Vector3>();
        List<Vector3> normals = new List<Vector3>();
        List<Vector2> uv = new List<Vector2>();
        List<int> indices = new List<int>();

        float h = Mathf.Max(l * 0.5f - r, 0);

        var focus0 = Vector3.right * h;
        var focus1 = Vector3.left * h;

        // Tip 0 fan
        {
            positions.Add(focus0 + Vector3.right * r);
            normals.Add(Vector3.right);
            uv.Add(Vector2.zero);

            for (int i = 0; i < ringSteps; ++i)
            {
                var rot = Quaternion.AngleAxis(i * stepSize, Vector3.right) * Quaternion.AngleAxis(stepSize, Vector3.up);
                positions.Add(focus0 + rot * Vector3.right * r);
                normals.Add(rot * Vector3.right);
                uv.Add(Vector2.zero);
            }
            for (int i = 0; i < ringSteps; ++i)
            {
                indices.AddRange(new[] { 0, i + 1, ((i + 1) % ringSteps) + 1 });
            }
        }

        // Cap 0 rings
        for (int j = 1; j < quarterSteps; ++j)
        {
            for (int i = 0; i < ringSteps; ++i)
            {
                var rot = Quaternion.AngleAxis(i * stepSize, Vector3.right) * Quaternion.AngleAxis((j + 1) * stepSize, Vector3.up);
                positions.Add(focus0 + rot * Vector3.right * r);
                normals.Add(rot * Vector3.right);
                uv.Add(Vector2.zero);
            }
            int s = positions.Count - ringSteps;
            for (int i = 0; i < ringSteps; ++i)
            {
                indices.AddRange(new[] { s + i, s + (i + 1) % ringSteps, s + i - ringSteps,
                                         s + (i + 1) % ringSteps, s + (i + 1) % ringSteps - ringSteps, s + i - ringSteps });
            }
        }

        // Cylinder ring
        if (h > float.Epsilon)
        {
            for (int i = 0; i < ringSteps; ++i)
            {
                var rot = Quaternion.AngleAxis(i * stepSize, Vector3.right);
                positions.Add(focus1 + rot * Vector3.back * r);
                normals.Add(rot * Vector3.back);
                uv.Add(Vector2.zero);
            }
            int s = positions.Count - ringSteps;
            for (int i = 0; i < ringSteps; ++i)
            {
                indices.AddRange(new[] { s + i, s + (i + 1) % ringSteps, s + i - ringSteps,
                                         s + (i + 1) % ringSteps, s + (i + 1) % ringSteps - ringSteps, s + i - ringSteps });
            }
        }

        // Cap 1 rings
        for (int j = 1; j < quarterSteps; ++j)
        {
            for (int i = 0; i < ringSteps; ++i)
            {
                var rot = Quaternion.AngleAxis(i * stepSize, Vector3.right) * Quaternion.AngleAxis(j * stepSize, Vector3.up);
                positions.Add(focus1 + rot * Vector3.back * r);
                normals.Add(rot * Vector3.back);
                uv.Add(Vector2.zero);
            }
            int s = positions.Count - ringSteps;
            for (int i = 0; i < ringSteps; ++i)
            {
                indices.AddRange(new[] { s + i, s + (i + 1) % ringSteps, s + i - ringSteps,
                                         s + (i + 1) % ringSteps, s + (i + 1) % ringSteps - ringSteps, s + i - ringSteps });
            }
        }

        // Tip 1 fan
        {
            positions.Add(focus1 + Vector3.left * r);
            normals.Add(Vector3.left);
            uv.Add(Vector2.zero);
            int s = positions.Count - ringSteps - 1;
            for (int i = 0; i < ringSteps; ++i)
            {
                indices.AddRange(new[] { s + ringSteps, s + (i + 1) % ringSteps, s + i });
            }
        }

        var mesh = new Mesh();
        mesh.vertices = positions.ToArray();
        mesh.normals = normals.ToArray();
        mesh.uv = uv.ToArray();
        mesh.triangles = indices.ToArray();
        mesh.RecalculateBounds();
        return mesh;
    }

    protected PxConvexMesh CreateGemConvexMesh(PxCooking cooking)
    {
        const int SIDES = 6;
        const float s = 1.0f;

        int pointCount = 0;
        var points = new Vector3[17];
        for (int i = 0; i < SIDES; ++i)
        {
            var rot0 = Quaternion.Euler(0, i * 360.0f / SIDES, 0);
            var rot1 = Quaternion.Euler(0, (i + 0.5f) * 360.0f / SIDES, 0);
            points[pointCount++] = rot0 * new Vector3(0.5f, 0.5f, 0) * s;
            points[pointCount++] = rot1 * new Vector3(1.0f, 0.0f, 0) * s;
        }
        points[pointCount++] = new Vector3(0.0f, -1.0f, 0) * s;

        // Managed memory should be pinned before passing it to native function
        var pinPoints = GCHandle.Alloc(points, GCHandleType.Pinned);

        // Allocate and initialize PxConvexMeshDesc structure
        var desc = new PxConvexMeshDesc();
        desc.points.count = (uint)points.Length;
        desc.points.stride = sizeof(float) * 3;
        desc.points.data = Marshal.UnsafeAddrOfPinnedArrayElement(points, 0);
        desc.flags = PxConvexFlag.COMPUTE_CONVEX;

        // Create PxConvexMesh
        PxConvexMeshCookingResult condition;
        var convexMesh = cooking.createConvexMesh(desc, physics.getPhysicsInsertionCallback(), out condition);
        if (condition != PxConvexMeshCookingResult.SUCCESS) Debug.LogError("PhysX Cooking: Failed to create convex mesh.");

        desc.destroy(); // This destroys native instance of PxConvexMeshDesc. If not called explicitly, GC will call it when finalizing
        pinPoints.Free(); // Unpin managed memory

        return convexMesh;
    }

    protected Mesh CreateMeshFromConvexMesh(PxConvexMesh convexMesh)
    {
        List<Vector3> positions = new List<Vector3>();
        List<Vector3> normals = new List<Vector3>();
        List<Vector2> uvs = new List<Vector2>();
        List<int> indices = new List<int>();
        for (uint i = 0; i < convexMesh.getNbPolygons(); ++i)
        {
            PxHullPolygon data;
            if (convexMesh.getPolygonData(i, out data))
            {
                int start = positions.Count;
                for (int j = 0; j < data.mNbVerts; ++j)
                {
                    positions.Add(convexMesh.getVertex(convexMesh.getIndex((uint)(data.mIndexBase + j))).ToVector3());
                    normals.Add(data.mPlane.n.ToVector3());
                    uvs.Add(Vector2.zero);
                    if (j > 1) indices.AddRange(new[] { start, start + j - 1, start + j });
                }
            }
        }
        var mesh = new Mesh();
        mesh.vertices = positions.ToArray();
        mesh.normals = normals.ToArray();
        mesh.uv = uvs.ToArray();
        mesh.triangles = indices.ToArray();
        mesh.RecalculateBounds();
        return mesh;
    }

    protected void UpdatePicker(PxScene scene)
    {
        if (Input.GetMouseButtonDown(0))
        {
            m_pickerJoint?.release();
            m_pickerJoint = null;
            var camera = Camera.main;
            var ray = camera.ScreenPointToRay(Input.mousePosition);
            var distance = 100.0f;
            var buffer = new PxRaycastBuffer();
            var fd = new PxQueryFilterData();
            fd.flags = PxQueryFlag.DYNAMIC;
            if (scene.raycast(ray.origin.ToPxVec3(), ray.direction.ToPxVec3(), distance, buffer, PxHitFlag.DEFAULT, fd))
            {
                var hit = buffer.block;
                var rb = hit.actor.getRigidBody();
                if (rb != null)
                {
                    m_pickerJoint = physics.createDistanceJoint(hit.actor, hit.actor.getGlobalPose().transformInv(new PxTransform(hit.position)), null, new PxTransform(hit.position));
                    m_pickerJoint.setMaxDistance(0);
                    m_pickerJoint.setMinDistance(0);
                    m_pickerJoint.setDistanceJointFlag(PxDistanceJointFlag.SPRING_ENABLED, true);
                    float mass = rb.getMass();
                    m_pickerJoint.setStiffness(1e9f * mass);
                    m_pickerJoint.setDamping(1e8f * mass);
                    m_pickedDamping = hit.actor.getRigidBody().getAngularDamping();
                    rb.setAngularDamping(5.0f);
                }
            }
        }

        if (Input.GetMouseButton(0))
        {
            if (m_pickerJoint != null)
            {
                var camera = Camera.main;
                var ray = camera.ScreenPointToRay(Input.mousePosition);
                var distance = Vector3.Distance(ray.origin, m_pickerJoint.getLocalPose(PxJointActorIndex.ACTOR1).p.ToVector3());
                m_pickerJoint.setLocalPose(PxJointActorIndex.ACTOR1, new PxTransform((ray.origin + ray.direction * distance).ToPxVec3()));
                m_pickerJoint.getActor0().getRigidDynamic()?.wakeUp();
                m_pickerJoint.getActor0().getArticulationLink()?.getArticulation().wakeUp();
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            m_pickerJoint?.getActor0().getRigidBody().setAngularDamping(m_pickedDamping);
            m_pickerJoint?.release();
            m_pickerJoint = null;
        }
    }

    protected Color COLOR0 = new Color32(73, 80, 84, 255);
    protected Color COLOR1 = new Color32(112, 122, 126, 255);
    protected Color COLOR2 = new Color32(155, 168, 174, 255);
    protected Color COLOR3 = new Color32(188, 202, 208, 255);
    protected Color COLOR4 = new Color32(227, 232, 234, 255);
    protected Color COLOR5 = new Color32(255, 0, 0, 255);
    protected Color COLOR6 = new Color32(127, 0, 0, 255);

    #endregion

    #region Private

    void OnCameraPreCull(Camera camera)
    {
        if (camera.cameraType != CameraType.Preview) OnRender(camera);
    }

    void RenderStatics(Camera camera)
    {
        foreach (var i in m_staticActors)
            Graphics.DrawMesh(i.Value.mesh, i.Key.getGlobalMatrix().ToMatrix4x4(), i.Value.material,
                              0, camera, 0, null, UnityEngine.Rendering.ShadowCastingMode.On, true);
    }

    void RenderDynamics(Camera camera)
    {
        foreach (var i in m_dynamicActors)
            Graphics.DrawMesh(i.Value.mesh, i.Key.getGlobalMatrix().ToMatrix4x4(), i.Key.isSleeping() ? i.Value.inactiveMaterial : i.Value.activeMaterial,
                              0, camera, 0, null, UnityEngine.Rendering.ShadowCastingMode.On, true);
    }

    struct RenderRigidStatic
    {
        public Mesh mesh;
        public Material material;
    }

    struct RenderRigidDynamic
    {
        public Mesh mesh;
        public Material activeMaterial;
        public Material inactiveMaterial;
    }

    class ErrorCallback : PxErrorCallback
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

    Dictionary<PxRigidStatic, RenderRigidStatic> m_staticActors = new Dictionary<PxRigidStatic, RenderRigidStatic>();
    Dictionary<PxRigidDynamic, RenderRigidDynamic> m_dynamicActors = new Dictionary<PxRigidDynamic, RenderRigidDynamic>();
    PxDistanceJoint m_pickerJoint;
    float m_pickedDamping;

    #endregion
}
