using System;
using UnityEngine;
using PX = NVIDIA.PhysX;
using NVIDIA.PhysX.UnityExtensions;

namespace NVIDIA.PhysX.Unity
{
    public class PxSimpleShape : PxShape
    {
        #region Properties

        public override bool valid { get { return m_shape != null; } }

        public PX.PxShape apiShape { get { return m_shape; } }

        public override PX.PxShape[] apiShapes { get { return new[] { m_shape }; } }

        public override float[] densities { get { return new[] { 0.0f }; } }

        public Vector3 position { get { return m_position; } set { m_position = value; ValidateAndRecreate(); } }

        public Quaternion rotation { get { return Quaternion.Euler(m_rotation); } set { m_rotation = value.eulerAngles; ValidateAndRecreate(); } }

        public PxMaterial[] materials { get { return m_materials; } set { m_materials = value; ValidateAndApply(); } }

        public bool simulationShape { get { return m_simulationShape; } set { m_simulationShape = value; ValidateAndRecreate(); } }

        public bool sceneQueryShape { get { return m_sceneQueryShape; } set { m_sceneQueryShape = value; ValidateAndApply(); } }

        public bool solveCollision { get { return m_solveCollision; } set { m_solveCollision = value; ValidateAndApply(); } }

        public int collisionLayer { get { return m_collisionLayer; } set { m_collisionLayer = value; ValidateAndApply(); } }

        #endregion

        #region Protected

        protected override IPxDependency[] GetDependencies()
        {
            return base.GetDependencies().Append(m_materials);
        }

        protected override void CreateShape()
        {
            CreateSimpleShape();
        }

        protected override void DestroyShape()
        {
            DestroySimpleShape();
        }

        protected override void ValidateShape()
        {
            if (m_materials == null || m_materials.Length == 0 || m_materials[0] == null)
                m_materials = new[] { Resources.Load<PxMaterial>("Default Material") };
            m_collisionLayer = Mathf.Clamp(m_collisionLayer, 0, 7);

            base.ValidateShape();
        }

        protected override void ResetShape()
        {
            m_materials = new[] { Resources.Load<PxMaterial>("Default Material") };
        }

        protected virtual PxGeometry CreateGeometry()
        {
            return null;
        }

        protected override void ApplyProperties()
        {
            base.ApplyProperties();

            if (valid)
            {
                m_shape.setName(name);
                var materials = GetMaterials();
                m_shape.setMaterials(materials, (ushort)materials.Length);
                m_shape.setGeometry(CreateGeometry());
                m_shape.setLocalPose(new PxTransform(position.ToPxVec3(), rotation.ToPxQuat()));
                //m_shape.setFlag(PxShapeFlag.VISUALIZATION, m_visualization);
                m_shape.setFlag(PxShapeFlag.SIMULATION_SHAPE, m_simulationShape);
                m_shape.setFlag(PxShapeFlag.SCENE_QUERY_SHAPE, m_sceneQueryShape);
                m_shapeCollision.index = (uint)m_collisionLayer;
                m_shapeCollision.solveContacts = m_solveCollision;
            }
        }

        #endregion

        #region Private

        void CreateSimpleShape()
        {
            var materials = GetMaterials();
            var geometry = CreateGeometry();
            if (geometry != null)
            {
                m_shape = PxPhysics.apiPhysics.createShape(CreateGeometry(), materials, (ushort)materials.Length, false);

                m_shapeCollision = new PxShapeCollision();
                PxUnityCollisionFiltering.setCollision(m_shape, m_shapeCollision);

                ApplyProperties();
            }

            //Debug.Log("PxSimpleShape '" + name + "' created");
        }

        void DestroySimpleShape()
        {
            m_shapeCollision?.destroy();
            m_shapeCollision = null;

            m_shape?.release();
            m_shape = null;

            //Debug.Log("PxSimpleShape '" + name + "' destroyed");
        }

        PX.PxMaterial[] GetMaterials()
        {
            return m_materials.Length > 0 ?
                Array.ConvertAll(m_materials, m => (m != null && m.valid) ?
                m.apiMaterial : PxPhysics.noMaterial) : new[] { PxPhysics.noMaterial };
        }

        [NonSerialized]
        PX.PxShape m_shape;
        [NonSerialized]
        PxShapeCollision m_shapeCollision = null;

        [SerializeField]
        Vector3 m_position = Vector3.zero;
        [SerializeField]
        Vector3 m_rotation = Vector3.zero;
        [SerializeField]
        PxMaterial[] m_materials = new PxMaterial[1];
        //[SerializeField]
        //bool m_visualization = true;
        [SerializeField]
        bool m_simulationShape = true;
        [SerializeField]
        bool m_sceneQueryShape = true;
        [SerializeField]
        int m_collisionLayer = 0;
        [SerializeField]
        bool m_solveCollision = true;

        #endregion
    }
}
