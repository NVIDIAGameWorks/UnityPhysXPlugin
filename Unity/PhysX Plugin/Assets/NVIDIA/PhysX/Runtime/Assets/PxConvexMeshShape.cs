using NVIDIA.PhysX.UnityExtensions;
using UnityEngine;

namespace NVIDIA.PhysX.Unity
{
    public class PxConvexMeshShape : PxConvexShape
    {
        #region Properties

        public PxConvexMesh convexMesh { get { return m_convexMesh; } set { m_convexMesh = value; ValidateShape(); } }

        public Vector3 scale { get { return m_scale; } set { m_scale = value; ValidateShape(); } }

        public Quaternion scaleRotation { get { return Quaternion.Euler(m_scaleRotation); } set { m_scaleRotation = value.eulerAngles; ValidateShape(); } }

        #endregion

        #region Messsages

        #endregion

        #region Protected

        protected override IPxDependency[] GetDependencies()
        {
            return base.GetDependencies().Append(new[] { m_convexMesh });
        }

        protected override PxGeometry CreateGeometry()
        {
            if (m_convexMesh == null || !m_convexMesh.valid) return null;
            return new PxConvexMeshGeometry(m_convexMesh.apiConvexMesh, new PxMeshScale(m_scale.ToPxVec3(), Quaternion.Euler(m_scaleRotation).ToPxQuat()));
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
            float eps = 0.000001f;
            m_scale.x = Mathf.Max(m_scale.x, eps);
            m_scale.y = Mathf.Max(m_scale.y, eps);
            m_scale.z = Mathf.Max(m_scale.z, eps);

            base.ValidateShape();
        }

        #endregion

        #region Private

        [SerializeField]
        PxConvexMesh m_convexMesh = null;
        [SerializeField]
        Vector3 m_scale = Vector3.one;
        [SerializeField]
        Vector3 m_scaleRotation = Vector3.zero;

        #endregion
    }
}
