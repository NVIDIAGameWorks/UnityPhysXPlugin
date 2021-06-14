using NVIDIA.PhysX.UnityExtensions;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using PX = NVIDIA.PhysX;

namespace NVIDIA.PhysX.Unity
{
    [AddComponentMenu("NVIDIA/PhysX/Px D6 Joint", 450)]
    public class PxD6Joint : PxJoint
    {
        #region Properties

        public override bool valid { get { return m_d6Joint != null; } }

        public override PX.PxJoint apiJoint { get { return m_d6Joint; } }

        public PX.PxD6Joint apiD6Joint { get { return m_d6Joint; } }

        public SwingConstraintType swingConstraint { get { return m_swingConstraint; } set { m_swingConstraint = value; ValidateAndApply(); } }

        #endregion

        #region Methods

        public PxD6Motion AxisMotion(PxD6Axis axis)
        {
            return m_axes[(int)axis];
        }

        public void SetAxisMotion(PxD6Axis axis, PxD6Motion motion)
        {
            m_axes[(int)axis] = motion;
            ValidateAndApply();
        }

        public LinearConstraint GetLinearConstraint(PxD6Axis axis)
        {
            if (axis >= PxD6Axis.X && axis <= PxD6Axis.Z)
            {
                return DeepCopy(m_linearConstraints[(int)axis]);
            }

            Debug.LogError("Bad axis.");
            return null;
        }

        public void SetLinearConstraint(PxD6Axis axis, LinearConstraint constraint)
        {
            if (axis >= PxD6Axis.X && axis <= PxD6Axis.Z)
            {
                m_linearConstraints[(int)axis] = DeepCopy(constraint);
                ValidateAndApply();
            }

            Debug.LogError("Bad axis.");
        }

        public DistanceConstraint GetDistanceConstraint()
        {
            return DeepCopy(m_distanceConstraint);
        }

        public void SetDistanceConstraint(DistanceConstraint constraint)
        {
            m_distanceConstraint = DeepCopy(constraint);
            ValidateAndApply();
        }

        public TwistConstraint GetTwistConstraint()
        {
            return DeepCopy(m_twistConstraint);
        }

        public void SetTwistConstraint(TwistConstraint constraint)
        {
            m_twistConstraint = DeepCopy(constraint);
            ValidateAndApply();
        }

        public ConeConstraint GetConeConstraint()
        {
            return DeepCopy(m_coneConstraint);
        }

        public void SetConeConstraint(ConeConstraint constraint)
        {
            m_coneConstraint = DeepCopy(constraint);
            ValidateAndApply();
        }

        public PyramidConstraint GetPyramidConstraint()
        {
            return DeepCopy(m_pyramidConstraint);
        }

        public void SetPyramidConstraint(PyramidConstraint constraint)
        {
            m_pyramidConstraint = DeepCopy(constraint);
            ValidateAndApply();
        }

        public DriveInfo GetLinearDrive()
        {
            return DeepCopy(m_drive[0]);
        }

        public void SetLinearDrive(DriveInfo drive)
        {
            m_drive[0] = DeepCopy(drive);
            ValidateAndApply();
        }

        public DriveInfo GetAngularDrive()
        {
            return DeepCopy(m_drive[1]);
        }

        public void SetAngularDrive(DriveInfo drive)
        {
            m_drive[1] = DeepCopy(drive);
            ValidateAndApply();
        }

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
                m_d6Joint = PxPhysics.apiPhysics.createD6Joint(a0, localFrame0, a1, localFrame1);
                m_d6Joint.userData = this;
                ApplyProperties();
            }

            //Debug.Log("PxFixedJoint '" + name + "' created");
        }

        void DestroyFixedJoint()
        {
            m_d6Joint?.release();
            m_d6Joint = null;

            //Debug.Log("PxFixedJoint '" + name + "' destroyed");
        }

        protected override void ApplyProperties()
        {
            base.ApplyProperties();

            if (valid)
            {
                for (int i = 0; i < 6; ++i)
                    m_d6Joint.setMotion((PxD6Axis)i, m_axes[i]);

                for (int i = 0; i < 3; ++i)
                {
                    var info = m_linearConstraints[i];
                    var linearLimit = m_d6Joint.getLinearLimit(PxD6Axis.X + i);
                    linearLimit.lower = info.upper - info.lower > float.Epsilon ? info.lower : (info.upper + info.lower) * 0.5f - float.Epsilon;
                    linearLimit.upper = info.upper - info.lower > float.Epsilon ? info.upper : (info.upper + info.lower) * 0.5f + float.Epsilon;
                    linearLimit.restitution = info.restitution;
                    linearLimit.bounceThreshold = info.bounceThreshold;
                    linearLimit.stiffness = info.stiffness;
                    linearLimit.damping = info.damping;
                    linearLimit.contactDistance = info.contactDistance;
                    m_d6Joint.setLinearLimit(PxD6Axis.X + i, linearLimit);
                }

                var distanceLimit = m_d6Joint.getDistanceLimit();
                distanceLimit.value = Mathf.Max(m_distanceConstraint.maxDistance, float.Epsilon);
                distanceLimit.restitution = m_distanceConstraint.restitution;
                distanceLimit.bounceThreshold = m_distanceConstraint.bounceThreshold;
                distanceLimit.stiffness = m_distanceConstraint.stiffness;
                distanceLimit.damping = m_distanceConstraint.damping;
                distanceLimit.contactDistance = m_distanceConstraint.contactDistance;
                m_d6Joint.setDistanceLimit(distanceLimit);

                var twistLimit = m_d6Joint.getTwistLimit();
                float lower = m_twistConstraint.minAngle * Mathf.Deg2Rad, upper = m_twistConstraint.maxAngle * Mathf.Deg2Rad;
                twistLimit.lower = upper - lower > float.Epsilon ? lower : (upper + lower) * 0.5f - float.Epsilon;
                twistLimit.upper = upper - lower > float.Epsilon ? upper : (upper + lower) * 0.5f + float.Epsilon;
                twistLimit.restitution = m_twistConstraint.restitution;
                twistLimit.bounceThreshold = m_twistConstraint.bounceThreshold;
                twistLimit.stiffness = m_twistConstraint.stiffness;
                twistLimit.damping = m_twistConstraint.damping;
                twistLimit.contactDistance = m_twistConstraint.contactDistance;
                m_d6Joint.setTwistLimit(twistLimit);

                float eps = 0.00001f;
                if (m_swingConstraint == SwingConstraintType.CONE)
                {
                    var swingLimit = m_d6Joint.getSwingLimit();
                    swingLimit.yAngle = Mathf.Clamp(m_coneConstraint.yAngle * Mathf.Deg2Rad, eps, Mathf.PI - eps);
                    swingLimit.zAngle = Mathf.Clamp(m_coneConstraint.zAngle * Mathf.Deg2Rad, eps, Mathf.PI - eps);
                    swingLimit.restitution = m_coneConstraint.restitution;
                    swingLimit.bounceThreshold = m_coneConstraint.bounceThreshold;
                    swingLimit.stiffness = m_coneConstraint.stiffness;
                    swingLimit.damping = m_coneConstraint.damping;
                    swingLimit.contactDistance = m_coneConstraint.contactDistance;
                    m_d6Joint.setSwingLimit(swingLimit);
                }
                else
                {
                    var pyramidLimit = m_d6Joint.getPyramidSwingLimit();
                    pyramidLimit.yAngleMin = Mathf.Clamp(m_pyramidConstraint.yAngleMin * Mathf.Deg2Rad, -Mathf.PI + eps, Mathf.PI - eps);
                    pyramidLimit.yAngleMax = Mathf.Clamp(m_pyramidConstraint.yAngleMax * Mathf.Deg2Rad, -Mathf.PI + eps, Mathf.PI - eps);
                    pyramidLimit.zAngleMin = Mathf.Clamp(m_pyramidConstraint.zAngleMin * Mathf.Deg2Rad, -Mathf.PI + eps, Mathf.PI - eps);
                    pyramidLimit.zAngleMax = Mathf.Clamp(m_pyramidConstraint.zAngleMax * Mathf.Deg2Rad, -Mathf.PI + eps, Mathf.PI - eps);
                    pyramidLimit.restitution = m_pyramidConstraint.restitution;
                    pyramidLimit.bounceThreshold = m_pyramidConstraint.bounceThreshold;
                    pyramidLimit.stiffness = m_pyramidConstraint.stiffness;
                    pyramidLimit.damping = m_pyramidConstraint.damping;
                    pyramidLimit.contactDistance = m_pyramidConstraint.contactDistance;
                    m_d6Joint.setPyramidSwingLimit(pyramidLimit);
                }

                m_d6Joint.setDrivePosition(new PxTransform(m_drive[0].position.ToPxVec3(), Quaternion.Euler(m_drive[1].position).ToPxQuat()));
                m_d6Joint.setDriveVelocity(m_drive[0].velocity.ToPxVec3(), m_drive[1].velocity.ToPxVec3());
                var drive = new PxD6JointDrive();
                drive.stiffness = m_drive[0].stiffness.x;
                drive.damping = m_drive[0].damping.x;
                drive.forceLimit = m_drive[0].maxForce.x;
                m_d6Joint.setDrive(PxD6Drive.X, drive);
                drive.stiffness = m_drive[0].stiffness.y;
                drive.damping = m_drive[0].damping.y;
                drive.forceLimit = m_drive[0].maxForce.y;
                m_d6Joint.setDrive(PxD6Drive.Y, drive);
                drive.stiffness = m_drive[0].stiffness.z;
                drive.damping = m_drive[0].damping.z;
                drive.forceLimit = m_drive[0].maxForce.z;
                m_d6Joint.setDrive(PxD6Drive.Z, drive);
                drive.stiffness = m_drive[1].stiffness.x;
                drive.damping = m_drive[1].damping.x;
                drive.forceLimit = m_drive[1].maxForce.x;
                m_d6Joint.setDrive(PxD6Drive.TWIST, drive);
                //drive.stiffness = Mathf.Max(m_drive[1].stiffness.y, m_drive[1].stiffness.z);
                //drive.damping = Mathf.Max(m_drive[1].damping.y, m_drive[1].damping.z);
                //drive.forceLimit = Mathf.Max(m_drive[1].maxForce.y, m_drive[1].maxForce.z);
                //m_d6Joint.setDrive(PxD6Drive.SWING, drive);
                drive.stiffness = m_drive[1].stiffness.y;
                drive.damping = m_drive[1].damping.y;
                drive.forceLimit = m_drive[1].maxForce.y;
                m_d6Joint.setDrive(PxD6Drive.SWING, drive);
                drive.stiffness = m_drive[1].stiffness.z;
                drive.damping = m_drive[1].damping.z;
                drive.forceLimit = m_drive[1].maxForce.z;
                m_d6Joint.setDrive(PxD6Drive.SLERP, drive);
            }
        }

        static T DeepCopy<T>(T other)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(ms, other);
                ms.Position = 0;
                return (T)formatter.Deserialize(ms);
            }
        }

        [Serializable]
        public class LinearConstraint
        {
            public float lower = 0;
            public float upper = 0;
            public float restitution = 0;
            public float bounceThreshold = 0;
            public float stiffness = 0;
            public float damping = 0;
            public float contactDistance = 0;
        }

        [Serializable]
        public class DistanceConstraint
        {
            public float maxDistance = 0;
            public float restitution = 0;
            public float bounceThreshold = 0;
            public float stiffness = 0;
            public float damping = 0;
            public float contactDistance = 0;
        }

        [Serializable]
        public class TwistConstraint
        {
            public float minAngle = -180;
            public float maxAngle = 180;
            public float restitution = 0;
            public float bounceThreshold = 0;
            public float stiffness = 0;
            public float damping = 0;
            public float contactDistance = 0;
        }

        public enum SwingConstraintType { CONE, PYRAMID };

        [Serializable]
        public class ConeConstraint
        {
            public float yAngle = 180;
            public float zAngle = 180;
            public float restitution = 0;
            public float bounceThreshold = 0;
            public float stiffness = 0;
            public float damping = 0;
            public float contactDistance = 0;
        }

        [Serializable]
        public class PyramidConstraint
        {
            public float yAngleMin = -180;
            public float yAngleMax = 180;
            public float zAngleMin = -180;
            public float zAngleMax = 180;
            public float restitution = 0;
            public float bounceThreshold = 0;
            public float stiffness = 0;
            public float damping = 0;
            public float contactDistance = 0;
        }

        [Serializable]
        public class DriveInfo
        {
            public Vector3 position = Vector3.zero;
            public Vector3 velocity = Vector3.zero;
            public Vector3 stiffness = Vector3.zero;
            public Vector3 damping = Vector3.zero;
            public Vector3 maxForce = Vector3.one * float.MaxValue;
        }

        [NonSerialized]
        PX.PxD6Joint m_d6Joint;

        [SerializeField]
        PX.PxD6Motion[] m_axes = new[] { new PxD6Motion(), new PxD6Motion(), new PxD6Motion(), new PxD6Motion(), new PxD6Motion(), new PxD6Motion() };
        [SerializeField]
        LinearConstraint[] m_linearConstraints = new[] { new LinearConstraint(), new LinearConstraint(), new LinearConstraint() };
        [SerializeField]
        DistanceConstraint m_distanceConstraint = new DistanceConstraint();
        [SerializeField]
        TwistConstraint m_twistConstraint = new TwistConstraint();
        [SerializeField]
        SwingConstraintType m_swingConstraint = SwingConstraintType.CONE;
        [SerializeField]
        ConeConstraint m_coneConstraint = new ConeConstraint();
        [SerializeField]
        PyramidConstraint m_pyramidConstraint = new PyramidConstraint();
        [SerializeField]
        DriveInfo[] m_drive = new[] { new DriveInfo(), new DriveInfo() };

        #endregion
    }
}