using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrisonerControl : MonoBehaviour
{

    GameObject[] prisoner;
    int TotalPrisoner,count;
    bool ReadytoEscape;
    // Start is called before the first frame update
    void Start()
    {
        count = 0;
        TotalPrisoner = transform.childCount;
        Debug.Log(transform.GetChild(count).gameObject.name);
        GameManager.instance.TotalPrisonerCount(TotalPrisoner);
    }

    // Update is called once per frame
    void Update()
    {
        ReadytoEscape = PathCreator.instance.Ready;
        if (ReadytoEscape)
        {
            transform.GetChild(0).GetChild(0).gameObject.SetActive(true);
        }
        if (ReadytoEscape)
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (count < TotalPrisoner) {
                    PathCreator.instance.Escape(transform.GetChild(count).gameObject);
                    Debug.Log(transform.GetChild(count).gameObject.name);
                    count++;
                }
            }
        }
    }
}
