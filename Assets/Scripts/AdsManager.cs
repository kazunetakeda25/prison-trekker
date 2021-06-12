using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.Advertisements;
//using GoogleMobileAds.Api;
using System;
public class AdsManager : MonoBehaviour
{
    private static AdsManager instance;
    public static AdsManager Instance
    {
        get
        {
            return instance;
        }
    }
    void Awake()
    {
        if (instance != null) Destroy(instance.gameObject);
        instance = this;
        DontDestroyOnLoad(this.gameObject);
    }

    [HideInInspector]
    public string UNITYADS_GAMEID = "";
    [HideInInspector]
    public string ADMOB_GAMEID = "";

    //private InterstitialAd interstitial;
    //private RewardBasedVideoAd rewardBasedVideo;


    // Use this for initialization
    void Start()
    {
        AppLovin.PreloadInterstitial();
    }

    // Update is called once per frame
    void Update()
    {

    }
    #region REWARD
    //    public void LoadRewardedVideoAd()
    //    {
    //        LoadRewardedVideoAd_Admob();
    //        LoadRewardedVideoAd_UnityAds();
    //    }

    //    public bool IsRewardedVideoLoaded()
    //    {
    //        //if ((Advertisement.isSupported && Advertisement.IsReady("rewardedVideo")) || rewardBasedVideo.IsLoaded())
    //        //{
    //        //    return true;
    //        //}

    //        return false;
    //    }

    //    private int indexRewardedAds = 0;
    //    public void ShowRewardedAds()
    //    {
    ////#if UNITY_EDITOR
    ////        if (Advertisement.isSupported && Advertisement.IsReady("rewardedVideo"))
    ////        {
    ////            ShowRewardedVideoAds_UnityAds();
    ////        }
    ////#else
    //        indexRewardedAds++;
    //        if (indexRewardedAds % 2 == 1)
    //        {
    //            //if (Advertisement.isSupported && Advertisement.IsReady("rewardedVideo"))
    //            //{
    //            //    ShowRewardedVideoAds_UnityAds();
    //            //}
    //            //else
    //            //{
    //            //    ShowRewardedVideoAds_Admob();
    //            //}
    //        }
    //        else
    //        {
    //            if (rewardBasedVideo.IsLoaded())
    //            {
    //                ShowRewardedVideoAds_Admob();
    //            }
    //            else
    //            {
    //                ShowRewardedVideoAds_UnityAds();
    //            }
    //        }
    //    }

    //    public void LoadRewardedVideoAd_Admob()
    //    {
    //#if UNITY_EDITOR
    //        string adUnitId = "";
    //#elif UNITY_ANDROID
    //        string adUnitId = "ca-app-pub-9740541809422718/6056071761";
    //#elif UNITY_IPHONE
    //		string adUnitId = "";
    //#else
    //		string adUnitId = "unexpected_platform";
    //#endif

    //        rewardBasedVideo.LoadAd(Admob_CreateAdRequest(), adUnitId);
    //    }

    //    public void ShowRewardedVideoAds_Admob()
    //    {
    //        if (rewardBasedVideo.IsLoaded())
    //        {
    //            rewardBasedVideo.Show();
    //        }
    //    }

    //    public void LoadRewardedVideoAd_UnityAds()
    //    {

    //    }


    //    public void ShowRewardedVideoAds_UnityAds()
    //    {
    //        //if (Advertisement.isSupported && Advertisement.IsReady("rewardedVideo"))
    //        //{
    //        //    ShowOptions options = new ShowOptions();
    //        //    options.resultCallback = RewardedVideoAds_UnityAds_HandleShowResult;

    //        //    Advertisement.Show("rewardedVideo", options);
    //        //}
    //    }

    //    //void RewardedVideoAds_UnityAds_HandleShowResult(ShowResult result)
    //    //{
    //    //    if (result == ShowResult.Finished)
    //    //    {
    //    //        Debug.Log("Video completed - Offer a reward to the player");
    //    //        // Reward your player here.
    //    //        StartCoroutine(HandRewardAds());
    //    //    }
    //    //    else if (result == ShowResult.Skipped)
    //    //    {
    //    //        Debug.LogWarning("Video was skipped - Do NOT reward the player");

    //    //    }
    //    //    else if (result == ShowResult.Failed)
    //    //    {
    //    //        Debug.LogError("Video failed to show");
    //    //    }
    //    //}


    //    public void Admob_HandleOnRewarded(object sender, Reward args)
    //    {
    //        //Save me
    //        string type = args.Type;
    //        double amount = args.Amount;
    //        StartCoroutine(HandRewardAds());
    //    }

    //    public IEnumerator HandRewardAds()
    //    {
    //        yield return null;

    //    }

    //    private void Admob_HandleOnRewardedVideoAdClosed(object sender, EventArgs args)
    //    {
    //        LoadRewardedVideoAd_Admob();

    //    }
    #endregion

    #region INTERSTITIALAD
    // Returns an ad request with custom ad targeting.
    //private AdRequest Admob_CreateAdRequest()
    //{
    //    return new AdRequest.Builder()
    //        .Build();
    //}


    //public void LoadInterstitialAd()
    //{
    //    LoadInterstitialAd_Admob();
    //    LoadInterstitialAd_UnityAds();
    //}


    //    public void LoadInterstitialAd_Admob()
    //    {
    //        // These ad units are configured to always serve test ads.
    //#if UNITY_EDITOR
    //        string adUnitId = "unused";
    //#elif UNITY_ANDROID
    //			string adUnitId = "ca-app-pub-9740541809422718/6056071761";
    //#elif UNITY_IPHONE
    //			string adUnitId = ";
    //#else
    //			string adUnitId = "unexpected_platform";
    //#endif

    //        // Clean up interstitial ad before creating a new one.
    //        if (this.interstitial != null)
    //        {
    //            this.interstitial.Destroy();
    //        }

    //        // Create an interstitial.
    //        this.interstitial = new InterstitialAd(adUnitId);

    //        // Register for ad events.
    //        this.interstitial.OnAdClosed += this.Admob_OnInterstitialClosed;

    //        // Load an interstitial ad.
    //        this.interstitial.LoadAd(this.Admob_CreateAdRequest());
    //    }

    //    private void Admob_OnInterstitialClosed(object sender, EventArgs args)
    //    {
    //        LoadInterstitialAd_Admob();
    //    }

    //    public void LoadInterstitialAd_UnityAds()
    //    {

    //    }


    private int countFinishedGame = 0;

    public void ShowInterstitialAd_FinishedGame()
    {
        countFinishedGame++;
        Debug.Log("ShowInterstitialAd:" + countFinishedGame);
        //if (countFinishedGame % 4 == 0 || countFinishedGame % 4 == 2)
        //{
        //    Debug.Log("ShowInterstitialAd_FinishedGame:" + countFinishedGame);
        //    if (IsInterstitialAd_Loaded())
        //    {
        //        ShowInterstitialAd();
        //    }
        //}
    }

    //    public bool IsInterstitialAd_Loaded()
    //    {
    //        if (this.interstitial.IsLoaded() || (Advertisement.isSupported && Advertisement.IsReady("video")))
    //        {
    //            return true;
    //        }
    //        return false;
    //    }
    //private int indexInterstitialAds = 0;
    //public void ShowInterstitialAd()
    //{
    //    indexInterstitialAds++;
    //    if (indexInterstitialAds % 2 == 0)
    //    {
    //        if (Advertisement.isSupported && Advertisement.IsReady("video"))
    //        {
    //            ShowInterstitialAd_UnityAds();
    //        }
    //        else
    //        {
    //            ShowInterstitialAd_Admob();
    //        }
    //    }
    //    else
    //    {
    //        if (this.interstitial.IsLoaded())
    //        {
    //            ShowInterstitialAd_Admob();
    //        }
    //        else
    //        {
    //            ShowInterstitialAd_UnityAds();
    //        }
    //    }

    //}


    //public void ShowInterstitialAd_Admob()
    //{
    //    if (this.interstitial.IsLoaded())
    //    {
    //        this.interstitial.Show();
    //    }
    //}

    //public void ShowInterstitialAd_UnityAds()
    //{
    //    if (Advertisement.isSupported && Advertisement.IsReady("video"))
    //    {
    //        Advertisement.Show("video");
    //    }
    //}

    #endregion

    private void OnDestroy()
    {
        //rewardBasedVideo.OnAdRewarded -= Admob_HandleOnRewarded;
        //rewardBasedVideo.OnAdClosed -= Admob_HandleOnRewardedVideoAdClosed;
    }
}
