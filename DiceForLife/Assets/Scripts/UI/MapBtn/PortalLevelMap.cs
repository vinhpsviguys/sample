using SimpleJSON;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PortalLevelMap : MonoBehaviour {

    public GameObject _settingPanel,_portalBattleScene;

    public Transform _battlePortalContent;

    public Sprite _normalState, _autoState;

    public Button _battleBtn;
    public Button _getRewardBtn;

    public Text goldNumber, eventPointNumber, diamondNumber;
    Dictionary<int, GameObject> _dicLevelMapObj = new Dictionary<int, GameObject>();

    Action<object> _UpdateTextValueEventRef;

    public UnityEngine.Object _portalLvlBtn;

    int levelMapChoosed = -1;
    private void OnEnable()
    {
        if (CharacterManager.Instance._meCharacter == null)
        {
            CharacterManager.Instance.CreatePlayerProperties();
            CharacterManager.Instance._meCharacter.transform.position = new Vector3(-4.8f, -1f, 0f);
        } else
        {
            CharacterManager.Instance._meCharacter.gameObject.SetActive(true);
        }
        UpdateTextValue();
        CreatePortalLevel();
        _UpdateTextValueEventRef = (param) => UpdateTextValue();
        this.RegisterListener(EventID.OnPropertiesChange, _UpdateTextValueEventRef);
        foreach (Transform child in _battlePortalContent)
        {
            //child.GetComponent<Button>().onClick.AddListener(() => ChooseAutoPortalBattle(child));
            child.GetComponent<Button>().onClick.AddListener(() => ChoosePortalLevelToBattle(child.gameObject));
        }
        _battleBtn.onClick.AddListener(OpenBattleScene);
        _getRewardBtn.onClick.AddListener(GetRewardAutofarm);
        ChooseAutoPortalLevel();
    }

    void CreatePortalLevel()
    {
        if (_battlePortalContent.childCount == 0)
        {
            for(int i = 0; i < 20; i++)
            {
                GameObject _tempPortalLevel = Instantiate(_portalLvlBtn as GameObject);
                _tempPortalLevel.transform.parent = _battlePortalContent;
                _tempPortalLevel.transform.localScale = Vector3.one;
                _tempPortalLevel.transform.GetChild(0).GetComponent<Text>().text = "" + (CampaignMapUI.indexMap + 1) + " - " + (i + 1);
                _dicLevelMapObj.Add((CampaignMapUI.indexMap) * 20 + (i + 1), _tempPortalLevel);
            }
        }
            else
        {
            _dicLevelMapObj.Clear();
            int index = 1;
            foreach(Transform child in _battlePortalContent)
            {
                child.GetChild(0).GetComponent<Text>().text = "" + (CampaignMapUI.indexMap + 1) + " - " + index;
                _dicLevelMapObj.Add((CampaignMapUI.indexMap) * 20 + index, child.gameObject);
                index++;
            }
        }
    }

    void ChooseAutoPortalLevel()
    {
        if (CharacterInfo._instance._baseProperties.LevelMap == 0)
        {
            ChooseAutoPortalBattle(_dicLevelMapObj[1].transform);
            _battleBtn.interactable = true;
        } else
        {
            ChooseAutoPortalBattle(_dicLevelMapObj[CharacterInfo._instance._baseProperties.LevelMap].transform);
            _battleBtn.interactable = false;
        }
    }

    private void OnDisable()
    {
        EventDispatcher.Instance.RemoveListener(EventID.OnPropertiesChange, _UpdateTextValueEventRef);
        foreach (Transform child in _battlePortalContent)
        {
            child.GetComponent<Button>().onClick.RemoveListener(() => ChooseAutoPortalBattle(child));
        }
        _battleBtn.onClick.RemoveListener(OpenBattleScene);
        _getRewardBtn.onClick.RemoveListener(GetRewardAutofarm);
    }


    public void UpdateTextValue()
    {
        goldNumber.text = CharacterInfo._instance._baseProperties.Gold.ToString();
        eventPointNumber.text = CharacterInfo._instance._baseProperties.EventPoint.ToString();
        diamondNumber.text = CharacterInfo._instance._baseProperties.Diamond.ToString();
    }


    public void OpenSettingPanel()
    {
        _settingPanel.SetActive(true);
    }

    public void ChooseAutoPortalBattle(Transform _btn)
    {
        foreach (Transform child in _battlePortalContent)
        {
            if (child != _btn)
            {
                child.GetComponent<Image>().sprite = _normalState;
            }
        }
        _btn.GetComponent<Image>().sprite = _autoState;
    }

    public void ChoosePortalLevelToBattle(GameObject portalGameObj)
    {
        if (levelMapChoosed == -1)
        {
            levelMapChoosed = getLevelMonsterInPortalLevel(portalGameObj);
            CharacterManager.Instance.CreateMonster("Caimpaign", 1);
        }
        else
        {
            if(levelMapChoosed != getLevelMonsterInPortalLevel(portalGameObj))
            {
                levelMapChoosed = getLevelMonsterInPortalLevel(portalGameObj);
                CharacterManager.Instance.CreateMonster("Caimpaign", 1);
            }
        }
       
        if (checkCanBattlePortalLevel(portalGameObj))
        {
            _battleBtn.interactable = true;
        } else
        {
            _battleBtn.interactable = false;
        }
    }

    int getLevelMonsterInPortalLevel(GameObject thisPortalLevel)
    {
        foreach (KeyValuePair<int, GameObject> pair in _dicLevelMapObj)
        {
            if (pair.Value == thisPortalLevel)
            {
               
                  return pair.Key;
            }
        }
        return -1;
    }

    bool checkCanBattlePortalLevel(GameObject thisPortalLevel)
    {
        foreach (KeyValuePair<int, GameObject> pair in _dicLevelMapObj)
        {
            if (pair.Value == thisPortalLevel)
            {
                if (pair.Key - 1 == CharacterInfo._instance._baseProperties.LevelMap)
                {
                    return true;
                }
            }
        }
        return false;
    }

    public void OpenBattleScene()
    {
        _portalBattleScene.SetActive(true);
    }

    public void GetRewardAutofarm()
    {
         StartCoroutine(ServerAdapter.GetRewardFromMonster(CharacterInfo._instance._baseProperties.idHero, CharacterInfo._instance._baseProperties.idCodeHero, 10, 100, result => {
             if (result.StartsWith("Error"))
            {
                Debug.Log("do nothing");
            }
            else
            {
                Debug.Log(result);
                 var N = JSONNode.Parse(result);
                 int numberItemEquipReceved = N.Count;
                 EquipItemReward[] _listItemReceived = new EquipItemReward[numberItemEquipReceved];
                 for (int i = 0; i < numberItemEquipReceved; i++)
                 {
                     _listItemReceived[i] = JsonUtility.FromJson<EquipItemReward>(N[i].ToString());
                 }
             }

        }));
    }

}
