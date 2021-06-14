using NVIDIA.PhysX.UnityExtensions;
using UnityEngine;
using PX = NVIDIA.PhysX;

namespace NVIDIA.PhysX.Unity
{
    public delegate void OnBreak(PxJoint joint);

    [AddComponentMenu("")]
    public class PxJoint : PxComponent
    {
        #region Properties

        public virtual PX.PxJoint apiJoint { get { return null; } }

        public PxActor actor0 { get { return m_actor0; } set { m_actor0 = value; ValidateAndRecreate(); } }

        public int body0Index { get { return m_body0Index; } set { m_body0Index = value; ValidateAndRecreate(); } }

        public Vector3 anchor0Position { get { return m_position0; } set { m_position0 = value; ValidateAndApply(); } }

        public Quaternion anchor0Rotation { get { return Quaternion.Euler(m_rotation0); } set { m_rotation0 = value.eulerAngles; ValidateAndApply(); } }

        public PxActor actor1 { get { return m_actor1; } set { m_actor1 = value; ValidateAndRecreate(); } }

        public int body1Index { get { return m_body1Index; } set { m_body1Index = value; ValidateAndRecreate(); } }

        public Vector3 anchor1Position { get { return m_position1; } set { m_position1 = value; ValidateAndApply(); } }

        public Quaternion anchor1Rotation { get { return Quaternion.Euler(m_rotation1); } set { m_rotation1 = value.eulerAngles; ValidateAndApply(); } }

        public float breakForce { get { return m_breakForce; } set { m_breakForce = value; ValidateAndApply(); } }

        public float breakTorque { get { return m_breakTorque; } set { m_breakTorque = value; ValidateAndApply(); } }

        public bool enableCollision { get { return m_enableCollision; } set { m_enableCollision = value; ValidateAndApply(); } }

        public bool disablePreprocessing { get { return m_disablePreprocessing; } set { m_disablePreprocessing = value; ValidateAndApply(); } }

        public float invMassScale0 { get { return m_invMassScale0; } set { m_invMassScale0 = value; ValidateAndApply(); } }

        public float invInertiaScale0 { get { return m_invInertiaScale0; } set { m_invInertiaScale0 = value; ValidateAndApply(); } }

        public float invMassScale1 { get { return m_invMassScale1; } set { m_invMassScale1 = value; ValidateAndApply(); } }

        public float invInertiaScale1 { get { return m_invInertiaScale1; } set { m_invInertiaScale1 = value; ValidateAndApply(); } }

        #endregion

        #region Events

        public OnBreak onBreak;

        #endregion

        #region Methods

        public void SetJointAnchors(Vector3 position0, Quaternion rotation0, bool computeAnchor1)
        {
            m_position0 = position0;
            m_rotation0 = rotation0.eulerAngles;
            if (computeAnchor1)
            {
                var transform0 = m_actor0 ? m_actor0.transform.ToPxTransform() : PxTransform.identity;
                var transform1 = m_actor1 ? m_actor1.transform.ToPxTransform() : PxTransform.identity;
                var parentPose = transform1.transformInv(transform0.transform(new PxTransform(position0.ToPxVec3(), rotation0.ToPxQuat())));
                m_position1 = parentPose.p.ToVector3();
                m_rotation1 = parentPose.q.ToQuaternion().eulerAngles;
            }
        }

        public void SetJointAnchots(Vector3 position0, Quaternion rotation0, Vector3 position1, Quaternion rotation1)
        {
            m_position0 = position0;
            m_rotation0 = rotation0.eulerAngles;
            m_position1 = position1;
            m_rotation1 = rotation1.eulerAngles;
        }

        #endregion

        #region Protected

        protected override IPxDependency[] GetDependencies()
        {
            return base.GetDependencies().Append(new[] { m_actor0, m_actor1 });
        }

        protected override void CreateComponent()
        {
            CreateJoint();
            ApplyProperties();
        }

        protected override void DestroyComponent()
        {
            DestroyJoint();
        }

        protected override void ValidateComponent()
        {
            ValidateJoint();
        }

        protected override void ResetComponent()
        {
            ResetJoint();
        }

        protected virtual void CreateJoint()
        {
        }

        protected virtual void DestroyJoint()
        {
        }

        protected virtual void ValidateJoint()
        {
        }

        protected virtual void ResetJoint()
        {
            var actor0 = GetComponent<PxActor>();
            if (actor0)
            {
                m_actor0 = actor0;
                SetJointAnchors(Vector3.zero, Quaternion.identity, true);
            }
        }

        #endregion

        #region Private

        protected override void ApplyProperties()
        {
            base.ApplyProperties();

            if (valid)
            {
                apiJoint.setBreakForce(m_breakForce, m_breakTorque);
                apiJoint.setConstraintFlag(PxConstraintFlag.COLLISION_ENABLED, m_enableCollision);
                apiJoint.setConstraintFlag(PxConstraintFlag.DISABLE_PREPROCESSING, m_disablePreprocessing);
                apiJoint.setInvMassScale0(m_invMassScale0);
                apiJoint.setInvInertiaScale0(m_invInertiaScale0);
                apiJoint.setInvMassScale1(m_invMassScale1);
                apiJoint.setInvInertiaScale1(m_invInertiaScale1);
                apiJoint.setLocalPose(PxJointActorIndex.ACTOR0, new PxTransform(m_position0.ToPxVec3(), Quaternion.Euler(m_rotation0).ToPxQuat()));
                apiJoint.setLocalPose(PxJointActorIndex.ACTOR1, new PxTransform(m_position1.ToPxVec3(), Quaternion.Euler(m_rotation1).ToPxQuat()));
            }
        }

        [SerializeField]
        PxActor m_actor0 = null;
        [SerializeField]
        int m_body0Index = 0;
        [SerializeField]
        Vector3 m_position0 = Vector3.zero;
        [SerializeField]
        Vector3 m_rotation0 = Vector3.zero;
        [SerializeField]
        PxActor m_actor1 = null;
        [SerializeField]
        int m_body1Index = 0;
        [SerializeField]
        Vector3 m_position1 = Vector3.zero;
        [SerializeField]
        Vector3 m_rotation1 = Vector3.zero;
        [SerializeField]
        float m_breakForce = float.MaxValue;
        [SerializeField]
        float m_breakTorque = float.MaxValue;
        [SerializeField]
        bool m_enableCollision = false;
        [SerializeField]
        bool m_disablePreprocessing = false;
        [SerializeField]
        float m_invMassScale0 = 1.0f;
        [SerializeField]
        float m_invInertiaScale0 = 1.0f;
        [SerializeField]
        float m_invMassScale1 = 1.0f;
        [SerializeField]
        float m_invInertiaScale1 = 1.0f;

        #endregion
    }
}