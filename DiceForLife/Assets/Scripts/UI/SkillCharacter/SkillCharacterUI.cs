using CoreLib;
using UnityEngine;
using UnityEngine.UI;

public class SkillCharacterUI : MonoBehaviour
{


    public static SkillCharacterUI Instance;
    [SerializeField]
    Button _basicBtn, _advanceBtn, _backBtn;
    [SerializeField]
    GameObject _basicSkillParent, _advanceSkillParent, _skillInfoObj;
    [SerializeField]
    Transform _passiveSkillParent, _buffSkillParent, _activeSkillParent;
    [SerializeField]
    Image _skillInfoImg;
    [SerializeField]
    Text _nameSkillInfo, _levelSkillInfo, _typeSkillInfo, _desSkillCurrent, _valueSkillCurrentLevel, _valueSkillNextLevel;
    [SerializeField]
    Text _skillClass;
    [SerializeField]
    Text _skillPoint;
    [SerializeField]
    Transform _skillEquipParent;
    [SerializeField]
    Sprite[] _chooseSkillBtnSprite;
    [SerializeField]
    Sprite[] _btnStateSprite;
    [SerializeField]
    Button _closeInfo, _learnSkill;
    public Transform _dragItem;

    bool isBasicTab = true;
    bool haveSkillReady = false;

    private NewSkill _skillDataInfo = null;
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

    private void OnEnable()
    {
        _backBtn.onClick.AddListener(CloseThisDialog);
        _basicBtn.onClick.AddListener(ShowBasicSkillTab);
        _advanceBtn.onClick.AddListener(ShowAdvanceSkillTab);
        _learnSkill.onClick.AddListener(LearnNewSkill);
        _closeInfo.onClick.AddListener(() => { _skillDataInfo = null; _skillInfoObj.SetActive(false); });
        _skillPoint.text = CharacterInfo._instance._baseProperties.SkillPoint.ToString();
        LoadSkillEquipInCharacter();
        ShowBasicSkillTab();
        ListSkillEachClass(ConverClassCharacterEnumToId(CharacterInfo._instance._baseProperties._classCharacter));
    }


    private void OnDisable()
    {
        _backBtn.onClick.RemoveListener(CloseThisDialog);
        _basicBtn.onClick.RemoveListener(ShowBasicSkillTab);
        _advanceBtn.onClick.RemoveListener(ShowAdvanceSkillTab);
        _learnSkill.onClick.RemoveListener(LearnNewSkill);
        _closeInfo.onClick.RemoveListener(() => { _skillDataInfo = null; _skillInfoObj.SetActive(false); });
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.Escape))
        {
            CloseThisDialog();
        }
    }

    void LoadSkillEquipInCharacter()
    {
        int index = 0;
        foreach (NewSkill skill in SplitDataFromServe._heroSkill)
        {
            if (skill.data["typewear"].AsInt == 1 && skill.data["type"].Value != "passive")
            {
                if (index < 10)
                {
                    //PlayerPrefs.DeleteKey(skill.data["idhk"]);
                    GameObject skillObj = null;
                    if (PlayerPrefs.HasKey(skill.data["idhk"]))
                    {
                        if (haveSkillReady)
                            skillObj = _skillEquipParent.GetChild(PlayerPrefs.GetInt(skill.data["idhk"])).transform.GetChild(0).gameObject;
                        else
                            skillObj = Instantiate(Resources.Load("Prefabs/Skill") as GameObject);
                        skillObj.transform.parent = _skillEquipParent.GetChild(PlayerPrefs.GetInt(skill.data["idhk"])).transform;
                    }
                    else
                    {
                        skillObj = Instantiate(Resources.Load("Prefabs/Skill") as GameObject);
                        skillObj.transform.parent = _skillEquipParent.GetChild(index).transform;
                        PlayerPrefs.SetInt(skill.data["idhk"], index);
                    }
                    skillObj.transform.localPosition = Vector3.zero;
                    skillObj.GetComponent<RectTransform>().localScale = new Vector3(1f, 1f, 1f);
                    skillObj.transform.GetComponent<Image>().sprite = Resources.Load<Sprite>("Textures/skillAss/" + skill.data["sprite"]);
                    if (skillObj.GetComponent<DragHandeler>() == null)
                        skillObj.AddComponent<DragHandeler>().dataSkill = skill;
                    if (skillObj.GetComponent<DropHandle>() == null)
                        skillObj.AddComponent<DropHandle>().dataSkill = skill;
                    skillObj.tag = "Skill";
                    index++;
                }
            }
        }
        haveSkillReady = true;
    }

    void LoadInfoCharacter()
    {

    }

    public void ShowInfoSkill(NewSkill skillData)
    {
        bool skillLeard = false;
        foreach (NewSkill _tempSkill in SplitDataFromServe._heroSkill)
        {
            if (_tempSkill.data["idk"].AsInt == skillData.data["idInit"].AsInt)
            {
                skillLeard = true;
                break;
            }
        }

        _skillPoint.text = CharacterInfo._instance._baseProperties.SkillPoint.ToString();
        int levelRequiredToLearn = getLevelRequiredToLearnNextLevel(skillData);
        int spRequiredToLearn = getSPRequiredToUpgrade(skillData);
        if (skillData.data["level"].AsInt < 10)
        {
            int levelRequiredToLearnTemp = getLevelRequiredToLearnNextLevel(skillData);
            int spRequiredToLearnTemp = getSPRequiredToUpgrade(skillData);
            if (skillLeard)
            {
                _desSkillCurrent.text = skillData.data["description"].Value + "\n" + "Level require to learn next level :" + levelRequiredToLearnTemp + "\n" + "Skillpoint require to learn next level :" + spRequiredToLearnTemp;
            }
            else
            {
                _desSkillCurrent.text = skillData.data["description"].Value + "\n" + "Level require to learn :" + int.Parse(skillData.data["levelrequired"].Value.Split('/')[0].ToString()) + "\n" + "Skillpoint require to learn :" + Constant.UPGRADE_SKILLPOINT_REQUIRE[int.Parse(skillData.data["levelrequired"].Value.Split('/')[0].ToString())];
            }
        }
        else if (skillData.data["level"].AsInt == 10)
        {
            _desSkillCurrent.text = skillData.data["description"].Value;
        }
        if (skillData.data["level"].AsInt < 10)
        {
            if ((skillData.data["class"].Value == CharacterInfo._instance._baseProperties._classCharacter.ToString() || skillData.data["class"].Value == "Ice"
                || skillData.data["class"].Value == "Lightning" || skillData.data["class"].Value == "Fire" || skillData.data["class"].Value == "Earth") && CharacterInfo._instance._baseProperties.Level >= levelRequiredToLearn)
            {
                _learnSkill.gameObject.SetActive(true);
            }
            else
            {
                _learnSkill.gameObject.SetActive(false);
            }
        }
        else if (skillData.data["level"].AsInt == 10)
        {
            _learnSkill.gameObject.SetActive(false);
        }
        _skillDataInfo = skillData;
        _skillInfoObj.SetActive(true);
        _nameSkillInfo.text = skillData.data["name"].Value;
        if (skillLeard)
        {
            _levelSkillInfo.gameObject.SetActive(true);
            _levelSkillInfo.text = "Level " + skillData.data["level"].Value;
        }
        else
        {
            _levelSkillInfo.gameObject.SetActive(false);
        }
        _typeSkillInfo.text = skillData.data["type"].Value;
        _skillInfoImg.sprite = Resources.Load<Sprite>("Textures/skillAss/" + skillData.data["sprite"].Value);
        ShowValueSkill(skillData.data["value"].Value, skillData.data["level"].AsInt);
    }

    void ShowValueSkill(string data, int currentLevel)
    {
        string _valueContentCurrentLevel = "";
        string _valueContentNextLevel = "";
        string[] _value = data.Split(';');
        foreach (string temp in _value)
        {
            string[] _tempValue = temp.Split('/');
            if (_tempValue.Length == 4)
            {
                if (_tempValue[3] == "%")
                {
                    string _tempContent = _tempValue[0] + " : " + (float.Parse(_tempValue[1]) + float.Parse(_tempValue[2]) * (currentLevel - 1)).ToString() + " %";
                    string _tempContentNextLevel = "";
                    if (currentLevel < 10)
                    {
                        _tempContentNextLevel = _tempValue[0] + " : " + (float.Parse(_tempValue[1]) + float.Parse(_tempValue[2]) * (currentLevel)).ToString() + " %";
                    }
                    _valueContentCurrentLevel = _valueContentCurrentLevel + _tempContent + "\n";
                    _valueContentNextLevel = _valueContentNextLevel + _tempContentNextLevel + "\n";
                }
                else if (_tempValue[3] == "float")
                {
                    string _tempContentNextLevel = "";
                    string _tempContent = _tempValue[0] + " : " + (float.Parse(_tempValue[1]) + float.Parse(_tempValue[2]) * (currentLevel - 1)).ToString();
                    if (currentLevel < 10)
                    {
                        _tempContentNextLevel = _tempValue[0] + " : " + (float.Parse(_tempValue[1]) + float.Parse(_tempValue[2]) * (currentLevel)).ToString();
                    }
                    _valueContentCurrentLevel = _valueContentCurrentLevel + _tempContent + "\n";
                    _valueContentNextLevel = _valueContentNextLevel + _tempContentNextLevel + "\n";
                }
            }
        }
        _valueSkillCurrentLevel.text = _valueContentCurrentLevel;
        _valueSkillNextLevel.text = _valueContentNextLevel;
    }


    void ShowBasicSkillTab()
    {
        _basicSkillParent.SetActive(true);
        _basicBtn.GetComponent<Image>().sprite = _btnStateSprite[0];
        _basicBtn.transform.GetChild(0).GetComponent<Image>().sprite = _chooseSkillBtnSprite[0];
        _advanceBtn.GetComponent<Image>().sprite = _btnStateSprite[1];
        _advanceBtn.transform.GetChild(0).GetComponent<Image>().sprite = _chooseSkillBtnSprite[2];
        _advanceSkillParent.SetActive(false);
    }

    void ShowAdvanceSkillTab()
    {
        _basicSkillParent.SetActive(false);
        _basicBtn.GetComponent<Image>().sprite = _btnStateSprite[1];
        _basicBtn.transform.GetChild(0).GetComponent<Image>().sprite = _chooseSkillBtnSprite[1];
        _advanceBtn.GetComponent<Image>().sprite = _btnStateSprite[0];
        _advanceBtn.transform.GetChild(0).GetComponent<Image>().sprite = _chooseSkillBtnSprite[3];
        _advanceSkillParent.SetActive(true);
    }

    public void ListSkillEachClass(int typeClass)
    {
        _skillClass.text = ConvertIdClassToClassName(typeClass).ToString();
        int indexPassive = 0;
        int indexBuff = 0;
        int indexActive = 0;
        foreach (NewSkill skill in SplitDataFromServe.skillInit.Values)
        {
            if (skill.data["class"].Value == ConvertIdClassToClassName(typeClass).ToString())
            {
                if (skill.data["type"].Value == "passive")
                {
                    if (_passiveSkillParent.childCount < 4)
                    {
                        GameObject skillObj = Instantiate(Resources.Load("Prefabs/SkillInListPrefab") as GameObject);
                        skillObj.transform.parent = _passiveSkillParent;
                        skillObj.transform.localPosition = Vector3.zero;
                        skillObj.GetComponent<RectTransform>().localScale = new Vector3(1f, 1f, 1f);
                        skillObj.transform.GetComponent<Image>().sprite = Resources.Load<Sprite>("Textures/skillAss/" + skill.data["sprite"].Value);
                        if (skillObj.GetComponent<DragHandeler>() == null)
                        {
                            skillObj.AddComponent<DragHandeler>().dataSkill = skill;
                        }
                        if (SplitDataFromServe._heroSkill.Count > 0)
                        {
                            foreach (NewSkill _skill in SplitDataFromServe._heroSkill)
                            {

                                if (_skill.data["idk"].AsInt == skill.data["idInit"].AsInt)
                                {
                                    skillObj.GetComponent<DragHandeler>().dataSkill = _skill;
                                    skillObj.transform.GetComponent<Image>().color = new Color(1f, 1f, 1f, 1f);
                                    skillObj.transform.GetChild(0).GetComponent<Text>().text = _skill.data["level"].AsInt == 10 ? "Max" : "Lvl " + _skill.data["level"].AsInt;
                                    break;
                                }
                                else if (_skill.data["idk"].AsInt != skill.data["idInit"].AsInt)
                                {
                                    skillObj.transform.GetComponent<Image>().color = new Color(152f / 255f, 152f / 255f, 152f / 255f, 1f);
                                    skillObj.transform.GetChild(0).GetComponent<Text>().text = "";
                                }
                            }
                        }
                        else if (SplitDataFromServe._heroSkill.Count == 0)
                        {
                            skillObj.transform.GetComponent<Image>().color = new Color(152f / 255f, 152f / 255f, 152f / 255f, 1f);
                            skillObj.transform.GetChild(0).GetComponent<Text>().text = "";
                        }
                        skillObj.tag = "SkillInList";
                    }
                    else if (_passiveSkillParent.childCount == 4)
                    {
                        _passiveSkillParent.GetChild(indexPassive).GetComponent<Image>().sprite = Resources.Load<Sprite>("Textures/skillAss/" + skill.data["sprite"].Value);
                        if (_passiveSkillParent.GetChild(indexPassive).GetComponent<DragHandeler>() != null)
                        {
                            _passiveSkillParent.GetChild(indexPassive).GetComponent<DragHandeler>().dataSkill = skill;
                        }
                        if (SplitDataFromServe._heroSkill.Count > 0)
                        {
                            foreach (NewSkill _skill in SplitDataFromServe._heroSkill)
                            {
                                if (_skill.data["idInit"].AsInt == skill.data["idInit"].AsInt)
                                {
                                    _passiveSkillParent.GetChild(indexPassive).GetComponent<DragHandeler>().dataSkill = _skill;
                                    _passiveSkillParent.GetChild(indexPassive).GetComponent<Image>().color = new Color(1f, 1f, 1f, 1f);
                                    _passiveSkillParent.GetChild(indexPassive).GetChild(0).GetComponent<Text>().text = _skill.data["level"].AsInt == 10 ? "Max" : "Lvl " + _skill.data["level"].AsInt;
                                    break;
                                }
                                else
                                {
                                    _passiveSkillParent.GetChild(indexPassive).GetComponent<Image>().color = new Color(152f / 255f, 152f / 255f, 152f / 255f, 1f);
                                    _passiveSkillParent.GetChild(indexPassive).GetChild(0).GetComponent<Text>().text = "";
                                }
                            }
                        }
                        else if (SplitDataFromServe._heroSkill.Count == 0)
                        {
                            _passiveSkillParent.GetChild(indexPassive).GetComponent<Image>().color = new Color(152f / 255f, 152f / 255f, 152f / 255f, 1f);
                            _passiveSkillParent.GetChild(indexPassive).GetChild(0).GetComponent<Text>().text = "";
                        }
                        indexPassive++;
                    }


                }
                else if (skill.data["type"].Value == "buff")
                {
                    if (_buffSkillParent.childCount < 8)
                    {
                        GameObject skillObj = Instantiate(Resources.Load("Prefabs/SkillInListPrefab") as GameObject);
                        skillObj.transform.parent = _buffSkillParent;
                        skillObj.transform.localPosition = Vector3.zero;
                        skillObj.GetComponent<RectTransform>().localScale = new Vector3(1f, 1f, 1f);
                        skillObj.transform.GetComponent<Image>().sprite = Resources.Load<Sprite>("Textures/skillAss/" + skill.data["sprite"].Value);
                        if (skillObj.GetComponent<DragHandeler>() == null)
                        {
                            skillObj.AddComponent<DragHandeler>().dataSkill = skill;
                        }
                        if (SplitDataFromServe._heroSkill.Count > 0)
                        {
                            foreach (NewSkill _skill in SplitDataFromServe._heroSkill)
                            {
                                if (_skill.data["idk"].AsInt == skill.data["idInit"].AsInt)
                                {
                                    skillObj.GetComponent<DragHandeler>().dataSkill = _skill;
                                    skillObj.transform.GetComponent<Image>().color = new Color(1f, 1f, 1f, 1f);
                                    skillObj.transform.GetChild(0).GetComponent<Text>().text = _skill.data["level"].AsInt == 10 ? "Max" : "Lvl " + _skill.data["level"].AsInt;
                                    break;
                                }
                                else
                                {
                                    skillObj.transform.GetComponent<Image>().color = new Color(152f / 255f, 152f / 255f, 152f / 255f, 1f);
                                    skillObj.transform.GetChild(0).GetComponent<Text>().text = "";
                                }
                            }
                        }
                        else if (SplitDataFromServe._heroSkill.Count == 0)
                        {
                            skillObj.transform.GetComponent<Image>().color = new Color(152f / 255f, 152f / 255f, 152f / 255f, 1f);
                            skillObj.transform.GetChild(0).GetComponent<Text>().text = "";
                        }
                        skillObj.tag = "SkillInList";
                    }
                    else if (_buffSkillParent.childCount == 8)
                    {
                        _buffSkillParent.GetChild(indexBuff).GetComponent<Image>().sprite = Resources.Load<Sprite>("Textures/skillAss/" + skill.data["sprite"].Value);
                        if (_buffSkillParent.GetChild(indexBuff).GetComponent<DragHandeler>() != null)
                        {
                            _buffSkillParent.GetChild(indexBuff).GetComponent<DragHandeler>().dataSkill = skill;
                        }
                        if (SplitDataFromServe._heroSkill.Count > 0)
                        {
                            foreach (NewSkill _skill in SplitDataFromServe._heroSkill)
                            {
                                if (_skill.data["idInit"].AsInt == skill.data["idInit"].AsInt)
                                {
                                    _buffSkillParent.GetChild(indexBuff).GetComponent<DragHandeler>().dataSkill = _skill;
                                    _buffSkillParent.GetChild(indexBuff).GetComponent<Image>().color = new Color(1f, 1f, 1f, 1f);
                                    _buffSkillParent.GetChild(indexBuff).GetChild(0).GetComponent<Text>().text = _skill.data["level"].AsInt == 10 ? "Max" : "Lvl " + _skill.data["level"].AsInt;
                                    break;
                                }
                                else
                                {
                                    _buffSkillParent.GetChild(indexBuff).GetComponent<Image>().color = new Color(152f / 255f, 152f / 255f, 152f / 255f, 1f);
                                    _buffSkillParent.GetChild(indexBuff).GetChild(0).GetComponent<Text>().text = "";
                                }
                            }
                        }
                        else if (SplitDataFromServe._heroSkill.Count == 0)
                        {
                            _buffSkillParent.GetChild(indexBuff).GetComponent<Image>().color = new Color(152f / 255f, 152f / 255f, 152f / 255f, 1f);
                            _buffSkillParent.GetChild(indexBuff).GetChild(0).GetComponent<Text>().text = "";
                        }
                        indexBuff++;
                    }

                }
                else if (skill.data["type"].Value == "active")
                {
                    if (_activeSkillParent.childCount < 8)
                    {
                        GameObject skillObj = Instantiate(Resources.Load("Prefabs/SkillInListPrefab") as GameObject);
                        skillObj.transform.parent = _activeSkillParent;
                        skillObj.transform.localPosition = Vector3.zero;
                        skillObj.GetComponent<RectTransform>().localScale = new Vector3(1f, 1f, 1f);
                        skillObj.transform.GetComponent<Image>().sprite = Resources.Load<Sprite>("Textures/skillAss/" + skill.data["sprite"].Value);
                        if (skillObj.GetComponent<DragHandeler>() == null)
                        {
                            skillObj.AddComponent<DragHandeler>().dataSkill = skill;
                        }
                        if (SplitDataFromServe._heroSkill.Count > 0)
                        {
                            foreach (NewSkill _skill in SplitDataFromServe._heroSkill)
                            {
                                if (_skill.data["idk"].AsInt == skill.data["idInit"].AsInt)
                                {
                                    skillObj.GetComponent<DragHandeler>().dataSkill = _skill;
                                    skillObj.transform.GetComponent<Image>().color = new Color(1f, 1f, 1f, 1f);
                                    skillObj.transform.GetChild(0).GetComponent<Text>().text = _skill.data["level"].AsInt == 10 ? "Max" : "Lvl " + _skill.data["level"].AsInt;
                                    break;
                                }
                                else
                                {
                                    skillObj.transform.GetComponent<Image>().color = new Color(152f / 255f, 152f / 255f, 152f / 255f, 1f);
                                    skillObj.transform.GetChild(0).GetComponent<Text>().text = "";
                                }
                            }
                        }
                        else if (SplitDataFromServe._heroSkill.Count == 0)
                        {
                            skillObj.transform.GetComponent<Image>().color = new Color(152f / 255f, 152f / 255f, 152f / 255f, 1f);
                            skillObj.transform.GetChild(0).GetComponent<Text>().text = "";
                        }
                        skillObj.tag = "SkillInList";
                    }
                    else if (_activeSkillParent.childCount == 8)
                    {
                        _activeSkillParent.GetChild(indexActive).GetComponent<Image>().sprite = Resources.Load<Sprite>("Textures/skillAss/" + skill.data["sprite"].Value);
                        if (_activeSkillParent.GetChild(indexActive).GetComponent<DragHandeler>() != null)
                        {
                            _activeSkillParent.GetChild(indexActive).GetComponent<DragHandeler>().dataSkill = skill;
                        }
                        if (SplitDataFromServe._heroSkill.Count > 0)
                        {
                            foreach (NewSkill _skill in SplitDataFromServe._heroSkill)
                            {
                                if (_skill.data["idInit"].AsInt == skill.data["idInit"].AsInt)
                                {
                                    _activeSkillParent.GetChild(indexActive).GetComponent<DragHandeler>().dataSkill = _skill;
                                    _activeSkillParent.GetChild(indexActive).GetComponent<Image>().color = new Color(1f, 1f, 1f, 1f);
                                    _activeSkillParent.GetChild(indexActive).GetChild(0).GetComponent<Text>().text = _skill.data["level"].AsInt == 10 ? "Max" : "Lvl " + _skill.data["level"].AsInt;
                                    break;
                                }
                                else
                                {
                                    _activeSkillParent.GetChild(indexActive).GetComponent<Image>().color = new Color(152f / 255f, 152f / 255f, 152f / 255f, 1f);
                                    _activeSkillParent.GetChild(indexActive).GetChild(0).GetComponent<Text>().text = "";
                                }
                            }
                        }
                        else if (SplitDataFromServe._heroSkill.Count == 0)
                        {
                            _activeSkillParent.GetChild(indexActive).GetComponent<Image>().color = new Color(152f / 255f, 152f / 255f, 152f / 255f, 1f);
                            _activeSkillParent.GetChild(indexActive).GetChild(0).GetComponent<Text>().text = "";
                        }
                        indexActive++;
                    }
                }
            }
        }
    }

    void CloseThisDialog()
    {
        this.gameObject.SetActive(false);
    }


    bool isLearningSkill = false;
    void LearnNewSkill()
    {
        if (!isLearningSkill)
        {
            isLearningSkill = true;
            bool skillLeard = false;
            foreach (NewSkill _tempSkill in SplitDataFromServe._heroSkill)
            {
                if (_tempSkill.data["idk"].AsInt == _skillDataInfo.data["idInit"].AsInt)
                {
                    skillLeard = true;
                    break;
                }
            }
            if (skillLeard == false)
            {
                int SPRequire = getSPRequiredToLearn(_skillDataInfo);
                if (CharacterInfo._instance._baseProperties.SkillPoint >= SPRequire)
                {
                    StartCoroutine(ServerAdapter.LearnSkill(CharacterInfo._instance._baseProperties.idHero, CharacterInfo._instance._baseProperties.idCodeHero, _skillDataInfo.data["idInit"].AsInt, _skillDataInfo.data["type"].Value, SPRequire, result =>
                     {
                         if (result.StartsWith("Error"))
                         {
                             isLearningSkill = false;
                             Debug.Log("Do nothing");
                             if (ScrollviewDontDestroy.Instance != null)
                                 ScrollviewDontDestroy.Instance.SetLog("Learn skill fail, result return is " + result);
                         }
                         else
                         {
                             CharacterInfo._instance._baseProperties.SkillPoint -= SPRequire;
                             var N = SimpleJSON.JSON.Parse(result.ToString());
                             _skillDataInfo.addField("idhk", N["idhk"].AsInt);
                             _skillDataInfo.addField("idk", N["idk"].AsInt);
                             _skillDataInfo.addField("typewear", N["typewear"].AsInt);
                             _skillDataInfo.addField("level", 1);
                             SplitDataFromServe._heroSkill.Add(_skillDataInfo);
                             ShowInfoSkill(_skillDataInfo);
                             //_skillInfoObj.SetActive(false);
                             ListSkillEachClass(ConverClassCharacterEnumToId(CharacterInfo._instance._baseProperties._classCharacter));
                             isLearningSkill = false;
                             if (ScrollviewDontDestroy.Instance != null)
                                 ScrollviewDontDestroy.Instance.SetLog("Learn skill successfully");
                         }
                     }));
                }
                else
                {
                    PopupErrorController.Instance.ShowErrorWithContent("Don't have enought skillpoint to learn!!");
                    isLearningSkill = false;
                }
            }
            else if (skillLeard == true)
            {
                Debug.Log("Upgrade Skill");
                int SPRequire = getSPRequiredToUpgrade(_skillDataInfo);
                Debug.Log("sp learn skill" + SPRequire);
                if (CharacterInfo._instance._baseProperties.SkillPoint >= SPRequire)
                {
                    StartCoroutine(ServerAdapter.UpLevelSkill(CharacterInfo._instance._baseProperties.idHero, CharacterInfo._instance._baseProperties.idCodeHero, _skillDataInfo.data["idhk"].AsInt, _skillDataInfo.data["level"].AsInt, SPRequire, result =>
                    {
                        if (result.StartsWith("Error"))
                        {
                            Debug.Log("Do nothing");
                            isLearningSkill = false;
                            if (ScrollviewDontDestroy.Instance != null)
                                ScrollviewDontDestroy.Instance.SetLog("Upgrade skill fail, result return is " + result);
                        }
                        else
                        {
                            CharacterInfo._instance._baseProperties.SkillPoint -= SPRequire;
                            var N = SimpleJSON.JSON.Parse(result.ToString());
                            _skillDataInfo.addField("level", _skillDataInfo.data["level"].AsInt + 1);
                            ShowInfoSkill(_skillDataInfo);
                            ListSkillEachClass(ConverClassCharacterEnumToId(CharacterInfo._instance._baseProperties._classCharacter));
                            isLearningSkill = false;
                            if (ScrollviewDontDestroy.Instance != null)
                                ScrollviewDontDestroy.Instance.SetLog("Upgrade skill successfully");
                        }
                    }));
                }
                else
                {
                    PopupErrorController.Instance.ShowErrorWithContent("Don't have enought skillpoint to learn!!");
                    isLearningSkill = false;
                }
            }
        }
    }

    int ConverClassCharacterEnumToId(ClassCharacter _tempClass)
    {
        int id = 1;
        switch (_tempClass)
        {
            case ClassCharacter.Assassin:
                id = 1;
                break;
            case ClassCharacter.Paladin:
                id = 2;
                break;
            case ClassCharacter.Cleric:
                id = 3;
                break;
            case ClassCharacter.Sorceress:
                id = 4;
                break;
            case ClassCharacter.Wizard:
                id = 5;
                break;
            case ClassCharacter.Marksman:
                id = 6;
                break;
            case ClassCharacter.Orc:
                id = 7;
                break;
            case ClassCharacter.Barbarian:
                id = 8;
                break;
        }
        return id;
    }

    string ConvertIdClassToClassName(int id)
    {
        string tempClass = "";
        switch (id)
        {
            case 1:
                tempClass = "Assassin";
                break;
            case 2:
                tempClass = "Paladin";
                break;
            case 3:
                tempClass = "Cleric";
                break;
            case 4:
                tempClass = "Sorceress";
                break;
            case 5:
                tempClass = "Wizard";
                break;
            case 6:
                tempClass = "Marksman";
                break;
            case 7:
                tempClass = "Orc";
                break;
            case 8:
                tempClass = "Barbarian";
                break;
            case 9:
                tempClass = "Ice";
                break;
            case 10:
                tempClass = "Fire";
                break;
            case 11:
                tempClass = "Light";
                break;
            case 12:
                tempClass = "Earth";
                break;

        }
        return tempClass;
    }

    int getLevelRequiredToLearnNextLevel(NewSkill _tempSkill)
    {
        int requireLevel = 0;
        int levelSkill = _tempSkill.data["level"].AsInt;
        if (levelSkill < 10)
        {
            requireLevel = int.Parse(_tempSkill.data["levelrequired"].Value.Split('/')[levelSkill].ToString());
        }
        else
        {
            requireLevel = int.Parse(_tempSkill.data["levelrequired"].Value.Split('/')[9].ToString());
        }
        return requireLevel;
    }

    int getSPRequiredToUpgrade(NewSkill _tempSkill)
    {
        int levelSkill = _tempSkill.data["level"].AsInt;
        int levelRequiredNextLevel = 0;
        if (levelSkill < 10)
        {
            levelRequiredNextLevel = int.Parse(_tempSkill.data["levelrequired"].Value.Split('/')[levelSkill].ToString());
        }
        else if (levelSkill == 10)
        {
            levelRequiredNextLevel = int.Parse(_tempSkill.data["levelrequired"].Value.Split('/')[levelSkill - 1].ToString());
        }
        return Constant.UPGRADE_SKILLPOINT_REQUIRE[levelRequiredNextLevel];
    }
    int getSPRequiredToLearn(NewSkill _tempSkill)
    {
        int levelSkill = 0;
        int levelRequiredNextLevel = 0;
        if (levelSkill < 10)
        {
            levelRequiredNextLevel = int.Parse(_tempSkill.data["levelrequired"].Value.Split('/')[levelSkill].ToString());
        }
        else if (levelSkill == 10)
        {
            levelRequiredNextLevel = int.Parse(_tempSkill.data["levelrequired"].Value.Split('/')[levelSkill - 1].ToString());
        }
        return Constant.UPGRADE_SKILLPOINT_REQUIRE[levelRequiredNextLevel];
    }

}
