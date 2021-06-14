using UnityEngine;
using NVIDIA.PhysX.Unity;
using PX = NVIDIA.PhysX;
using System.Collections.Generic;

[RequireComponent(typeof(PxActor))]
public class HandleTriggerReport : MonoBehaviour
{
    public Material inactiveMaterial;
    public Material activeMaterial;

    void Start()
    {
        m_actor = GetComponent<PxActor>();
        m_actor.onContact += OnContact;

        m_scene = m_actor.currentScene;
        m_scene.onAfterSimulation += OnAfterSimulation;

        m_renderer = GetComponent<Renderer>();
        m_renderer.material = inactiveMaterial;
    }

    void OnDisable()
    {
        m_actor.onContact -= OnContact;
        m_scene.onAfterSimulation -= OnAfterSimulation;
    }

    void OnContact(PxActor thisActor, PxActor otherActor, PX.PxContactPairHeader pairHeader, PX.PxContactPairList pairs)
    {
        for (uint i = 0; i < pairs.count; ++i)
        {
            var pair = pairs.get(i);

            if ((pair.flags & PX.PxContactPairFlag.ACTOR_PAIR_HAS_FIRST_TOUCH) != 0) ++m_counter;
            if ((pair.flags & PX.PxContactPairFlag.ACTOR_PAIR_LOST_TOUCH) != 0) --m_counter;
        }

        if (m_counter > 0) m_renderer.material = activeMaterial;
        else m_renderer.material = inactiveMaterial;

        var dynamicActor = otherActor as PxDynamicActor;
        if (dynamicActor) m_pushActors.Add(dynamicActor);
    }

    void OnAfterSimulation()
    {
        foreach (var actor in m_pushActors)
            actor.linearVelocity = Vector3.up * 10;

        m_pushActors.Clear();
    }

    int m_counter = 0;
    PxActor m_actor;
    PxScene m_scene;
    Renderer m_renderer;
    List<PxDynamicActor> m_pushActors = new List<PxDynamicActor>();
}
