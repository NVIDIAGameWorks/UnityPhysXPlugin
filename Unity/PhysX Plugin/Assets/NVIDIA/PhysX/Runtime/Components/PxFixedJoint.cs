using NVIDIA.PhysX.UnityExtensions;
using System;
using UnityEngine;
using PX = NVIDIA.PhysX;

namespace NVIDIA.PhysX.Unity
{
    [AddComponentMenu("NVIDIA/PhysX/Px Fixed Joint", 400)]
    public class PxFixedJoint : PxJoint
    {
        #region Properties

        public override bool valid { get { return m_fixedJoint != null; } }

        public override PX.PxJoint apiJoint { get { return m_fixedJoint; } }

        #endregion

        #region Protected

        protected override void CreateJoint()
        {
            base.CreateJoint();
            CreateFixedJoint();
        }

        protected override void DestroyJoint()
        {
            DestroyFixedJoint();
            base.DestroyJoint();
        }

        protected override void ValidateJoint()
        {
            base.ValidateJoint();
            Recreate();
        }

        #endregion

        #region Private

        void CreateFixedJoint()
        {
            if ((actor0 && actor0.valid) || (actor1 && actor1.valid))
            {
                var bodies0 = (actor0 && actor0.valid) ? actor0.apiActors : new PX.PxActor[] { null };
                var bodies1 = (actor1 && actor1.valid) ? actor1.apiActors : new PX.PxActor[] { null };
                var a0 = bodies0[Mathf.Clamp(body0Index, 0, bodies0.Length)] as PxRigidActor;
                var a1 = bodies1[Mathf.Clamp(body1Index, 0, bodies1.Length)] as PxRigidActor;
                var localFrame0 = new PxTransform(anchor0Position.ToPxVec3(), anchor0Rotation.ToPxQuat());
                var localFrame1 = new PxTransform(anchor1Position.ToPxVec3(), anchor1Rotation.ToPxQuat());
                m_fixedJoint = PxPhysics.apiPhysics.createFixedJoint(a0, localFrame0, a1, localFrame1);
                m_fixedJoint.userData = this;
            }

            //Debug.Log("PxFixedJoint '" + name + "' created");
        }

        void DestroyFixedJoint()
        {
            m_fixedJoint?.release();
            m_fixedJoint = null;

            //Debug.Log("PxFixedJoint '" + name + "' destroyed");
        }

        [NonSerialized]
        PX.PxFixedJoint m_fixedJoint;

        #endregion
    }
}