
﻿using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CampaignMapUI : MonoBehaviour {

    public Button _closeBtn;


    public Button _iceBtn,_sandBtn,_deathBtn,_fireBtn,_fieldBtn,_forestBtn;

    public Sprite _iceBg, _sandBg, _deathBg, _fireBg, _fieldBg, _forestBg;

    public GameObject _scrollViewMap;
    public GameObject _portalLevel;

    public Text goldNumber, eventPointNumber, diamondNumber;
    public static int indexMap = -1;

    Action<object> _UpdateTextValueEventRef;


    private void Start()
    {
      
        _iceBtn.image.alphaHitTestMinimumThreshold = 0.1f;
        _sandBtn.image.alphaHitTestMinimumThreshold = 0.1f;
        _deathBtn.image.alphaHitTestMinimumThreshold = 0.1f;
        _fireBtn.image.alphaHitTestMinimumThreshold = 0.1f;
        _fieldBtn.image.alphaHitTestMinimumThreshold = 0.1f;
        _forestBtn.image.alphaHitTestMinimumThreshold = 0.1f;
    }

    private void OnEnable()
    {
        UpdateTextValue();
        _UpdateTextValueEventRef = (param) => UpdateTextValue();
        this.RegisterListener(EventID.OnPropertiesChange, _UpdateTextValueEventRef);
        indexMap = -1;
        _portalLevel.SetActive(false);
        _closeBtn.onClick.AddListener(CloseThisDialog);
        _forestBtn.onClick.AddListener(()=>OpenMap(0));
        _iceBtn.onClick.AddListener(() => OpenMap(1));
        _sandBtn.onClick.AddListener(() => OpenMap(2));
        _fieldBtn.onClick.AddListener(() => OpenMap(3));
        _fireBtn.onClick.AddListener(() => OpenMap(4));
        _deathBtn.onClick.AddListener(() => OpenMap(5));
        if (CharacterInfo._instance._baseProperties.LevelMap == 0)
        {
            UpdateDisplayMap();
        }
    }
    private void OnDisable()
    {
        EventDispatcher.Instance.RemoveListener(EventID.OnPropertiesChange, _UpdateTextValueEventRef);
        _closeBtn.onClick.RemoveListener(CloseThisDialog);
    }


    void UpdateDisplayMap()
    {
        if (CharacterInfo._instance._baseProperties.LevelMap <= 20)
        {
            _forestBtn.transform.GetChild(0).gameObject.SetActive(false);
            _forestBtn.transform.GetChild(1).gameObject.SetActive(false);
        } else if (CharacterInfo._instance._baseProperties.LevelMap >=21 && CharacterInfo._instance._baseProperties.LevelMap <= 40)
        {
            _forestBtn.transform.GetChild(0).gameObject.SetActive(false);
            _forestBtn.transform.GetChild(1).gameObject.SetActive(false);
            _iceBtn.transform.GetChild(0).gameObject.SetActive(false);
            _iceBtn.transform.GetChild(1).gameObject.SetActive(false);

        } else if (CharacterInfo._instance._baseProperties.LevelMap >= 41 && CharacterInfo._instance._baseProperties.LevelMap <= 60)
        {
            _forestBtn.transform.GetChild(0).gameObject.SetActive(false);
            _forestBtn.transform.GetChild(1).gameObject.SetActive(false);
            _iceBtn.transform.GetChild(0).gameObject.SetActive(false);
            _iceBtn.transform.GetChild(1).gameObject.SetActive(false);
            _sandBtn.transform.GetChild(0).gameObject.SetActive(false);
            _sandBtn.transform.GetChild(1).gameObject.SetActive(false);
        } else if (CharacterInfo._instance._baseProperties.LevelMap >= 61 && CharacterInfo._instance._baseProperties.LevelMap <= 80)
        {
            _forestBtn.transform.GetChild(0).gameObject.SetActive(false);
            _forestBtn.transform.GetChild(1).gameObject.SetActive(false);
            _iceBtn.transform.GetChild(0).gameObject.SetActive(false);
            _iceBtn.transform.GetChild(1).gameObject.SetActive(false);
            _sandBtn.transform.GetChild(0).gameObject.SetActive(false);
            _sandBtn.transform.GetChild(1).gameObject.SetActive(false);
            _fieldBtn.transform.GetChild(0).gameObject.SetActive(false);
            _fieldBtn.transform.GetChild(1).gameObject.SetActive(false);
        } else if (CharacterInfo._instance._baseProperties.LevelMap >= 81 && CharacterInfo._instance._baseProperties.LevelMap <= 100)
        {
            _forestBtn.transform.GetChild(0).gameObject.SetActive(false);
            _forestBtn.transform.GetChild(1).gameObject.SetActive(false);
            _iceBtn.transform.GetChild(0).gameObject.SetActive(false);
            _iceBtn.transform.GetChild(1).gameObject.SetActive(false);
            _sandBtn.transform.GetChild(0).gameObject.SetActive(false);
            _sandBtn.transform.GetChild(1).gameObject.SetActive(false);
            _fieldBtn.transform.GetChild(0).gameObject.SetActive(false);
            _fieldBtn.transform.GetChild(1).gameObject.SetActive(false);
            _fireBtn.transform.GetChild(0).gameObject.SetActive(false);
            _fireBtn.transform.GetChild(1).gameObject.SetActive(false);
        } else if (CharacterInfo._instance._baseProperties.LevelMap >= 101 && CharacterInfo._instance._baseProperties.LevelMap <= 120)
        {
            _forestBtn.transform.GetChild(0).gameObject.SetActive(false);
            _forestBtn.transform.GetChild(1).gameObject.SetActive(false);
            _iceBtn.transform.GetChild(0).gameObject.SetActive(false);
            _iceBtn.transform.GetChild(1).gameObject.SetActive(false);
            _sandBtn.transform.GetChild(0).gameObject.SetActive(false);
            _sandBtn.transform.GetChild(1).gameObject.SetActive(false);
            _fieldBtn.transform.GetChild(0).gameObject.SetActive(false);
            _fieldBtn.transform.GetChild(1).gameObject.SetActive(false);
            _fireBtn.transform.GetChild(0).gameObject.SetActive(false);
            _fireBtn.transform.GetChild(1).gameObject.SetActive(false);
            _deathBtn.transform.GetChild(0).gameObject.SetActive(false);
            _deathBtn.transform.GetChild(1).gameObject.SetActive(false);

        }
    }

    bool checkCanOpenMap(int indexMap)
    {
        switch (indexMap)
        {
            case 0:
                if (CharacterInfo._instance._baseProperties.LevelMap <= 20) return true;
                break;
            case 1:
                if (CharacterInfo._instance._baseProperties.LevelMap >= 21 && CharacterInfo._instance._baseProperties.LevelMap <= 40) return true;
                break;
            case 2:
                if (CharacterInfo._instance._baseProperties.LevelMap >= 41 && CharacterInfo._instance._baseProperties.LevelMap <= 60) return true;
                break;
            case 3:
                if (CharacterInfo._instance._baseProperties.LevelMap >= 61 && CharacterInfo._instance._baseProperties.LevelMap <= 80) return true;
                break;
            case 4:
                if (CharacterInfo._instance._baseProperties.LevelMap >= 81 && CharacterInfo._instance._baseProperties.LevelMap <= 100) return true;
                break;
            case 5:
                if (CharacterInfo._instance._baseProperties.LevelMap >= 101 && CharacterInfo._instance._baseProperties.LevelMap <= 120) return true;
                break;
        }
        return false;
    }

    public void UpdateTextValue()
    {
        if (goldNumber.gameObject.activeSelf == true)
        {
            goldNumber.text = CharacterInfo._instance._baseProperties.Gold.ToString();
            eventPointNumber.text = CharacterInfo._instance._baseProperties.EventPoint.ToString();
            diamondNumber.text = CharacterInfo._instance._baseProperties.Diamond.ToString();
        }
    }

    void OpenMap(int idBtn)
    {
        switch (idBtn)
        {
            case 0:
                indexMap = idBtn;
                if(checkCanOpenMap(indexMap))
                OpenPortalPanel(indexMap);
                break;
            case 1:
                indexMap = idBtn;
                if (checkCanOpenMap(indexMap))
                    OpenPortalPanel(indexMap);
                break;
            case 2:
                indexMap = idBtn;
                if (checkCanOpenMap(indexMap))
                    OpenPortalPanel(indexMap);
                break;
            case 3:
                indexMap = idBtn;
                if (checkCanOpenMap(indexMap))
                    OpenPortalPanel(indexMap);
                break;
            case 4:
                indexMap = idBtn;
                if (checkCanOpenMap(indexMap))
                    OpenPortalPanel(indexMap);
                break;
            case 5:
              
                indexMap = idBtn;
                if (checkCanOpenMap(indexMap))
                    OpenPortalPanel(indexMap);
                break;
        }
    }

    public void OpenPortalPanel(int level)
    {
        _scrollViewMap.SetActive(false);
        _portalLevel.SetActive(true);
        switch (indexMap)
        {
            case 0:
                break;
            case 1:
                break;
            case 2:
                break;
            case 3:
                break;
            case 4:
                break;
            case 5:
                break;
            case 6:
                break;
        }
        switch (level)
        {
            case 1:
                break;
            case 2:
                break;
            case 3:
                break;
            case 4:
                break;
            case 5:
                break;
            case 6:
                break;
            case 7:
                break;
            case 8:
                break;
            case 9:
                break;
            case 10:
                break;
        }
    }
    public void BackToWorldMap()
    {
        _scrollViewMap.SetActive(true);
        _portalLevel.SetActive(false);
        UpdateDisplayMap();
    }

    public void CloseThisDialog()
    {
        Destroy(GameObject.Find("CharacterManager"));
        if (CharacterManager.Instance._meCharacter != null)
            Destroy(CharacterManager.Instance._meCharacter.gameObject);
        if (CharacterManager.Instance._enemyCharacter != null)
        {
            Destroy(CharacterManager.Instance._enemyCharacter.gameObject);
        }
        SceneLoader._instance.LoadScene(2);
    }
}
