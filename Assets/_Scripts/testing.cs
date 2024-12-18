using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class testing : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        List<Vector2> uvs = new List<Vector2>();
        var mesh = GetComponent<MeshFilter>().mesh;
        mesh.GetUVs(0, uvs);
        Debug.Log($"{uvs}");
        for (int i = 0; i < uvs.Count; i++)
        {
            Debug.Log($"UV {i}: {uvs[i]}");
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
