using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOver : MonoBehaviour
{
    public void Start()
    {
       // AdsManager.Instance.ShowInterstitialAd_FinishedGame();
    }

    public void StartGame()
    {
        //SceneManager.LoadScene("Game");
        SceneManager.LoadScene("LoadGame");
    }

    public void GoToMenu()
    {
        SceneManager.LoadScene("Menu");
    }
}
