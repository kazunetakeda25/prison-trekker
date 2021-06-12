using Firebase;
using Firebase.Analytics;
using Firebase.Unity.Editor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class FirebaseManager : MonoBehaviour
{
    public static FirebaseManager instance;

    public void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

    }
    // Start is called before the first frame update
    void Start()
    {
        InitFirebase();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public bool firebaseInitialized = false;

    DependencyStatus dependencyStatus = DependencyStatus.UnavailableOther;
    DependencyStatus dependencyStatusRC = DependencyStatus.UnavailableOther;
    public void InitFirebase()
    {
        dependencyStatus = FirebaseApp.CheckDependencies();
        if (dependencyStatus != DependencyStatus.Available)
        {
            FirebaseApp.FixDependenciesAsync().ContinueWith(task =>
            {
                dependencyStatus = FirebaseApp.CheckDependencies();
                if (dependencyStatus == DependencyStatus.Available)
                {
                    InitializeFirebase();
                }
                else
                {
                    Debug.LogError(
                        "Could not resolve all Firebase dependencies: " + dependencyStatus);
                }
            });
        }
        else
        {
            InitializeFirebase();
        }
    }

    public void InitializeFirebase()
    {
        firebaseInitialized = true;
        FirebaseAnalytics.SetAnalyticsCollectionEnabled(true);

        // Set the user's sign up method.
#if UNITY_IPHONE
		FirebaseAnalytics.SetUserProperty(
		"System",
		"IOS");
#elif UNITY_ANDROID
        FirebaseAnalytics.SetUserProperty(
            "System",
            "Android");
#else
		FirebaseAnalytics.SetUserProperty(
		"System",
		"Other");
#endif
        Config.userIdentify = SystemInfo.deviceUniqueIdentifier;
        FirebaseAnalytics.SetUserId(Config.userIdentify);
        Debug.Log("InitializeFirebaseInitializeFirebaseInitializeFirebaseInitializeFirebase");
        FirebaseApp.DefaultInstance.SetEditorDatabaseUrl("https://pirson-escape-plan.firebaseio.com/");

        if (FirebaseApp.DefaultInstance.Options.DatabaseUrl != null) FirebaseApp.DefaultInstance.SetEditorDatabaseUrl(FirebaseApp.DefaultInstance.Options.DatabaseUrl);

        
    }

    
}
