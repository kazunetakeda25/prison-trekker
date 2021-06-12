using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Config
{
    public const string USERNAME = "username";

    public static void SetUserName(string _username) {
        PlayerPrefs.SetString(USERNAME, _username);
        PlayerPrefs.Save();
    }

    public static string GetUserName() {
        if (PlayerPrefs.GetString(USERNAME, "").Equals("")) {
            string defaultName = "pirson" + Random.Range(100, 1000);
            SetUserName(defaultName);
            return defaultName;
        }
        return PlayerPrefs.GetString(USERNAME, "");
    }


    public static int lastUpdateRannk_Level = 1;
    public static string userIdentify = "";



    #region FIREBASE_DATABASE_NAME
    public const string ID = "i";
    public const string NAME = "n";
    public const string LEVEL = "l";



    public const float MAX_TIME_LOADING = 15f;
    #endregion
}
