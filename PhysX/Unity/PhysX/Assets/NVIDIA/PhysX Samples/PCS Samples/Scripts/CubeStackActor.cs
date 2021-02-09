using NVIDIA.PhysX.Unity;
using NVIDIA.PhysX.UnityExtensions;
using System;
using UnityEngine;
using PX = NVIDIA.PhysX;

public class CubeStackActor : PxActor
{
    #region Properties

    public override bool valid { get { return m_actors.Length > 0 && m_actors[0] != null; } }

    public override PX.PxActor[] apiActors { get { return m_actors; } }

    public override string[] apiActorNames { get { return Array.ConvertAll(m_actors, a => "Body " + Array.IndexOf(m_actors, a)); } }

    #endregion

    #region Protected

    protected override void CreateActor()
    {
        base.CreateActor();
        CreateCubeStack();
    }

    protected override void DestroyActor()
    {
        DestroyCubeScack();
        base.DestroyActor();
    }

    protected override void AddActorToScene(PxScene scene)
    {
        if (scene.valid)
        {
            foreach (var a in m_actors)
                scene.apiScene.addActor(a);
        }
    }

    protected override void RemoveActorFromScene(PxScene scene)
    {
        if (scene.valid)
        {
            foreach (var a in m_actors)
                scene.apiScene.removeActor(a);
        }
    }

    #endregion

    #region Private

    void CreateCubeStack()
    {
        m_actors = new PX.PxRigidDynamic[m_size.x * m_size.y * m_size.z];
        m_actorList = new PX.PxRigidActorList();

        m_cubeShape = PxPhysics.apiPhysics.createShape(new PX.PxBoxGeometry(0.5f, 0.5f, 0.5f), PxPhysics.noMaterial);

        for (int x = 0; x < m_size.x; ++x)
        {
            for (int y = 0; y < m_size.y; ++y)
            {
                for (int z = 0; z < m_size.z; ++z)
                {
                    var actor = PxPhysics.apiPhysics.createRigidDynamic(new PX.PxTransform(x - 5.0f, y + 0.5f, z - 5.0f));
                    actor.attachShape(m_cubeShape);
                    actor.updateMassAndInertia();
                    actor.userData = this;

                    m_actors[x + y * m_size.x + z * m_size.x * m_size.y] = actor;
                    m_actorList.addRigidActor(actor);
                }
            }
        }

        Camera.onPreCull += OnCameraPreCull;
    }

    void DestroyCubeScack()
    {
        Camera.onPreCull -= OnCameraPreCull;

        foreach (var a in m_actors)
            a.release();

        m_actors = new PX.PxRigidDynamic[0];

        m_cubeShape.release();
        m_cubeShape = null;

        m_actorList.destroy();
        m_actorList = null;
    }

    void OnCameraPreCull(Camera camera)
    {
        if (camera.cameraType != CameraType.Preview && m_cubeMesh != null && m_cubeMaterial != null)
        {
            var list = m_actorList;
            int actorCount = (int)list.getNbRigidActors();
            for (int i = 0; i < actorCount; i += m_batch.Length)
            {
                int count = Mathf.Min(actorCount - i, m_batch.Length);
                list.getRigidActorMatrices(m_batch, i, count);
                Graphics.DrawMeshInstanced(m_cubeMesh, 0, m_cubeMaterial, m_batch, count,
                                           null, UnityEngine.Rendering.ShadowCastingMode.On, true, 0, camera);
            }
        }
    }

    [NonSerialized]
    PX.PxShape m_cubeShape = null;
    [NonSerialized]
    PX.PxRigidDynamic[] m_actors = new PX.PxRigidDynamic[0];
    [NonSerialized]
    PX.PxRigidActorList m_actorList = null;
    [NonSerialized]
    Matrix4x4[] m_batch = new Matrix4x4[1023];

    [SerializeField]
    Mesh m_cubeMesh = null;
    [SerializeField]
    Material m_cubeMaterial = null;
    [SerializeField]
    Vector3Int m_size = new Vector3Int(10, 40, 10);

    #endregion
}
