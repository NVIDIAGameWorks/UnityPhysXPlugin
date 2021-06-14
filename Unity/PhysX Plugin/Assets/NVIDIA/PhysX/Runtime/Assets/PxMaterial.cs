using System;
using UnityEngine;
using PX = NVIDIA.PhysX;

namespace NVIDIA.PhysX.Unity
{
    public class PxMaterial : PxAsset
    {
        #region Properties

        public override bool valid { get { return m_material != null; } }

        public PX.PxMaterial apiMaterial { get { return m_material; } }

        public float staticFriction { get { return m_staticFriction; } set { m_staticFriction = value; ValidateAndApply(); } }

        public float dynamicFriction { get { return m_dynamicFriction; } set { m_dynamicFriction = value; ValidateAndApply(); } }

        public PxCombineMode frictionCombine { get { return m_frictionCombine; } set { m_frictionCombine = value; ValidateAndApply(); } }

        public float restitution { get { return m_restitution; } set { m_restitution = value; ValidateAndApply(); } }

        public PxCombineMode restitutionCombine { get { return m_restitutionCombine; } set { m_restitutionCombine = value; ValidateAndApply(); } }

        public bool disableFriction { get { return m_disableFriction; } set { m_disableFriction = value; ValidateAndApply(); } }

        public bool disableStrongFriction { get { return m_disableStrongFriction; } set { m_disableStrongFriction = value; ValidateAndApply(); } }

        public bool improvedPatchFriction { get { return m_improvedPatchFriction; } set { m_improvedPatchFriction = value; ValidateAndApply(); } }

        #endregion

        #region Protected

        protected override void CreateAsset()
        {
            CreateMaterial();
        }

        protected override void DestroyAsset()
        {
            DestroyMaterial();
        }

        protected override void ValidateAsset()
        {
            m_staticFriction = Mathf.Max(0, m_staticFriction);
            m_dynamicFriction = Mathf.Clamp(m_dynamicFriction, 0, m_staticFriction);
            m_restitution = Mathf.Clamp01(m_restitution);
        }

        protected override void ApplyProperties()
        {
            base.ApplyProperties();

            if (valid)
            {
                m_material.setStaticFriction(m_staticFriction);
                m_material.setDynamicFriction(m_dynamicFriction);
                m_material.setRestitution(m_restitution);
                m_material.setFrictionCombineMode(m_frictionCombine);
                m_material.setRestitutionCombineMode(m_restitutionCombine);
                m_material.setFlag(PxMaterialFlag.DISABLE_FRICTION, m_disableFriction);
                m_material.setFlag(PxMaterialFlag.DISABLE_STRONG_FRICTION, m_disableStrongFriction);
                m_material.setFlag(PxMaterialFlag.IMPROVED_PATCH_FRICTION, m_improvedPatchFriction);
            }
        }

        #endregion

        #region Private

        void CreateMaterial()
        {
            m_material = PxPhysics.apiPhysics.createMaterial(m_staticFriction, m_dynamicFriction, m_restitution);
            ApplyProperties();

            //Debug.Log("PxMaterial '" + name + "' created");
        }

        void DestroyMaterial()
        {
            m_material?.release();
            m_material = null;

            //Debug.Log("PxMaterial '" + name + "' destroyed");
        }

        [NonSerialized]
        PX.PxMaterial m_material;

        [SerializeField]
        float m_staticFriction = 0.5f;
        [SerializeField]
        float m_dynamicFriction = 0.5f;
        [SerializeField]
        float m_restitution = 0.0f;
        [SerializeField]
        PxCombineMode m_frictionCombine = PxCombineMode.MIN;
        [SerializeField]
        PxCombineMode m_restitutionCombine = PxCombineMode.MULTIPLY;
        [SerializeField]
        bool m_disableFriction = false;
        [SerializeField]
        bool m_disableStrongFriction = false;
        [SerializeField]
        bool m_improvedPatchFriction = false;

        #endregion
    }
}
