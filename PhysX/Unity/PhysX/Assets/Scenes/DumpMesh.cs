using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using UnityEngine;

public class DumpMesh : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        var meshFilter = GetComponent<MeshFilter>();
        if (meshFilter)
        {
            var mesh = meshFilter.sharedMesh;
            var str = "Vector3[] positions = { ";
            foreach (var p in mesh.vertices)
            {
                var v = transform.TransformPoint(p);
                str += string.Format(new CultureInfo("en-US"), "new Vector3({0}f * s, {1}f * s, {2}f * s), ", v.x, v.y, v.z);
            }
            str += "};\r\n";
            str += "Vector3[] normals = { ";
            foreach (var p in mesh.normals)
            {
                var v = transform.TransformDirection(p);
                str += string.Format(new CultureInfo("en-US"), "new Vector3({0}f, {1}f, {2}f), ", v.x, v.y, v.z);
            }
            str += "};\r\n";
            str += "Vector2[] uvs = { ";
            foreach (var v in mesh.uv)
            {
                str += string.Format(new CultureInfo("en-US"), "new Vector2({0}f, {1}f), ", v.x, v.y);
            }
            str += "};\r\n";
            str += "int[] indices = { ";
            foreach (var i in mesh.triangles)
            {
                str += string.Format(new CultureInfo("en-US"), "{0}, ", i);
            }
            str += "};\r\n";

            //Debug.Log(str);
            using (var stream = File.CreateText(".\\mesh_data.txt"))
            {
                stream.Write(str);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
