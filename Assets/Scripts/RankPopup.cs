using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Threading.Tasks;
using Firebase.Database;
using LitJson;
using moon;
public class RankPopup : MonoBehaviour
{
    public static RankPopup instance;

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
        ShowMyInfo();

        SendUpdateMyRank();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnEnable()
    {
        OpenPopup();
    }

    public NotificationPopup notificationPopup;
         
    public GameObject popup;
    public void OpenPopup()
    {
        popup.transform.localScale = Vector3.zero;
        popup.transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack);
    }

    public void ClosePopup()
    {
        popup.transform.DOScale(Vector3.zero, 0.2f).SetEase(Ease.OutExpo).OnComplete(() => {
            gameObject.SetActive(false);
        });
    }

    public void TouchClose() {
        ClosePopup();
    }


    [Header("My Rank")]
    public Text txtMyIndex;
    public Text txtMyName;
    public Text txtmyLevel;

    public void ShowMyInfo() {
        txtMyIndex.text = "---";
        txtMyName.text = Config.GetUserName();
        txtmyLevel.text = "" + PlayerPrefs.GetInt("Level", 0);
    }


    public void ShowMyInfo_Rank(int _rank) {
        txtMyIndex.text = "" + _rank;
    }


    [Header("Loading")]
    public GameObject loading;
    public void ShowLoading() {
        loading.SetActive(true);
    }

    public void HideLoading() {
        loading.SetActive(false);
    }


    public void SendUpdateMyRank() {
        if (Config.lastUpdateRannk_Level != PlayerPrefs.GetInt("Level", 0))
        {
            Config.lastUpdateRannk_Level = PlayerPrefs.GetInt("Level", 0);

            StartCoroutine(RequestAddNewMyRank());
        }
        else {
            SendGetListRank();
        }
    }

    Task taskCheckMyUser;
    public IEnumerator CheckUserMyRank() {
        yield return new WaitForEndOfFrame();
        Debug.Log("CheckUserMyRankCheckUserMyRankCheckUserMyRank");
        coroutineCheckLoading = StartCoroutine(CheckLoading());
        FirebaseDatabase.DefaultInstance.GetReference("User").Child(Config.userIdentify).GetValueAsync().ContinueWith(task => {
            if (task.IsFaulted)
            {
                Debug.Log("CheckUserMyRank NOnonononono");
                //StartCoroutine(RequestAddNewMyRank());
            }
            else if (task.IsCompleted)
            {
                Debug.Log("CheckUserMyRank YES");
                //StartCoroutine(RequestUpdateMyRank());
            }
        });
    }

    Task taskUpdateMyRank;
    public IEnumerator RequestUpdateMyRank()
    {
        yield return new WaitForEndOfFrame();
        var newUser = new Dictionary<string, object>();
        newUser[Config.NAME] = Config.GetUserName();
        newUser[Config.LEVEL] = PlayerPrefs.GetInt("Level", 0);
        Debug.Log(Config.userIdentify);
        FirebaseDatabase.DefaultInstance.GetReference("User").Child(Config.userIdentify).Child(Config.NAME).SetValueAsync(Config.GetUserName());
        FirebaseDatabase.DefaultInstance.GetReference("User").Child(Config.userIdentify).Child(Config.LEVEL).SetValueAsync(PlayerPrefs.GetInt("Level", 0)).ContinueWith(task => {
            if (task.IsFaulted)
            {
                notificationPopup.ShowInfo("Check your internet connection and try again!");
                StopCoroutine(coroutineCheckLoading);
            }
            else if (task.IsCompleted)
            {
                SendGetListRank();
            }
        });
    }


    Task taskAddNewRank;
    public IEnumerator RequestAddNewMyRank() {
        Debug.Log("RequestAddNewMyRankRequestAddNewMyRank");
        yield return new WaitForEndOfFrame();
        coroutineCheckLoading = StartCoroutine(CheckLoading());
        var newUser = new Dictionary<string, object>();
        newUser[Config.NAME] = Config.GetUserName();
        newUser[Config.LEVEL] = PlayerPrefs.GetInt("Level", 0);
        Debug.Log(Config.userIdentify);
        taskAddNewRank = FirebaseDatabase.DefaultInstance.GetReference("User").Child(Config.userIdentify).SetRawJsonValueAsync(JsonMapper.ToJson(newUser))
            .ContinueWith(task1 =>
            {

                if (task1.IsFaulted)
                {
                    notificationPopup.ShowInfo("Check your internet connection and try again!");
                    StopCoroutine(coroutineCheckLoading);
                }
                else if (task1.IsCompleted)
                {
                    Debug.Log("RequestUpdateMyRankRequestUpdateMyRank           Finishedddddddddddddddddddddd");
                    
                }
            });
        while (!taskAddNewRank.IsCompleted) { yield return null; }

        SendGetListRank();

    }


    public void SendGetListRank()
    {
        Debug.Log("SendGetListRankSendGetListRankSendGetListRank");
        StartCoroutine(RequestGetListRank_Check());
    }
    Task taskListRank;
    DataSnapshot listRanksnapshot;
    private List<InfoUserFirebase> listUserRanks = new List<InfoUserFirebase>();
    public IEnumerator RequestGetListRank_Check() {
        Debug.Log("RequestGetListRank_CheckRequestGetListRank_CheckRequestGetListRank_CheckRequestGetListRank_Check");
        yield return new WaitForSeconds(0.02f);
        listUserRanks.Clear();
        taskListRank = FirebaseDatabase.DefaultInstance.GetReference("User").OrderByChild(Config.LEVEL).LimitToLast(200).GetValueAsync().ContinueWith(task1 =>
        {

            if (task1.IsFaulted)
            {
                Debug.Log("IsFaultedIsFaultedIsFaultedIsFaulted");
                notificationPopup.ShowInfo("Check your internet connection and try again!");
                StopCoroutine(coroutineCheckLoading);
            }
            else if (task1.IsCompleted)
            {
                listRanksnapshot = task1.Result;
            }
        });

        while (!taskListRank.IsCompleted) { yield return null; }

        StopCoroutine(coroutineCheckLoading);
        int indexRank = 0;
        Debug.Log(listRanksnapshot);
        if (listRanksnapshot != null)
        {
            var data = listRanksnapshot.Children;
            foreach (var Value in data)
            {
                indexRank++;

                string userID = Value.Key;
                Debug.Log(userID);
                string name = Value.Child(Config.NAME).Value.ToString();
                Debug.Log(name);
                int level = int.Parse(Value.Child(Config.LEVEL).Value.ToString());
                Debug.Log(level);
                InfoUserFirebase infoUserFirebase = new InfoUserFirebase(indexRank, userID, name, level);
                listUserRanks.Add(infoUserFirebase);
            }
        }

        ShowListRank();
        StopCoroutine(coroutineCheckLoading);
        HideLoading();
    }

    private Coroutine coroutineCheckLoading;
    public IEnumerator CheckLoading()
    {
        yield return new WaitForSeconds(Config.MAX_TIME_LOADING);
        HideLoading();
        notificationPopup.ShowInfo("Check your internet connection and try again!");

    }

    public RankBasicListAdapter rankBasicListAdapter;
    public void ShowListRank() {
        listUserRanks.Reverse();

        int indexMyRank = 0;
        for (int i = 0; i < listUserRanks.Count; i++) {
            if (listUserRanks[i].userID.Equals(Config.userIdentify)) {
                indexMyRank = i +1;
                RankPopup.instance.ShowMyInfo_Rank(indexMyRank);
                break;
            }
        }
        rankBasicListAdapter.gameObject.SetActive(true);
        rankBasicListAdapter.RemoveAllItems();
        rankBasicListAdapter.AddItemsAt_ListUserRanks(0, listUserRanks);
    }
}
