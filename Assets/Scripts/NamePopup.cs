using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using DG.Tweening;
public class NamePopup : MonoBehaviour
{
    public InputField infName;
    public Action OnChangeName = delegate () { };

    public GameObject popup;
    // Start is called before the first frame update
    void Start()
    {
        infName.text = Config.GetUserName();
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    private void OnEnable()
    {
        OpenPopup();
    }

    public void OpenPopup() {
        popup.transform.localScale = Vector3.zero;
        popup.transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack);
    }

    public void ClosePopup() {
        popup.transform.DOScale(Vector3.zero, 0.2f).SetEase(Ease.OutExpo).OnComplete(()=> {
            gameObject.SetActive(false);
        });
    }

    public void TouchOkie() {
        Config.SetUserName(infName.text.Trim());
        OnChangeName();
        ClosePopup();
    }

    public void TouchClose() {
        ClosePopup();
    }
}
