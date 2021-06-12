using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class LoadGameScene : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(loadGame());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator loadGame()
    {
        yield return new WaitForSeconds(0.1f);
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("Game");
        asyncLoad.allowSceneActivation = false;
        // Wait until the asynchronous scene fully loads
        while (asyncLoad.progress < 0.9f)
        {
            Debug.Log(asyncLoad.progress);
            yield return null;
        }
        asyncLoad.allowSceneActivation = true;
    }
}
