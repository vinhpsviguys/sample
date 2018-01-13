using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CoreLib;
using UnityEngine.UI;
using System;
using UnityEngine.EventSystems;
using DG.Tweening;
using Spine.Unity;

public class BattlePVEController : MonoBehaviour {


    public static BattlePVEController Instance;

    Dictionary<GameObject, int> playerSkillDatawithObjectKey = new Dictionary<GameObject, int>();
    Dictionary<string, GameObject> playerSkillDatawithSkillKey = new Dictionary<string, GameObject>();
    Dictionary<NewSkill, GameObject> playerskillwithObjectDictionary = new Dictionary<NewSkill, GameObject>();

    internal CharacterPlayer me;
    internal CharacterPlayer monster;

    NewCharacterStatus attStatus;
    NewCharacterStatus defStatus;
    NewLogic logic;
    Status status;
    bool isMe = true;
    int turn = 0;
    private List<GameObject> _orderButtonActionList = new List<GameObject>();
    RollDiceResult diceResult;

    Action<object> _OnRollingDice, _DoActionOrderListEventRef, _OnCharacterUpdateUIStateEventRef;

    BeginTurnResult beginTurn;
    InTurnResult inResult;
    EndTurnResult endResult;
    private void Awake()
    {
        _OnRollingDice = (param) => RollingDice();
        this.RegisterListener(EventID.OnRollingDice, _OnRollingDice);

        _DoActionOrderListEventRef = (param) => DoActionOrderList();
        this.RegisterListener(EventID.OnCharacterOrderAction, _DoActionOrderListEventRef);

        _OnCharacterUpdateUIStateEventRef = (param) => UpdateUIAfterEachState((string)param);
        this.RegisterListener(EventID.OnCharacterUpdateUIState, _OnCharacterUpdateUIStateEventRef);
    }

    // Use this for initialization
    void Start () {
        status = Status.INIT_GAME;
        isMe = true;
        me = CharacterManager.Instance._meCharacter;
        me.playerId = 1;
        me.loadDictionaries(SplitDataFromServe._heroSkill, SplitDataFromServe._enemySkill, SplitDataFromServe._heroAbs);
        monster = CharacterManager.Instance._enemyCharacter;
        monster.playerId = 2;
        monster.loadDictionaries(SplitDataFromServe._enemySkill, SplitDataFromServe._heroSkill, SplitDataFromServe._heroAbs);

        CreateBattleSceneUI();
        SetupLogicGame();
        CreateMeSkillUI();
        UpdateSkillState();
        AddPassiveSkill();
        status = Status.BEGIN_TURN;
        BeginTurnRender();
    }

    void AddPassiveSkill()
    {
        ArrayList _passiveSkillMe = new ArrayList();
        foreach (NewSkill _tempSkill in SplitDataFromServe._heroSkill)
        {
            if (_tempSkill.data["type"].Value == "passive") _passiveSkillMe.Add(_tempSkill.getID());
        }

        logic.addPassiveSkills(_passiveSkillMe, attStatus);

        ArrayList _passiveSkilMonster = new ArrayList();
        foreach (NewSkill _tempSkill in SplitDataFromServe._enemySkill)
        {
            if (_tempSkill.data["type"].Value == "passive") _passiveSkilMonster.Add(_tempSkill.getID());
        }

        logic.addPassiveSkills(_passiveSkilMonster, defStatus);
    }

    public void CreateBattleSceneUI()
    {


        CharacterManager.Instance._meCharacter.gameObject.transform.position = new Vector3(-4f, -2.5f, 90f);
        BattleSceneUI.Instance._effectParentMe.transform.position = new Vector3(-4f, -2.5f, 90f);
        BattleSceneUI.Instance._meCharacterPos = new Vector3(-4f, -2.5f, 0f);

        CharacterManager.Instance._enemyCharacter.gameObject.transform.position = new Vector3(4f, -2.5f, 90f);
        BattleSceneUI.Instance._effectParentEnemy.transform.position = new Vector3(4f, -2.5f, 90f);
        BattleSceneUI.Instance._enemyCharacterPos = new Vector3(4f, -2.5f, 90f);

        BattleSystemManager.Instance.CreateBattleInfo();
        BattleSceneUI.Instance.TurnOfPlayerName.gameObject.SetActive(false);
        UpdatePropertiesCharacterUI();


        BattleSceneUI.Instance.DiceBtn.interactable = false;
        BattleSceneUI.Instance._startOrderAction.interactable = false;
    }

    public void UpdatePropertiesCharacterUI()
    {

        BattleSceneUI.Instance._meCharacterName.text = me._baseProperties.name.ToString();
        BattleSceneUI.Instance.UpdateDisplayBar(BattleSceneUI.Instance.heathBarPlayer, me.characteristic.Health, me.characteristic.Max_Health);
        if (me.characteristic.Health > 0)
        {
            BattleSceneUI.Instance._meCharacterHP.text = me.characteristic.Health.ToString();
        }
        else if (me.characteristic.Health <= 0)
        {
            BattleSceneUI.Instance._meCharacterHP.text = "0";
        }
        BattleSceneUI.Instance.UpdateDisplayBar(BattleSceneUI.Instance.actionPointBarPlayer, CharacterManager.Instance._meCharacter._actionPoints, 18f);
        BattleSceneUI.Instance._meCharacterActionPoint.text = "Action Point: " + me._actionPoints.ToString();
        BattleSceneUI.Instance._meLevel.text = "Level: " + me._baseProperties.Level.ToString();
        BattleSceneUI.Instance.avatarMe.sprite = CharacterItemInGame.Instance._avatarCircle[(int)(CharacterManager.Instance._meCharacter._baseProperties._classCharacter)];

        BattleSceneUI.Instance._enemyCharacterName.text = CharacterManager.Instance._enemyCharacter._baseProperties.name.ToString();
        BattleSceneUI.Instance.UpdateDisplayBar(BattleSceneUI.Instance.heathBarEnemy, monster.characteristic.Health, monster.characteristic.Max_Health);
        if (monster.characteristic.Health > 0)
        {
            BattleSceneUI.Instance._enemyCharacterHP.text = monster.characteristic.Health.ToString();
        }
        else if (monster.characteristic.Health <= 0)
        {
            BattleSceneUI.Instance._enemyCharacterHP.text = "0";
        }
        BattleSceneUI.Instance.UpdateDisplayBar(BattleSceneUI.Instance.actionPointBarEnemy, CharacterManager.Instance._enemyCharacter._actionPoints, 18f);
        BattleSceneUI.Instance._enemyCharacterActionPoint.text = "Action Point: " + monster._actionPoints.ToString();
        BattleSceneUI.Instance._enemyLevel.text = "Level: " + monster._baseProperties.Level.ToString();
        //BattleSceneUI.Instance.avatarEnemy.sprite = CharacterItemInGame.Instance._avatarCircle[(int)(CharacterManager.Instance._enemyCharacter._baseProperties._classCharacter)];

        //UpdateSkillState();
    }

    void SetupLogicGame()
    {
        Debug.Log("setup logic game");
        logic = new NewLogic(me, monster);
        attStatus = logic.getStatusByPlayerID(1);
        attStatus.setConditionManager(new ConditionManager(attStatus, this.logic));
        defStatus = logic.getStatusByPlayerID(2);
        defStatus.setConditionManager(new ConditionManager(defStatus, this.logic));

        Debug.Log("Start");

    }


    public void CreateMeSkillUI()
    {
        Debug.Log("create me skill ui");
        BattleSceneUI.Instance.NumberSkill.text = "Number skill " + SplitDataFromServe._heroSkill.Count.ToString();
        int index = 0;
        int indexPassive = 0;
        foreach (NewSkill skill in SplitDataFromServe._heroSkill)
        {

            if (skill.data["typewear"].AsInt == 1 && skill.data["type"].Value != "passive")
            {
                Debug.Log(skill.data["idhk"]);
                GameObject skillObj = Instantiate(Resources.Load("Prefabs/SkillCharacter") as GameObject);
                if (PlayerPrefs.HasKey(skill.data["idhk"]))
                {
                    skillObj.transform.parent = BattleSceneUI.Instance._panelSkill.transform.GetChild(PlayerPrefs.GetInt(skill.data["idhk"])).transform;
                }
                else
                {
                    skillObj.transform.parent = BattleSceneUI.Instance._panelSkill.transform.GetChild(index).transform;
                }
                skillObj.GetComponent<RectTransform>().localPosition = Vector3.zero;
                skillObj.GetComponent<RectTransform>().localScale = new Vector3(1f, 1f, 1f);
                skillObj.transform.GetComponent<Image>().sprite = Resources.Load<Sprite>("Textures/skillAss/" + skill.data["sprite"]);
                skillObj.transform.GetChild(1).GetComponent<Text>().text = skill.data["idInit"].ToString();
                skillObj.transform.GetChild(2).GetChild(0).GetComponent<Text>().text = skill.data["aps"].ToString();
                playerSkillDatawithObjectKey.Add(skillObj, skill.data["aps"].AsInt);
                playerSkillDatawithSkillKey.Add("Skill" + skill.data["idInit"].AsInt.ToString(), skillObj);
                playerskillwithObjectDictionary.Add(skill, skillObj);
                skillObj.GetComponent<Button>().onClick.AddListener(() => ChooseAction(skill, skillObj.transform));
                if (PlayerPrefs.HasKey(skill.data["idhk"]))
                {
                    OnHoverToUI(skillObj, skill, new Vector3(-600 + PlayerPrefs.GetInt(skill.data["idhk"]) * 150, -200, 0));
                }
                else
                {
                    OnHoverToUI(skillObj, skill, new Vector3(-600 + index * 150, -200, 0));
                }
                index++;
            }
            else if (skill.data["type"].Value == "passive")
            {
                GameObject skillObj = Instantiate(Resources.Load("Prefabs/Effect") as GameObject);
                skillObj.transform.parent = BattleSceneUI.Instance.ListSkillPassive.transform;
                skillObj.GetComponent<RectTransform>().localScale = new Vector3(1f, 1f, 1f);
                skillObj.transform.GetComponent<Image>().sprite = Resources.Load<Sprite>("Textures/skillAss/" + skill.data["sprite"]);
                skillObj.transform.GetChild(0).gameObject.SetActive(false);
                OnHoverToUI(skillObj, skill, new Vector3(-600 + indexPassive * 100, 180, 0));
                indexPassive++;
            }
        }

    }

    public void OnHoverToUI(GameObject skillObj, NewSkill skill, Vector3 pos)
    {

        // Event when hover mouse to button
        EventTrigger trigger = skillObj.AddComponent<EventTrigger>();

        EventTrigger.Entry pointerEnter = new EventTrigger.Entry();
        pointerEnter.eventID = EventTriggerType.PointerEnter;

        // create the listener and attach to callback
        EventTrigger.TriggerEvent pointerEnterEvent = new EventTrigger.TriggerEvent();
        string name = skill.data["name"].Value;
        string value = getValueSkill(skill.data["value"].Value, skill.data["level"].AsInt);
        pointerEnterEvent.AddListener((x) =>
        {
            BattleSceneUI.Instance.ShowDialog(name, value, pos);
        });

        pointerEnter.callback = pointerEnterEvent;

        EventTrigger.Entry pointerExit = new EventTrigger.Entry();
        pointerExit.eventID = EventTriggerType.PointerExit;

        // create the listener and attach to callback
        EventTrigger.TriggerEvent pointerExitEvent = new EventTrigger.TriggerEvent();
        pointerExitEvent.AddListener((x) =>
        {
            BattleSceneUI.Instance.HideDialog();
        });

        pointerExit.callback = pointerExitEvent;

        // add this to delegates list, ensure delegates is not null
        if (trigger.triggers == null)
        {
            trigger.triggers = new List<EventTrigger.Entry>();
        }

        trigger.triggers.Add(pointerEnter);
        trigger.triggers.Add(pointerExit);
    }
    string getValueSkill(string data, int currentLevel)
    {
        string _valueContentCurrentLevel = "";
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
                }
            }
        }
        return _valueContentCurrentLevel;
    }

    public void ChooseAction(NewSkill skill, Transform _thisButton)
    {
        Debug.Log("choose skill");
        if (status == Status.IN_TURN && isMe && logic.getStatusByPlayerID(1).character.newSkillDic["Skill" + skill.data["idInit"].ToString()].getCoolDown() == 0 && logic.getStatusByPlayerID(1).character.newSkillDic["Skill" + skill.data["idInit"].ToString()].canCastSkill(logic))
        {
            
                if (!me._listAction.Contains(CharacterItemInGame.Instance.LoadActionHandle(me._listAction, skill.data["idInit"].AsInt)))
                {
                    ActionHandle _action = new ActionHandle(2, skill.data["idInit"].AsInt, 2f, me._listAction.Count + 1);
                    me._listAction.Add(_action);
                    _orderButtonActionList.Add(_thisButton.gameObject);
                    _thisButton.GetChild(1).gameObject.SetActive(true);
                    //_thisButton.GetChild(1).GetComponent<Text>().text = (_action.index).ToString();
                    _thisButton.DOLocalMoveY(150f, 0.1f);
                    me._actionPoints -= skill.data["aps"].AsInt;
                    BattleSceneUI.Instance.UpdateDisplayBar(BattleSceneUI.Instance.actionPointBarPlayer, me._actionPoints, 18f);
                    BattleSceneUI.Instance._meCharacterActionPoint.text = "Action Point: " + me._actionPoints.ToString();
                }
                else
                {
                    int indexActRemove = CharacterItemInGame.Instance.LoadActionHandle(me._listAction, skill.data["idInit"].AsInt).index;
                    CharacterManager.Instance._meCharacter._listAction.Remove(CharacterItemInGame.Instance.LoadActionHandle(me._listAction, skill.data["idInit"].AsInt));
                    _orderButtonActionList.RemoveAt(indexActRemove - 1);
                    foreach (ActionHandle _act in me._listAction)
                    {
                        if (_act.index > indexActRemove)
                        {
                            _act.index--;
                        }
                    }
                    //for (int i = 0; i < _orderButtonActionList.Count; i++)
                    //{
                    //    _orderButtonActionList[i].transform.GetChild(1).GetComponent<Text>().text = me._listAction[i].index.ToString();
                    //}
                    _thisButton.GetChild(1).gameObject.SetActive(true);
                    _thisButton.DOLocalMoveY(0f, 0.1f);
                    me._actionPoints += skill.data["aps"].AsInt;
                    BattleSceneUI.Instance.UpdateDisplayBar(BattleSceneUI.Instance.actionPointBarPlayer, CharacterManager.Instance._meCharacter._actionPoints, 18f);
                    BattleSceneUI.Instance._meCharacterActionPoint.text = "Action Point: " + me._actionPoints.ToString();
                }
            UpdateSkillState();
        }
    }
    public void UpdateSkillState()
    {
        int actionPointMe = 0;

        actionPointMe = me._actionPoints;
        foreach (Transform child in BattleSceneUI.Instance._panelSkill.transform)
        {
            try
            {
                if (playerSkillDatawithObjectKey.ContainsKey(child.GetChild(0).gameObject))
                {
                   
                    child.GetChild(0).GetChild(1).gameObject.SetActive(true);
                    if (playerSkillDatawithObjectKey[child.GetChild(0).gameObject] <= actionPointMe)
                    {
                        child.GetChild(0).GetComponent<Button>().interactable = true;

                    }
                    else if (playerSkillDatawithObjectKey[child.GetChild(0).gameObject] > actionPointMe && !_orderButtonActionList.Contains(child.GetChild(0).gameObject))
                    {
                        child.GetChild(0).GetComponent<Button>().interactable = false;

                    }
                }
            }
            catch (Exception e) { };
        }


    }

    void ResetUISkill()
    {
        foreach (GameObject child in _orderButtonActionList)
        {
            child.transform.DOLocalMoveY(0f, 0.1f);

        }
    }

    public void UpdateActionPointOfSkillUI()
    {
        foreach (NewSkill tempSkill in playerskillwithObjectDictionary.Keys)
        {
            playerskillwithObjectDictionary[tempSkill].transform.GetChild(2).GetChild(0).GetComponent<Text>().text = tempSkill.getActionPoints().ToString();
        }
    }



    // Update is called once per frame
    void Update () {
		
	}


    void BeginTurnRender()
    {
        if (isMe)
        {
            turn++;
        }
        ResetUISkill();
        UpdateSkillState();
        UpdateActionPointOfSkillUI();
        beginTurn = logic.beginTurn(attStatus, defStatus);
        bool continued = beginTurn.continued;
        ArrayList states = beginTurn.states;
        UpdateUICharacter(states);
        UpdateUIOnTurnBegin();
        status = Status.ROLL_DICE;
        if (!isMe)
        {
            RollingDice();
        }
    }


    public void RollingDice()
    {
        System.Random random = new System.Random();
        diceResult = new RollDiceResult(random.Next(6) + 1, random.Next(6) + 1, random.Next(6) + 1);
        if (isMe)
        {
            me._actionPoints = diceResult.dice1 + diceResult.dice2 + diceResult.dice3;
        }
        else if (!isMe)
        {
            monster._actionPoints = diceResult.dice1 + diceResult.dice2 + diceResult.dice3;
        }
    
        StartCoroutine(ExecuteDisplayDicePoint(diceResult.dice1, diceResult.dice2, diceResult.dice3));
    }


    public IEnumerator ExecuteDisplayDicePoint(int _dicePoint1, int _dicePoint2, int _dicePoint3)
    {

        BattleSceneUI.Instance.DiceDialog.SetActive(true);
        BattleSceneUI.Instance._dice1.gameObject.SetActive(true);
        BattleSceneUI.Instance._dice1.GetComponent<SkeletonAnimation>().AnimationName = _dicePoint1.ToString();

        yield return new WaitForSeconds(1f);

        BattleSceneUI.Instance._dice2.gameObject.SetActive(true);
        BattleSceneUI.Instance._dice2.GetComponent<SkeletonAnimation>().AnimationName = _dicePoint2.ToString();


        yield return new WaitForSeconds(1f);

        BattleSceneUI.Instance._dice3.gameObject.SetActive(true);
        BattleSceneUI.Instance._dice3.GetComponent<SkeletonAnimation>().AnimationName = _dicePoint3.ToString();

        yield return new WaitForSeconds(1f);
        BattleSceneUI.Instance.InfoDiceDialog.gameObject.SetActive(true);
        int _totalPoint = _dicePoint1 + _dicePoint2 + _dicePoint3;
        BattleSceneUI.Instance.InfoDiceDialog.text = "You have got " + _totalPoint + " action points";
        //BattleSceneUI.Instance.InfoDiceDialog.text = "Your enemy have got " + _totalPoint + " action points";
        yield return new WaitForSeconds(0.5f);
        BattleSceneUI.Instance.DiceDialog.SetActive(false);
        BattleSceneUI.Instance._dice1.GetComponent<SkeletonAnimation>().AnimationName = "idle";
        BattleSceneUI.Instance._dice2.GetComponent<SkeletonAnimation>().AnimationName = "idle";
        BattleSceneUI.Instance._dice3.GetComponent<SkeletonAnimation>().AnimationName = "idle";
        BattleSceneUI.Instance._dice1.gameObject.SetActive(false);
        BattleSceneUI.Instance._dice2.gameObject.SetActive(false);
        BattleSceneUI.Instance._dice3.gameObject.SetActive(false);
        if (isMe)
        {
            BattleSceneUI.Instance.UpdateDisplayBar(BattleSceneUI.Instance.actionPointBarPlayer, me._actionPoints, 18f);
            BattleSceneUI.Instance._meCharacterActionPoint.text = "Action Point: " + me._actionPoints.ToString();
        }
        else
        {
            BattleSceneUI.Instance.UpdateDisplayBar(BattleSceneUI.Instance.actionPointBarEnemy, monster._actionPoints, 18f);
            BattleSceneUI.Instance._enemyCharacterActionPoint.text = "Action Point: " + monster._actionPoints.ToString();
        }
        _orderButtonActionList.Clear();
        UpdateSkillState();
       
        
        if (isMe)
        {
            BattleSceneUI.Instance._startOrderAction.interactable = true;
        } else
        {
            BattleSceneUI.Instance._startOrderAction.interactable = false;
            DoActionOrderListByAI();
        }

    }

    void DoActionOrderListByAI()
    {
        status = Status.IN_TURN;
        ArrayList choices = new ArrayList();
        MyDictionary<string, NewSkill> dicts = attStatus.character.newSkillDicOfCharacter;
        Debug.Log(dicts.Keys.Count);
        if (dicts.Keys.Count > 0)
        {
            foreach (string skill in dicts.Keys)
            {
                NewSkill act = dicts[skill];
                if (act.canCastSkill(logic))
                    choices.Add(dicts[skill].getID());
            }
        }
        RenderDataBattleState(choices);
    }

    public void DoActionOrderList()
    {
        BattleSceneUI.Instance._startOrderAction.interactable = false;
        Debug.Log("DO ACTION LIST !!");
        ArrayList skills = new ArrayList();
        foreach (ActionHandle _tempAct in me._listAction)
        {
            skills.Add(_tempAct.idSkill);
        }
      
        me._listAction.Clear();
        foreach (GameObject child in _orderButtonActionList)
        {
            child.transform.DOLocalMoveY(0f, 0.1f);

        }
        BattleSceneUI.Instance.isOrderAction = false;
        status = Status.IN_TURN;
        RenderDataBattleState(skills);
    }

    public void RenderDataBattleState(ArrayList skills)
    {
        foreach (int skill in skills)
        {// su dung
            attStatus.character.newSkillDic["Skill" + skill].use();
        }

        ArrayList actions = new ArrayList();
        foreach (int skill in skills)
        {

            ActionHandle a = new ActionHandle(0, skill, 0, 0);
            actions.Add(a);
        }
       inResult = new InTurnResult(logic.inTurn(actions));
        StartCoroutine(ExecuteDataBatle(inResult));
    }


    IEnumerator ExecuteDataBatle(InTurnResult inResult)
    {
        int index = 0;
        foreach (State state in inResult.states)
        {
            Debug.Log("run anim");
            Debug.Log(state.toJSON().ToString());
            var N = state.toJSON();
            if (N["attacker"].AsInt==1)
            {
                if (N["idSkill"].AsInt != 0)
                {
                    NewSkill _tempSkill = SplitDataFromServe.getSkill(N["idSkill"].AsInt);
                    if (_tempSkill != null)
                    {
                        if (N["attacker"].AsInt != N["defender"].AsInt)
                        {
                            if (_tempSkill.data["isMove"].AsBool == false)
                            {
                               
                                    CharacterManager.Instance._meCharacter.GetComponent<AnimationController>().UseSkill(CharacterManager.Instance._meCharacter, CharacterManager.Instance._enemyCharacter, N["attacker"].AsInt, index + 1, N["hpPlayer2"].AsInt, _tempSkill);
                             
                            }
                            else if (_tempSkill.data["isMove"].AsBool == true)
                            {
                                if (CharacterManager.Instance._meCharacter._baseProperties.typeAttack == 0)
                                {
                                  
                                        CharacterManager.Instance._meCharacter.GetComponent<AnimationController>().MeeleAttack(CharacterManager.Instance._meCharacter, CharacterManager.Instance._enemyCharacter, N["attacker"].AsInt, index + 1, N["hpPlayer2"].AsInt, false, _tempSkill);
                                 
                                }
                            }
                        }
                        else
                        {
                            CharacterManager.Instance._meCharacter.GetComponent<AnimationController>().Donothing(CharacterManager.Instance._meCharacter, CharacterManager.Instance._meCharacter, N["attacker"].AsInt, index + 1, N["hpPlayer1"].AsInt, _tempSkill);
                        }
                    }
                }
                else if (N["idSkill"].AsInt == 0)
                {
                    if (N["attacker"].AsInt != N["defender"].AsInt)
                    {
                        if (me._baseProperties.typeAttack == 0)
                        {
                           
                                CharacterManager.Instance._meCharacter.GetComponent<AnimationController>().MeeleAttack(CharacterManager.Instance._meCharacter, CharacterManager.Instance._enemyCharacter, N["attacker"].AsInt, index + 1, N["hpPlayer2"].AsInt, true, null);
                           
                        }
                        else if (me._baseProperties.typeAttack == 1)
                        {
                          
                                CharacterManager.Instance._meCharacter.GetComponent<AnimationController>().RangeAttack(CharacterManager.Instance._meCharacter, CharacterManager.Instance._enemyCharacter, N["attacker"].AsInt, index + 1, N["hpPlayer2"].AsInt, true, null);
                         
                        }
                    }
                    else
                    {
                        CharacterManager.Instance._meCharacter.GetComponent<AnimationController>().Donothing(CharacterManager.Instance._meCharacter, CharacterManager.Instance._meCharacter, N["attacker"].AsInt, index + 1, N["hpPlayer1"].AsInt, null);
                    }
                }
            }
            else if (N["attacker"].AsInt==2)
            {
                if (N["idSkill"].AsInt != 0)
                {
                    NewSkill _tempSkill = SplitDataFromServe.getSkill(N["idSkill"].AsInt);
                    if (_tempSkill != null)
                    {
                        if (N["attacker"].AsInt != N["defender"].AsInt)
                        {
                            if (_tempSkill.data["isMove"].AsBool == false)
                            {
                              
                                    CharacterManager.Instance._enemyCharacter.GetComponent<AnimationController>().UseSkill(CharacterManager.Instance._meCharacter, CharacterManager.Instance._enemyCharacter, N["attacker"].AsInt, index + 1, N["hpPlayer1"].AsInt, _tempSkill);
                            }
                            else if (_tempSkill.data["isMove"].AsBool == true)
                            {
                                if (monster._baseProperties.typeAttack == 0)
                                {
                                  
                                        CharacterManager.Instance._enemyCharacter.GetComponent<AnimationController>().MeeleAttack(CharacterManager.Instance._enemyCharacter, CharacterManager.Instance._meCharacter, N["attacker"].AsInt, index + 1, N["hpPlayer1"].AsInt, false, _tempSkill);
                                }
                            }
                        }
                        else
                        {
                            CharacterManager.Instance._enemyCharacter.GetComponent<AnimationController>().Donothing(CharacterManager.Instance._enemyCharacter, CharacterManager.Instance._enemyCharacter, N["attacker"].AsInt, index + 1, N["hpPlayer2"].AsInt, null);
                        }
                    }
                }
                else if (N["idSkill"].AsInt == 0)
                {
                    Debug.Log("attack "+ N["attacker"].AsInt + " " + N["defender"].AsInt);
                    if (N["attacker"].AsInt != N["defender"].AsInt)
                    {
                        Debug.Log("type atk "+ monster._baseProperties.typeAttack);
                        if (monster._baseProperties.typeAttack == 0)
                        {
                            Debug.Log("melee atk");
                                CharacterManager.Instance._enemyCharacter.GetComponent<AnimationController>().MeeleAttack(CharacterManager.Instance._enemyCharacter, CharacterManager.Instance._meCharacter, N["attacker"].AsInt, index + 1, N["hpPlayer1"].AsInt, true, null);
                        }
                        else if (monster._baseProperties.typeAttack == 1)
                        {
                            Debug.Log("range atk");
                            CharacterManager.Instance._enemyCharacter.GetComponent<AnimationController>().RangeAttack(CharacterManager.Instance._enemyCharacter, CharacterManager.Instance._meCharacter, N["attacker"].AsInt, index + 1, N["hpPlayer1"].AsInt, true, null);
                        }
                    }
                    else
                    {
                        Debug.Log("do nothing");
                        CharacterManager.Instance._enemyCharacter.GetComponent<AnimationController>().Donothing(CharacterManager.Instance._enemyCharacter, CharacterManager.Instance._enemyCharacter, N["attacker"].AsInt, index + 1, N["hpPlayer2"].AsInt, null);
                    }
                }
            }
            yield return new WaitForSeconds(1.5f);
            index++;
        }

        if (index == inResult.states.Count)
        {
            status = Status.END_TURN;
            EndTurnRender();
        }
    }


    void EndTurnRender()
    {
        endResult = logic.endTurn(attStatus, defStatus);

        NewCharacterStatus temp = attStatus;
        attStatus = defStatus;
        defStatus = temp;

        NewCharacterStatus one = logic.getStatusByPlayerID(1);
        NewCharacterStatus two = logic.getStatusByPlayerID(2);
        foreach (string key in one.character.newSkillDic.Keys)
        {
            one.character.newSkillDic[key].decreaseCoolDown();
        }
        foreach (string key in two.character.newSkillDic.Keys)
        {
            two.character.newSkillDic[key].decreaseCoolDown();
        }
        // phai cap nhat coolDown khi doi phuong va minh chon skill
        UpdateSkillCooldownOnCharacter();
        if (endResult != null)
            ReleaseEffectExpire(endResult.releasedState);
         
        if (endResult.combatResult > 0)
        {
            //BattleSceneController.Instance.ShowGameOverPanel(endResult.combatResult);
        }
        else
        {
            if (isMe)
            {
                isMe = false;
            }
            else
            {
                isMe = true;
            }
            beginTurn = null;
            status = Status.BEGIN_TURN;
            BeginTurnRender();
        }
    }

    public void UpdateSkillCooldownOnCharacter()
    {
        NewCharacterStatus meStatus = logic.getStatusByPlayerID(1);
        foreach (string key in playerSkillDatawithSkillKey.Keys)
        {
            playerSkillDatawithSkillKey[key].transform.GetChild(0).GetComponent<Image>().fillAmount = meStatus.character.newSkillDic[key].getCoolDown() * 1f / meStatus.character.newSkillDic[key].getCoolDownLimit();

        }

    }

    internal void ReleaseEffectExpire(ArrayList states)
    {
        foreach (NewEffect state in states)
        {
            var N = SimpleJSON.JSON.Parse(state.toJSON().ToString());
            if (N["playerID"].AsInt == 1)
            {
                DestroyEffectObj(1, N["name"].Value.ToString());
            }
            else if (N["playerID"].AsInt == 2)
            {
                DestroyEffectObj(2, N["name"].Value.ToString());
            }
            else if (N["playerID"].AsInt == 3)
            {
                DestroyEffectObj(1, N["name"].Value.ToString());
                DestroyEffectObj(2, N["name"].Value.ToString());
            }
        }
    }

    void DestroyEffectObj(int playerId, string nameEff)
    {
        if (me.playerId == playerId)
        {
            foreach (Transform child in BattleSceneUI.Instance.EffectPossitiveSkillMe.transform)
            {
                if (child.name == nameEff)
                {
                    Destroy(child.gameObject);
                }
            }
            foreach (Transform child in BattleSceneUI.Instance.EffectNegativeSkillMe.transform)
            {
                if (child.name == nameEff)
                {
                    Destroy(child.gameObject);
                }
            }
            DestroyAbnormalStatusEffectOnCharacter(nameEff, false);

        }
        else if (monster.playerId == playerId)
        {
            foreach (Transform child in BattleSceneUI.Instance.EffectPossitiveSkillEnemy.transform)
            {
                if (child.name == nameEff)
                {
                    Destroy(child.gameObject);
                }
            }
            foreach (Transform child in BattleSceneUI.Instance.EffectNegativeSkillEnemy.transform)
            {
                if (child.name == nameEff)
                {
                    Destroy(child.gameObject);
                }
            }
            DestroyAbnormalStatusEffectOnCharacter(nameEff, true);
        }
    }

    void DestroyAbnormalStatusEffectOnCharacter(string nameEff, bool isEnemy)
    {
        if (isEnemy)
        {
            if (BattleSceneUI.Instance._effectParentEnemy.transform.Find(nameEff + "enemy") != null)
            {
                Destroy(BattleSceneUI.Instance._effectParentEnemy.transform.Find(nameEff + "enemy").gameObject);
            }
        }
        else
        {
            if (BattleSceneUI.Instance._effectParentMe.transform.Find(nameEff + "me") != null)
            {
                Destroy(BattleSceneUI.Instance._effectParentMe.transform.Find(nameEff + "me").gameObject);
            }
        }
    }

    public void UpdateUIOnTurnBegin()
    {
        BattleSystemManager.Instance.turn++;
        BattleSceneUI.Instance.TypeOfMatch.gameObject.SetActive(true);
        BattleSceneUI.Instance.TypeOfMatch.text = "Turn " + (turn.ToString());
 
            if (isMe)
            {
                BattleSceneUI.Instance.DiceBtn.interactable = true;
                BattleSceneUI.Instance.TurnOfPlayerName.gameObject.SetActive(true);
                BattleSceneUI.Instance.TurnOfPlayerName.text = "Your turn";
            }
            else if (!isMe)
            {
                BattleSceneUI.Instance.DiceBtn.interactable = false;
                BattleSceneUI.Instance.TurnOfPlayerName.gameObject.SetActive(true);
                BattleSceneUI.Instance.TurnOfPlayerName.text = "Enemy turn";
            }
     
    }



    public void UpdateUICharacter(ArrayList states)
    {

        foreach (State state in states)
        {
            // initIndexes la cac chi so cua nhan vat khong mac equipment, cac chi so tinh(khong co min max) cua equipment va cac chi so duration cua abs
            // midIndexes la cac chi so bao gom initIndexes kem theo cac chi so dong (co min max) cua equipment va cac chi so phu phat sinh trong qua trinh tinh toan cong thuc
            // curIndexes la chi so chi dung de doc tat ca cac thong so
            me._actionPoints = 0;
            monster._actionPoints = 0;
            BattleSceneUI.Instance.UpdateDisplayBar(BattleSceneUI.Instance.actionPointBarPlayer, me._actionPoints, 18f);
            BattleSceneUI.Instance._meCharacterActionPoint.text = "Action Point: " + me._actionPoints.ToString();
            BattleSceneUI.Instance.UpdateDisplayBar(BattleSceneUI.Instance.actionPointBarEnemy, monster._actionPoints, 18f);
            BattleSceneUI.Instance._enemyCharacterActionPoint.text = "Action Point: " + monster._actionPoints.ToString();

            var N = SimpleJSON.JSON.Parse(state.toJSON().ToString());
            int damagePlayer1 = 0, damagePlayer2 = 0;
            if (me.playerId == 1)
            {
                damagePlayer1 = (int)(N["hpPlayer1"].AsFloat - me._baseProperties.hp);
                if (damagePlayer1 > 0)
                {
                    UpdateDameEffect(damagePlayer1, 1);
                }
                else if (damagePlayer1 < 0)
                {
                    UpdateDameEffect(damagePlayer1, 1);
                }
                damagePlayer2 = (int)(N["hpPlayer2"].AsFloat - monster._baseProperties.hp);

                if (damagePlayer2 > 0)
                {
                    UpdateDameEffect(damagePlayer2, 2);
                }
                else if (damagePlayer2 < 0)
                {
                    UpdateDameEffect(damagePlayer2, 2);
                }

                me._baseProperties.hp = N["hpPlayer1"].AsFloat;
                monster._baseProperties.hp = N["hpPlayer2"].AsFloat;
                if (me._baseProperties.hp <= 0)
                {
                    me._baseProperties.hp = 0;
                }

                if (monster._baseProperties.hp <= 0)
                {
                    monster._baseProperties.hp = 0;
                }

                if (isMe)
                {

                    BattleSceneUI.Instance.UpdateDisplayBar(BattleSceneUI.Instance.heathBarPlayer, N["hpPlayer1"].AsFloat, Mathf.Round((float)logic.attStatus.getCurrentIndex(Indexes.max_hp_na)));
                    BattleSceneUI.Instance._meCharacterHP.text = ((int)me._baseProperties.hp).ToString() + "/" + logic.attStatus.getCurrentIndex(Indexes.max_hp_na).ToString();
                    BattleSceneUI.Instance.UpdateDisplayBar(BattleSceneUI.Instance.heathBarEnemy, N["hpPlayer2"].AsFloat, Mathf.Round((float)logic.defStatus.getCurrentIndex(Indexes.max_hp_na)));

                    BattleSceneUI.Instance._enemyCharacterHP.text = ((int)monster._baseProperties.hp).ToString() + "/" + logic.defStatus.getCurrentIndex(Indexes.max_hp_na).ToString();
                }
                else if (!isMe)
                {

                    BattleSceneUI.Instance.UpdateDisplayBar(BattleSceneUI.Instance.heathBarPlayer, N["hpPlayer1"].AsFloat, Mathf.Round((float)logic.defStatus.getCurrentIndex(Indexes.max_hp_na)));
                    BattleSceneUI.Instance._meCharacterHP.text = ((int)me._baseProperties.hp).ToString() + "/" + logic.defStatus.getCurrentIndex(Indexes.max_hp_na).ToString();
                    BattleSceneUI.Instance.UpdateDisplayBar(BattleSceneUI.Instance.heathBarEnemy, N["hpPlayer2"].AsFloat, Mathf.Round((float)logic.attStatus.getCurrentIndex(Indexes.max_hp_na)));

                    BattleSceneUI.Instance._enemyCharacterHP.text = ((int)monster._baseProperties.hp).ToString() + "/" + logic.attStatus.getCurrentIndex(Indexes.max_hp_na).ToString();
                }
            }
            
            int numberEff = N["effects"].Count;
            for (int i = 0; i < numberEff; i++)
            {
                Debug.Log("render eff" + numberEff);
                UpdateEffectUIOnCharacter(N["effects"][i]["name"].Value, N["effects"][i]["duration"].AsInt, N["effects"][i]["playerID"].AsInt);
            }
        }

    }

    void UpdateEffectUIOnCharacter(string _name, int _duration, int _playerId)
    {
        if (_playerId == 3)
        {
            UpdateEffectByIdPlayer(_name, _duration, 1);
            UpdateEffectByIdPlayer(_name, _duration, 2);
        }
        else if (_playerId == 2)
        {
            UpdateEffectByIdPlayer(_name, _duration, 2);
        }
        else if (_playerId == 1)
        {
            UpdateEffectByIdPlayer(_name, _duration, 1);
        }
    }

    void UpdateEffectByIdPlayer(string _name, int _duration, int _playerId)
    {
        Debug.Log("name " + _name);
        NewCharacterStatus _temp = logic.getStatusByPlayerID(_playerId);
        if (_temp.effects.ContainsKey(_name) || _temp.op_effects.ContainsKey(_name))
        {
            NewEffect _tempEff = _temp.effects.ContainsKey(_name) ? _temp.effects[_name] : _temp.op_effects[_name];
            bool isOpEff = _temp.effects.ContainsKey(_name) ? false : true;
            string nick = _tempEff.nick;
            Debug.Log("nick " + nick);
            if (nick.Contains("Skill"))
            {
                int idInitSkill = int.Parse(nick.Replace("Skill", ""));
                Debug.Log("id skill la " + idInitSkill);
                foreach (NewSkill _tempSkill in SplitDataFromServe.skillInit.Values)
                {

                    if (idInitSkill == _tempSkill.data["idInit"].AsInt)
                    {

                        string _spriteName = _tempSkill.data["sprite"].Value.ToString();
                        UpdateEffectStaticCharacter(_name, _spriteName, _duration, _playerId, true, isOpEff);
                    }
                }
            }
            else if (nick.Contains("AS"))
            {
                int idInitAbs = int.Parse(nick.Replace("AS", ""));
                foreach (AbnormalStatus _tempAbs in SplitDataFromServe.absInit.Values)
                {

                    if (idInitAbs == _tempAbs.data["idInit"].AsInt)
                    {

                        string _spriteName = _tempAbs.data["sprite"].Value.ToString();
                        UpdateEffectStaticCharacter(_name, _spriteName, _duration, _playerId, false, isOpEff);
                    }
                }
            }
        }
        else
        {
            UpdateEffectDynamic(_name, _playerId);
        }
    }

    void UpdateEffectDynamic(string name, int playerid)
    {
        Debug.Log(name);
        if (name != "HealthChanges")
        {
            GameObject damageText = Instantiate(Resources.Load("Prefabs/BattleStateText") as GameObject);
            damageText.GetComponent<Text>().text = "" + name.ToString();
            damageText.transform.parent = GameObject.Find("UI").transform;
            damageText.GetComponent<RectTransform>().localScale = new Vector3(1f, 1f, 1f);
            if (playerid == 1)
            {
                damageText.transform.position = new Vector3(me.transform.position.x, me.transform.position.y + me.transform.GetComponent<BoxCollider2D>().bounds.size.y, me.transform.position.z);
            }
            else if (playerid == 2)
            {
                damageText.transform.position = new Vector3(monster.transform.position.x, monster.transform.position.y + monster.transform.GetComponent<BoxCollider2D>().bounds.size.y, monster.transform.position.z);
            }
        }
        else
        {
            if (me.playerId == playerid)
            {
                GameObject damagedEffect = EffectManager.Instance.CreateEffect("Effect/ParticlesHeal", me.transform);
                damagedEffect.transform.position = new Vector3(me.transform.position.x, me.transform.position.y + me.transform.gameObject.transform.GetComponent<BoxCollider2D>().bounds.size.y / 1.5f, me.transform.position.z);
            }
            else if (monster.playerId == playerid)
            {
                GameObject damagedEffect = EffectManager.Instance.CreateEffect("Effect/ParticlesHeal", monster.transform);
                damagedEffect.transform.position = new Vector3(monster.transform.position.x, monster.transform.position.y + monster.transform.gameObject.transform.GetComponent<BoxCollider2D>().bounds.size.y / 1.5f, monster.transform.position.z);
            }
        }
    }


    public void UpdateEffectStaticCharacter(string _nameEff, string _spriteSrc, int _duration, int _playerid, bool _isSkill, bool _isEffOp)
    {
        Debug.Log("play id " + _playerid);
        if (_playerid == 1)
        {

            bool hasExistSkill = false;
            foreach (Transform child in BattleSceneUI.Instance.EffectPossitiveSkillMe.transform)
            {
                if (child.name == _nameEff)
                {
                    child.GetChild(1).GetComponent<Text>().text = _duration.ToString();
                    hasExistSkill = true;
                    break;
                }
            }
            foreach (Transform child in BattleSceneUI.Instance.EffectNegativeSkillMe.transform)
            {
                if (child.name == _nameEff)
                {
                    child.GetChild(1).GetComponent<Text>().text = _duration.ToString();
                    hasExistSkill = true;
                    break;
                }
            }
            if (_isSkill)
            {
                if (!hasExistSkill)
                {
                    GameObject effectObj = Instantiate(Resources.Load("Prefabs/Effect") as GameObject);
                    if (!_isEffOp)
                    {
                        effectObj.transform.parent = BattleSceneUI.Instance.EffectPossitiveSkillMe.transform;
                        effectObj.transform.GetComponent<Image>().sprite = BattleSceneUI.Instance._boundEffect[0];
                    }
                    else
                    {
                        effectObj.transform.parent = BattleSceneUI.Instance.EffectNegativeSkillMe.transform;
                        effectObj.transform.GetComponent<Image>().sprite = BattleSceneUI.Instance._boundEffect[1];
                    }
                    effectObj.transform.GetChild(0).GetComponent<Image>().sprite = Resources.Load<Sprite>("Textures/skillAss/" + _spriteSrc);
                    effectObj.GetComponent<RectTransform>().localScale = new Vector3(1f, 1f, 1f);
                    effectObj.transform.GetChild(1).GetComponent<Text>().text = _duration.ToString();
                    effectObj.name = _nameEff;
                    OnHoverToEffect(effectObj, _nameEff, _duration.ToString(), new Vector3(-500, 0, 0));
                }
            }
            else
            {
                if (!hasExistSkill)
                {
                    GameObject absObj = Instantiate(Resources.Load("Prefabs/Effect") as GameObject);
                    if (!_isEffOp)
                    {
                        absObj.transform.parent = BattleSceneUI.Instance.EffectPossitiveSkillMe.transform;
                        absObj.transform.GetComponent<Image>().sprite = BattleSceneUI.Instance._boundEffect[0];
                    }
                    else
                    {
                        absObj.transform.parent = BattleSceneUI.Instance.EffectNegativeSkillMe.transform;
                        absObj.transform.GetComponent<Image>().sprite = BattleSceneUI.Instance._boundEffect[1];
                    }
                    absObj.transform.GetChild(0).GetComponent<Image>().sprite = Resources.Load<Sprite>("Textures/abs/" + _spriteSrc);
                    absObj.GetComponent<RectTransform>().localScale = new Vector3(1f, 1f, 1f);
                    absObj.transform.GetChild(1).GetComponent<Text>().text = _duration.ToString();
                    absObj.name = _nameEff;
                    OnHoverToEffect(absObj, _nameEff, _duration.ToString(), new Vector3(-500, 0, 0));
                    if (_isEffOp)
                        CreateAbnormalStatusEffectOnCharacter(_nameEff, me.transform, false);
                }
            }
        }
        else if (_playerid == 2)
        {
            Debug.Log("render in master");
            Debug.Log("is skill " + _isSkill);
            bool hasExistSkill = false;
            foreach (Transform child in BattleSceneUI.Instance.EffectPossitiveSkillEnemy.transform)
            {
                if (child.name == _nameEff)
                {
                    child.GetChild(1).GetComponent<Text>().text = _duration.ToString();
                    hasExistSkill = true;
                    break;
                }
            }
            foreach (Transform child in BattleSceneUI.Instance.EffectNegativeSkillEnemy.transform)
            {
                if (child.name == _nameEff)
                {
                    child.GetChild(1).GetComponent<Text>().text = _duration.ToString();
                    hasExistSkill = true;
                    break;
                }
            }

            if (_isSkill)
            {
                if (!hasExistSkill)
                {
                    GameObject effectObj = Instantiate(Resources.Load("Prefabs/Effect") as GameObject);
                    if (!_isEffOp)
                    {
                        effectObj.transform.parent = BattleSceneUI.Instance.EffectPossitiveSkillEnemy.transform;
                        effectObj.transform.GetComponent<Image>().sprite = BattleSceneUI.Instance._boundEffect[0];
                    }
                    else
                    {
                        effectObj.transform.parent = BattleSceneUI.Instance.EffectNegativeSkillEnemy.transform;
                        effectObj.transform.GetComponent<Image>().sprite = BattleSceneUI.Instance._boundEffect[1];
                    }
                    effectObj.transform.GetChild(0).GetComponent<Image>().sprite = Resources.Load<Sprite>("Textures/skillAss/" + _spriteSrc);
                    effectObj.GetComponent<RectTransform>().localScale = new Vector3(1f, 1f, 1f);
                    effectObj.transform.GetChild(1).GetComponent<Text>().text = _duration.ToString();
                    effectObj.name = _nameEff;
                    OnHoverToEffect(effectObj, _nameEff, _duration.ToString(), new Vector3(500, 0, 0));
                }
            }
            else
            {
                Debug.Log("render abs");
                if (!hasExistSkill)
                {
                    Debug.Log("render abs ok");
                    GameObject absObj = Instantiate(Resources.Load("Prefabs/Effect") as GameObject);
                    if (!_isEffOp)
                    {
                        absObj.transform.parent = BattleSceneUI.Instance.EffectPossitiveSkillEnemy.transform;
                        absObj.transform.GetComponent<Image>().sprite = BattleSceneUI.Instance._boundEffect[0];
                    }
                    else
                    {
                        absObj.transform.parent = BattleSceneUI.Instance.EffectNegativeSkillEnemy.transform;
                        absObj.transform.GetComponent<Image>().sprite = BattleSceneUI.Instance._boundEffect[1];
                    }
                    absObj.transform.GetChild(0).GetComponent<Image>().sprite = Resources.Load<Sprite>("Textures/abs/" + _spriteSrc);
                    absObj.GetComponent<RectTransform>().localScale = new Vector3(1f, 1f, 1f);
                    absObj.transform.GetChild(1).GetComponent<Text>().text = _duration.ToString();
                    absObj.name = _nameEff;
                    OnHoverToEffect(absObj, _nameEff, _duration.ToString(), new Vector3(500, 0, 0));
                    if (_isEffOp)
                        CreateAbnormalStatusEffectOnCharacter(_nameEff, monster.transform, true);
                }
            }

        }
    }


    void CreateAbnormalStatusEffectOnCharacter(string name, Transform _character, bool isEnemy)
    {
        if (name == "bleed")
        {
            if (isEnemy == false)
            {
                if (BattleSceneUI.Instance._effectParentMe.transform.Find("bleedme") == null)
                {
                    GameObject damagedEffect = EffectManager.Instance.CreateEffect("Effect/Bleeding", _character);
                    damagedEffect.transform.position = new Vector3(_character.position.x, _character.position.y + _character.gameObject.transform.GetComponent<BoxCollider2D>().bounds.size.y / 2, _character.position.z);
                    damagedEffect.name = "bleedme";
                    damagedEffect.transform.parent = BattleSceneUI.Instance._effectParentMe.transform;
                }
            }
            else
            {
                if (BattleSceneUI.Instance._effectParentEnemy.transform.Find("bleedenemy") == null)
                {
                    GameObject damagedEffect = EffectManager.Instance.CreateEffect("Effect/Bleeding", _character);
                    damagedEffect.transform.position = new Vector3(_character.position.x, _character.position.y + _character.gameObject.transform.GetComponent<BoxCollider2D>().bounds.size.y / 2, _character.position.z);
                    damagedEffect.name = "bleedenemy";
                    damagedEffect.transform.parent = BattleSceneUI.Instance._effectParentEnemy.transform;
                    damagedEffect.transform.localScale = new Vector3(damagedEffect.transform.localScale.x * -1, damagedEffect.transform.localScale.y, damagedEffect.transform.localScale.z);
                }
            }
        }
        if (name == "burn")
        {
            if (isEnemy == false)
            {
                if (BattleSceneUI.Instance._effectParentEnemy.transform.Find("burnme") == null)
                {
                    GameObject damagedEffect = EffectManager.Instance.CreateEffect("Effect/Burn", _character);
                    damagedEffect.transform.position = new Vector3(_character.position.x + _character.gameObject.transform.GetComponent<BoxCollider2D>().bounds.size.x / 4, _character.position.y + _character.gameObject.transform.GetComponent<BoxCollider2D>().bounds.size.y / 2, _character.position.z);
                    damagedEffect.name = "burnme";
                    damagedEffect.transform.parent = BattleSceneUI.Instance._effectParentEnemy.transform;
                }
            }
            else
            {
                if (BattleSceneUI.Instance._effectParentEnemy.transform.Find("burnenemy") == null)
                {
                    GameObject damagedEffect = EffectManager.Instance.CreateEffect("Effect/Burn", _character);
                    damagedEffect.transform.position = new Vector3(_character.position.x - _character.gameObject.transform.GetComponent<BoxCollider2D>().bounds.size.x / 4, _character.position.y + _character.gameObject.transform.GetComponent<BoxCollider2D>().bounds.size.y / 2, _character.position.z);
                    damagedEffect.name = "burnenemy";
                    damagedEffect.transform.parent = BattleSceneUI.Instance._effectParentEnemy.transform;
                    damagedEffect.transform.localScale = new Vector3(damagedEffect.transform.localScale.x * -1, damagedEffect.transform.localScale.y, damagedEffect.transform.localScale.z);
                }
            }
        }

        if (name == "crazy")
        {
            if (isEnemy == false)
            {
                if (BattleSceneUI.Instance._effectParentMe.transform.Find("crazyme") == null)
                {
                    GameObject damagedEffect = EffectManager.Instance.CreateEffect("Effect/Crazy", _character);
                    damagedEffect.transform.position = new Vector3(_character.position.x, _character.position.y + _character.gameObject.transform.GetComponent<BoxCollider2D>().bounds.size.y + 0.5f, _character.position.z);
                    damagedEffect.name = "crazyme";
                    damagedEffect.transform.parent = BattleSceneUI.Instance._effectParentMe.transform;

                }
            }
            else
            {
                if (BattleSceneUI.Instance._effectParentEnemy.transform.Find("crazyenemy") == null)
                {
                    GameObject damagedEffect = EffectManager.Instance.CreateEffect("Effect/Crazy", _character);
                    damagedEffect.transform.position = new Vector3(_character.position.x, _character.position.y + _character.gameObject.transform.GetComponent<BoxCollider2D>().bounds.size.y + 0.5f, _character.position.z);
                    damagedEffect.name = "crazyenemy";
                    damagedEffect.transform.parent = BattleSceneUI.Instance._effectParentEnemy.transform;
                    damagedEffect.transform.localScale = new Vector3(damagedEffect.transform.localScale.x * -1, damagedEffect.transform.localScale.y, damagedEffect.transform.localScale.z);
                }
            }
        }
        if (name == "dull")
        {
            if (isEnemy == false)
            {
                if (BattleSceneUI.Instance._effectParentMe.transform.Find("dullme") == null)
                {
                    GameObject damagedEffect = EffectManager.Instance.CreateEffect("Effect/Dull", _character);
                    damagedEffect.transform.position = new Vector3(_character.position.x, _character.position.y + _character.gameObject.transform.GetComponent<BoxCollider2D>().bounds.size.y + 0.5f, _character.position.z);
                    damagedEffect.name = "dullme";
                    damagedEffect.transform.parent = BattleSceneUI.Instance._effectParentMe.transform;

                }
            }
            else
            {
                if (BattleSceneUI.Instance._effectParentEnemy.transform.Find("dullenemy") == null)
                {
                    GameObject damagedEffect = EffectManager.Instance.CreateEffect("Effect/Dull", _character);
                    damagedEffect.transform.position = new Vector3(_character.position.x, _character.position.y + _character.gameObject.transform.GetComponent<BoxCollider2D>().bounds.size.y + 0.5f, _character.position.z);
                    damagedEffect.name = "dullenemy";
                    damagedEffect.transform.parent = BattleSceneUI.Instance._effectParentEnemy.transform;
                    damagedEffect.transform.localScale = new Vector3(damagedEffect.transform.localScale.x * -1, damagedEffect.transform.localScale.y, damagedEffect.transform.localScale.z);
                }
            }
        }
        if (name == "fear")
        {
            if (isEnemy == false)
            {
                if (BattleSceneUI.Instance._effectParentMe.transform.Find("fearme") == null)
                {
                    GameObject damagedEffect = EffectManager.Instance.CreateEffect("Effect/Fear", _character);
                    damagedEffect.transform.position = new Vector3(_character.position.x, _character.position.y + _character.gameObject.transform.GetComponent<BoxCollider2D>().bounds.size.y + 0.5f, _character.position.z);
                    damagedEffect.name = "fearme";
                    damagedEffect.transform.parent = BattleSceneUI.Instance._effectParentMe.transform;

                }
            }
            else
            {
                if (BattleSceneUI.Instance._effectParentEnemy.transform.Find("fearenemy") == null)
                {
                    GameObject damagedEffect = EffectManager.Instance.CreateEffect("Effect/Fear", _character);
                    damagedEffect.transform.position = new Vector3(_character.position.x, _character.position.y + _character.gameObject.transform.GetComponent<BoxCollider2D>().bounds.size.y + 0.5f, _character.position.z);
                    damagedEffect.name = "fearenemy";
                    damagedEffect.transform.parent = BattleSceneUI.Instance._effectParentEnemy.transform;
                    damagedEffect.transform.localScale = new Vector3(damagedEffect.transform.localScale.x * -1, damagedEffect.transform.localScale.y, damagedEffect.transform.localScale.z);
                }
            }
        }
        if (name == "glamour")
        {
            if (isEnemy == false)
            {
                if (BattleSceneUI.Instance._effectParentMe.transform.Find("glamourme") == null)
                {
                    GameObject damagedEffect = EffectManager.Instance.CreateEffect("Effect/Glamour", _character);
                    damagedEffect.transform.position = new Vector3(_character.position.x, _character.position.y + _character.gameObject.transform.GetComponent<BoxCollider2D>().bounds.size.y + 0.5f, _character.position.z);
                    damagedEffect.name = "glamourme";
                    damagedEffect.transform.parent = BattleSceneUI.Instance._effectParentMe.transform;

                }
            }
            else
            {
                if (BattleSceneUI.Instance._effectParentEnemy.transform.Find("glamourenemy") == null)
                {
                    GameObject damagedEffect = EffectManager.Instance.CreateEffect("Effect/Glamour", _character);
                    damagedEffect.transform.position = new Vector3(_character.position.x, _character.position.y + _character.gameObject.transform.GetComponent<BoxCollider2D>().bounds.size.y + 0.5f, _character.position.z);
                    damagedEffect.name = "glamourenemy";
                    damagedEffect.transform.parent = BattleSceneUI.Instance._effectParentEnemy.transform;
                    damagedEffect.transform.localScale = new Vector3(damagedEffect.transform.localScale.x * -1, damagedEffect.transform.localScale.y, damagedEffect.transform.localScale.z);
                }
            }
        }
        if (name == "hypnotic")
        {
            if (isEnemy == false)
            {
                if (BattleSceneUI.Instance._effectParentMe.transform.Find("hypnoticme") == null)
                {
                    GameObject damagedEffect = EffectManager.Instance.CreateEffect("Effect/Hypnotic", _character);
                    damagedEffect.transform.position = new Vector3(_character.position.x, _character.position.y + _character.gameObject.transform.GetComponent<BoxCollider2D>().bounds.size.y + 0.5f, _character.position.z);
                    damagedEffect.name = "hypnoticme";
                    damagedEffect.transform.parent = BattleSceneUI.Instance._effectParentMe.transform;

                }
            }
            else
            {
                if (BattleSceneUI.Instance._effectParentEnemy.transform.Find("hypnoticenemy") == null)
                {
                    GameObject damagedEffect = EffectManager.Instance.CreateEffect("Effect/Hypnotic", _character);
                    damagedEffect.transform.position = new Vector3(_character.position.x, _character.position.y + _character.gameObject.transform.GetComponent<BoxCollider2D>().bounds.size.y + 0.5f, _character.position.z);
                    damagedEffect.name = "hypnoticenemy";
                    damagedEffect.transform.parent = BattleSceneUI.Instance._effectParentEnemy.transform;
                    damagedEffect.transform.localScale = new Vector3(damagedEffect.transform.localScale.x * -1, damagedEffect.transform.localScale.y, damagedEffect.transform.localScale.z);
                }
            }
        }

        if (name == "knockback")
        {
            if (isEnemy == false)
            {
                if (BattleSceneUI.Instance._effectParentMe.transform.Find("knockbackme") == null)
                {
                    GameObject damagedEffect = EffectManager.Instance.CreateEffect("Effect/Knockback", _character);
                    damagedEffect.transform.position = new Vector3(_character.position.x, _character.position.y + _character.gameObject.transform.GetComponent<BoxCollider2D>().bounds.size.y + 0.5f, _character.position.z);
                    damagedEffect.name = "knockbackme";
                    damagedEffect.transform.parent = BattleSceneUI.Instance._effectParentMe.transform;

                }
            }
            else
            {
                if (BattleSceneUI.Instance._effectParentEnemy.transform.Find("knockbackenemy") == null)
                {
                    GameObject damagedEffect = EffectManager.Instance.CreateEffect("Effect/Knockback", _character);
                    damagedEffect.transform.position = new Vector3(_character.position.x, _character.position.y + _character.gameObject.transform.GetComponent<BoxCollider2D>().bounds.size.y + 0.5f, _character.position.z);
                    damagedEffect.name = "knockbackenemy";
                    damagedEffect.transform.parent = BattleSceneUI.Instance._effectParentEnemy.transform;
                    damagedEffect.transform.localScale = new Vector3(damagedEffect.transform.localScale.x * -1, damagedEffect.transform.localScale.y, damagedEffect.transform.localScale.z);
                }
            }
        }
        if (name == "rot")
        {
            if (isEnemy == false)
            {
                if (BattleSceneUI.Instance._effectParentMe.transform.Find("rotme") == null)
                {
                    GameObject damagedEffect = EffectManager.Instance.CreateEffect("Effect/Rot", _character);
                    damagedEffect.transform.position = new Vector3(_character.position.x, _character.position.y + _character.gameObject.transform.GetComponent<BoxCollider2D>().bounds.size.y / 4, _character.position.z);
                    damagedEffect.name = "rotme";
                    damagedEffect.transform.parent = BattleSceneUI.Instance._effectParentMe.transform;

                }
            }
            else
            {
                if (BattleSceneUI.Instance._effectParentEnemy.transform.Find("rotenemy") == null)
                {
                    GameObject damagedEffect = EffectManager.Instance.CreateEffect("Effect/Rot", _character);
                    damagedEffect.transform.position = new Vector3(_character.position.x, _character.position.y + _character.gameObject.transform.GetComponent<BoxCollider2D>().bounds.size.y / 4, _character.position.z);
                    damagedEffect.name = "rotenemy";
                    damagedEffect.transform.parent = BattleSceneUI.Instance._effectParentEnemy.transform;
                    damagedEffect.transform.localScale = new Vector3(damagedEffect.transform.localScale.x * -1, damagedEffect.transform.localScale.y, damagedEffect.transform.localScale.z);
                }
            }
        }

        if (name == "shock")
        {
            if (isEnemy == false)
            {
                if (BattleSceneUI.Instance._effectParentMe.transform.Find("shockme") == null)
                {
                    GameObject damagedEffect = EffectManager.Instance.CreateEffect("Effect/Shock", _character);
                    damagedEffect.transform.position = new Vector3(_character.position.x, _character.position.y + _character.gameObject.transform.GetComponent<BoxCollider2D>().bounds.size.y / 2, _character.position.z);
                    damagedEffect.name = "shockme";
                    damagedEffect.transform.parent = BattleSceneUI.Instance._effectParentMe.transform;

                }
            }
            else
            {
                if (BattleSceneUI.Instance._effectParentEnemy.transform.Find("shockenemy") == null)
                {
                    GameObject damagedEffect = EffectManager.Instance.CreateEffect("Effect/Shock", _character);
                    damagedEffect.transform.position = new Vector3(_character.position.x, _character.position.y + _character.gameObject.transform.GetComponent<BoxCollider2D>().bounds.size.y / 2, _character.position.z);
                    damagedEffect.name = "shockenemy";
                    damagedEffect.transform.parent = BattleSceneUI.Instance._effectParentEnemy.transform;
                    damagedEffect.transform.localScale = new Vector3(damagedEffect.transform.localScale.x * -1, damagedEffect.transform.localScale.y, damagedEffect.transform.localScale.z);
                }
            }
        }

        if (name == "sleep")
        {
            if (isEnemy == false)
            {
                if (BattleSceneUI.Instance._effectParentMe.transform.Find("sleepme") == null)
                {
                    GameObject damagedEffect = EffectManager.Instance.CreateEffect("Effect/Sleep", _character);
                    damagedEffect.transform.position = new Vector3(_character.position.x + _character.gameObject.transform.GetComponent<BoxCollider2D>().bounds.size.x / 2, _character.position.y + _character.gameObject.transform.GetComponent<BoxCollider2D>().bounds.size.y, _character.position.z);
                    damagedEffect.name = "sleepme";
                    damagedEffect.transform.parent = BattleSceneUI.Instance._effectParentMe.transform;

                }
            }
            else
            {
                if (BattleSceneUI.Instance._effectParentEnemy.transform.Find("sleepenemy") == null)
                {
                    GameObject damagedEffect = EffectManager.Instance.CreateEffect("Effect/Sleep", _character);
                    damagedEffect.transform.position = new Vector3(_character.position.x - _character.gameObject.transform.GetComponent<BoxCollider2D>().bounds.size.x / 2, _character.position.y + _character.gameObject.transform.GetComponent<BoxCollider2D>().bounds.size.y, _character.position.z);
                    damagedEffect.name = "sleepenemy";
                    damagedEffect.transform.parent = BattleSceneUI.Instance._effectParentEnemy.transform;
                    damagedEffect.transform.localScale = new Vector3(damagedEffect.transform.localScale.x * -1, damagedEffect.transform.localScale.y, damagedEffect.transform.localScale.z);
                }
            }
        }

        if (name == "stun")
        {
            if (isEnemy == false)
            {
                if (BattleSceneUI.Instance._effectParentMe.transform.Find("stunme") == null)
                {
                    GameObject damagedEffect = EffectManager.Instance.CreateEffect("Effect/Stunned", _character);
                    damagedEffect.transform.position = new Vector3(_character.position.x, _character.position.y + _character.gameObject.transform.GetComponent<BoxCollider2D>().bounds.size.y + 0.5f, _character.position.z);
                    damagedEffect.name = "stunme";
                    damagedEffect.transform.parent = BattleSceneUI.Instance._effectParentMe.transform;
                    damagedEffect.transform.localEulerAngles = new Vector3(-90, 0, 60);
                }
            }
            else
            {
                if (BattleSceneUI.Instance._effectParentEnemy.transform.Find("stunenemy") == null)
                {
                    GameObject damagedEffect = EffectManager.Instance.CreateEffect("Effect/Stunned", _character);
                    damagedEffect.transform.position = new Vector3(_character.position.x, _character.position.y + _character.gameObject.transform.GetComponent<BoxCollider2D>().bounds.size.y + 0.5f, _character.position.z);
                    damagedEffect.name = "stunenemy";
                    damagedEffect.transform.parent = BattleSceneUI.Instance._effectParentEnemy.transform;
                    damagedEffect.transform.localEulerAngles = new Vector3(90, 0, 60);
                }
            }
        }
        if (name == "poisoning")
        {
            if (isEnemy == false)
            {
                if (BattleSceneUI.Instance._effectParentMe.transform.Find("poisoningme") == null)
                {
                    GameObject damagedEffect = EffectManager.Instance.CreateEffect("Effect/Poison", _character);
                    damagedEffect.transform.position = new Vector3(_character.transform.position.x, _character.position.y + _character.gameObject.transform.GetComponent<BoxCollider2D>().bounds.size.y / 2, _character.position.z);
                    damagedEffect.name = "poisoningme";
                    damagedEffect.transform.parent = BattleSceneUI.Instance._effectParentMe.transform;

                }
            }
            else
            {
                if (BattleSceneUI.Instance._effectParentEnemy.transform.Find("poisoningenemy") == null)
                {
                    GameObject damagedEffect = EffectManager.Instance.CreateEffect("Effect/Poison", _character);
                    damagedEffect.transform.position = new Vector3(_character.transform.position.x, _character.position.y + _character.gameObject.transform.GetComponent<BoxCollider2D>().bounds.size.y / 2, _character.position.z);
                    damagedEffect.name = "poisoningenemy";
                    damagedEffect.transform.parent = BattleSceneUI.Instance._effectParentEnemy.transform;
                    damagedEffect.transform.localScale = new Vector3(damagedEffect.transform.localScale.x * -1, damagedEffect.transform.localScale.y, damagedEffect.transform.localScale.z);

                }
            }
        }
    }


    public void OnHoverToEffect(GameObject skillObj, string _name, string _duration, Vector3 pos)
    {

        // Event when hover mouse to button
        EventTrigger trigger = skillObj.AddComponent<EventTrigger>();

        EventTrigger.Entry pointerEnter = new EventTrigger.Entry();
        pointerEnter.eventID = EventTriggerType.PointerEnter;

        // create the listener and attach to callback
        EventTrigger.TriggerEvent pointerEnterEvent = new EventTrigger.TriggerEvent();
        string name = "" + _name;
        string value = "Duration " + _duration + " turn";
        pointerEnterEvent.AddListener((x) =>
        {
            BattleSceneUI.Instance.ShowDialog(name, value, pos);
        });

        pointerEnter.callback = pointerEnterEvent;

        EventTrigger.Entry pointerExit = new EventTrigger.Entry();
        pointerExit.eventID = EventTriggerType.PointerExit;

        // create the listener and attach to callback
        EventTrigger.TriggerEvent pointerExitEvent = new EventTrigger.TriggerEvent();
        pointerExitEvent.AddListener((x) =>
        {
            BattleSceneUI.Instance.HideDialog();
        });

        pointerExit.callback = pointerExitEvent;

        // add this to delegates list, ensure delegates is not null
        if (trigger.triggers == null)
        {
            trigger.triggers = new List<EventTrigger.Entry>();
        }

        trigger.triggers.Add(pointerEnter);
        trigger.triggers.Add(pointerExit);
    }

    void UpdateUIAfterEachState(string data)
    {
        int playerid = int.Parse(data.Split(',')[0]);
        int indexState = int.Parse(data.Split(',')[1]);
        int healthPlayer = int.Parse(data.Split(',')[2]);
        string sendDamge = data.Split(',')[3];
        if (sendDamge == "senddamge")
        {
            if (me.playerId == playerid)
            {
                if (healthPlayer <= 0)
                {
                    foreach (Transform child in BattleSceneUI.Instance._effectParentEnemy.transform)
                    {
                        Destroy(child.gameObject);
                    }
                    BattleSceneUI.Instance._effectParentEnemy.gameObject.SetActive(false);
                    monster.GetComponent<AnimationController>().Die(playerid, monster.transform);
                }
                else
                {
                    monster.GetComponent<AnimationController>().GetDamage(playerid, monster.transform);
                }

            }
            else if (monster.playerId == playerid)
            {

                if (healthPlayer <= 0)
                {
                    foreach (Transform child in BattleSceneUI.Instance._effectParentMe.transform)
                    {
                        Destroy(child.gameObject);
                    }
                    BattleSceneUI.Instance._effectParentMe.gameObject.SetActive(false);
                    me.GetComponent<AnimationController>().Die(playerid, me.transform);
                }
                else
                {
                    me.GetComponent<AnimationController>().GetDamage(playerid, me.transform);
                }

            }
        }
        InTurnRenderWithStateIndex(indexState);
    }

    public void InTurnRenderWithStateIndex(int index)
    {
        ArrayList states = inResult.states;
        UpdateUICharacterWithIndexState(states, index);
    }

    public void UpdateUICharacterWithIndexState(ArrayList states, int indexState)
    {
        int index = 0;
        foreach (State state in states)
        {
            index++;
            if (index == indexState)
            {
                // initIndexes la cac chi so cua nhan vat khong mac equipment, cac chi so tinh(khong co min max) cua equipment va cac chi so duration cua abs
                // midIndexes la cac chi so bao gom initIndexes kem theo cac chi so dong (co min max) cua equipment va cac chi so phu phat sinh trong qua trinh tinh toan cong thuc
                // curIndexes la chi so chi dung de doc tat ca cac thong so
                me._actionPoints = 0;
                monster._actionPoints = 0;
                BattleSceneUI.Instance.UpdateDisplayBar(BattleSceneUI.Instance.actionPointBarPlayer, me._actionPoints, 18f);
                BattleSceneUI.Instance._meCharacterActionPoint.text = "Action Point: " + me._actionPoints.ToString();
                BattleSceneUI.Instance.UpdateDisplayBar(BattleSceneUI.Instance.actionPointBarEnemy, monster._actionPoints, 18f);
                BattleSceneUI.Instance._enemyCharacterActionPoint.text = "Action Point: " + monster._actionPoints.ToString();

                var N = SimpleJSON.JSON.Parse(state.toJSON().ToString());
                int damagePlayer1 = 0, damagePlayer2 = 0;
                if (isMe)
                {
                    damagePlayer1 = (int)(N["hpPlayer1"].AsFloat - me._baseProperties.hp);
                    if (damagePlayer1 > 0)
                    {
                        UpdateDameEffect(damagePlayer1, 1);
                    }
                    else if (damagePlayer1 < 0)
                    {
                        UpdateDameEffect(damagePlayer1, 1);
                    }
                    damagePlayer2 = (int)(N["hpPlayer2"].AsFloat - monster._baseProperties.hp);

                    if (damagePlayer2 > 0)
                    {
                        UpdateDameEffect(damagePlayer2, 2);
                    }
                    else if (damagePlayer2 < 0)
                    {
                        UpdateDameEffect(damagePlayer2, 2);
                    }
                    me._baseProperties.hp = N["hpPlayer1"].AsFloat;
                    monster._baseProperties.hp = N["hpPlayer2"].AsFloat;
                    if (me._baseProperties.hp <= 0)
                    {
                        me._baseProperties.hp = 0;
                    }
                    if (monster._baseProperties.hp <= 0)
                    {
                        monster._baseProperties.hp = 0;
                    }
                    if (isMe)
                    {

                        BattleSceneUI.Instance.UpdateDisplayBar(BattleSceneUI.Instance.heathBarPlayer, N["hpPlayer1"].AsFloat, Mathf.Round((float)logic.attStatus.getCurrentIndex(Indexes.max_hp_na)));
                        BattleSceneUI.Instance._meCharacterHP.text = ((int)me._baseProperties.hp).ToString() + "/" + logic.attStatus.getCurrentIndex(Indexes.max_hp_na).ToString();
                        BattleSceneUI.Instance.UpdateDisplayBar(BattleSceneUI.Instance.heathBarEnemy, N["hpPlayer2"].AsFloat, Mathf.Round((float)logic.defStatus.getCurrentIndex(Indexes.max_hp_na)));

                        BattleSceneUI.Instance._enemyCharacterHP.text = ((int)monster._baseProperties.hp).ToString() + "/" + logic.defStatus.getCurrentIndex(Indexes.max_hp_na).ToString();
                    }
                    else if (!isMe)
                    {

                        BattleSceneUI.Instance.UpdateDisplayBar(BattleSceneUI.Instance.heathBarPlayer, N["hpPlayer1"].AsFloat, Mathf.Round((float)logic.defStatus.getCurrentIndex(Indexes.max_hp_na)));
                        BattleSceneUI.Instance._meCharacterHP.text = ((int)me._baseProperties.hp).ToString() + "/" + logic.defStatus.getCurrentIndex(Indexes.max_hp_na).ToString();
                        BattleSceneUI.Instance.UpdateDisplayBar(BattleSceneUI.Instance.heathBarEnemy, N["hpPlayer2"].AsFloat, Mathf.Round((float)logic.attStatus.getCurrentIndex(Indexes.max_hp_na)));

                        BattleSceneUI.Instance._enemyCharacterHP.text = ((int)monster._baseProperties.hp).ToString() + "/" + logic.attStatus.getCurrentIndex(Indexes.max_hp_na).ToString();
                    }
                }
                else if (!isMe)
                {
                    damagePlayer1 = (int)(N["hpPlayer1"].AsFloat - monster._baseProperties.hp);
                    if (damagePlayer1 > 0)
                    {
                        UpdateDameEffect(damagePlayer1, 1);
                    }
                    else if (damagePlayer1 < 0)
                    {
                        UpdateDameEffect(damagePlayer1, 1);
                    }
                    damagePlayer2 = (int)(N["hpPlayer2"].AsFloat - me._baseProperties.hp);

                    if (damagePlayer2 > 0)
                    {
                        UpdateDameEffect(damagePlayer2, 2);
                    }
                    else if (damagePlayer2 < 0)
                    {
                        UpdateDameEffect(damagePlayer2, 2);
                    }
                    me._baseProperties.hp = N["hpPlayer2"].AsFloat;
                    monster._baseProperties.hp = N["hpPlayer1"].AsFloat;
                    if (me._baseProperties.hp <= 0)
                    {
                        me._baseProperties.hp = 0;
                    }
                    if (monster._baseProperties.hp <= 0)
                    {
                        monster._baseProperties.hp = 0;
                    }
                    if (!isMe)
                    {

                        BattleSceneUI.Instance.UpdateDisplayBar(BattleSceneUI.Instance.heathBarPlayer, N["hpPlayer2"].AsFloat, Mathf.Round((float)logic.defStatus.getCurrentIndex(Indexes.max_hp_na)));
                        BattleSceneUI.Instance._meCharacterHP.text = ((int)me._baseProperties.hp).ToString() + "/" + logic.defStatus.getCurrentIndex(Indexes.max_hp_na).ToString();
                        BattleSceneUI.Instance.UpdateDisplayBar(BattleSceneUI.Instance.heathBarEnemy, N["hpPlayer1"].AsFloat, Mathf.Round((float)logic.attStatus.getCurrentIndex(Indexes.max_hp_na)));

                        BattleSceneUI.Instance._enemyCharacterHP.text = ((int)monster._baseProperties.hp).ToString() + "/" + logic.attStatus.getCurrentIndex(Indexes.max_hp_na).ToString();
                    }
                    else if (isMe)
                    {

                        BattleSceneUI.Instance.UpdateDisplayBar(BattleSceneUI.Instance.heathBarEnemy, N["hpPlayer1"].AsFloat, Mathf.Round((float)logic.defStatus.getCurrentIndex(Indexes.max_hp_na)));
                        BattleSceneUI.Instance._enemyCharacterHP.text = ((int)monster._baseProperties.hp).ToString() + "/" + logic.defStatus.getCurrentIndex(Indexes.max_hp_na).ToString();
                        BattleSceneUI.Instance.UpdateDisplayBar(BattleSceneUI.Instance.heathBarPlayer, N["hpPlayer2"].AsFloat, Mathf.Round((float)logic.attStatus.getCurrentIndex(Indexes.max_hp_na)));

                        BattleSceneUI.Instance._meCharacterHP.text = ((int)me._baseProperties.hp).ToString() + "/" + logic.attStatus.getCurrentIndex(Indexes.max_hp_na).ToString();
                    }
                }
                int numberEff = N["effects"].Count;
                for (int i = 0; i < numberEff; i++)
                {
                    Debug.Log("render eff" + numberEff);
                    UpdateEffectUIOnCharacter(N["effects"][i]["name"].Value, N["effects"][i]["duration"].AsInt, N["effects"][i]["playerID"].AsInt);
                }
            }
        }

    }




    void UpdateDameEffect(int damage, int playerid)
    {
        GameObject damageText = Instantiate(Resources.Load("Prefabs/BattleStateText") as GameObject);
        if (damage < 0)
        {
            damageText.GetComponent<Text>().text = "" + damage.ToString();
        }
        else
        {
            damageText.GetComponent<Text>().text = "+ " + damage.ToString();
        }
        damageText.transform.parent = GameObject.Find("UI").transform;
        damageText.GetComponent<RectTransform>().localScale = new Vector3(1f, 1f, 1f);
        if (damage < 0)
        {
            damageText.GetComponent<Text>().color = Color.red;
        }
        else
        {
            damageText.GetComponent<Text>().color = Color.green;
        }
        if (playerid==1)
        {
            damageText.transform.position = new Vector3(me.transform.position.x, me.transform.position.y + me.transform.GetComponent<BoxCollider2D>().bounds.size.y / 2, me.transform.position.z);
        }
        else if (playerid == 2)
        {
            damageText.transform.position = new Vector3(monster.transform.position.x, monster.transform.position.y + monster.transform.GetComponent<BoxCollider2D>().bounds.size.y / 2, monster.transform.position.z);
        }
    }
    private void OnDestroy()
    {
        EventDispatcher.Instance.RemoveListener(EventID.OnRollingDice, _OnRollingDice);
        EventDispatcher.Instance.RemoveListener(EventID.OnCharacterOrderAction, _DoActionOrderListEventRef);
        EventDispatcher.Instance.RemoveListener(EventID.OnCharacterUpdateUIState, _OnCharacterUpdateUIStateEventRef);
    }
}
