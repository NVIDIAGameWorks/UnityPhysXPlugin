using UnityEngine;

namespace NVIDIA.PhysX.Unity
{
    public class PxSphereShape : PxConvexShape
    {
        #region Properties

        public float radius { get { return m_radius; } set { m_radius = value; ValidateShape(); } }

        #endregion

        #region Messsages

        #endregion

        #region Protected

        protected override PxGeometry CreateGeometry()
        {
            return new PxSphereGeometry(m_radius);
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

            base.ValidateShape();
        }

        #endregion

        #region Private

        [SerializeField]
        float m_radius = 0.5f;

        #endregion
    }
}
