using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InboxUI : MonoBehaviour {

    public Button _closeBtn;
    public Button _newplayerBtn;
    public Button _evengiftBtn;
    public Button _messenger;

    public GameObject _newPlayerTabPanel;
    public GameObject _eventTabPanel;
    public GameObject _messTabPanel;

    private void OnEnable()
    {
        _closeBtn.onClick.AddListener(CloseThisDialog);
        _newplayerBtn.onClick.AddListener(ShowNewPlayerTabPanel);
        _evengiftBtn.onClick.AddListener(ShowEventTabPanel);
        _messenger.onClick.AddListener(ShowMessTabPanel);
        ShowNewPlayerTabPanel();
    }
    private void OnDisable()
    {
        _closeBtn.onClick.RemoveListener(CloseThisDialog);
        _newplayerBtn.onClick.RemoveListener(ShowNewPlayerTabPanel);
        _evengiftBtn.onClick.RemoveListener(ShowEventTabPanel);
        _messenger.onClick.RemoveListener(ShowMessTabPanel);
    }

    void ShowNewPlayerTabPanel()
    {
        _newPlayerTabPanel.SetActive(true);
        _eventTabPanel.SetActive(false);
        _messTabPanel.SetActive(false);
    }
    void ShowEventTabPanel()
    {
        _newPlayerTabPanel.SetActive(false);
        _eventTabPanel.SetActive(true);
        _messTabPanel.SetActive(false);
    }
    void ShowMessTabPanel()
    {
        _newPlayerTabPanel.SetActive(false);
        _eventTabPanel.SetActive(false);
        _messTabPanel.SetActive(true);
    }

    void CloseThisDialog()
    {
        this.gameObject.SetActive(false);
    }
}
