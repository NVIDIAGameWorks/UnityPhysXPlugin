//using NVIDIA.PhysX.Unity;
//using UnityEngine;
//using PX = NVIDIA.PhysX;
//using NVIDIA.PhysX.UnityExtensions;

//public class StackOfBoxes : MonoBehaviour
//{
//    public PxScene m_scene = null;
//    public Vector3Int m_size = Vector3Int.one;
//    public Mesh m_mesh = null;
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
//        if (m_list != null)
//        {
//            m_list.releaseRigidActors();
//            m_list.destroy();
//            m_list = null;
//        }

//        Camera.onPreCull -= DrawMeshInstanced;
//    }

//    //private void OnRenderObject()
//    //{
//    //}

//    void DrawMeshInstanced(Camera camera)
//    {
//        if (m_mesh != null && m_material != null && m_matrices != null)
//        {
//            m_list.getRigidActorMatrices(ref m_matrices[0].m00, 0, (uint)m_matrices.Length);
//            for (int i = 0; i < m_matrices.Length; i += m_batch.Length)
//            {
//                int count = Mathf.Min(m_matrices.Length - i, m_batch.Length);
//                System.Array.Copy(m_matrices, i, m_batch, 0, count);
//                Graphics.DrawMeshInstanced(m_mesh, 0, m_material, m_batch, count, null, UnityEngine.Rendering.ShadowCastingMode.On, true, 0, camera);
//            }
//        }
//    }

//    void OnSceneCreated()
//    {
//        m_scene.onSceneCreated -= OnSceneCreated;

//        int count = m_size.x * m_size.y * m_size.z;
//        m_list = new PX.PxRigidActorList();
//        m_matrices = new Matrix4x4[count];

//        for (int z = 0; z < m_size.z; ++z)
//        {
//            for (int y = 0; y < m_size.y; ++y)
//            {
//                for (int x = 0; x < m_size.x; ++x)
//                {
//                    var body = PxPhysics.apiPhysics.createRigidDynamic(new PX.PxTransform(new PX.PxVec3(x * 1.1f, y * 1.1f + 10.5f, z * 1.1f)));
//                    //var shape = PxPhysics.apiPhysics.createShape(new PX.PxBoxGeometry(0.5f, 0.5f, 0.5f), PxPhysics.defaultMaterial);
//                    body.createExclusiveShape(new PX.PxBoxGeometry(0.5f, 0.5f, 0.5f), PxPhysics.defaultMaterial);
//                    //body.attachShape(shape);
//                    //shape.release();
//                    body.updateMassAndInertia();
//                    m_list.addRigidActor(body);
//                    m_scene.apiScene.addActor(body);
//                    int index = x + y * m_size.x + z * m_size.x * m_size.y;
//                    m_matrices[index] = body.getGlobalPose().ToMatrix4x4();
//                }
//            }
//        }

//        //m_scene.onSceneUpdated += OnSceneUpdated;
//    }

//    //void OnSceneUpdated()
//    //{
//    //    m_list.getRigidActorMatrices(ref m_matrices[0].m00, 0, (uint)m_matrices.Length);
//    //}

//    PX.PxRigidActorList m_list;
//    Matrix4x4[] m_matrices;
//    Matrix4x4[] m_batch = new Matrix4x4[1023];
//}
