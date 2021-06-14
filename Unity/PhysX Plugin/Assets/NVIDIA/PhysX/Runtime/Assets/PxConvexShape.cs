using UnityEngine;

namespace NVIDIA.PhysX.Unity
{
    public class PxConvexShape : PxSolidShape
    {
        #region Properties

        public float density { get { return m_density; } set { m_density = value; ValidateAndRecreate(); } }

        public override float[] densities { get { return new[] { m_density }; } }

        #endregion

        #region Protected

        protected override void ValidateShape()
        {
            m_density = Mathf.Max(m_density, 0);

            base.ValidateShape();
        }

        #endregion

        #region Private

        [SerializeField]
        float m_density = 1000;

        #endregion
    }
}
