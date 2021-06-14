using PX = NVIDIA.PhysX;

namespace NVIDIA.PhysX.Unity
{
    public class PxShape : PxAsset
    {
        #region Properties

        public virtual PX.PxShape[] apiShapes { get { return new PX.PxShape[0]; } }

        public virtual float[] densities { get { return new float[0]; } }

        #endregion

        #region Protected

        protected override void CreateAsset()
        {
            CreateShape();
        }

        protected override void DestroyAsset()
        {
            DestroyShape();
        }

        protected override void ValidateAsset()
        {
            ValidateShape();
            base.ValidateAsset();
        }

        protected override void ResetAsset()
        {
            ResetShape();
        }

        protected override void ApplyProperties()
        {
            base.ApplyProperties();

            if (valid)
            {
            }
        }

        protected virtual void CreateShape()
        {
        }

        protected virtual void DestroyShape()
        {
        }

        protected virtual void ValidateShape()
        {
        }

        protected virtual void ResetShape()
        {
        }

        #endregion
    }
}
