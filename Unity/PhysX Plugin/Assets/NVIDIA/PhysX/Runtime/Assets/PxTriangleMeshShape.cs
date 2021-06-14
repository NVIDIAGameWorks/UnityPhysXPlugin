using NVIDIA.PhysX.UnityExtensions;
using UnityEngine;

namespace NVIDIA.PhysX.Unity
{
    public class PxTriangleMeshShape : PxSimpleShape
    {
        #region Properties

        public PxTriangleMesh triangleMesh { get { return m_triangleMesh; } }

        public Vector3 scale { get { return m_scale; } set { m_scale = value; ValidateShape(); } }

        public Quaternion scaleRotation { get { return Quaternion.Euler(m_scaleRotation); } set { m_scaleRotation = value.eulerAngles; ValidateShape(); } }

        public bool doubleSided { get { return m_doubleSided; } set { m_doubleSided = value; ValidateShape(); } }

        #endregion

        #region Messsages

        #endregion

        #region Protected

        protected override IPxDependency[] GetDependencies()
        {
            return base.GetDependencies().Append(new[] { m_triangleMesh });
        }

        protected override PxGeometry CreateGeometry()
        {
            if (m_triangleMesh == null || !m_triangleMesh.valid) return null;
            var geometry = new PxTriangleMeshGeometry(m_triangleMesh.apiTriangleMesh, new PxMeshScale(m_scale.ToPxVec3(), Quaternion.Euler(m_scaleRotation).ToPxQuat()));
            if (m_doubleSided) geometry.meshFlags = PxMeshGeometryFlag.DOUBLE_SIDED;
            return geometry;
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
        PxTriangleMesh m_triangleMesh = null;
        [SerializeField]
        Vector3 m_scale = Vector3.one;
        [SerializeField]
        Vector3 m_scaleRotation = Vector3.zero;
        [SerializeField]
        bool m_doubleSided = false;

        #endregion
    }
}
