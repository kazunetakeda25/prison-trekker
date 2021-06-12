using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class parked : MonoBehaviour
{
    public float necessaryTime = 1f;
    float elapsed;
    bool isParked;
    GameObject pnt;
    public GameObject blast;
    AudioSource audioManager,audioManager2;
    // Start is called before the first frame update
    void Start()
    {
        isParked = false;
       pnt = gameObject.transform.parent.gameObject;
        audioManager = GameObject.Find("AudioManager").GetComponent<AudioSource>();
        audioManager2 = GameObject.Find("AudioManager2").GetComponent<AudioSource>();

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Obstacle"))
        {
            GameManager.instance.GameOver();
            Instantiate(blast, transform.position,Quaternion.identity);
            audioManager.Play();
        }
    }
    public void OnTriggerStay(Collider other)
    {
        if (!isParked)
        {
            if (other.gameObject.CompareTag("park"))
            {

                elapsed += Time.fixedDeltaTime;
                if (elapsed > necessaryTime)
                {
                    audioManager2.Play();
                    GameManager.instance.UpdateScore();
                    isParked = true;
                }
            }

        }
    }

}
