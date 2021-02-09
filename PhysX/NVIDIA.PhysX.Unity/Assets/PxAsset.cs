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

using System;
using UnityEngine;

namespace NVIDIA.PhysX.Unity
{
    public class PxAsset : ScriptableObject, IPxDependency
    {
        #region Properties

        public bool created { get { return m_created; } }

        public virtual bool valid { get { return false; } }

        #endregion

        #region Events

        public event DependencyAction onAfterCreate;
        public event DependencyAction onBeforeDestroy;
        public event DependencyAction onBeforeRecreate;
        public event DependencyAction onAfterRecreate;

        #endregion

        #region Messages

        void OnEnable()
        {
            CheckCreate();
        }

        void OnDisable()
        {
            ClearDestroy();
        }

        void OnValidate()
        {
            ValidateAsset();

            if (m_forceRecreate) Recreate();
            else ApplyProperties();

            m_forceRecreate = false;
        }

        void Reset()
        {
            ResetAsset();
        }

        #endregion

        #region Protected

        protected virtual IPxDependency[] GetDependencies()
        {
            return new[] { PxPhysics.settings };
        }

        protected virtual void CreateAsset()
        {
        }

        protected virtual void DestroyAsset()
        {
        }

        protected virtual void ValidateAsset()
        {
        }

        protected virtual void ResetAsset()
        {
        }

        protected void Recreate()
        {
            if (created)
            {
                onBeforeRecreate?.Invoke(this);

                ClearDestroy();
                CheckCreate();

                onAfterRecreate?.Invoke(this);
            }
        }

        protected virtual void ApplyProperties()
        {
        }

        protected void ValidateAndApply()
        {
            ValidateAsset();
            ApplyProperties();
        }

        protected void ValidateAndRecreate()
        {
            ValidateAsset();
            Recreate();
        }

        #endregion

        #region Private

        void CheckCreate()
        {
            if (created) return;

            // Unity may try to import assets before the plugin.
            // In this case any call to a native function will fail.
            try { new PxTolerancesScale().destroy(); }
            catch { return; }

            m_dependencies = GetDependencies();
            foreach (var dependency in m_dependencies)
            {
                if (dependency != null && !dependency.created)
                {
                    dependency.onAfterCreate += DependencyCreated;
                    return;
                }
            }

            CreateAsset();

            foreach (var dependency in m_dependencies)
            {
                if (dependency != null)
                {
                    dependency.onBeforeDestroy += DependencyDestroying;
                    dependency.onBeforeRecreate += BeforeDependencyRecreate;
                }
            }

            m_created = true;
            onAfterCreate?.Invoke(this);
        }

        void ClearDestroy()
        {
            if (!created) return;

            onBeforeDestroy?.Invoke(this);
            m_created = false;

            foreach (var dependency in m_dependencies)
            {
                if (dependency != null)
                {
                    dependency.onBeforeDestroy -= DependencyDestroying;
                    dependency.onBeforeRecreate -= BeforeDependencyRecreate;
                }
            }

            DestroyAsset();
        }

        void DependencyCreated(IPxDependency dependency)
        {
            dependency.onAfterCreate -= DependencyCreated;
            CheckCreate();
        }

        void DependencyDestroying(IPxDependency dependency)
        {
            ClearDestroy();
        }

        void BeforeDependencyRecreate(IPxDependency dependency)
        {
            onBeforeRecreate?.Invoke(this);

            ClearDestroy();
            dependency.onAfterRecreate += AfterDependencyRecreate;
        }

        void AfterDependencyRecreate(IPxDependency dependency)
        {
            dependency.onAfterRecreate -= AfterDependencyRecreate;
            CheckCreate();

            onAfterRecreate?.Invoke(this);
        }

        [NonSerialized]
        bool m_created = false;
        [NonSerialized]
        IPxDependency[] m_dependencies = new IPxDependency[0];

        [SerializeField, HideInInspector]
        bool m_forceRecreate = false;

        #endregion
    }
}