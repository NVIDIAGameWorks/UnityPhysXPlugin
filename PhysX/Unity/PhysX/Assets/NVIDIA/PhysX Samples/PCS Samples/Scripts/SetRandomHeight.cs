using UnityEngine;

public class SetRandomHeight : MonoBehaviour
{
    public float min = -2;
    public float max = 2;

    void Start()
    {
        var pos = transform.position;
        transform.position = new Vector3(pos.x, Random.Range(min, max), pos.z);
    }
}
