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
using UnityEngine;
using PX = NVIDIA.PhysX;

namespace NVIDIA.PhysX.Unity
{
    public delegate void OnContact(PxActor thisActor, PxActor otherActor, PxContactPairHeader pairHeader, PxContactPairList pairs);

    [DisallowMultipleComponent]
    [AddComponentMenu("")]
    public class PxActor : PxComponent
    {
        #region Properties

        public virtual PX.PxActor[] apiActors { get { return new PX.PxActor[0]; } }

        public virtual string[] apiActorNames { get { return new[] { "" }; } }

        public PxScene sceneOverride { get { return m_sceneOverride; } set { m_sceneOverride = value; } }

        public PxScene currentScene { get { return m_currentScene; } }

        public bool collisionEvents { get { return m_collisionEvents; } set { m_collisionEvents = value; } }

        #endregion

        #region Events

        public OnContact onContact;

        #endregion

        #region Messages


        #endregion

        #region Protected

        protected override void CreateComponent()
        {
            CreateActor();

            foreach (var a in apiActors)
                if (a != null)
                    PxUnityCollisionFiltering.setCollision(a, m_actorCollision);

            AddToScene();
        }

        protected override void DestroyComponent()
        {
            RemoveFromScene();
            DestroyActor();
        }

        protected override void ValidateComponent()
        {
            ValidateActor();
        }

        protected override void ResetComponent()
        {
            ResetActor();
        }

        protected virtual void CreateActor()
        {
            m_actorCollision = new PxActorCollision();

            m_actorCollision.collisionEvents = m_collisionEvents;
            m_actorCollision.index = (uint)m_collisionLayer;

            int layerCount = m_collisionLayers.Length;
            for (int i = 0; i < layerCount; ++i) for (int j = 0; j <= i; ++j)
                    m_actorCollision.setCanCollide((uint)i, (uint)j, m_collisionMatrix[j * layerCount - j * (j + 1) / 2 + i]);
        }

        protected virtual void DestroyActor()
        {
            m_actorCollision?.destroy();
            m_actorCollision = null;
        }

        protected virtual void ValidateActor()
        {
            m_collisionLayer = Mathf.Clamp(m_collisionLayer, 0, 15);
        }

        protected virtual void ResetActor()
        {
            m_collisionLayers[0] = "Default";
            Array.Clear(m_collisionMatrix, 0, m_collisionMatrix.Length);
            m_collisionMatrix[0] = true;
        }

        protected virtual void AddActorToScene(PxScene scene)
        {
        }

        protected virtual void RemoveActorFromScene(PxScene scene)
        {
        }

        protected override void ApplyProperties()
        {
            base.ApplyProperties();

            if (valid)
            {
                m_actorCollision.collisionEvents = m_collisionEvents;
                m_actorCollision.index = (uint)m_collisionLayer;

                int layerCount = m_collisionLayers.Length;
                for (int i = 0; i < layerCount; ++i) for (int j = 0; j <= i; ++j)
                        m_actorCollision.setCanCollide((uint)i, (uint)j, m_collisionMatrix[j * layerCount - j * (j + 1) / 2 + i]);

                foreach (var a in apiActors) if (a != null)
                        PxUnityCollisionFiltering.setCollision(a, m_actorCollision);
            }
        }

        #endregion

        #region Private

        void AddToScene()
        {
            if (valid)
            {
                var scene = FindScene();
                if (scene != null)
                {
                    if (!scene.created)
                    {
                        scene.onAfterCreate += AfterSceneCreate;
                        return;
                    }

                    AddActorToScene(scene);

                    scene.onBeforeDestroy += BeforeSceneDestroy;
                    scene.onBeforeRecreate += BeforeSceneRecreate;

                    m_currentScene = scene;
                }
            }
        }

        void RemoveFromScene()
        {
            if (valid)
            {
                var scene = m_currentScene;
                if (scene != null)
                {
                    scene.onBeforeDestroy -= BeforeSceneDestroy;
                    scene.onBeforeRecreate -= BeforeSceneRecreate;

                    RemoveActorFromScene(scene);

                    m_currentScene = null;
                }
            }
        }

        void AfterSceneCreate(IPxDependency scene)
        {
            scene.onAfterCreate -= AfterSceneCreate;
            AddToScene();
        }

        void BeforeSceneDestroy(IPxDependency scene)
        {
            RemoveFromScene();
        }

        void BeforeSceneRecreate(IPxDependency scene)
        {
            RemoveFromScene();
            scene.onAfterRecreate += AfterSceneRecreate;
        }

        void AfterSceneRecreate(IPxDependency scene)
        {
            scene.onAfterRecreate -= AfterSceneRecreate;
            AddToScene();
        }

        PxScene FindScene()
        {
            if (m_sceneOverride != null) return m_sceneOverride;

            var scene = GameObject.FindWithTag("MainPxScene")?.GetComponent<PxScene>();
            if (scene != null) return scene;

            return null;
        }

        static Vector3 InertiaToEllipsoid(Vector3 inertia)
        {
            Vector3 ellipsoid = Vector3.zero;
            ellipsoid.x = Mathf.Sqrt(2.5f * (inertia.y + inertia.z - inertia.x));
            ellipsoid.y = Mathf.Sqrt(2.5f * (inertia.x + inertia.z - inertia.y));
            ellipsoid.z = Mathf.Sqrt(2.5f * (inertia.x + inertia.y - inertia.z));
            return ellipsoid;
        }

        [NonSerialized]
        PxScene m_currentScene = null;
        [NonSerialized]
        PxActorCollision m_actorCollision = null;

        [SerializeField]
        PxScene m_sceneOverride;
        [SerializeField]
        bool m_collisionEvents = false;
        [SerializeField]
        int m_collisionLayer = 0;
        [SerializeField]
        string[] m_collisionLayers = new string[8] { "Default", "", "", "", "", "", "", "" };
        [SerializeField]
        bool[] m_collisionMatrix = new bool[8 * 9 / 2] { true, false, false, false, false, false, false, false, false, false, false, false,
                                                         false, false, false, false, false, false, false, false, false, false, false, false,
                                                         false, false, false, false, false, false, false, false, false, false, false, false };

        #endregion
    }
}