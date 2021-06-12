using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Facebook.Unity;

public class LevelControl : MonoBehaviour
{
    public Text txtLevel;
    public Text txtNextLevel;
    public int level;
    private void Awake()
    {

    }
    // Start is called before the first frame update
    void Start()
    {
        if (txtLevel!=null)
            txtLevel.text = "" + (level);
        if (txtNextLevel != null)
        {
            level = PlayerPrefs.GetInt("Level");
            txtNextLevel.text = "" + (level + 1);
            if (AppLovin.HasPreloadedInterstitial())
            {
                // An ad is currently available, so show the interstitial.
                AppLovin.ShowInterstitial();
            }
            else
            {
                // No ad is available.  Perform failover logic...
            }
            LogLevel_achievedEvent(level + 1, 0);
        }
            
    }

 
    public void LogLevel_achievedEvent(int level, double valToSum)
    {
        var parameters = new Dictionary<string, object>();
        parameters["level"] = level;
        FB.LogAppEvent(
            "level_achieved",
            (float)valToSum,
            parameters
        );
    }


    public void TouchStartGame() {
        PlayerPrefs.SetInt("Level", level-1);
        PlayerPrefs.Save();
        SceneManager.LoadScene("LoadGame");
    }

    public void NextGame()
    {
        level = PlayerPrefs.GetInt("Level");
        PlayerPrefs.SetInt("Level", level);
        if(level < 20)
        {
            SceneManager.LoadScene("LoadGame");
        }
        else if (PlayerPrefs.GetString("MODE") == "EASY")
        {
            PlayerPrefs.SetString("MODE", "HARD");
        }
        else
        {
            SceneManager.LoadScene("Menu");
            PlayerPrefs.SetInt("Level", 1);
            PlayerPrefs.SetString("MODE", "EASY");
        }
        
    }
}
