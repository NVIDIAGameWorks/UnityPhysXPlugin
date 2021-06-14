using NVIDIA.PhysX.UnityExtensions;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PX = NVIDIA.PhysX;

namespace NVIDIA.PhysX.Unity
{
    [AddComponentMenu("NVIDIA/PhysX/Px Articulated Actor", 220), SelectionBase]
    public class PxArticulatedActor : PxActor
    {
        #region Properties

        public override bool valid { get { return m_articulation != null; } }

        public override PX.PxActor[] apiActors { get { return m_links; } }

        public override string[] apiActorNames { get { return GetLinkNames(); } }

        public PxArticulationReducedCoordinate apiArticulation { get { return m_articulation; } }

        public int linkCount { get { return m_linkInfos.Length; } }

        public int dofCount { get { return m_jointPosition.Length; } }

        public float[] dofPositions { get { return m_jointPosition; } }

        public float[] dofVelocities { get { return m_jointVelocity; } }

        #endregion

        #region Methods

        public void PopulateWithChildren()
        {
            BeginRecreate();

            m_linkInfos = new[] { m_linkInfos[0] };
            m_linkInfos[0].transform = transform;

            Func<int, Transform, int> addChildLink = null;
            addChildLink = (parentLink, transform) =>
            {
                int linkIndex = AddLink(parentLink);
                SetLinkTransform(linkIndex, transform);
                SetLinkJointPoses(linkIndex, Vector3.zero, Quaternion.identity, true);
                for (int i = 0; i < transform.childCount; ++i)
                    addChildLink(linkIndex, transform.GetChild(i));
                return 0;
            };
            for (int i = 0; i < transform.childCount; ++i)
                addChildLink(0, transform.GetChild(i));

            ValidateAndRecreate();

            EndRecreate();
        }

        public int AddLink(int parentLink)
        {
            if (parentLink < 0 || parentLink >= m_linkInfos.Length)
            {
                Debug.LogError("Bad link index.");
                return -1;
            }

            var linkInfo = new LinkInfo() { parentLink = parentLink };
            var parentLinkInfo = m_linkInfos[parentLink];
            Array.Resize(ref parentLinkInfo.childLinks, parentLinkInfo.childLinks.Length + 1);
            parentLinkInfo.childLinks[parentLinkInfo.childLinks.Length - 1] = m_linkInfos.Length;
            Array.Resize(ref m_linkInfos, m_linkInfos.Length + 1);
            m_linkInfos[m_linkInfos.Length - 1] = linkInfo;

            linkInfo.jointInfo = new JointInfo();

            ValidateAndRecreate();

            return m_linkInfos.Length - 1;
        }

        public void MoveLinkUp(int linkIndex)
        {
            if (linkIndex < 1 || linkIndex >= m_linkInfos.Length)
            {
                Debug.LogError("Bad link index.");
                return;
            }

            if (m_linkInfos[linkIndex].parentLink == 0)
                return;

            int oldParent = m_linkInfos[linkIndex].parentLink;
            var oldParentChildren = new List<int>(m_linkInfos[oldParent].childLinks);
            oldParentChildren.Remove(linkIndex);
            m_linkInfos[oldParent].childLinks = oldParentChildren.ToArray();

            int newParent = m_linkInfos[oldParent].parentLink;
            var newParentChildren = new List<int>(m_linkInfos[newParent].childLinks);
            newParentChildren.Add(linkIndex);
            m_linkInfos[newParent].childLinks = newParentChildren.ToArray();

            m_linkInfos[linkIndex].parentLink = newParent;

            ValidateAndRecreate();
        }

        public void RemoveLink(int linkIndex)
        {
            if (linkIndex < 1 || linkIndex >= m_linkInfos.Length)
            {
                Debug.LogError("Bad link index.");
                return;
            }

            Func<int, int> removeLink = null;
            removeLink = index =>
            {
                for (int child = m_linkInfos[index].childLinks.Length - 1; child >= 0; --child)
                    removeLink(m_linkInfos[index].childLinks[child]);

                int parent = m_linkInfos[index].parentLink;
                var parentChildren = new List<int>(m_linkInfos[parent].childLinks);
                parentChildren.Remove(index);
                m_linkInfos[parent].childLinks = parentChildren.ToArray();

                var linkInfos = new List<LinkInfo>(m_linkInfos);
                linkInfos.RemoveAt(index);
                foreach (var item in linkInfos)
                {
                    if (item.parentLink >= index)
                        item.parentLink = item.parentLink - 1;
                    for (int i = 0; i < item.childLinks.Length; ++i)
                        if (item.childLinks[i] >= index)
                            item.childLinks[i] = item.childLinks[i] - 1;
                }
                m_linkInfos = linkInfos.ToArray();

                return 0;
            };
            removeLink(linkIndex);

            ValidateAndRecreate();
        }

        public void SetLinkTransform(int linkIndex, Transform transform)
        {
            if (linkIndex < 1 || linkIndex >= m_linkInfos.Length)
            {
                Debug.LogError("Bad link index.");
                return;
            }

            m_linkInfos[linkIndex].transform = transform;

            ValidateAndRecreate();
        }

        public void SetLinkJointPoses(int linkIndex, Vector3 position, Quaternion rotation, Vector3 parentPosition, Quaternion parentRotation)
        {
            if (linkIndex < 1 || linkIndex >= m_linkInfos.Length)
            {
                Debug.LogError("Bad link index.");
                return;
            }

            m_linkInfos[linkIndex].jointInfo.position = position;
            m_linkInfos[linkIndex].jointInfo.rotation = rotation.eulerAngles;
            m_linkInfos[linkIndex].jointInfo.parentPosition = parentPosition;
            m_linkInfos[linkIndex].jointInfo.parentRotation = parentRotation.eulerAngles;

            ValidateAndRecreate();
        }

        public void SetLinkJointPoses(int linkIndex, Vector3 position, Quaternion rotation, bool computeParent)
        {
            if (linkIndex < 1 || linkIndex >= m_linkInfos.Length)
            {
                Debug.LogError("Bad link index.");
                return;
            }

            var linkInfo = m_linkInfos[linkIndex];
            linkInfo.jointInfo.position = position;
            linkInfo.jointInfo.rotation = rotation.eulerAngles;
            if (computeParent)
            {
                var parentTransformIndex = linkInfo.parentLink;
                while (m_linkInfos[parentTransformIndex].transform == null) parentTransformIndex = m_linkInfos[parentTransformIndex].parentLink;
                var parentTransform = m_linkInfos[parentTransformIndex].transform;
                var linkTransform = linkInfo.transform == null ? parentTransform : linkInfo.transform;
                var parentPose = parentTransform.ToPxTransform().transformInv(linkTransform.ToPxTransform().transform(new PxTransform(position.ToPxVec3(), rotation.ToPxQuat())));
                m_linkInfos[linkIndex].jointInfo.parentPosition = parentPose.p.ToVector3();
                m_linkInfos[linkIndex].jointInfo.parentRotation = parentPose.q.ToQuaternion().eulerAngles;
            }

            ValidateAndRecreate();
        }

        public Vector3 GetLinkJointPosition(int linkIndex)
        {
            if (linkIndex < 1 || linkIndex >= m_linkInfos.Length)
            {
                Debug.LogError("Bad link index.");
                return Vector3.zero;
            }

            return m_linkInfos[linkIndex].jointInfo.position;
        }

        public void SetLinkJointPosition(int linkIndex, Vector3 jointPosition)
        {
            if (linkIndex < 1 || linkIndex >= m_linkInfos.Length)
            {
                Debug.LogError("Bad link index.");
                return;
            }

            m_linkInfos[linkIndex].jointInfo.position = jointPosition;

            ValidateAndRecreate();
        }

        public Quaternion GetLinkJointRotation(int linkIndex)
        {
            if (linkIndex < 1 || linkIndex >= m_linkInfos.Length)
            {
                Debug.LogError("Bad link index.");
                return Quaternion.identity;
            }

            return Quaternion.Euler(m_linkInfos[linkIndex].jointInfo.rotation);
        }

        public void SetLinkJointRotation(int linkIndex, Quaternion jointRotation)
        {
            if (linkIndex < 1 || linkIndex >= m_linkInfos.Length)
            {
                Debug.LogError("Bad link index.");
                return;
            }

            m_linkInfos[linkIndex].jointInfo.rotation = jointRotation.eulerAngles;

            ValidateAndRecreate();
        }

        public Vector3 GetLinkJointParentPosition(int linkIndex)
        {
            if (linkIndex < 1 || linkIndex >= m_linkInfos.Length)
            {
                Debug.LogError("Bad link index.");
                return Vector3.zero;
            }

            return m_linkInfos[linkIndex].jointInfo.parentPosition;
        }

        public void SetLinkJointParentPosition(int linkIndex, Vector3 jointParentPosition)
        {
            if (linkIndex < 1 || linkIndex >= m_linkInfos.Length)
            {
                Debug.LogError("Bad link index.");
                return;
            }

            m_linkInfos[linkIndex].jointInfo.parentPosition = jointParentPosition;

            ValidateAndRecreate();
        }

        public Quaternion GetLinkJointParentRotation(int linkIndex)
        {
            if (linkIndex < 1 || linkIndex >= m_linkInfos.Length)
            {
                Debug.LogError("Bad link index.");
                return Quaternion.identity;
            }

            return Quaternion.Euler(m_linkInfos[linkIndex].jointInfo.parentRotation);
        }

        public void SetLinkJointParentRotation(int linkIndex, Quaternion jointParentRotation)
        {
            if (linkIndex < 1 || linkIndex >= m_linkInfos.Length)
            {
                Debug.LogError("Bad link index.");
                return;
            }

            m_linkInfos[linkIndex].jointInfo.parentRotation = jointParentRotation.eulerAngles;

            ValidateAndRecreate();
        }

        public Transform GetLinkTransform(int linkIndex)
        {
            if (linkIndex < 0 || linkIndex >= m_linkInfos.Length)
            {
                Debug.LogError("Bad link index.");
                return null;
            }

            return m_linkInfos[linkIndex].transform;
        }

        public PxShape GetLinkCollisionShape(int linkIndex)
        {
            if (linkIndex < 0 || linkIndex >= m_linkInfos.Length)
            {
                Debug.LogError("Bad link index.");
                return null;
            }

            return m_linkInfos[linkIndex].collisionShape;
        }

        public void SetLinkCollisionShape(int linkIndex, PxShape collisionShape)
        {
            if (linkIndex < 0 || linkIndex >= m_linkInfos.Length)
            {
                Debug.LogError("Bad link index.");
                return;
            }

            m_linkInfos[linkIndex].collisionShape = collisionShape;

            ValidateAndRecreate();
        }

        public int GetLinkParentLinkIndex(int linkIndex)
        {
            if (linkIndex < 0 || linkIndex >= m_linkInfos.Length)
            {
                Debug.LogError("Bad link index.");
                return -1;
            }

            return m_linkInfos[linkIndex].parentLink;
        }

        public int GetLinkChildLinkCount(int linkIndex)
        {
            if (linkIndex < 0 || linkIndex >= m_linkInfos.Length)
            {
                Debug.LogError("Bad link index.");
                return -1;
            }

            return m_linkInfos[linkIndex].childLinks.Length;
        }

        public int GetLinkChildLinkIndex(int linkIndex, int childIndex)
        {
            if (linkIndex < 0 || linkIndex >= m_linkInfos.Length)
            {
                Debug.LogError("Bad link index.");
                return -1;
            }
            if (childIndex < 0 || childIndex >= m_linkInfos[linkIndex].childLinks.Length)
            {
                Debug.LogError("Bad child index.");
                return -1;
            }

            return m_linkInfos[linkIndex].childLinks[childIndex];
        }

        public bool GetLinkAutoMass(int linkIndex)
        {
            if (linkIndex < 0 || linkIndex >= m_linkInfos.Length)
            {
                Debug.LogError("Bad link index.");
                return false;
            }

            return m_linkInfos[linkIndex].massInfo.autoMass;
        }

        public void SetLinkAutoMass(int linkIndex, bool yes)
        {
            if (linkIndex < 0 || linkIndex >= m_linkInfos.Length)
            {
                Debug.LogError("Bad link index.");
                return;
            }

            m_linkInfos[linkIndex].massInfo.autoMass = yes;

            ValidateAndRecreate();
        }

        public float GetLinkMass(int linkIndex)
        {
            if (linkIndex < 0 || linkIndex >= m_linkInfos.Length)
            {
                Debug.LogError("Bad link index.");
                return 0;
            }

            return m_linkInfos[linkIndex].massInfo.mass;
        }

        public void SetLinkMass(int linkIndex, float mass)
        {
            if (linkIndex < 0 || linkIndex >= m_linkInfos.Length)
            {
                Debug.LogError("Bad link index.");
                return;
            }

            m_linkInfos[linkIndex].massInfo.mass = mass;

            ValidateAndRecreate();
        }

        public Vector3 GetLinkLocalInertia(int linkIndex)
        {
            if (linkIndex < 0 || linkIndex >= m_linkInfos.Length)
            {
                Debug.LogError("Bad link index.");
                return Vector3.zero;
            }

            return m_linkInfos[linkIndex].massInfo.inertia;
        }

        public void SetLinkLocalInertia(int linkIndex, Vector3 localInertia)
        {
            if (linkIndex < 0 || linkIndex >= m_linkInfos.Length)
            {
                Debug.LogError("Bad link index.");
                return;
            }

            m_linkInfos[linkIndex].massInfo.inertia = localInertia;

            ValidateAndRecreate();
        }

        public Vector3 GetLinkMassPosition(int linkIndex)
        {
            if (linkIndex < 0 || linkIndex >= m_linkInfos.Length)
            {
                Debug.LogError("Bad link index.");
                return Vector3.zero;
            }

            return m_linkInfos[linkIndex].massInfo.position;
        }

        public void SetLinkMassPosition(int linkIndex, Vector3 massPosition)
        {
            if (linkIndex < 0 || linkIndex >= m_linkInfos.Length)
            {
                Debug.LogError("Bad link index.");
                return;
            }

            m_linkInfos[linkIndex].massInfo.position = massPosition;

            ValidateAndRecreate();
        }

        public Quaternion GetLinkMassRotation(int linkIndex)
        {
            if (linkIndex < 0 || linkIndex >= m_linkInfos.Length)
            {
                Debug.LogError("Bad link index.");
                return Quaternion.identity;
            }

            return Quaternion.Euler(m_linkInfos[linkIndex].massInfo.rotation);
        }

        public void SetLinkMassRotation(int linkIndex, Quaternion massRotation)
        {
            if (linkIndex < 0 || linkIndex >= m_linkInfos.Length)
            {
                Debug.LogError("Bad link index.");
                return;
            }

            m_linkInfos[linkIndex].massInfo.rotation = massRotation.eulerAngles;

            ValidateAndRecreate();
        }

        public PxArticulationJointType GetLinkJointType(int linkIndex)
        {
            if (linkIndex < 1 || linkIndex >= m_linkInfos.Length)
            {
                Debug.LogError("Bad link index.");
                return PxArticulationJointType.FIX;
            }

            return m_linkInfos[linkIndex].jointInfo.type;
        }

        public void SetLinkJointType(int linkIndex, PxArticulationJointType jointType)
        {
            if (linkIndex < 1 || linkIndex >= m_linkInfos.Length)
            {
                Debug.LogError("Bad link index.");
                return;
            }

            m_linkInfos[linkIndex].jointInfo.type = jointType;

            ValidateAndRecreate();
        }

        public float GetLinkJointFriction(int linkIndex)
        {
            if (linkIndex < 1 || linkIndex >= m_linkInfos.Length)
            {
                Debug.LogError("Bad link index.");
                return 0;
            }

            return m_linkInfos[linkIndex].jointInfo.friction;
        }

        public void SetLinkJointFriction(int linkIndex, float jointFriction)
        {
            if (linkIndex < 1 || linkIndex >= m_linkInfos.Length)
            {
                Debug.LogError("Bad link index.");
                return;
            }

            m_linkInfos[linkIndex].jointInfo.friction = jointFriction;

            if (!m_deferApply) ValidateAndApply();
        }

        public float GetLinkJointMaxVelocity(int linkIndex)
        {
            if (linkIndex < 1 || linkIndex >= m_linkInfos.Length)
            {
                Debug.LogError("Bad link index.");
                return 0;
            }

            return m_linkInfos[linkIndex].jointInfo.maxVelocity;
        }

        public void SetLinkJointMaxVelocity(int linkIndex, float jointMaxVelocity)
        {
            if (linkIndex < 1 || linkIndex >= m_linkInfos.Length)
            {
                Debug.LogError("Bad link index.");
                return;
            }

            m_linkInfos[linkIndex].jointInfo.maxVelocity = jointMaxVelocity;

            if (!m_deferApply) ValidateAndApply();
        }

        public float GetLinkJointAxisLowerLimit(int linkIndex, PxArticulationAxis jointAxis)
        {
            if (linkIndex < 1 || linkIndex >= m_linkInfos.Length)
            {
                Debug.LogError("Bad link index.");
                return 0;
            }

            return m_linkInfos[linkIndex].jointInfo.axisInfos[(int)jointAxis].limits.lower;
        }

        public void SetLinkJointAxisLowerLimit(int linkIndex, PxArticulationAxis jointAxis, float lowerLimit)
        {
            if (linkIndex < 1 || linkIndex >= m_linkInfos.Length)
            {
                Debug.LogError("Bad link index.");
                return;
            }

            m_linkInfos[linkIndex].jointInfo.axisInfos[(int)jointAxis].limits.lower = lowerLimit;

            if (!m_deferApply) ValidateAndApply();
        }

        public float GetLinkJointAxisUpperLimit(int linkIndex, PxArticulationAxis jointAxis)
        {
            if (linkIndex < 1 || linkIndex >= m_linkInfos.Length)
            {
                Debug.LogError("Bad link index.");
                return 0;
            }

            return m_linkInfos[linkIndex].jointInfo.axisInfos[(int)jointAxis].limits.upper;
        }

        public void SetLinkJointAxisUpperLimit(int linkIndex, PxArticulationAxis jointAxis, float upperLimit)
        {
            if (linkIndex < 1 || linkIndex >= m_linkInfos.Length)
            {
                Debug.LogError("Bad link index.");
                return;
            }

            m_linkInfos[linkIndex].jointInfo.axisInfos[(int)jointAxis].limits.upper = upperLimit;

            if (!m_deferApply) ValidateAndApply();
        }

        public float GetLinkJointAxisDriveStiffness(int linkIndex, PxArticulationAxis jointAxis)
        {
            if (linkIndex < 1 || linkIndex >= m_linkInfos.Length)
            {
                Debug.LogError("Bad link index.");
                return 0;
            }

            return m_linkInfos[linkIndex].jointInfo.axisInfos[(int)jointAxis].drive.stiffness;
        }

        public void SetLinkJointAxisDriveStiffness(int linkIndex, PxArticulationAxis jointAxis, float driveStiffness)
        {
            if (linkIndex < 1 || linkIndex >= m_linkInfos.Length)
            {
                Debug.LogError("Bad link index.");
                return;
            }

            m_linkInfos[linkIndex].jointInfo.axisInfos[(int)jointAxis].drive.stiffness = driveStiffness;

            if (!m_deferApply) ValidateAndApply();
        }

        public float GetLinkJointAxisDriveDamping(int linkIndex, PxArticulationAxis jointAxis)
        {
            if (linkIndex < 1 || linkIndex >= m_linkInfos.Length)
            {
                Debug.LogError("Bad link index.");
                return 0;
            }

            return m_linkInfos[linkIndex].jointInfo.axisInfos[(int)jointAxis].drive.damping;
        }

        public void SetLinkJointAxisDriveDamping(int linkIndex, PxArticulationAxis jointAxis, float driveDamping)
        {
            if (linkIndex < 1 || linkIndex >= m_linkInfos.Length)
            {
                Debug.LogError("Bad link index.");
                return;
            }

            m_linkInfos[linkIndex].jointInfo.axisInfos[(int)jointAxis].drive.damping = driveDamping;

            if (!m_deferApply) ValidateAndApply();
        }

        public float GetLinkJointAxisDriveMaxForce(int linkIndex, PxArticulationAxis jointAxis)
        {
            if (linkIndex < 1 || linkIndex >= m_linkInfos.Length)
            {
                Debug.LogError("Bad link index.");
                return 0;
            }

            return m_linkInfos[linkIndex].jointInfo.axisInfos[(int)jointAxis].drive.maxForce;
        }

        public void SetLinkJointAxisDriveMaxForce(int linkIndex, PxArticulationAxis jointAxis, float driveMaxForce)
        {
            if (linkIndex < 1 || linkIndex >= m_linkInfos.Length)
            {
                Debug.LogError("Bad link index.");
                return;
            }

            m_linkInfos[linkIndex].jointInfo.axisInfos[(int)jointAxis].drive.maxForce = driveMaxForce;

            if (!m_deferApply) ValidateAndApply();
        }

        public float GetLinkJointAxisDriveTarget(int linkIndex, PxArticulationAxis jointAxis)
        {
            if (linkIndex < 1 || linkIndex >= m_linkInfos.Length)
            {
                Debug.LogError("Bad link index.");
                return 0;
            }

            return m_linkInfos[linkIndex].jointInfo.axisInfos[(int)jointAxis].drive.target;
        }

        public void SetLinkJointAxisDriveTarget(int linkIndex, PxArticulationAxis jointAxis, float driveTarget)
        {
            if (linkIndex < 1 || linkIndex >= m_linkInfos.Length)
            {
                Debug.LogError("Bad link index.");
                return;
            }

            m_linkInfos[linkIndex].jointInfo.axisInfos[(int)jointAxis].drive.target = driveTarget;

            if (!m_deferApply) ValidateAndApply();
        }

        public float GetLinkJointAxisDriveVelocity(int linkIndex, PxArticulationAxis jointAxis)
        {
            if (linkIndex < 1 || linkIndex >= m_linkInfos.Length)
            {
                Debug.LogError("Bad link index.");
                return 0;
            }

            return m_linkInfos[linkIndex].jointInfo.axisInfos[(int)jointAxis].drive.velocity;
        }

        public void SetLinkJointAxisDriveVelocity(int linkIndex, PxArticulationAxis jointAxis, float driveVelocity)
        {
            if (linkIndex < 1 || linkIndex >= m_linkInfos.Length)
            {
                Debug.LogError("Bad link index.");
                return;
            }

            m_linkInfos[linkIndex].jointInfo.axisInfos[(int)jointAxis].drive.velocity = driveVelocity;

            if (!m_deferApply) ValidateAndApply();
        }

        public int GetLinkDofStart(int linkIndex)
        {
            if (linkIndex < 0 || linkIndex >= m_links.Length)
            {
                Debug.LogError("Bad link index.");
                return 0;
            }

            int dofStart = 0;
            for (int i = 0; i < linkIndex; ++i)
                dofStart += GetLinkDofCount(i);

            return dofStart;
        }

        public int GetLinkDofCount(int linkIndex)
        {
            if (linkIndex < 0 || linkIndex >= m_links.Length)
            {
                Debug.LogError("Bad link index.");
                return 0;
            }

            return (int)m_links[linkIndex].getInboundJointDof();
        }

        public void ClearLinks()
        {
            m_linkInfos = new[] { new LinkInfo() { transform = transform } };

            ValidateAndRecreate();
        }

        public void BeginPropertyChange()
        {
            m_deferApply = true;
        }

        public void EndPropertyChange()
        {
            ValidateAndApply();
            m_deferApply = false;
        }

        #endregion

        #region Protected

        protected override IPxDependency[] GetDependencies()
        {
            return base.GetDependencies().Append(Array.ConvertAll(m_linkInfos, x => x.collisionShape));
        }

        protected override void CreateActor()
        {
            base.CreateActor();
            CreateArticulatedActor();
        }

        protected override void DestroyActor()
        {
            DestroyArticulatedActor();
            base.DestroyActor();
        }

        protected override void ValidateActor()
        {
            base.ValidateActor();

            if (m_linkInfos == null || m_linkInfos[0].transform == null)
                m_linkInfos = new[] { new LinkInfo() { transform = transform } };
        }

        protected override void ResetActor()
        {
            m_linkInfos = new[] { new LinkInfo() { transform = transform } };
            PopulateWithChildren();
        }

        protected override void UpdateComponentInEditor()
        {
            base.UpdateComponentInEditor();
            BeforeSimulation();
        }

        protected override void AddActorToScene(PxScene scene)
        {
            if (scene.valid)
            {
                scene.apiScene.addArticulation(m_articulation);
                scene.onBeforeSimulation += BeforeSimulation;
                scene.onAfterSimulation += AfterSimulation;

                int dofCount = (int)m_articulation.getDofs();
                Array.Resize(ref m_jointPosition, dofCount);
                Array.Resize(ref m_jointVelocity, dofCount);

                m_cache = m_articulation.createCache();
                m_cache.writeJointPositions(m_jointPosition, 0, m_jointPosition.Length);
                m_cache.writeJointVelocities(m_jointVelocity, 0, m_jointVelocity.Length);
                PxArticulationRootLinkData rootLinkData = new PxArticulationRootLinkData();
                rootLinkData.transform = transform.ToPxTransform();
                rootLinkData.worldLinVel = m_velocity.linear.ToPxVec3();
                rootLinkData.worldAngVel = m_velocity.angular.ToPxVec3();
                m_cache.rootLinkData = rootLinkData;
                m_articulation.applyCache(m_cache, PxArticulationCache.Flags.POSITION | PxArticulationCache.Flags.VELOCITY | PxArticulationCache.Flags.ROOT);
            }

            //Debug.Log("'" + name + "' added to '" + scene.name + "'");
        }

        protected override void RemoveActorFromScene(PxScene scene)
        {
            if (scene.valid)
            {
                m_articulation.releaseCache(m_cache);
                m_cache = null;

                scene.onBeforeSimulation -= BeforeSimulation;
                scene.onAfterSimulation -= AfterSimulation;
                scene.apiScene.removeArticulation(m_articulation);
            }

            //Debug.Log("'" + name + "' removed from '" + scene.name + "'");
        }

        protected override void ApplyProperties()
        {
            base.ApplyProperties();

            if (valid)
            {
                m_articulation.setArticulationFlag(PxArticulationFlag.FIX_BASE, m_fixedRoot);
                m_articulation.setSolverIterationCounts((uint)m_positionIterations, (uint)m_velocityIterations);

                for (uint i = 1; i < m_links.Length; ++i)
                {
                    var linkInfo = m_linkInfos[i];
                    var link = m_links[i];// m_articulation.getLink(i);
                    if (link != null)
                    {
                        var joint = m_joints[i];// link.getInboundJointReducedCoordinate();
                        if (joint != null)
                        {
                            joint.setFrictionCoefficient(linkInfo.jointInfo.friction);
                            joint.setMaxJointVelocity(linkInfo.jointInfo.maxVelocity);

                            for (int j = 0; j < 6; ++j)
                            {
                                var axisInfo = linkInfo.jointInfo.axisInfos[j];
                                float scale = 1.0f;
                                if (j >= (int)PxArticulationAxis.TWIST && j <= (int)PxArticulationAxis.SWING2) scale = Mathf.Deg2Rad;
                                joint.setLimit((PxArticulationAxis)j, axisInfo.limits.lower * scale, axisInfo.limits.upper * scale);
                                joint.setDrive((PxArticulationAxis)j, axisInfo.drive.stiffness, axisInfo.drive.damping, axisInfo.drive.maxForce);
                                joint.setDriveTarget((PxArticulationAxis)j, axisInfo.drive.target * scale);
                                joint.setDriveVelocity((PxArticulationAxis)j, axisInfo.drive.velocity * scale); // @@@ ???
                            }
                        }
                    }
                }
            }
        }

        #endregion

        #region Private

        void CreateArticulatedActor()
        {
            m_articulation = PxPhysics.apiPhysics.createArticulationReducedCoordinate();

            m_articulation.setArticulationFlag(PxArticulationFlag.FIX_BASE, m_fixedRoot);
            m_articulation.setSolverIterationCounts((uint)m_positionIterations, (uint)m_velocityIterations);

            int linkCount = m_linkInfos.Length;
            m_links = new PxArticulationLink[linkCount];

            m_joints = new PxArticulationJointReducedCoordinate[linkCount];

            // Root link
            {
                var linkInfo = m_linkInfos[0];
                linkInfo.transform = transform; // @@@
                var link = m_articulation.createLink(null, transform.ToPxTransform());
                if (link != null)
                {
                    if (linkInfo.collisionShape != null && linkInfo.collisionShape.valid)
                    {
                        var shapes = linkInfo.collisionShape.apiShapes;
                        foreach (var s in shapes) link.attachShape(s);
                    }

                    if (linkInfo.massInfo.autoMass)
                    {
                        if (linkInfo.collisionShape != null && linkInfo.collisionShape.valid)
                        {
                            var densities = linkInfo.collisionShape.densities;
                            link.updateMassAndInertia(densities, (uint)densities.Length);
                        }
                        linkInfo.massInfo.mass = link.getMass();
                        var massPose = link.getCMassLocalPose();
                        linkInfo.massInfo.position = massPose.p.ToVector3();
                        linkInfo.massInfo.rotation = massPose.q.ToQuaternion().eulerAngles;
                        linkInfo.massInfo.inertia = link.getMassSpaceInertiaTensor().ToVector3();
                    }
                    else
                    {
                        link.setMass(linkInfo.massInfo.mass);
                        link.setCMassLocalPose(new PxTransform(linkInfo.massInfo.position.ToPxVec3(), Quaternion.Euler(linkInfo.massInfo.rotation).ToPxQuat()));
                        link.setMassSpaceInertiaTensor(linkInfo.massInfo.inertia.ToPxVec3());
                    }

                    link.userData = this;

                    m_links[0] = link;
                }
            }

            // Child links
            for (int i = 1; i < linkCount; ++i)
            {
                var linkInfo = m_linkInfos[i];
                var parentLink = m_links[linkInfo.parentLink];
                //var parentTransformIndex = linkInfo.parentLink;
                //while (m_linkInfos[parentTransformIndex].transform == null) parentTransformIndex = m_linkInfos[parentTransformIndex].parentLink;
                //var linkTransform = linkInfo.transform == null ? m_linkInfos[parentTransformIndex].transform : linkInfo.transform;
                //var localPose = m_linkInfos[parentTransformIndex].transform.ToPxTransform().transformInv(linkTransform.ToPxTransform());
                var link = m_articulation.createLink(parentLink, PxTransform.identity/*localPose*/);
                if (link != null)
                {
                    if (linkInfo.collisionShape != null && linkInfo.collisionShape.valid)
                    {
                        var shapes = linkInfo.collisionShape.apiShapes;
                        foreach (var s in shapes) link.attachShape(s);
                    }

                    if (linkInfo.massInfo.autoMass)
                    {
                        if (linkInfo.collisionShape != null && linkInfo.collisionShape.valid)
                        {
                            var densities = linkInfo.collisionShape.densities;
                            link.updateMassAndInertia(densities, (uint)densities.Length);
                        }
                        linkInfo.massInfo.mass = link.getMass();
                        var massPose = link.getCMassLocalPose();
                        linkInfo.massInfo.position = massPose.p.ToVector3();
                        linkInfo.massInfo.rotation = massPose.q.ToQuaternion().eulerAngles;
                        linkInfo.massInfo.inertia = link.getMassSpaceInertiaTensor().ToVector3();
                    }
                    else
                    {
                        link.setMass(linkInfo.massInfo.mass);
                        link.setCMassLocalPose(new PxTransform(linkInfo.massInfo.position.ToPxVec3(), Quaternion.Euler(linkInfo.massInfo.rotation).ToPxQuat()));
                        link.setMassSpaceInertiaTensor(linkInfo.massInfo.inertia.ToPxVec3());
                    }

                    var joint = link.getInboundJointReducedCoordinate();
                    if (joint != null)
                    {
                        joint.setJointType(linkInfo.jointInfo.type);
                        joint.setFrictionCoefficient(linkInfo.jointInfo.friction);
                        joint.setMaxJointVelocity(linkInfo.jointInfo.maxVelocity);

                        for (int j = 0; j < 6; ++j)
                        {
                            var axisInfo = linkInfo.jointInfo.axisInfos[j];
                            float scale = 1.0f;
                            if (j >= (int)PxArticulationAxis.TWIST && j <= (int)PxArticulationAxis.SWING2) scale = Mathf.Deg2Rad;
                            joint.setMotion((PxArticulationAxis)j, axisInfo.motion);
                            joint.setLimit((PxArticulationAxis)j, axisInfo.limits.lower * scale, axisInfo.limits.upper * scale);
                            joint.setDrive((PxArticulationAxis)j, axisInfo.drive.stiffness, axisInfo.drive.damping, axisInfo.drive.maxForce);
                            joint.setDriveTarget((PxArticulationAxis)j, axisInfo.drive.target * scale);
                            joint.setDriveVelocity((PxArticulationAxis)j, axisInfo.drive.velocity * scale); // @@@ ???
                        }

                        joint.setChildPose(new PxTransform(linkInfo.jointInfo.position.ToPxVec3(), Quaternion.Euler(linkInfo.jointInfo.rotation).ToPxQuat()));
                        joint.setParentPose(new PxTransform(linkInfo.jointInfo.parentPosition.ToPxVec3(), Quaternion.Euler(linkInfo.jointInfo.parentRotation).ToPxQuat()));

                        m_joints[i] = joint;
                    }

                    link.userData = this;

                    m_links[i] = link;
                }
            }

            m_articulation.userData = this;

            //Debug.Log("PxArticulatedActor '" + name + "' created");
        }

        void DestroyArticulatedActor()
        {
            m_links = null;
            m_joints = null;

            m_articulation?.release();
            m_articulation = null;

            //Debug.Log("PxArticulatedActor '" + name + "' destroyed");
        }

        void BeforeSimulation()
        {
            if (transform.hasChanged)
            {
                m_articulation.teleportRootLink(transform.ToPxTransform(), true);
                transform.hasChanged = false;
                //StartCoroutine(ResetTransformChanged(transform));

                if (m_cache != null)
                {
                    var rootLinkData = m_cache.rootLinkData;
                    rootLinkData.transform = transform.ToPxTransform();
                    m_cache.rootLinkData = rootLinkData;
                    m_articulation.applyCache(m_cache, PxArticulationCache.Flags.ROOT);
                }
            }
        }

        void AfterSimulation()
        {
            if (!m_articulation.isSleeping())
            {
                if (m_cache != null)
                {
                    m_articulation.copyInternalStateToCache(m_cache, PxArticulationCache.Flags.POSITION | PxArticulationCache.Flags.VELOCITY | PxArticulationCache.Flags.ROOT);
                    m_cache.readJointPositions(m_jointPosition, 0, m_jointPosition.Length);
                    m_cache.readJointVelocities(m_jointVelocity, 0, m_jointVelocity.Length);
                    var rootLinkData = m_cache.rootLinkData;
                    m_velocity.linear = rootLinkData.worldLinVel.ToVector3();
                    m_velocity.angular = rootLinkData.worldAngVel.ToVector3();
                }

                for (int i = 0; i < m_linkInfos.Length; ++i)
                {
                    var linkInfo = m_linkInfos[i];
                    if (linkInfo.transform != null)
                    {
                        var link = m_links[i];
                        if (link != null)
                        {
                            link.getGlobalPose().ToTransform(linkInfo.transform);
                            linkInfo.transform.hasChanged = false;
                            //StartCoroutine(ResetTransformChanged(linkInfo.transform));
                        }
                    }
                }
            }
        }

        //IEnumerator ResetTransformChanged(Transform xform)
        //{
        //    yield return new WaitForEndOfFrame();
        //    xform.hasChanged = false;
        //}

        string[] GetLinkNames()
        {
            var names = new string[m_linkInfos.Length];
            for (int i = 0; i < names.Length; ++i)
                names[i] = m_linkInfos[i].transform ? m_linkInfos[i].transform.name : "Link " + i;

            return names;
        }

        [Serializable]
        class LinkInfo
        {
            public int parentLink = -1;
            public Transform transform = null;
            public PxShape collisionShape = null;
            public MassInfo massInfo = new MassInfo();
            public JointInfo jointInfo = null;
            public int[] childLinks = new int[0];
        }

        [Serializable]
        class MassInfo
        {
            public bool autoMass = true;
            public float mass = 1.0f;
            public Vector3 position = Vector3.zero;
            public Vector3 rotation = Vector3.zero;
            public Vector3 inertia = Vector3.one;
        }

        [Serializable]
        class JointInfo
        {
            public Vector3 position = Vector3.zero;
            public Vector3 rotation = Vector3.zero;
            public Vector3 parentPosition = Vector3.zero;
            public Vector3 parentRotation = Vector3.zero;
            public PxArticulationJointType type = PxArticulationJointType.FIX;
            public float friction = 0;
            public float maxVelocity = float.MaxValue;
            public AxisInfo[] axisInfos = new[] { new AxisInfo(), new AxisInfo(), new AxisInfo(), new AxisInfo(), new AxisInfo(), new AxisInfo() };
        }

        [Serializable]
        class AxisInfo
        {
            public PxArticulationMotion motion = PxArticulationMotion.LOCKED;
            public AxisLimits limits = new AxisLimits();
            public AxisDrive drive = new AxisDrive();
            public int dofIndex = -1;
        }

        [Serializable]
        class AxisLimits
        {
            public float lower = 0;
            public float upper = 0;
        }

        [Serializable]
        class AxisDrive
        {
            public float stiffness = 0;
            public float damping = 0;
            public float maxForce = 0;
            public float target = 0;
            public float velocity = 0;
        }

        [Serializable]
        class VelocityInfo
        {
            public Vector3 linear = Vector3.zero;
            public Vector3 angular = Vector3.zero;
        }

        [NonSerialized]
        PxArticulationReducedCoordinate m_articulation;
        [NonSerialized]
        PxArticulationLink[] m_links = new PxArticulationLink[0];
        [NonSerialized]
        PxArticulationCache m_cache = null;
        [NonSerialized]
        PxArticulationJointReducedCoordinate[] m_joints = null;
        [NonSerialized]
        bool m_deferApply = false;

        [SerializeField]
        LinkInfo[] m_linkInfos = new[] { new LinkInfo() };
        [SerializeField]
        bool m_fixedRoot = false;
        [SerializeField]
        int m_positionIterations = 4;
        [SerializeField]
        int m_velocityIterations = 1;
        [SerializeField]
        float[] m_jointPosition = new float[0];
        [SerializeField]
        float[] m_jointVelocity = new float[0];
        [SerializeField]
        VelocityInfo m_velocity = new VelocityInfo();

        #endregion
    }
}