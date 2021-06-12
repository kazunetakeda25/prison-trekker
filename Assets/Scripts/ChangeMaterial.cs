using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeMaterial : MonoBehaviour
{

    public Material[] materials;
    // Start is called before the first frame update
    void Start()
    {
        int RandNum = Random.Range(0, materials.Length);
        gameObject.GetComponent<MeshRenderer>().material = materials[RandNum];

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
