using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SimpleJSON;
using System;
using System.IO;

public class SelectStartHeroScript : MonoBehaviour
{
    private int numberClass = 8;
    private string[] listClass = new string[] { "Barbarian", "Orc", "Marksman", "Wizard", "Sorceress", "Cleric", "Paladin", "Assassin" };


    [SerializeField] private Transform _imgRock, _BtnParent;
    [SerializeField] private Text _txtNameClass, _txtScriptClass;
    [SerializeField] private GameObject _itemHeroPrefabs;
    [SerializeField] private Sprite[] _listElementClass;
    [SerializeField] private Image _imgElementClass;
    [SerializeField] private GameObject _enterNameObject;

    [SerializeField] private GameObject _panelElement, _panelSkill;
    [SerializeField] private InputField _nameInputField;
    [SerializeField] private Text _txtMessage;


    private GameObject _heroObject, _tempObject;

    private GameObject[] _characterPrefabs;
    private int lastIdHeroSelected = -1;

    ReadXML _localDataHero;

    Action<object> _ItemHeroClickEventRef;
    private void Awake()
    {
        _localDataHero = new ReadXML("XMLFiles/Characters");
    }
    IEnumerator Start()
    {
        numberClass = SplitDataFromServe._heroInits.Length;
        _characterPrefabs = new GameObject[numberClass];

        for (int i = 0; i < numberClass; i++)
        {
            yield return StartCoroutine(LoadingResourceController._instance.LoadAssetBundleObjectAsync(listClass[i].ToLower(), listClass[i], result =>
            {
                _characterPrefabs[i] = result;
            }));
        }

        CreateButtonHeros();
        BtnSelectTabElement();
        ItemHeroClick(3);

        _ItemHeroClickEventRef = (param) => ItemHeroClick((int)param);
        this.RegisterListener(EventID.ItemHeroSelected, _ItemHeroClickEventRef);
    }
    void ItemHeroClick(int idHero)
    {
        if (lastIdHeroSelected != idHero)
        {
            lastIdHeroSelected = idHero;
            CreateSkeletonCharacter(idHero);
            _txtNameClass.text = listClass[idHero];
            _imgElementClass.sprite = _listElementClass[idHero];
            _txtScriptClass.text = _localDataHero.getDataByIndex(idHero).Attributes["script"].Value.Replace(";", "\n");
        }
    }
    void CreateSkeletonCharacter(int idHero)
    {
        if (_heroObject != null) GameObject.DestroyObject(_heroObject);
        _heroObject = Instantiate(_characterPrefabs[idHero]);
        _heroObject.transform.SetParent(_imgRock);
        _heroObject.name = "Hero " + idHero;
        _heroObject.transform.localScale = Vector3.one * 100;
        _heroObject.transform.localPosition = new Vector3(0, 136);
    }
    void CreateButtonHeros()
    {
        for (int i = 0; i < numberClass; i++)
        {
            _tempObject = Instantiate(_itemHeroPrefabs);
            _tempObject.transform.SetParent(_BtnParent);
            _tempObject.transform.localScale = Vector3.one;
            _tempObject.name = listClass[i];
            ItemHeroSelect sc = _tempObject.GetComponent<ItemHeroSelect>();
            sc.myIdHero = i;
            sc.myIcon.sprite = CharacterItemInGame.Instance._avatarRect[i];
            if(listClass[i]== "Orc" || listClass[i] == "Marksman" || listClass[i] == "Sorceress" || listClass[i] == "Barbarian")
            {
                _tempObject.SetActive(false);
            }
        }

    }

    public void BtnSelectedClass()
    {
        _enterNameObject.SetActive(true);
        _txtMessage.text = string.Empty;
    }
    public void BtnSelectTabElement()
    {
        _panelElement.SetActive(true);
        _panelSkill.SetActive(false);
    }
    public void BtnSelectTabSkill()
    {
        _panelSkill.SetActive(true);
        _panelElement.SetActive(false);
    }
    public void BtnBack()
    {
        _enterNameObject.SetActive(false);
    }

    public void BtnStartGame()
    {
        string _txtInput = _nameInputField.text.Trim();
        if (_txtInput.Length < 6)
        {
            _txtMessage.text = "Please enter a password at least 6 characters!";
        }
        else
        {
            _txtMessage.text = "Checking name...";
            StartCoroutine(ServerAdapter.CheckNameCreateHero(_txtInput, result =>
            {
                if (result.StartsWith("Error"))
                {
                    _txtMessage.text = result;
                }
                else
                {
                    _txtMessage.text = "Creating hero...";
                    int idih = -1;
                    for (int i = 0; i < SplitDataFromServe._heroInits.Length; i++)
                    {
                        if (SplitDataFromServe._heroInits[i].name == listClass[lastIdHeroSelected])
                        {
                            idih = int.Parse(SplitDataFromServe._heroInits[lastIdHeroSelected].idih);
                            break;
                        }
                    }
                    if (idih == -1)
                    {
                        _txtMessage.text = "Id class hero is wrong!";
                    }
                    else StartCoroutine(ServerAdapter.ExecuteCreateHero(idih, _txtInput, result2 =>
                      {
                          if (result2.StartsWith("Error"))
                          {
                              _txtMessage.text = result2;
                          }
                          else
                          {
                              _txtMessage.text = "Create your hero successfully!";
                              var N = JSON.Parse(result2);
                              PlayerPrefabsController.SetStringData(Constant.IDHERO_CURRENTPLAY, N["hero"]["idh"].Value);
                              StartCoroutine(ServerAdapter.LoadDetailHero(N["hero"]["idcode"].Value, int.Parse(N["hero"]["idh"].Value), result3 =>
                              {
                                  if (result3.StartsWith("Error"))
                                  {
                                      _txtMessage.text = result;
                                  }
                                  else
                                  {
                                      SplitDataFromServe.ReadDetailDataHeroCurrentPlay(result3);
                                      Debug.Log("Load scene MainMenu");
                                      StartCoroutine(SceneLoader._instance.LoadNewScene(2));
                                  }
                              }));
                          }
                      }));
                }
            }));
        }
    }

}
