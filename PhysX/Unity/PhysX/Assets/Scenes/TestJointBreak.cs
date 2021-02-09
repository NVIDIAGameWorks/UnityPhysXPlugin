using NVIDIA.PhysX.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestJointBreak : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        var joint = GetComponent<PxJoint>();
        if (joint) joint.onBreak = OnBreak;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnBreak(PxJoint joint)
    {
        Debug.Log("Joint " + joint.name + " broken");
    }
}
