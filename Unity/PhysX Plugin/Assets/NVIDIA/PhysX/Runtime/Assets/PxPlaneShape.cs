namespace NVIDIA.PhysX.Unity
{
    public class PxPlaneShape : PxSolidShape
    {
        #region Protected

        protected override PxGeometry CreateGeometry()
        {
            return new PxPlaneGeometry();
        }

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
