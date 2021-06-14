//using NVIDIA.PhysX.Unity;
//using UnityEngine;
//using PX = NVIDIA.PhysX;
//using NVIDIA.PhysX.UnityExtensions;

//public class StaticPlane : MonoBehaviour
//{
//    public PxScene m_scene = null;

//    private void OnEnable()
//    {
//        if (m_scene != null)
//            m_scene.onSceneCreated += OnSceneCreated;

//        if (m_scene != null && m_scene.valid)
//            OnSceneCreated();
//    }

//    void OnSceneCreated()
//    {
//        m_scene.onSceneCreated -= OnSceneCreated;

//        var rigidStatic = PxPhysics.apiPhysics.createRigidStatic(new PX.PxTransform(Quaternion.Euler(0, 0, 90.0f).ToPxQuat()));
//        rigidStatic.attachShape(PxPhysics.apiPhysics.createShape(new PX.PxPlaneGeometry(), PxPhysics.defaultMaterial));
//        m_scene.apiScene.addActor(rigidStatic);
//    }
//}
