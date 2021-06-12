using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtOther : MonoBehaviour
{

    Transform target;
    // Start is called before the first frame update
    void Start()
    {
        target = GameObject.FindGameObjectWithTag("Stay").transform;
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.LookAt(target);

    }
}
