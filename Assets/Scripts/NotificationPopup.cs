using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
public class NotificationPopup : MonoBehaviour
{
    public Text titleText;
    public GameObject popup;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnEnable()
    {
        OpenPopup();
    }

    public void ShowInfo(string _content) {
        titleText.text = _content;
        gameObject.SetActive(true);
    }


    public void TouchOkie() {
        ClosePopup();
    }


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
}
