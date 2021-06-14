using UnityEngine;

namespace NVIDIA.PhysX.Unity
{
    public class PxSolidShape : PxSimpleShape
    {
        #region Properties

        public PxMaterial material { get { return materials[0]; } set { materials = new[] { value }; } }

        #endregion

        #region Protected

        protected override void ApplyProperties()
        {
            base.ApplyProperties();

            if (valid)
            {
            }
        }

        #endregion
    }
}
