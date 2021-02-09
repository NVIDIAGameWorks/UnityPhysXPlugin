using NVIDIA.PhysX.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PX = NVIDIA.PhysX;

public class TestCollision : MonoBehaviour
{
    private void OnEnable()
    {
        var actor = GetComponent<PxActor>();
        if (actor)
        {
            actor.onContact += OnContact;
        }
    }

    void OnContact(PxActor thisActor, PxActor otherActor, PX.PxContactPairHeader pairHeader, PX.PxContactPairList pairs)
    {
        for (uint i = 0; i < pairs.count; ++i)
        {
            var pair = pairs.get(i);
            if ((pair.flags & PX.PxContactPairFlag.ACTOR_PAIR_HAS_FIRST_TOUCH) != 0)
            {
                if (otherActor)
                    Debug.Log(otherActor.name + " touched");
            }
            else if ((pair.flags & PX.PxContactPairFlag.ACTOR_PAIR_LOST_TOUCH) != 0)
            {
                if (otherActor)
                    Debug.Log(otherActor.name + " detouched");
            }
        }
    }
}
