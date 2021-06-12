using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineMaker : MonoBehaviour
{
    public Vector2 newPosition;
    private LineRenderer line;

    void Awake()
    {
        line = GetComponent<LineRenderer>();
    }

    void Update()
    {
        line.material.SetTextureOffset("_MainTex", new Vector2(Time.timeSinceLevelLoad * 4f, 0f));
        line.material.SetTextureScale("_MainTex", new Vector2(newPosition.magnitude, 1f));
       // line.SetPosition(0, newPosition);
    }
}