using System;
using UnityEngine;

namespace NVIDIA.PhysX.Unity
{
    [ExecuteInEditMode]
    [AddComponentMenu("")]
    public class PxComponent : MonoBehaviour, IPxDependency
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
            ValidateComponent();

            if (m_forceRecreate) Recreate();
            else ApplyProperties();

            m_forceRecreate = false;
        }

        void Reset()
        {
            ResetComponent();
        }

        void Update()
        {
            if (!Application.isPlaying)
                UpdateComponentInEditor();
        }

        #endregion

        #region Protected

        protected virtual IPxDependency[] GetDependencies()
        {
            return new[] { PxPhysics.settings };
        }

        protected virtual void CreateComponent() { }

        protected virtual void DestroyComponent() { }

        protected virtual void ValidateComponent() { }

        protected virtual void ResetComponent() { }

        protected virtual void UpdateComponentInEditor() { }

        protected void BeginRecreate()
        {
            if (created)
            {
                onBeforeRecreate?.Invoke(this);
                ClearDestroy();
            }
        }

        protected void EndRecreate()
        {
            if (!created)
            {
                CheckCreate();
                onAfterRecreate?.Invoke(this);
            }
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

        protected virtual void ApplyProperties() { }

        protected void ValidateAndApply()
        {
            ValidateComponent();
            ApplyProperties();
        }

        protected void ValidateAndRecreate()
        {
            ValidateComponent();
            Recreate();
        }

        #endregion

        #region Private

        void CheckCreate()
        {
            if (created) return;

            m_dependencies = GetDependencies();
            foreach (var dependency in m_dependencies)
            {
                if (dependency != null && !dependency.created)
                {
                    dependency.onAfterCreate += DependencyCreated;
                    return;
                }
            }

            CreateComponent();

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

            DestroyComponent();
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