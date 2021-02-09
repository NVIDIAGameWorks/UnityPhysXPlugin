// This code contains NVIDIA Confidential Information and is disclosed to you
// under a form of NVIDIA software license agreement provided separately to you.
//
// Notice
// NVIDIA Corporation and its licensors retain all intellectual property and
// proprietary rights in and to this software and related documentation and
// any modifications thereto. Any use, reproduction, disclosure, or
// distribution of this software and related documentation without an express
// license agreement from NVIDIA Corporation is strictly prohibited.
//
// ALL NVIDIA DESIGN SPECIFICATIONS, CODE ARE PROVIDED "AS IS.". NVIDIA MAKES
// NO WARRANTIES, EXPRESSED, IMPLIED, STATUTORY, OR OTHERWISE WITH RESPECT TO
// THE MATERIALS, AND EXPRESSLY DISCLAIMS ALL IMPLIED WARRANTIES OF NONINFRINGEMENT,
// MERCHANTABILITY, AND FITNESS FOR A PARTICULAR PURPOSE.
//
// Information and code furnished is believed to be accurate and reliable.
// However, NVIDIA Corporation assumes no responsibility for the consequences of use of such
// information or for any infringement of patents or other rights of third parties that may
// result from its use. No license is granted by implication or otherwise under any patent
// or patent rights of NVIDIA Corporation. Details are subject to change without notice.
// This code supersedes and replaces all information previously supplied.
// NVIDIA Corporation products are not authorized for use as critical
// components in life support devices or systems without express written approval of
// NVIDIA Corporation.
//
// Copyright (c) 2019 NVIDIA Corporation. All rights reserved.

using NVIDIA.PhysX.UnityExtensions;
using System;
using System.Collections;
using UnityEngine;
using PX = NVIDIA.PhysX;

namespace NVIDIA.PhysX.Unity
{
    [AddComponentMenu("NVIDIA/PhysX/Px Static Box Actor", 200)]
    public class PxStaticBoxActor : PxActor
    {
        #region Properties

        public override bool valid { get { return m_rigidStatic != null; } }

        public override PX.PxActor[] apiActors { get { return new[] { m_rigidStatic }; } }

        public override string[] apiActorNames { get { return new[] { name }; } }

        public PX.PxRigidStatic apiRigidStatic { get { return m_rigidStatic; } }

        //public PxShape collisionShape { get { return m_collisionShape; } set { m_collisionShape = value; ValidateAndRecreate(); } }

        public bool disableSimulation { get { return m_disableSimulation; } set { m_disableSimulation = value; ValidateAndApply(); } }

        public bool simulationShape { get { return m_simulationShape; } set { m_simulationShape = value; ValidateAndRecreate(); } }

        public bool sceneQueryShape { get { return m_sceneQueryShape; } set { m_sceneQueryShape = value; ValidateAndApply(); } }

        public bool solveCollision { get { return m_solveCollision; } set { m_solveCollision = value; ValidateAndApply(); } }

        #endregion

        #region Protected

        protected override IPxDependency[] GetDependencies()
        {
            return base.GetDependencies().Append(new[] { m_material });
        }

        protected override void CreateActor()
        {
            base.CreateActor();
            CreateStaticBoxActor();
        }

        protected override void DestroyActor()
        {
            DestroyStaticBoxActor();
            base.DestroyActor();
        }

        protected override void ValidateActor()
        {
            base.ValidateActor();
        }

        protected override void ResetActor()
        {
            base.ResetActor();
            m_material = Resources.Load<PxMaterial>("Default Material");
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
                var shape = m_rigidStatic.getShape(0);
                shape.setFlag(PxShapeFlag.SIMULATION_SHAPE, m_simulationShape);
                shape.setFlag(PxShapeFlag.SCENE_QUERY_SHAPE, m_sceneQueryShape);
                m_shapeCollision.index = 0;
                m_shapeCollision.solveContacts = m_solveCollision;
            }
        }

        #endregion

        #region Private

        void CreateStaticBoxActor()
        {
            m_rigidStatic = PxPhysics.apiPhysics.createRigidStatic(transform.ToPxTransform());

            m_rigidStatic.createExclusiveShape(new PxBoxGeometry(ValidSize(transform.lossyScale * 0.5f).ToPxVec3()),
                                               m_material && m_material.valid ? m_material.apiMaterial : PxPhysics.noMaterial);

            var shape = m_rigidStatic.getShape(0);
            m_shapeCollision = new PxShapeCollision();
            m_shapeCollision.solveContacts = m_solveCollision;
            PxUnityCollisionFiltering.setCollision(shape, m_shapeCollision);

            m_rigidStatic.userData = this;

            ApplyProperties();

            //Debug.Log("PxStaticActor '" + name + "' created");
        }

        void DestroyStaticBoxActor()
        {
            m_rigidStatic?.release();
            m_rigidStatic = null;

            m_shapeCollision?.destroy();
            m_shapeCollision = null;

            //Debug.Log("PxStaticActor '" + name + "' destroyed");
        }

        void BeforeSimulation()
        {
            if (transform.hasChanged)
            {
                m_rigidStatic.setGlobalPose(transform.ToPxTransform());
                m_rigidStatic.getShape(0).setGeometry(new PxBoxGeometry((ValidSize(transform.lossyScale * 0.5f)).ToPxVec3()));
                transform.hasChanged = false;
                //StartCoroutine(ResetTransformChanged());
            }
        }

        //IEnumerator ResetTransformChanged()
        //{
        //    yield return new WaitForEndOfFrame();
        //    transform.hasChanged = false;
        //}

        Vector3 ValidSize(Vector3 size)
        {
            float eps = 0.00001f;
            return new Vector3(Mathf.Max(size.x, eps), Mathf.Max(size.y, eps), Mathf.Max(size.z, eps));
        }

        [NonSerialized]
        PX.PxRigidStatic m_rigidStatic;
        [NonSerialized]
        PxShapeCollision m_shapeCollision = null;

        [SerializeField]
        PxMaterial m_material = null;
        //[SerializeField]
        //bool m_visualization = true;
        [SerializeField]
        bool m_disableSimulation = false;
        [SerializeField]
        bool m_simulationShape = true;
        [SerializeField]
        bool m_sceneQueryShape = true;
        [SerializeField]
        bool m_solveCollision = true;

        #endregion
    }
}