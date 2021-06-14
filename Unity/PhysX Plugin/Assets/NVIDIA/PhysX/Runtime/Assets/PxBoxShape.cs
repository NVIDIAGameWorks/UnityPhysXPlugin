using NVIDIA.PhysX.UnityExtensions;
using UnityEngine;

namespace NVIDIA.PhysX.Unity
{
    public class PxBoxShape : PxConvexShape
    {
        #region Properties

        public Vector3 halfExtents { get { return m_halfExtents; } set { m_halfExtents = value; ValidateShape(); } }

        #endregion

        #region Messsages

        #endregion

        #region Protected

        protected override PxGeometry CreateGeometry()
        {
            return new PxBoxGeometry(m_halfExtents.ToPxVec3());
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
            m_halfExtents.x = Mathf.Max(m_halfExtents.x, eps);
            m_halfExtents.y = Mathf.Max(m_halfExtents.y, eps);
            m_halfExtents.z = Mathf.Max(m_halfExtents.z, eps);

            base.ValidateShape();
        }

        #endregion

        #region Private

        [SerializeField]
        Vector3 m_halfExtents = Vector3.one * 0.5f;

        #endregion
    }
}
