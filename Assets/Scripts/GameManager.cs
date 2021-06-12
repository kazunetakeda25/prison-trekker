using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public Text LevelText;
    [Header("UI")]
    public GameObject GameOverUI;
    public GameObject GameWinUI;
    public GameObject Tutorial;
    public GameObject RedAlert;
    [Header("Level")]
    public GameObject[] NormalLevels;

    public GameObject[] HardLevels;

    public GameObject[] Levels;
    // Start is called before the first frame update
    int Level;
    int TotalPrisoner,PCount;

    bool isGameEnd;

    public void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

    }

    void Start()
    {
        string mode = PlayerPrefs.GetString("MODE");

        if(mode == "EASY")
        {
            Levels = NormalLevels;
        }
        else
        {
            Levels = HardLevels;
        }
        RedAlert.SetActive(false);
        TotalPrisoner = 0;
        isGameEnd = false;
        GameOverUI.SetActive(false);
        GameWinUI.SetActive(false);
        Level = PlayerPrefs.GetInt("Level", 0);
        //Level = 9;
        if (Level > Levels.Length - 1) {
            Level = Levels.Length - 1;
        }
        
        LevelText.text = "Level " + (Level+1);
        Instantiate(Levels[Level]);
        if (Level == 0)
        {
            Tutorial.SetActive(true);
            StartCoroutine(EndTutorial());
        }

    }

    // Update is called once per frame
    void Update()
    {
        if (PathCreator.instance.Ready)
        {
            if (PCount == TotalPrisoner)
            {
                LevelCompleted();
            }
        }
        
    }

    public void LoadMenu() {

        SceneManager.LoadScene("Menu");
    }

    public void RetryLevel() {

        SceneManager.LoadScene("LoadGame");
    }

    public void GameOver() {
        isGameEnd = true;
        RedAlert.SetActive(true);
        StartCoroutine(GameOverSceen());

        // GameOverUI.SetActive(true);
    }

    public void LevelCompleted() {
        if (!isGameEnd)
        {
            //Adcontrol.instance.ShowInterstitial();
            //AdsManager.Instance.ShowInterstitialAd_FinishedGame();
            Level++;
            PlayerPrefs.SetInt("Level", Level);
            //GameWinUI.SetActive(true);
            SceneManager.LoadScene("GameWin");
            isGameEnd = true;

        }
    }

    IEnumerator EndTutorial() {
        yield return new WaitForSeconds(4.0f);
        Tutorial.SetActive(false);
    }

    public void UpdateScore() {
        PCount++;

    }

    public void TotalPrisonerCount(int count) {
        TotalPrisoner = count;
        Debug.Log(TotalPrisoner + "........................"+ TotalPrisoner);
    }


    IEnumerator GameOverSceen()
    {
        yield return new WaitForSeconds(2.0f);
        SceneManager.LoadScene("GameOver");
    }


}
