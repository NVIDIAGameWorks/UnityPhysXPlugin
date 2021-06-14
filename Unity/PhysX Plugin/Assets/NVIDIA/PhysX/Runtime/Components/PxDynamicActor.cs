using NVIDIA.PhysX.UnityExtensions;
using System;
using System.Collections;
using UnityEngine;
using PX = NVIDIA.PhysX;

namespace NVIDIA.PhysX.Unity
{
    [AddComponentMenu("NVIDIA/PhysX/Px Dynamic Actor", 210)]
    public class PxDynamicActor : PxActor
    {
        #region Properties

        public override bool valid { get { return m_rigidDynamic != null; } }

        public override PX.PxActor[] apiActors { get { return new[] { m_rigidDynamic }; } }

        public override string[] apiActorNames { get { return new[] { name }; } }

        public PX.PxRigidDynamic apiRigidDynamic { get { return m_rigidDynamic; } }

        public PxShape collisionShape { get { return m_collisionShape; } set { m_collisionShape = value; ValidateAndRecreate(); } }

        public Vector3 linearVelocity { get { return m_linearVelocity; } set { m_linearVelocity = value; ValidateAndApply(); } }

        public Vector3 angularVelocity { get { return m_angularVelocity; } set { m_angularVelocity = value; ValidateAndApply(); } }

        public bool disableGravity { get { return m_disableGravity; } set { m_disableGravity = value; ValidateAndApply(); } }

        public bool disableSimulation { get { return m_disableSimulation; } set { m_disableSimulation = value; ValidateAndApply(); } }

        public bool autoMass { get { return m_autoMass; } set { m_autoMass = value; ValidateAndRecreate(); } }

        public float mass { get { return m_mass; } set { m_mass = value; ValidateAndRecreate(); } }

        public Vector3 massPosition { get { return m_massPosition; } set { m_massPosition = value; ValidateAndRecreate(); } }

        public Quaternion massRotation { get { return Quaternion.Euler(m_massRotation); } set { m_massPosition = value.eulerAngles; ValidateAndRecreate(); } }

        public Vector3 localInertia { get { return m_localInertia; } set { m_localInertia = value; ValidateAndRecreate(); } }

        public int positionIterations { get { return m_positionIterations; } set { m_positionIterations = value; ValidateAndApply(); } }

        public int velocityIterations { get { return m_velocityIterations; } set { m_velocityIterations = value; ValidateAndApply(); } }

        public bool kinematic { get { return m_kinematic; } set { m_kinematic = value; ValidateAndApply(); } }

        public bool enableCCD { get { return m_enableCCD; } set { m_enableCCD = value; ValidateAndApply(); } }

        #endregion

        #region Protected

        protected override IPxDependency[] GetDependencies()
        {
            return base.GetDependencies().Append(new[] { m_collisionShape });
        }

        protected override void CreateActor()
        {
            base.CreateActor();
            CreateDynamicActor();
        }

        protected override void DestroyActor()
        {
            DestroyDynamicActor();
            base.DestroyActor();
        }

        protected override void ValidateActor()
        {
            base.ValidateActor();
            //Recreate();
            m_mass = Mathf.Max(m_mass, 0.0001f);
            m_positionIterations = Mathf.Max(m_positionIterations, 0);
            m_velocityIterations = Mathf.Max(m_velocityIterations, 0);
        }

        protected override void UpdateComponentInEditor()
        {
            base.UpdateComponentInEditor();
            BeforeSimulation();
        }

        protected override void AddActorToScene(PxScene scene)
        {
            if (scene.valid)
            {
                scene.apiScene.addActor(m_rigidDynamic);
                scene.onBeforeSimulation += BeforeSimulation;
                scene.onAfterSimulation += AfterSimulation;
            }

            //Debug.Log("'" + name + "' added to '" + scene.name + "'");
        }

        protected override void RemoveActorFromScene(PxScene scene)
        {
            if (scene.valid)
            {
                scene.onBeforeSimulation -= BeforeSimulation;
                scene.onAfterSimulation -= AfterSimulation;
                scene.apiScene.removeActor(m_rigidDynamic);
            }

            //Debug.Log("'" + name + "' removed from '" + scene.name + "'");
        }

        protected override void ApplyProperties()
        {
            base.ApplyProperties();

            if (valid)
            {
                m_rigidDynamic.setSolverIterationCounts((uint)m_positionIterations, (uint)m_velocityIterations);

                m_rigidDynamic.setActorFlag(PxActorFlag.VISUALIZATION, m_visualization);
                m_rigidDynamic.setActorFlag(PxActorFlag.DISABLE_GRAVITY, m_disableGravity);
                m_rigidDynamic.setActorFlag(PxActorFlag.DISABLE_SIMULATION, m_disableSimulation);

                m_rigidDynamic.setRigidBodyFlag(PxRigidBodyFlag.KINEMATIC, m_kinematic);
                m_rigidDynamic.setRigidBodyFlag(PxRigidBodyFlag.ENABLE_CCD, m_enableCCD);

                if (!m_kinematic) // @@@ ???
                {
                    m_rigidDynamic.setLinearVelocity(m_linearVelocity.ToPxVec3());
                    m_rigidDynamic.setAngularVelocity(m_angularVelocity.ToPxVec3());
                }
            }
        }

        #endregion

        #region Private

        void CreateDynamicActor()
        {
            m_rigidDynamic = PxPhysics.apiPhysics.createRigidDynamic(transform.ToPxTransform());

            if (m_collisionShape != null && m_collisionShape.valid)
            {
                var shapes = m_collisionShape.apiShapes;
                foreach (var s in shapes) m_rigidDynamic.attachShape(s);
            }

            if (m_autoMass)
            {
                if (m_collisionShape != null && m_collisionShape.valid)
                {
                    var densities = m_collisionShape.densities;
                    m_rigidDynamic.updateMassAndInertia(densities, (uint)densities.Length);
                }
                m_mass = m_rigidDynamic.getMass();
                var massPose = m_rigidDynamic.getCMassLocalPose();
                m_massPosition = massPose.p.ToVector3();
                m_massRotation = massPose.q.ToQuaternion().eulerAngles;
                m_localInertia = m_rigidDynamic.getMassSpaceInertiaTensor().ToVector3();
            }
            else
            {
                m_rigidDynamic.setMass(m_mass);
                m_rigidDynamic.setCMassLocalPose(new PxTransform(m_massPosition.ToPxVec3(), Quaternion.Euler(m_massRotation).ToPxQuat()));
                m_rigidDynamic.setMassSpaceInertiaTensor(m_localInertia.ToPxVec3());
            }

            m_rigidDynamic.userData = this;

            ApplyProperties();

            //Debug.Log("PxDynamicActor '" + name + "' created");
        }

        void DestroyDynamicActor()
        {
            m_rigidDynamic?.release();
            m_rigidDynamic = null;

            //Debug.Log("PxDynamicActor '" + name + "' destroyed");
        }

        void BeforeSimulation()
        {
            if (transform.hasChanged)
            {
                if (m_kinematic) m_rigidDynamic.setKinematicTarget(transform.ToPxTransform());
                else m_rigidDynamic.setGlobalPose(transform.ToPxTransform());
                transform.hasChanged = false;
                //StartCoroutine(ResetTransformChanged());
            }
        }

        void AfterSimulation()
        {
            if (!m_rigidDynamic.isSleeping())
            {
                m_rigidDynamic.getGlobalPose().ToTransform(transform);
                transform.hasChanged = false;
                //StartCoroutine(ResetTransformChanged());
                m_linearVelocity = m_rigidDynamic.getLinearVelocity().ToVector3();
                m_angularVelocity = m_rigidDynamic.getAngularVelocity().ToVector3();
            }
        }

        //IEnumerator ResetTransformChanged()
        //{
        //    yield return new WaitForEndOfFrame();
        //    transform.hasChanged = false;
        //}

        [NonSerialized]
        PX.PxRigidDynamic m_rigidDynamic;

        [SerializeField]
        PxShape m_collisionShape;
        [SerializeField]
        Vector3 m_linearVelocity = Vector3.zero;
        [SerializeField]
        Vector3 m_angularVelocity = Vector3.zero;
        [SerializeField]
        bool m_visualization = true;
        [SerializeField]
        bool m_disableGravity = false;
        [SerializeField]
        bool m_disableSimulation = false;
        [SerializeField]
        bool m_autoMass = true;
        [SerializeField]
        float m_mass = 1.0f;
        [SerializeField]
        Vector3 m_massPosition = Vector3.zero;
        [SerializeField]
        Vector3 m_massRotation = Vector3.zero;
        [SerializeField]
        Vector3 m_localInertia = Vector3.one;
        [SerializeField]
        int m_positionIterations = 4;
        [SerializeField]
        int m_velocityIterations = 1;
        [SerializeField]
        bool m_kinematic = false;
        [SerializeField]
        bool m_enableCCD = false;

        #endregion
    }
}