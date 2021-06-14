using NVIDIA.PhysX.UnityExtensions;
using System;
using System.Collections;
using UnityEngine;
using PX = NVIDIA.PhysX;

namespace NVIDIA.PhysX.Unity
{
    [AddComponentMenu("NVIDIA/PhysX/Px Static Actor", 200)]
    public class PxStaticActor : PxActor
    {
        #region Properties

        public override bool valid { get { return m_rigidStatic != null; } }

        public override PX.PxActor[] apiActors { get { return new[] { m_rigidStatic }; } }

        public override string[] apiActorNames { get { return new[] { name }; } }

        public PX.PxRigidStatic apiRigidStatic { get { return m_rigidStatic; } }

        public PxShape collisionShape { get { return m_collisionShape; } set { m_collisionShape = value; ValidateAndRecreate(); } }

        public bool disableSimulation { get { return m_disableSimulation; } set { m_disableSimulation = value; ValidateAndApply(); } }

        #endregion

        #region Protected

        protected override IPxDependency[] GetDependencies()
        {
            return base.GetDependencies().Append(new[] { m_collisionShape });
        }

        protected override void CreateActor()
        {
            base.CreateActor();
            CreateStaticActor();
        }

        protected override void DestroyActor()
        {
            DestroyStaticActor();
            base.DestroyActor();
        }

        protected override void ValidateActor()
        {
            base.ValidateActor();
            //Recreate();
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
                scene.apiScene.addActor(m_rigidStatic);
                scene.onBeforeSimulation += BeforeSimulation;
            }

            //Debug.Log("'" + name + "' added to '" + scene.name + "'");
        }

        protected override void RemoveActorFromScene(PxScene scene)
        {
            if (scene.valid)
            {
                scene.onBeforeSimulation -= BeforeSimulation;
                scene.apiScene.removeActor(m_rigidStatic);
            }

            //Debug.Log("'" + name + "' removed from '" + scene.name + "'");
        }

        protected override void ApplyProperties()
        {
            base.ApplyProperties();

            if (valid)
            {
                //m_rigidStatic.setActorFlag(PxActorFlag.VISUALIZATION, m_visualization);
                m_rigidStatic.setActorFlag(PxActorFlag.DISABLE_SIMULATION, m_disableSimulation);
            }
        }

        #endregion

        #region Private

        void CreateStaticActor()
        {
            m_rigidStatic = PxPhysics.apiPhysics.createRigidStatic(transform.ToPxTransform());

            if (m_collisionShape != null && m_collisionShape.valid)
            {
                var shapes = m_collisionShape.apiShapes;
                foreach (var s in shapes) m_rigidStatic.attachShape(s);
            }

            m_rigidStatic.userData = this;

            ApplyProperties();

            //Debug.Log("PxStaticActor '" + name + "' created");
        }

        void DestroyStaticActor()
        {
            m_rigidStatic?.release();
            m_rigidStatic = null;

            //Debug.Log("PxStaticActor '" + name + "' destroyed");
        }

        void BeforeSimulation()
        {
            if (transform.hasChanged)
            {
                m_rigidStatic.setGlobalPose(transform.ToPxTransform());
                transform.hasChanged = false;
                //StartCoroutine(ResetTransformChanged());
            }
        }

        //IEnumerator ResetTransformChanged()
        //{
        //    yield return new WaitForEndOfFrame();
        //    transform.hasChanged = false;
        //}

        [NonSerialized]
        PX.PxRigidStatic m_rigidStatic;

        [SerializeField]
        PxShape m_collisionShape;
        //[SerializeField]
        //bool m_visualization = true;
        [SerializeField]
        bool m_disableSimulation = false;

        #endregion
    }
}