using Facebook.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class LoadScene : MonoBehaviour
{
    private void Awake()
    {
#if UNITY_EDITOR
        Debug.unityLogger.logEnabled = true;
#else
        Debug.unityLogger.logEnabled = false;
#endif
        Application.targetFrameRate = 60;

        if (FB.IsInitialized)
        {
            FB.ActivateApp();
        }
        else
        {
            //Handle FB.Init
            FB.Init(() => {
                FB.ActivateApp();
            });
        }
        //PlayerPrefs.DeleteAll();
#if !UNITY_EDITOR
        AppLovin.InitializeSdk();
#endif
    }

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(loadMenu());
        //SendTest1();
    }

    // Update is called once per frame
    void Update()
    {

    }

    IEnumerator loadMenu()
    {
        yield return new WaitForSeconds(0.1f);
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("Menu");
        asyncLoad.allowSceneActivation = false;
        // Wait until the asynchronous scene fully loads
        while (asyncLoad.progress < 0.9f)
        {
            Debug.Log(asyncLoad.progress);
            yield return null;
        }
        asyncLoad.allowSceneActivation = true;
    }
    void OnApplicationPause(bool pauseStatus)
    {
        // Check the pauseStatus to see if we are in the foreground
        // or background
        if (!pauseStatus)
        {
            //app resume
            if (FB.IsInitialized)
            {
                FB.ActivateApp();
            }
            else
            {
                //Handle FB.Init
                FB.Init(() => {
                    FB.ActivateApp();
                });
            }
        }
    }

    public void SendTest1(){
        Debug.Log("SendTest1");
        StartCoroutine(RequestTest1());
    }

    public IEnumerator RequestTest1() {
        Debug.Log("RequestTest1");
        yield return new WaitForSeconds(0.2f);
        SendTest2();
    }

    public void SendTest2()
    {
        Debug.Log("SendTest2");
        StartCoroutine(RequestTest2());
    }

    public IEnumerator RequestTest2()
    {
        Debug.Log("RequestTest2");
        yield return new WaitForSeconds(0.2f);
    }
}
