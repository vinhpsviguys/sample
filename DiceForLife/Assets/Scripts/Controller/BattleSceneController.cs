using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;
using Newtonsoft.Json;
using CoreLib;
using System;
using Spine.Unity;

public class BattleSceneController : MonoBehaviour
{

    public static BattleSceneController Instance;

    Dictionary<GameObject, int> playerSkillDatawithObjectKey = new Dictionary<GameObject, int>();
    Dictionary<string, GameObject> playerSkillDatawithSkillKey = new Dictionary<string, GameObject>();
    //Dictionary<int, GameObject> enemySkillDataWithKeyId = new Dictionary<int, GameObject>();
    Dictionary<NewSkill, GameObject> playerskillwithObjectDictionary = new Dictionary<NewSkill, GameObject>();

    internal CharacterPlayer me;
    internal CharacterPlayer you;
    NewLogic logic;
    //internal NewCharacterStatus attStatus;
    //internal NewCharacterStatus defStatus;
    internal NewAdapter adapter;
    public int diceValue;

    private List<GameObject> _orderButtonActionList = new List<GameObject>();
    private List<string> _statesString = new List<string>();

    public GameObject contentScollView;


    Action<object> _OnRollingDice, _DoActionOrderListEventRef, _OnCharacterUpdateUIStateEventRef,
        _OnExecuteDataBattleEventRef;
    void Awake()
    {

        if (Instance != null && Instance != this)
        {
            // If that is the case, we destroy other instances
            Destroy(gameObject);
        }

        // Here we save our singleton instance
        Instance = this;

        _OnRollingDice = (param) => RollingDice();
        this.RegisterListener(EventID.OnRollingDice, _OnRollingDice);

        _DoActionOrderListEventRef = (param) => DoActionOrderList();
        this.RegisterListener(EventID.OnCharacterOrderAction, _DoActionOrderListEventRef);

        _OnCharacterUpdateUIStateEventRef = (param) => UpdateUIAfterEachState((string)param);
        this.RegisterListener(EventID.OnCharacterUpdateUIState, _OnCharacterUpdateUIStateEventRef);

        _OnExecuteDataBattleEventRef = (param) => StartCoroutine(ExecuteDataBatle());
        this.RegisterListener(EventID.OnEnemyReceivedDataBattleSucessfully, _OnExecuteDataBattleEventRef);

        CharacterManager.Instance._enemyCharacter.gameObject.SetActive(true);

    }

    private void Start()
    {
        SocketIOController.Instance.EnableListenersInBattleScene();

        //BattleSceneUI.Instance._normalAttackEnemy.onClick.AddListener(() => ChooseAction(-1, BattleSceneUI.Instance._normalAttackEnemy.transform));

        me = CharacterManager.Instance._meCharacter;
        me.loadDictionaries(SplitDataFromServe._heroSkill, SplitDataFromServe._enemySkill, SplitDataFromServe._heroAbs);
        //me.loadDictionaries(SplitDataFromServe.skillInit, SplitDataFromServe.absInit);
        you = CharacterManager.Instance._enemyCharacter;
        you.loadDictionaries(SplitDataFromServe._enemySkill, SplitDataFromServe._heroSkill, SplitDataFromServe._heroAbs);
        //you.loadDictionaries(SplitDataFromServe.skillInit, SplitDataFromServe.absInit);
        Debug.Log("list dic skill me" + me.newSkillDic.Count);
        Debug.Log("list dic skill you" + you.newSkillDic.Count);

        CreateBattleSceneUI();
        SetupLogicGame();
        CreateMeSkillUI();
        UpdateSkillState();
        ArrayList _passiveSkillMe = new ArrayList();
        foreach (NewSkill _tempSkill in SplitDataFromServe._heroSkill)
        {
            if (_tempSkill.data["type"].Value == "passive") _passiveSkillMe.Add(_tempSkill.getID());
        }

        adapter.addPassiveSkills(_passiveSkillMe, adapter.logic.getStatusByPlayerID(me.playerId));

        ArrayList _passiveSkillYou = new ArrayList();
        foreach (NewSkill _tempSkill in SplitDataFromServe._enemySkill)
        {
            if (_tempSkill.data["type"].Value == "passive") _passiveSkillYou.Add(_tempSkill.getID());
        }

        adapter.addPassiveSkills(_passiveSkillYou, adapter.logic.getStatusByPlayerID(you.playerId));

        //contentScollView.SetActive(Application.platform == RuntimePlatform.Android);

        adapter.setLog(contentScollView);
        if (adapter.getMyID() == 1)
            BattleSceneUI.Instance.PlayerPos.text = "Master";
        else if (adapter.getMyID() == 2)
            BattleSceneUI.Instance.PlayerPos.text = "Slave";
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

        //CreateMeSkillUI();
        //CreateEnemySkillUI();

        BattleSceneUI.Instance.TurnOfPlayerName.gameObject.SetActive(false);
        //UpdateEffectCharacter();
        UpdatePropertiesCharacterUI();


        BattleSceneUI.Instance.DiceBtn.interactable = false;
        BattleSceneUI.Instance._startOrderAction.interactable = false;

        //BattleSceneUI.Instance.ListEffectSkillUIMe.transform.position = new Vector3(BattleSceneUI.Instance.ListEffectSkillUIMe.transform.position.x, me.transform.position.y + me.transform.GetComponent<BoxCollider2D>().bounds.size.y, BattleSceneUI.Instance.ListEffectSkillUIMe.transform.position.z);
        //BattleSceneUI.Instance.ListEffectSkillUIEnemy.transform.position = new Vector3(BattleSceneUI.Instance.ListEffectSkillUIEnemy.transform.position.x, you.transform.position.y + you.transform.GetComponent<BoxCollider2D>().bounds.size.y, BattleSceneUI.Instance.ListEffectSkillUIEnemy.transform.position.z);

    }



    void SetupLogicGame()
    {
        Debug.Log("setup logic game");
        if (CharacterManager.Instance._meCharacter.keyPlayer == 1)
        {
            me.playerId = 1;
            you.playerId = 2;
            adapter = new NewAdapter(SocketIOController.sfs.mySocket, me, you, 1, true, false);
        }
        else
        {
            you.playerId = 1;
            me.playerId = 2;
            adapter = new NewAdapter(SocketIOController.sfs.mySocket, me, you, 1, false, false);
        }
        Debug.Log("Start");

    }

    private void Update()
    {
        adapter.Update();
        BattleSceneUI.Instance.TimeCountText.text = adapter.getEventTime().ToString();
        BattleSceneUI.Instance.DiceBtn.interactable = adapter.canRollDice();
        BattleSceneUI.Instance._startOrderAction.interactable = adapter.canSelectSkill();
        BattleSceneUI.Instance.StatusTurn.text = adapter.getStatus().ToString();
        BattleSceneUI.Instance.OnlineDetect.text = "Online :" + adapter.isOnLine().ToString();
        BattleSceneUI.Instance.PlayerPos.text = adapter.isMaster ? "Master" : "Slave";
    }






    public void SetTurnPlayerInFirstTime()
    {
        if (CharacterManager.Instance._meCharacter.keyPlayer == 1)
        {
            BattleSystemManager.Instance.playerTurn = true;
            //CheckStateBeginTurn(attStatus, deffStatus);
        }
        //UpdateUIOnTurnBegin();
    }


    public void CheckStateBeginTurn(CharacterStatus _att, CharacterStatus _def)
    {

        //BeginTurnResult beginResult = logic.beginTurn(_att, _def);
        //int idAttack = beginResult.attackerID;
        //bool continueTurn = beginResult.continued;
        //Debug.Log("continue turn " + continueTurn);
        //CheckTurnOfPlayer(continueTurn);
        //this.PostEvent(EventID.OnTurnBeginHostSendIDAttack, continueTurn);
    }

    public void CheckTurnOfPlayer(bool continueTurn)
    {

        if (!continueTurn)
        {
            BattleSystemManager.Instance.playerTurn = false;
        }
        UpdateUIOnTurnBegin();
    }

    public void UpdateUIOnTurnBegin()
    {
        BattleSystemManager.Instance.turn++;
        BattleSceneUI.Instance.TypeOfMatch.gameObject.SetActive(true);
        Debug.Log(adapter.getTurnTimes());
        BattleSceneUI.Instance.TypeOfMatch.text = "Turn " + (Mathf.Round((40 - adapter.getTurnTimes()) / 2 + 1).ToString());
        if (me.playerId == 1)
        {
            if (adapter.getTurn() == 1)
            {
                BattleSceneUI.Instance.DiceBtn.interactable = true;
                BattleSceneUI.Instance.TurnOfPlayerName.gameObject.SetActive(true);
                BattleSceneUI.Instance.TurnOfPlayerName.text = "Your turn";
            }
            else if (adapter.getTurn() == 2)
            {
                BattleSceneUI.Instance.DiceBtn.interactable = false;
                BattleSceneUI.Instance.TurnOfPlayerName.gameObject.SetActive(true);
                BattleSceneUI.Instance.TurnOfPlayerName.text = "Enemy turn";
            }
        }
        else if (me.playerId == 2)
        {
            if (adapter.getTurn() == 1)
            {
                BattleSceneUI.Instance.DiceBtn.interactable = false;
                BattleSceneUI.Instance.TurnOfPlayerName.gameObject.SetActive(true);
                BattleSceneUI.Instance.TurnOfPlayerName.text = "Enemy turn";

            }
            else if (adapter.getTurn() == 2)
            {
                BattleSceneUI.Instance.DiceBtn.interactable = true;
                BattleSceneUI.Instance.TurnOfPlayerName.gameObject.SetActive(true);
                BattleSceneUI.Instance.TurnOfPlayerName.text = "Your turn";
            }

            BattleSystemManager.Instance.battleStates = PerformAction.DICE;
        }
    }


    //IEnumerator BattleTurnInfo(string _text)
    //{
    //    BattleSceneUI.Instance.BattleBegin.gameObject.SetActive(true);
    //    BattleSceneUI.Instance.BattleBegin.text = _text;
    //    yield return new WaitForSeconds(0.5f);
    //    BattleSceneUI.Instance.BattleBegin.gameObject.SetActive(false);
    //    this.PostEvent(EventID.OnRollingDiceBegin);
    //}



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
        BattleSceneUI.Instance.UpdateDisplayBar(BattleSceneUI.Instance.heathBarEnemy, you.characteristic.Health, you.characteristic.Max_Health);
        if (you.characteristic.Health > 0)
        {
            BattleSceneUI.Instance._enemyCharacterHP.text = you.characteristic.Health.ToString();
        }
        else if (you.characteristic.Health <= 0)
        {
            BattleSceneUI.Instance._enemyCharacterHP.text = "0";
        }
        BattleSceneUI.Instance.UpdateDisplayBar(BattleSceneUI.Instance.actionPointBarEnemy, CharacterManager.Instance._enemyCharacter._actionPoints, 18f);
        BattleSceneUI.Instance._enemyCharacterActionPoint.text = "Action Point: " + you._actionPoints.ToString();
        BattleSceneUI.Instance._enemyLevel.text = "Level: " + you._baseProperties.Level.ToString();
        BattleSceneUI.Instance.avatarEnemy.sprite = CharacterItemInGame.Instance._avatarCircle[(int)(CharacterManager.Instance._enemyCharacter._baseProperties._classCharacter)];

        //UpdateSkillState();
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

    public void UpdateActionPointOfSkillUI()
    {
        foreach (NewSkill tempSkill in playerskillwithObjectDictionary.Keys)
        {
            NewCharacterStatus myStatus = adapter.logic.getStatusByPlayerID(adapter.me.playerId);
            playerskillwithObjectDictionary[tempSkill].transform.GetChild(2).GetChild(0).GetComponent<Text>().text = Mathf.RoundToInt((float)myStatus.getCurrentIndex("Skill" + tempSkill.getID() + "_aps")).ToString();//tempSkill.getActionPoints().ToString();
        }
    }

    public void InTurnRender()
    {
        ArrayList states = adapter.inResult.states;
        UpdateUICharacter(states);
    }
    public void InTurnRenderWithStateIndex(int index)
    {
        ArrayList states = adapter.inResult.states;
        UpdateUICharacterWithIndexState(states, index);
    }

    public void BeginTurnRender()
    {
        ResetUISkill();
        UpdateSkillState();
        UpdateActionPointOfSkillUI();
        ArrayList states = adapter.beginResult.states;
        UpdateUICharacter(states);
        UpdateUIOnTurnBegin();
    }

    public void UpdateUICharacter(ArrayList states)
    {

        foreach (State state in states)
        {
            // initIndexes la cac chi so cua nhan vat khong mac equipment, cac chi so tinh(khong co min max) cua equipment va cac chi so duration cua abs
            // midIndexes la cac chi so bao gom initIndexes kem theo cac chi so dong (co min max) cua equipment va cac chi so phu phat sinh trong qua trinh tinh toan cong thuc
            // curIndexes la chi so chi dung de doc tat ca cac thong so
            me._actionPoints = 0;
            you._actionPoints = 0;
            BattleSceneUI.Instance.UpdateDisplayBar(BattleSceneUI.Instance.actionPointBarPlayer, me._actionPoints, 18f);
            BattleSceneUI.Instance._meCharacterActionPoint.text = "Action Point: " + me._actionPoints.ToString();
            BattleSceneUI.Instance.UpdateDisplayBar(BattleSceneUI.Instance.actionPointBarEnemy, you._actionPoints, 18f);
            BattleSceneUI.Instance._enemyCharacterActionPoint.text = "Action Point: " + you._actionPoints.ToString();

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
                damagePlayer2 = (int)(N["hpPlayer2"].AsFloat - you._baseProperties.hp);

                if (damagePlayer2 > 0)
                {
                    UpdateDameEffect(damagePlayer2, 2);
                }
                else if (damagePlayer2 < 0)
                {
                    UpdateDameEffect(damagePlayer2, 2);
                }

                me._baseProperties.hp = N["hpPlayer1"].AsFloat;
                you._baseProperties.hp = N["hpPlayer2"].AsFloat;
                if (me._baseProperties.hp <= 0)
                {
                    me._baseProperties.hp = 0;
                }

                if (you._baseProperties.hp <= 0)
                {
                    you._baseProperties.hp = 0;
                }

                if (adapter.getTurn() == 1)
                {

                    BattleSceneUI.Instance.UpdateDisplayBar(BattleSceneUI.Instance.heathBarPlayer, N["hpPlayer1"].AsFloat, Mathf.Round((float)adapter.logic.attStatus.getCurrentIndex(Indexes.max_hp_na)));
                    BattleSceneUI.Instance._meCharacterHP.text = ((int)me._baseProperties.hp).ToString() + "/" + adapter.logic.attStatus.getCurrentIndex(Indexes.max_hp_na).ToString();
                    BattleSceneUI.Instance.UpdateDisplayBar(BattleSceneUI.Instance.heathBarEnemy, N["hpPlayer2"].AsFloat, Mathf.Round((float)adapter.logic.defStatus.getCurrentIndex(Indexes.max_hp_na)));

                    BattleSceneUI.Instance._enemyCharacterHP.text = ((int)you._baseProperties.hp).ToString() + "/" + adapter.logic.defStatus.getCurrentIndex(Indexes.max_hp_na).ToString();
                }
                else if (adapter.getTurn() == 2)
                {

                    BattleSceneUI.Instance.UpdateDisplayBar(BattleSceneUI.Instance.heathBarPlayer, N["hpPlayer1"].AsFloat, Mathf.Round((float)adapter.logic.defStatus.getCurrentIndex(Indexes.max_hp_na)));
                    BattleSceneUI.Instance._meCharacterHP.text = ((int)me._baseProperties.hp).ToString() + "/" + adapter.logic.defStatus.getCurrentIndex(Indexes.max_hp_na).ToString();
                    BattleSceneUI.Instance.UpdateDisplayBar(BattleSceneUI.Instance.heathBarEnemy, N["hpPlayer2"].AsFloat, Mathf.Round((float)adapter.logic.attStatus.getCurrentIndex(Indexes.max_hp_na)));

                    BattleSceneUI.Instance._enemyCharacterHP.text = ((int)you._baseProperties.hp).ToString() + "/" + adapter.logic.attStatus.getCurrentIndex(Indexes.max_hp_na).ToString();
                }
            }
            else if (me.playerId == 2)
            {
                damagePlayer1 = (int)(N["hpPlayer1"].AsFloat - you._baseProperties.hp);
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
                you._baseProperties.hp = N["hpPlayer1"].AsFloat;
                if (me._baseProperties.hp <= 0)
                {
                    me._baseProperties.hp = 0;
                }
                if (you._baseProperties.hp <= 0)
                {
                    you._baseProperties.hp = 0;
                }

                if (adapter.getTurn() == 1)
                {

                    BattleSceneUI.Instance.UpdateDisplayBar(BattleSceneUI.Instance.heathBarEnemy, N["hpPlayer1"].AsFloat, Mathf.Round((float)adapter.logic.attStatus.getCurrentIndex(Indexes.max_hp_na)));
                    BattleSceneUI.Instance._meCharacterHP.text = ((int)me._baseProperties.hp).ToString() + "/" + adapter.logic.attStatus.getCurrentIndex(Indexes.max_hp_na).ToString();
                    BattleSceneUI.Instance.UpdateDisplayBar(BattleSceneUI.Instance.heathBarPlayer, N["hpPlayer2"].AsFloat, Mathf.Round((float)adapter.logic.defStatus.getCurrentIndex(Indexes.max_hp_na)));

                    BattleSceneUI.Instance._enemyCharacterHP.text = ((int)you._baseProperties.hp).ToString() + "/" + adapter.logic.defStatus.getCurrentIndex(Indexes.max_hp_na).ToString();
                }
                else if (adapter.getTurn() == 2)
                {

                    BattleSceneUI.Instance.UpdateDisplayBar(BattleSceneUI.Instance.heathBarEnemy, N["hpPlayer1"].AsFloat, Mathf.Round((float)adapter.logic.defStatus.getCurrentIndex(Indexes.max_hp_na)));
                    BattleSceneUI.Instance._enemyCharacterHP.text = ((int)you._baseProperties.hp).ToString() + "/" + adapter.logic.defStatus.getCurrentIndex(Indexes.max_hp_na).ToString();
                    BattleSceneUI.Instance.UpdateDisplayBar(BattleSceneUI.Instance.heathBarPlayer, N["hpPlayer2"].AsFloat, Mathf.Round((float)adapter.logic.attStatus.getCurrentIndex(Indexes.max_hp_na)));

                    BattleSceneUI.Instance._meCharacterHP.text = ((int)me._baseProperties.hp).ToString() + "/" + adapter.logic.attStatus.getCurrentIndex(Indexes.max_hp_na).ToString();
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
                you._actionPoints = 0;
                BattleSceneUI.Instance.UpdateDisplayBar(BattleSceneUI.Instance.actionPointBarPlayer, me._actionPoints, 18f);
                BattleSceneUI.Instance._meCharacterActionPoint.text = "Action Point: " + me._actionPoints.ToString();
                BattleSceneUI.Instance.UpdateDisplayBar(BattleSceneUI.Instance.actionPointBarEnemy, you._actionPoints, 18f);
                BattleSceneUI.Instance._enemyCharacterActionPoint.text = "Action Point: " + you._actionPoints.ToString();

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
                    damagePlayer2 = (int)(N["hpPlayer2"].AsFloat - you._baseProperties.hp);

                    if (damagePlayer2 > 0)
                    {
                        UpdateDameEffect(damagePlayer2, 2);
                    }
                    else if (damagePlayer2 < 0)
                    {
                        UpdateDameEffect(damagePlayer2, 2);
                    }
                    me._baseProperties.hp = N["hpPlayer1"].AsFloat;
                    you._baseProperties.hp = N["hpPlayer2"].AsFloat;
                    if (me._baseProperties.hp <= 0)
                    {
                        me._baseProperties.hp = 0;
                    }
                    if (you._baseProperties.hp <= 0)
                    {
                        you._baseProperties.hp = 0;
                    }
                    if (adapter.getTurn() == 1)
                    {

                        BattleSceneUI.Instance.UpdateDisplayBar(BattleSceneUI.Instance.heathBarPlayer, N["hpPlayer1"].AsFloat, Mathf.Round((float)adapter.logic.attStatus.getCurrentIndex(Indexes.max_hp_na)));
                        BattleSceneUI.Instance._meCharacterHP.text = ((int)me._baseProperties.hp).ToString() + "/" + adapter.logic.attStatus.getCurrentIndex(Indexes.max_hp_na).ToString();
                        BattleSceneUI.Instance.UpdateDisplayBar(BattleSceneUI.Instance.heathBarEnemy, N["hpPlayer2"].AsFloat, Mathf.Round((float)adapter.logic.defStatus.getCurrentIndex(Indexes.max_hp_na)));

                        BattleSceneUI.Instance._enemyCharacterHP.text = ((int)you._baseProperties.hp).ToString() + "/" + adapter.logic.defStatus.getCurrentIndex(Indexes.max_hp_na).ToString();
                    }
                    else if (adapter.getTurn() == 2)
                    {

                        BattleSceneUI.Instance.UpdateDisplayBar(BattleSceneUI.Instance.heathBarPlayer, N["hpPlayer1"].AsFloat, Mathf.Round((float)adapter.logic.defStatus.getCurrentIndex(Indexes.max_hp_na)));
                        BattleSceneUI.Instance._meCharacterHP.text = ((int)me._baseProperties.hp).ToString() + "/" + adapter.logic.defStatus.getCurrentIndex(Indexes.max_hp_na).ToString();
                        BattleSceneUI.Instance.UpdateDisplayBar(BattleSceneUI.Instance.heathBarEnemy, N["hpPlayer2"].AsFloat, Mathf.Round((float)adapter.logic.attStatus.getCurrentIndex(Indexes.max_hp_na)));

                        BattleSceneUI.Instance._enemyCharacterHP.text = ((int)you._baseProperties.hp).ToString() + "/" + adapter.logic.attStatus.getCurrentIndex(Indexes.max_hp_na).ToString();
                    }
                }
                else if (me.playerId == 2)
                {
                    damagePlayer1 = (int)(N["hpPlayer1"].AsFloat - you._baseProperties.hp);
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
                    you._baseProperties.hp = N["hpPlayer1"].AsFloat;
                    if (me._baseProperties.hp <= 0)
                    {
                        me._baseProperties.hp = 0;
                    }
                    if (you._baseProperties.hp <= 0)
                    {
                        you._baseProperties.hp = 0;
                    }
                    if (adapter.getTurn() == 1)
                    {

                        BattleSceneUI.Instance.UpdateDisplayBar(BattleSceneUI.Instance.heathBarPlayer, N["hpPlayer2"].AsFloat, Mathf.Round((float)adapter.logic.defStatus.getCurrentIndex(Indexes.max_hp_na)));
                        BattleSceneUI.Instance._meCharacterHP.text = ((int)me._baseProperties.hp).ToString() + "/" + adapter.logic.defStatus.getCurrentIndex(Indexes.max_hp_na).ToString();
                        BattleSceneUI.Instance.UpdateDisplayBar(BattleSceneUI.Instance.heathBarEnemy, N["hpPlayer1"].AsFloat, Mathf.Round((float)adapter.logic.attStatus.getCurrentIndex(Indexes.max_hp_na)));

                        BattleSceneUI.Instance._enemyCharacterHP.text = ((int)you._baseProperties.hp).ToString() + "/" + adapter.logic.attStatus.getCurrentIndex(Indexes.max_hp_na).ToString();
                    }
                    else if (adapter.getTurn() == 2)
                    {

                        BattleSceneUI.Instance.UpdateDisplayBar(BattleSceneUI.Instance.heathBarEnemy, N["hpPlayer1"].AsFloat, Mathf.Round((float)adapter.logic.defStatus.getCurrentIndex(Indexes.max_hp_na)));
                        BattleSceneUI.Instance._enemyCharacterHP.text = ((int)you._baseProperties.hp).ToString() + "/" + adapter.logic.defStatus.getCurrentIndex(Indexes.max_hp_na).ToString();
                        BattleSceneUI.Instance.UpdateDisplayBar(BattleSceneUI.Instance.heathBarPlayer, N["hpPlayer2"].AsFloat, Mathf.Round((float)adapter.logic.attStatus.getCurrentIndex(Indexes.max_hp_na)));

                        BattleSceneUI.Instance._meCharacterHP.text = ((int)me._baseProperties.hp).ToString() + "/" + adapter.logic.attStatus.getCurrentIndex(Indexes.max_hp_na).ToString();
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
        NewCharacterStatus _temp = adapter.logic.getStatusByPlayerID(_playerId);
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
            if (me.playerId == playerid)
            {
                damageText.transform.position = new Vector3(me.transform.position.x, me.transform.position.y + me.transform.GetComponent<BoxCollider2D>().bounds.size.y, me.transform.position.z);
            }
            else if (you.playerId == playerid)
            {
                damageText.transform.position = new Vector3(you.transform.position.x, you.transform.position.y + you.transform.GetComponent<BoxCollider2D>().bounds.size.y, you.transform.position.z);
            }
        }
        else
        {
            if (me.playerId == playerid)
            {
                GameObject damagedEffect = EffectManager.Instance.CreateEffect("Effect/ParticlesHeal", me.transform);
                damagedEffect.transform.position = new Vector3(me.transform.position.x, me.transform.position.y + me.transform.gameObject.transform.GetComponent<BoxCollider2D>().bounds.size.y / 1.5f, me.transform.position.z);
            }
            else if (you.playerId == playerid)
            {
                GameObject damagedEffect = EffectManager.Instance.CreateEffect("Effect/ParticlesHeal", you.transform);
                damagedEffect.transform.position = new Vector3(you.transform.position.x, you.transform.position.y + you.transform.gameObject.transform.GetComponent<BoxCollider2D>().bounds.size.y / 1.5f, you.transform.position.z);
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
        if (me.playerId == playerid)
        {
            damageText.transform.position = new Vector3(me.transform.position.x, me.transform.position.y + me.transform.GetComponent<BoxCollider2D>().bounds.size.y / 2, me.transform.position.z);
        }
        else if (you.playerId == playerid)
        {
            damageText.transform.position = new Vector3(you.transform.position.x, you.transform.position.y + you.transform.GetComponent<BoxCollider2D>().bounds.size.y / 2, you.transform.position.z);
        }
    }

    public void UpdateEffectStaticCharacter(string _nameEff, string _spriteSrc, int _duration, int _playerid, bool _isSkill, bool _isEffOp)
    {
        Debug.Log("play id " + _playerid);
        if (me.playerId == _playerid)
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
                    OnHoverToEffect(effectObj, _nameEff, _duration.ToString(), new Vector3(-500,0, 0));
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
        else if (you.playerId == _playerid)
        {
            Debug.Log("render in you");
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
                        CreateAbnormalStatusEffectOnCharacter(_nameEff, you.transform, true);
                }
            }

        }
    }

    public void ChooseAction(NewSkill skill, Transform _thisButton)
    {

        if (adapter.getStatus() == Status.IN_TURN && adapter.logic.getStatusByPlayerID(me.playerId).character.newSkillDic["Skill" + skill.data["idInit"].ToString()].getCoolDown() == 0 && adapter.logic.getStatusByPlayerID(me.playerId).character.newSkillDic["Skill" + skill.data["idInit"].ToString()].canCastSkill(adapter.logic))
        {
            if (me.playerId == adapter.getTurn())
            {

                if (!me._listAction.Contains(CharacterItemInGame.Instance.LoadActionHandle(me._listAction, skill.data["idInit"].AsInt)))
                {
                    ActionHandle _action = new ActionHandle(2, skill.data["idInit"].AsInt, 2f, me._listAction.Count + 1);
                    me._listAction.Add(_action);
                    _orderButtonActionList.Add(_thisButton.gameObject);
                    _thisButton.GetChild(1).gameObject.SetActive(true);
                    //_thisButton.GetChild(1).GetComponent<Text>().text = (_action.index).ToString();
                    _thisButton.DOLocalMoveY(150f, 0.1f);
                    me._actionPoints -= adapter.getStatusOfMe().getApsOfSkill(skill.getID());//skill.data["aps"].AsInt;
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
                    me._actionPoints += adapter.getStatusOfMe().getApsOfSkill(skill.getID()); //skill.data["aps"].AsInt;
                    BattleSceneUI.Instance.UpdateDisplayBar(BattleSceneUI.Instance.actionPointBarPlayer, CharacterManager.Instance._meCharacter._actionPoints, 18f);
                    BattleSceneUI.Instance._meCharacterActionPoint.text = "Action Point: " + me._actionPoints.ToString();
                }



            }

            UpdateSkillState();
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
        string name = ""+_name;
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



    public void UpdateSkillState()// Update action
    {
        int actionPointMe = 0;

        actionPointMe = me._actionPoints;
        //skillObj.transform.GetChild(2).GetChild(0).GetComponent<Text>().text = skill.data["aps"].ToString();
        //playerSkillDatawithObjectKey.Add(skillObj, skill.data["aps"].AsInt);
        //playerSkillDatawithSkillKey.Add("Skill" + skill.data["idInit"].AsInt.ToString(), skillObj);
        //playerskillwithObjectDictionary.Add(skill, skillObj);

        foreach (NewSkill skill in SplitDataFromServe._heroSkill)
        {

            if (skill.data["typewear"].AsInt == 1 && skill.data["type"].Value != "passive")
            {
                
                GameObject skillObj = playerSkillDatawithSkillKey["Skill" + skill.getID()];
                playerSkillDatawithObjectKey.Remove(skillObj);
                
                playerSkillDatawithObjectKey.Add(skillObj, adapter.getStatusOfMe().getApsOfSkill(skill.getID()));
                skillObj.transform.GetChild(2).GetChild(0).GetComponent<Text>().text = playerSkillDatawithObjectKey[skillObj].ToString();
                Debug.Log(skill.getNick() + "_aps" + " Update Skill" + skill.getID() + " " + adapter.getStatusOfMe().getApsOfSkill(skill.getID()) + " "+ playerSkillDatawithObjectKey[skillObj]);
            }
            else if (skill.data["type"].Value == "passive")
            {

            }
        }

        foreach (Transform child in BattleSceneUI.Instance._panelSkill.transform)
        {
            try
            {
                if (playerSkillDatawithObjectKey.ContainsKey(child.GetChild(0).gameObject))
                {
                    if (me.playerId == adapter.getTurn())
                    {
                        child.GetChild(0).GetChild(1).gameObject.SetActive(true);
                    }
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



    public void BeginRollDice()
    {
        BattleSceneUI.Instance.InfoDiceDialog.text = "You have 0 action points";
        BattleSceneUI.Instance.DiceDialog.gameObject.SetActive(true);
        if (BattleSystemManager.Instance.playerTurn == false)
        {
            BattleSceneUI.Instance.DiceBtn.gameObject.SetActive(false);
        }
        else
        {
            BattleSceneUI.Instance.DiceBtn.gameObject.SetActive(true);
        }
        BattleSystemManager.Instance.battleStates = PerformAction.DICE;
    }




    public void RollingDice()
    {
        Debug.Log("adapter " + adapter);
        bool isRolling = adapter.rollDice();
        if (isRolling)
        {
            diceValue = adapter.getDiceResult().dice1 + adapter.getDiceResult().dice2 + adapter.getDiceResult().dice3;
        }
        else diceValue = 0;

    }

    bool callOneTimeDice = false;

    public void DisplayDicePoint(int dicePoint1, int dicePoint2, int dicePoint3)
    {
        if (!callOneTimeDice)
        {
            callOneTimeDice = true;
            if (me.playerId == adapter.getTurn())
            {
                me._actionPoints = dicePoint1 + dicePoint2 + dicePoint3;
            }
            else if (you.playerId == adapter.getTurn())
            {
                you._actionPoints = dicePoint1 + dicePoint2 + dicePoint3;
            }
            StartCoroutine(ExecuteDisplayDicePoint(dicePoint1, dicePoint2, dicePoint3));
        }
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
        if (me.playerId == adapter.getTurn())
        {
            BattleSceneUI.Instance.UpdateDisplayBar(BattleSceneUI.Instance.actionPointBarPlayer, me._actionPoints, 18f);
            BattleSceneUI.Instance._meCharacterActionPoint.text = "Action Point: " + me._actionPoints.ToString();
        }
        else if (you.playerId == adapter.getTurn())
        {
            BattleSceneUI.Instance.UpdateDisplayBar(BattleSceneUI.Instance.actionPointBarEnemy, you._actionPoints, 18f);
            BattleSceneUI.Instance._enemyCharacterActionPoint.text = "Action Point: " + you._actionPoints.ToString();
        }
        _orderButtonActionList.Clear();
        UpdateSkillState();

        if (this.adapter.isMaster)
        {
            adapter.Log("RollDice Master", "Finish");
            if (adapter.isMyTurn(adapter.attStatus))
            {
                adapter.status = Status.IN_TURN;
                if (adapter.beginResult.continued)
                    adapter.eventsTimeouter = adapter.createTimeouterForSendingData(Status.IN_TURN, Status.END_TURN, () => { }, 30 * 1000, 1, false);
                else adapter.status = Status.END_TURN; // neu khong tiep tuc thi ket thuc luon

            }
            else
            {
                adapter.status = Status.IN_TURN;
                if (!adapter.isOffline && adapter.beginResult.continued)
                {
                    adapter.eventsTimeouter = adapter.createTimeouterForSendingData(Status.IN_TURN, Status.END_TURN, () => { }, 30 * 1000, 1, false);
                }
                else if (!adapter.beginResult.continued)
                {
                    adapter.status = Status.END_TURN; // neu khong tiep tuc thi ket thuc luon
                }

            }
        }
        else
        {
            adapter.Log("RollDice Slave", "Finish");
            if (adapter.isMyTurn(adapter.attStatus))
            {
                adapter.status = Status.IN_TURN;
                if (adapter.beginResult.continued) // den lan minh thi neu tieo tuc thi se tiep tuc
                    adapter.eventsTimeouter = adapter.createTimeouterForSendingData(Status.IN_TURN, Status.IN_TURN_WAITING, () => { }, 30 * 1000, 1, false);
                else adapter.status = Status.END_TURN_WAITING;// khong tiep tuc thi chuyen toi status truoc EndTurn do slave khong hoat dong o EndTurn ngay

            }
            else
            {
                adapter.status = Status.IN_TURN;
                if (adapter.beginResult.continued) // den lan minh thi neu tieo tuc thi se tiep tuc
                    adapter.eventsTimeouter = adapter.createTimeouterForSendingData(Status.IN_TURN, Status.IN_TURN_WAITING, () => { }, 30 * 1000, 1, false);
                else adapter.status = Status.END_TURN_WAITING;// khong tiep tuc thi chuyen toi status truoc EndTurn do slave khong hoat dong o EndTurn ngay
            }
            this.adapter.processNewPacInSlave();
        }
        adapter.inResult = null;
        callOneTimeDice = false;
    }


    //void EnemyRollingDice()
    //{
    //    BattleSceneUI.Instance.BattleBegin.gameObject.SetActive(false);
    //    BattleSceneUI.Instance.DiceDialog.SetActive(true);
    //}

    void GetDataEnemyRollingDice(List<int> _listEnemyDataDice)
    {
        if (_listEnemyDataDice.Count > 0)
            BattleSceneUI.Instance.TurnOfPlayerName.gameObject.SetActive(false);
        BattleSceneUI.Instance.DiceDialog.SetActive(true);
        int totalPoint = 0;
        int dicePoint1 = _listEnemyDataDice[0];
        int dicePoint2 = _listEnemyDataDice[1];
        int dicePoint3 = _listEnemyDataDice[2];
        totalPoint = dicePoint1 + dicePoint2 + dicePoint3;
        CharacterManager.Instance._enemyCharacter._actionPoints += totalPoint;
        List<object> dicePointList = new List<object>();
        dicePointList.Add(dicePoint1);
        dicePointList.Add(dicePoint2);
        dicePointList.Add(dicePoint3);
        //StartCoroutine(GenerateDicePoint(dicePoint1, dicePoint2, dicePoint3, totalPoint));
    }


    //void UpdateActionOrderEnemyUI(List<int> _listActionHandleDataId)
    //{
    //    for(int i=0;i< _listActionHandleDataId.Count;i++)
    //    {
    //        if (enemySkillDataWithKeyId.ContainsKey(_listActionHandleDataId[i]))
    //        {

    //            enemySkillDataWithKeyId[_listActionHandleDataId[i]].GetComponent<Button>().interactable = true;
    //            enemySkillDataWithKeyId[_listActionHandleDataId[i]].transform.GetChild(0).GetChild(0).gameObject.SetActive(true);
    //            enemySkillDataWithKeyId[_listActionHandleDataId[i]].transform.GetChild(0).GetChild(0).GetComponent<Text>().text =(i+1).ToString();
    //        } 
    //    }
    //}





    public void DoActionOrderList()
    {
        Debug.Log("DO ACTION LIST !!");
        ArrayList skills = new ArrayList();
        foreach (ActionHandle _tempAct in me._listAction)
        {
            skills.Add(_tempAct.idSkill);
        }
        //if (skills.Count == 0) return;

        adapter.attackWithSkills(skills);
        me._listAction.Clear();
        foreach (GameObject child in _orderButtonActionList)
        {
            child.transform.DOLocalMoveY(0f, 0.1f);

        }
        BattleSceneUI.Instance.isOrderAction = false;
    }

    void ResetUISkill()
    {
        foreach (GameObject child in _orderButtonActionList)
        {
            child.transform.DOLocalMoveY(0f, 0.1f);

        }
    }

    //void GetDataEnemyListActionHandle(List<string> _listActionHandleData)
    //{
    //    _statesString.Clear();
    //    List<int> _tempIdAct = new List<int>();
    //    foreach (String temp in _listActionHandleData)
    //    {
    //        _statesString.Add(temp);
    //        var N = JSON.Parse(temp);
    //        int idAction = N["idSkill"].AsInt;
    //        _tempIdAct.Add(idAction);
    //    }
    //    UpdateActionOrderEnemyUI(_tempIdAct);
    //    //StartCoroutine(ExecuteDataBatle());
    //    //CharacterManager.Instance._enemyCharacter._listAction = _listActionHandleData;
    //    //UpdateActionOrderEnemyUI(_listActionHandleData);
    //    //DoActionOrderList();
    //}
    bool callOneTimeInturn = false;
    public void RenderDataBattleState()
    {
        if (callOneTimeInturn == false)
        {
            callOneTimeInturn = true;
            StartCoroutine(ExecuteDataBatle());
        }
    }

    IEnumerator ExecuteDataBatle()
    {
        int index = 0;
        foreach (State state in adapter.inResult.states)
        {
            Debug.Log("run anim");
            //Log("state in inTurn UpdateMeInMaster", state.toJSON().ToString());
            var N = state.toJSON();
            if (me.playerId == N["attacker"].AsInt)
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
                                if (me.playerId == 1)
                                    CharacterManager.Instance._meCharacter.GetComponent<AnimationController>().UseSkill(CharacterManager.Instance._meCharacter, CharacterManager.Instance._enemyCharacter, N["attacker"].AsInt, index + 1, N["hpPlayer2"].AsInt, _tempSkill);
                                else
                                    CharacterManager.Instance._meCharacter.GetComponent<AnimationController>().UseSkill(CharacterManager.Instance._meCharacter, CharacterManager.Instance._enemyCharacter, N["attacker"].AsInt, index + 1, N["hpPlayer1"].AsInt, _tempSkill);
                            }
                            else if (_tempSkill.data["isMove"].AsBool == true)
                            {
                                if (CharacterManager.Instance._meCharacter._baseProperties.typeAttack == 0)
                                {
                                    if (me.playerId == 1)
                                        CharacterManager.Instance._meCharacter.GetComponent<AnimationController>().MeeleAttack(CharacterManager.Instance._meCharacter, CharacterManager.Instance._enemyCharacter, N["attacker"].AsInt, index + 1, N["hpPlayer2"].AsInt, false, _tempSkill);
                                    else
                                        CharacterManager.Instance._meCharacter.GetComponent<AnimationController>().MeeleAttack(CharacterManager.Instance._meCharacter, CharacterManager.Instance._enemyCharacter, N["attacker"].AsInt, index + 1, N["hpPlayer1"].AsInt, false, _tempSkill);
                                }
                            }
                        } else
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
                            if (me.playerId == 1)
                                CharacterManager.Instance._meCharacter.GetComponent<AnimationController>().MeeleAttack(CharacterManager.Instance._meCharacter, CharacterManager.Instance._enemyCharacter, N["attacker"].AsInt, index + 1, N["hpPlayer2"].AsInt, true, null);
                            else
                                CharacterManager.Instance._meCharacter.GetComponent<AnimationController>().MeeleAttack(CharacterManager.Instance._meCharacter, CharacterManager.Instance._enemyCharacter, N["attacker"].AsInt, index + 1, N["hpPlayer1"].AsInt, true, null);
                        }
                        else if (me._baseProperties.typeAttack == 1)
                        {
                            if (me.playerId == 1)
                                CharacterManager.Instance._meCharacter.GetComponent<AnimationController>().RangeAttack(CharacterManager.Instance._meCharacter, CharacterManager.Instance._enemyCharacter, N["attacker"].AsInt, index + 1, N["hpPlayer2"].AsInt, true, null);
                            else
                                CharacterManager.Instance._meCharacter.GetComponent<AnimationController>().RangeAttack(CharacterManager.Instance._meCharacter, CharacterManager.Instance._enemyCharacter, N["attacker"].AsInt, index + 1, N["hpPlayer1"].AsInt, true, null);
                        }
                    } else
                    {
                        CharacterManager.Instance._meCharacter.GetComponent<AnimationController>().Donothing(CharacterManager.Instance._meCharacter, CharacterManager.Instance._meCharacter, N["attacker"].AsInt, index + 1, N["hpPlayer1"].AsInt, null);
                    }
                }
            }
            else if (you.playerId == N["attacker"].AsInt)
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
                                if (you.playerId == 1)
                                    CharacterManager.Instance._enemyCharacter.GetComponent<AnimationController>().UseSkill(CharacterManager.Instance._meCharacter, CharacterManager.Instance._enemyCharacter, N["attacker"].AsInt, index + 1, N["hpPlayer2"].AsInt, _tempSkill);
                                else
                                    CharacterManager.Instance._enemyCharacter.GetComponent<AnimationController>().UseSkill(CharacterManager.Instance._meCharacter, CharacterManager.Instance._enemyCharacter, N["attacker"].AsInt, index + 1, N["hpPlayer1"].AsInt, _tempSkill);
                            }
                            else if (_tempSkill.data["isMove"].AsBool == true)
                            {
                                if (you._baseProperties.typeAttack == 0)
                                {
                                    if (you.playerId == 1)
                                        CharacterManager.Instance._enemyCharacter.GetComponent<AnimationController>().MeeleAttack(CharacterManager.Instance._enemyCharacter, CharacterManager.Instance._meCharacter, N["attacker"].AsInt, index + 1, N["hpPlayer2"].AsInt, false, _tempSkill);
                                    else
                                        CharacterManager.Instance._enemyCharacter.GetComponent<AnimationController>().MeeleAttack(CharacterManager.Instance._enemyCharacter, CharacterManager.Instance._meCharacter, N["attacker"].AsInt, index + 1, N["hpPlayer1"].AsInt, false, _tempSkill);
                                }
                            }
                        } else
                        {
                            CharacterManager.Instance._enemyCharacter.GetComponent<AnimationController>().Donothing(CharacterManager.Instance._enemyCharacter, CharacterManager.Instance._enemyCharacter, N["attacker"].AsInt, index + 1, N["hpPlayer2"].AsInt, null);
                        }
                    }
                }
                else if (N["idSkill"].AsInt == 0)
                {
                    if (N["attacker"].AsInt != N["defender"].AsInt)
                    {
                        if (you._baseProperties.typeAttack == 0)
                        {
                            if (you.playerId == 1)
                                CharacterManager.Instance._enemyCharacter.GetComponent<AnimationController>().MeeleAttack(CharacterManager.Instance._enemyCharacter, CharacterManager.Instance._meCharacter, N["attacker"].AsInt, index + 1, N["hpPlayer2"].AsInt, true, null);
                            else
                                CharacterManager.Instance._enemyCharacter.GetComponent<AnimationController>().MeeleAttack(CharacterManager.Instance._enemyCharacter, CharacterManager.Instance._meCharacter, N["attacker"].AsInt, index + 1, N["hpPlayer1"].AsInt, true, null);
                        }
                        else if (you._baseProperties.typeAttack == 1)
                        {
                            if (you.playerId == 1)
                                CharacterManager.Instance._enemyCharacter.GetComponent<AnimationController>().RangeAttack(CharacterManager.Instance._enemyCharacter, CharacterManager.Instance._meCharacter, N["attacker"].AsInt, index + 1, N["hpPlayer2"].AsInt, true, null);
                            else
                                CharacterManager.Instance._enemyCharacter.GetComponent<AnimationController>().RangeAttack(CharacterManager.Instance._enemyCharacter, CharacterManager.Instance._meCharacter, N["attacker"].AsInt, index + 1, N["hpPlayer1"].AsInt, true, null);
                        }
                    } else
                    {
                        CharacterManager.Instance._enemyCharacter.GetComponent<AnimationController>().Donothing(CharacterManager.Instance._enemyCharacter, CharacterManager.Instance._enemyCharacter, N["attacker"].AsInt, index + 1, N["hpPlayer2"].AsInt, null);
                    }
                }
            }
            yield return new WaitForSeconds(1.5f);
            index++;
        }

        if (index == adapter.inResult.states.Count)
        {
            if (this.adapter.isMaster)
            {
                adapter.Log("InturnRender Master", "Finish");
                if (adapter.isMyTurn(adapter.attStatus))
                {
                    adapter.status = Status.END_TURN;
                }
                else adapter.status = Status.END_TURN;
            }
            else
            {
                adapter.Log("InturnRender Slave", "Finish");
                if (adapter.isMyTurn(adapter.attStatus)) adapter.status = Status.END_TURN_WAITING;
                else adapter.status = Status.END_TURN_WAITING;
                this.adapter.processNewPacInSlave();
            }
            adapter.endResult = null;
            callOneTimeInturn = false;
        }
    }

    void ExecuteEachDataStateInBattle(CharacterPlayer _attacker, CharacterPlayer _defender, int _idAction, int _indexState, int _health)
    {
        if (_idAction == 0)
        {
            if (_attacker._baseProperties.typeAttack == 0)
            {
                _attacker.GetComponent<AnimationController>().MeeleAttack(_attacker, _defender, _idAction, _indexState, _health, false, null);
            }
            else if (_attacker._baseProperties.typeAttack == 1)
            {
                _attacker.GetComponent<AnimationController>().RangeAttack(_attacker, _defender, _idAction, _indexState, _health, false, null);
            }

        }
        else
        {
            _attacker.GetComponent<AnimationController>().UseSkill(CharacterManager.Instance._meCharacter, CharacterManager.Instance._enemyCharacter, _idAction, _indexState, _health, null);
        }

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
                    you.GetComponent<AnimationController>().Die(playerid, you.transform);
                }
                else
                {
                    you.GetComponent<AnimationController>().GetDamage(playerid, you.transform);
                }

            }
            else if (you.playerId == playerid)
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


    public void UpdateSkillCooldownOnCharacter()
    {
        NewCharacterStatus meStatus = adapter.logic.getStatusByPlayerID(me.playerId);
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
        else if (you.playerId == playerId)
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

    public void ShowGameOverPanel(int playerId)
    {
        PlayerPrefs.SetString("idroom", "");
        PlayerPrefs.SetString("nameroom", "");
        if (me.playerId == playerId)
        {

            BattleSceneUI.Instance._gameOverPanel.SetActive(true);
            BattleSceneUI.Instance._gameOverPanel.transform.GetChild(0).gameObject.SetActive(false);
            BattleSceneUI.Instance._gameOverPanel.transform.GetChild(1).gameObject.SetActive(true);
            BattleSceneUI.Instance._gameOverPanel.transform.GetChild(2).GetComponent<Text>().color = Color.green;
            BattleSceneUI.Instance._gameOverPanel.transform.GetChild(2).GetComponent<Text>().text = "Rank point +100";
            this.PostEvent(EventID.LeaveRoom);
        }
        else
        {
            BattleSceneUI.Instance._gameOverPanel.SetActive(true);
            BattleSceneUI.Instance._gameOverPanel.transform.GetChild(0).gameObject.SetActive(true);
            BattleSceneUI.Instance._gameOverPanel.transform.GetChild(1).gameObject.SetActive(false);
            BattleSceneUI.Instance._gameOverPanel.transform.GetChild(2).GetComponent<Text>().color = Color.red;
            BattleSceneUI.Instance._gameOverPanel.transform.GetChild(2).GetComponent<Text>().text = "Rank point -100";
            this.PostEvent(EventID.LeaveRoom);
        }
    }



    private void OnDestroy()
    {

        EventDispatcher.Instance.RemoveListener(EventID.OnCharacterOrderAction, _DoActionOrderListEventRef);
        EventDispatcher.Instance.RemoveListener(EventID.OnCharacterUpdateUIState, _OnCharacterUpdateUIStateEventRef);
        EventDispatcher.Instance.RemoveListener(EventID.OnEnemyReceivedDataBattleSucessfully, _OnExecuteDataBattleEventRef);
        EventDispatcher.Instance.RemoveListener(EventID.OnRollingDice, _OnRollingDice);
        SocketIOController.Instance.DisableListenersInBattleScene();
    }

}
