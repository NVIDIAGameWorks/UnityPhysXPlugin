using NVIDIA.PhysX.UnityExtensions;
using UnityEngine;

namespace NVIDIA.PhysX.Unity
{
    public class PxHeightFieldShape : PxSimpleShape
    {
        #region Properties

        public PxHeightField heightField { get { return m_heightField; } }

        public float heightScale { get { return m_heightScale; } set { m_heightScale = value; ValidateShape(); } }

        public float rowScale { get { return m_rowScale; } set { m_rowScale = value; ValidateShape(); } }

        public float columnScale { get { return m_columnScale; } set { m_columnScale = value; ValidateShape(); } }

        public bool doubleSided { get { return m_doubleSided; } set { m_doubleSided = value; ValidateShape(); } }

        #endregion

        #region Messsages

        #endregion

        #region Protected

        protected override IPxDependency[] GetDependencies()
        {
            return base.GetDependencies().Append(new[] { m_heightField });
        }

        protected override PxGeometry CreateGeometry()
        {
            if (m_heightField == null || !m_heightField.valid) return null;
            var geometry = new PxHeightFieldGeometry(m_heightField.apiHeightField, m_heightScale / 0x7fff, m_rowScale / m_heightField.rowCount, m_columnScale / m_heightField.columnCount, m_doubleSided ? PxMeshGeometryFlag.DOUBLE_SIDED : 0);
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
            m_heightScale = Mathf.Max(m_heightScale, eps);
            m_rowScale = Mathf.Max(m_rowScale, eps);
            m_columnScale = Mathf.Max(m_columnScale, eps);

            base.ValidateShape();
        }

        #endregion

        #region Private

        [SerializeField]
        PxHeightField m_heightField = null;
        [SerializeField]
        float m_heightScale = 1.0f;
        [SerializeField]
        float m_rowScale = 1.0f;
        [SerializeField]
        float m_columnScale = 1.0f;
        [SerializeField]
        bool m_doubleSided = false;

        #endregion
    }
}
