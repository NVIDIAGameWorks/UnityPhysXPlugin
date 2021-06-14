using UnityEngine;

namespace NVIDIA.PhysX.Unity
{
    public class PxCapsuleShape : PxConvexShape
    {
        #region Properties

        public float radius { get { return m_radius; } set { m_radius = value; ValidateShape(); } }
        public float halfHeight { get { return m_halfHeight; } set { m_halfHeight = value; ValidateShape(); } }

        #endregion

        #region Messsages

        #endregion

        #region Protected

        protected override PxGeometry CreateGeometry()
        {
            return new PxCapsuleGeometry(m_radius, m_halfHeight);
        }

        protected override void ApplyProperties()
        {
            base.ApplyProperties();

            if (valid)
            {
            }
        }

        protected override void ValidateShape()
        {
            float eps = 0.0001f;
            m_radius = Mathf.Max(m_radius, eps);
            m_halfHeight = Mathf.Max(m_halfHeight, 0);

            base.ValidateShape();
        }

        #endregion

        #region Private

        [SerializeField]
        float m_radius = 0.5f;
        [SerializeField]
        float m_halfHeight = 0.5f;

        #endregion
    }
}
