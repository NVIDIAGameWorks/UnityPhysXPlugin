using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NVIDIA.PhysX.Unity;

[RequireComponent(typeof(PxArticulatedActor))]
public class AntController : MonoBehaviour
{
    void Start()
    {
        m_articulation = GetComponent<PxArticulatedActor>();
        if (m_articulation == null || m_articulation.linkCount != 19)
        {
            Debug.LogError("An articulation with 19 links expected");
            m_articulation = null;
            return;
        }

        m_scene = PxScene.main;
        m_scene.onBeforeSimulation += OnBeforeSimulation;

    }

    void OnDisable()
    {
        m_scene.onBeforeSimulation -= OnBeforeSimulation;
    }

    void OnBeforeSimulation()
    {
        Behaviour1();
    }

    void Behaviour1()
    {
        if (m_articulation)
        {
            m_articulation.BeginPropertyChange();

            int index = 0;

            m_articulation.SetLinkJointAxisDriveTarget(++index, NVIDIA.PhysX.PxArticulationAxis.SWING1, GetTarget(0, 0.3f, 0));
            m_articulation.SetLinkJointAxisDriveTarget(++index, NVIDIA.PhysX.PxArticulationAxis.SWING2, GetTarget(60, 0.3f, 0));
            m_articulation.SetLinkJointAxisDriveTarget(++index, NVIDIA.PhysX.PxArticulationAxis.SWING2, GetTarget(60, 0.3f, 0));

            m_articulation.SetLinkJointAxisDriveTarget(++index, NVIDIA.PhysX.PxArticulationAxis.SWING1, GetTarget(0, 0.3f, 0));
            m_articulation.SetLinkJointAxisDriveTarget(++index, NVIDIA.PhysX.PxArticulationAxis.SWING2, GetTarget(60, 0.3f, 0));
            m_articulation.SetLinkJointAxisDriveTarget(++index, NVIDIA.PhysX.PxArticulationAxis.SWING2, GetTarget(60, 0.3f, 0));

            m_articulation.SetLinkJointAxisDriveTarget(++index, NVIDIA.PhysX.PxArticulationAxis.SWING1, GetTarget(0, 0.3f, 0));
            m_articulation.SetLinkJointAxisDriveTarget(++index, NVIDIA.PhysX.PxArticulationAxis.SWING2, GetTarget(60, 0.3f, 0));
            m_articulation.SetLinkJointAxisDriveTarget(++index, NVIDIA.PhysX.PxArticulationAxis.SWING2, GetTarget(60, 0.3f, 0));

            m_articulation.SetLinkJointAxisDriveTarget(++index, NVIDIA.PhysX.PxArticulationAxis.SWING1, GetTarget(0, 0.3f, 0));
            m_articulation.SetLinkJointAxisDriveTarget(++index, NVIDIA.PhysX.PxArticulationAxis.SWING2, GetTarget(60, 0.3f, 0));
            m_articulation.SetLinkJointAxisDriveTarget(++index, NVIDIA.PhysX.PxArticulationAxis.SWING2, GetTarget(60, 0.3f, 0));

            m_articulation.SetLinkJointAxisDriveTarget(++index, NVIDIA.PhysX.PxArticulationAxis.SWING1, GetTarget(0, 0.3f, 0));
            m_articulation.SetLinkJointAxisDriveTarget(++index, NVIDIA.PhysX.PxArticulationAxis.SWING2, GetTarget(60, 0.3f, 0));
            m_articulation.SetLinkJointAxisDriveTarget(++index, NVIDIA.PhysX.PxArticulationAxis.SWING2, GetTarget(60, 0.3f, 0));

            m_articulation.SetLinkJointAxisDriveTarget(++index, NVIDIA.PhysX.PxArticulationAxis.SWING1, GetTarget(0, 0.3f, 0));
            m_articulation.SetLinkJointAxisDriveTarget(++index, NVIDIA.PhysX.PxArticulationAxis.SWING2, GetTarget(60, 0.3f, 0));
            m_articulation.SetLinkJointAxisDriveTarget(++index, NVIDIA.PhysX.PxArticulationAxis.SWING2, GetTarget(60, 0.3f, 0));

            m_articulation.EndPropertyChange();
        }
    }

    float GetTarget(float a, float f, float o, float s = 0)
    {
        return a * Mathf.Sin(2 * Mathf.PI * f * (Time.fixedTime + o)) + s;
    }

    PxArticulatedActor m_articulation;
    PxScene m_scene;
}
