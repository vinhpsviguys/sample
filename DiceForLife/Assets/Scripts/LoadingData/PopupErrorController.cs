using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
public class PopupErrorController : MonoBehaviour
{

    public static PopupErrorController Instance;
    [SerializeField]
    private Transform _myBorder;
    [SerializeField]
    private Text _myMessage;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            // If that is the case, we destroy other instances
            Destroy(gameObject);
        }

        // Here we save our singleton instance
        Instance = this;

    }

    void OnEnable()
    {
        _myBorder.localScale = Vector3.one * 0.5f;
        _myBorder.DOScale(1, 0.3f).SetEase(Ease.OutBack);
        
        switch (Constant.ID_ERROR)
        {
            case -1: _myMessage.text = "Error. Check internet connection!";
                break;
            case 0: _myMessage.text = "Can't connect to server.";
                break;
            case 1: _myMessage.text = "Verify account failed! Please try again.";
                break;
            case 2: _myMessage.text = "Unable to load update!";
                break;
            case 3: _myMessage.text = "Unable to load data.";
                break;
            case 4: _myMessage.text = "Unable to load game resource.";
                break;
            case 5: _myMessage.text = "Unable to login account.";
                break;
            case 6:
                _myMessage.text = "Unable to login.";
                break;
            case 7:
                _myMessage.text = "Hero can't equip this item because dont reach level require";
                break;
            case 8:
                _myMessage.text = "Name of hero is invalid";
                break;
            case 9:
                _myMessage.text = "Name of hero is aldready exist";
                break;
        }

        string _customString = string.Format("{0}|{1}", _myMessage.text, LoadingResource.dataError);
        _myMessage.text = _customString.Replace("|", "\n");
    }

    public void BtnOKClick()
    {
        //_myBorder.gameObject.SetActive(false);
        this.gameObject.SetActive(false);
    }
    public void ShowErrorWithContent(string content)
    {
        _myBorder.gameObject.SetActive(true);
        _myMessage.text = content;
    }
}
