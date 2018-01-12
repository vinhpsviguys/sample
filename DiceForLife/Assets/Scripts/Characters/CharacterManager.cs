using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;
using System;
using CoreLib;

public class CharacterManager : MonoBehaviour {


    public static CharacterManager Instance;

    public CharacterPlayer _meCharacter;
    public CharacterPlayer _enemyCharacter;


    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            this.gameObject.tag = "DontDestroyObject";
        }
        else
        {
            // If that is the case, we destroy other instances
            Destroy(gameObject);
        }
        // Furthermore we make sure that we don't destroy between scenes (this is optional)
    }

   

    public void CreatePlayerProperties()
    {
        GameObject playerMe = Instantiate(CharacterItemInGame.Instance._characterPrefabs[CharacterInfo._instance._baseProperties._classCharacter.ToString()]);
        playerMe.transform.position = new Vector3(-5.5f, -2f, 0f);
        playerMe.transform.localScale = Vector3.one;
        _meCharacter = playerMe.AddComponent<CharacterPlayer>();
        CharacterPlayer.LoadCharacterPlayer(_meCharacter);
        

        playerMe.AddComponent<AnimationController>();
        ApplyPropertyPlayer();
        //UpdatePropertiesCharacterUI();
        //UpdateEffectCharacter();
    }

    public void CreateMonster(string typeMap,int levelMonster)
    {
        switch (typeMap)
        {
            case "Caimpaign":
             JSONNode dataMonster= JSON.Parse(CharacterItemInGame.Instance._listTextAssetDataMonsterCaim[levelMonster-1].ToString());
                GameObject monsterObj = Instantiate(CharacterItemInGame.Instance._monstercaimPrefabs["Monster"+levelMonster]);
                monsterObj.transform.position = new Vector3(4.8f, -1f, 0f);
                monsterObj.transform.localScale = new Vector3(-0.8f,0.8f,0.8f);
                _enemyCharacter = monsterObj.AddComponent<CharacterPlayer>();
                CharacterPlayer.LoadCharacterMonster(_enemyCharacter,dataMonster);
                //monsterObj.AddComponent<AnimationController>();
                ReadMonsterSkillData(dataMonster);
                break;
            case "DailyChallenge":
                break;
            case "ForbiddenLand":
                break;
        }
    }

    void ReadMonsterSkillData(JSONNode dataMonster)
    {
        SplitDataFromServe._enemySkill.Clear();
        for (int i=0;i < dataMonster["skills"].Count; i++)
        {
            foreach (NewSkill _tempSkill in SplitDataFromServe.skillInit.Values)
            {
                JSONNode dataMonsterSkill = JSON.Parse(dataMonster["skills"][i].ToString());
                if (dataMonsterSkill["idskill"].AsInt == _tempSkill.data["idInit"].AsInt && !SplitDataFromServe._enemySkill.Contains(_tempSkill))
                {
                    _tempSkill.addField("level", dataMonsterSkill["level"].AsInt);
                    SplitDataFromServe._enemySkill.Add(_tempSkill);

                }
            }
        }
    }


    public void CreateEnemyProperties(int id, string idCode)
    {

        StartCoroutine(LoadDataDetailEnemyHero(id,idCode));
      
    }

    IEnumerator LoadDataDetailEnemyHero(int _idhero, string _idcode)
    {

        StartCoroutine(ServerAdapter.ListSkillOfHero(_idhero, _idcode, result =>
        {
            if (result.StartsWith("Error"))
            {
                Debug.Log("Do nothing");
            }
            else
            {
                SplitDataFromServe.ReadEnemySkillData(result.ToString());
            }
        }));

        WWWForm form = new WWWForm();
        form.AddField("idh", _idhero);
        form.AddField("idcode", _idcode);
        WWW w = new WWW(Constant.urlRequest + "/api/hero/detail", form);
        yield return w;
        string data = w.text;
        var N = JSONNode.Parse(data);
        if (Boolean.Parse(N["return"]))
        {
            SplitDataFromServe.ReadDetailDataHeroEnemyPlay(N["data"].ToString());
            GameObject enemy = Instantiate(CharacterItemInGame.Instance._characterPrefabs[ConvertIdClassToClassName(SplitDataFromServe._heroEnemyPlay.idclass).ToString()]);
            enemy.transform.position = new Vector3(5.5f, -2f, 0f);
            enemy.transform.localScale = new Vector3(-1f, 1f, 1f);
            _enemyCharacter = enemy.AddComponent<CharacterPlayer>();
           CharacterPlayer.LoadCharacterEnemy(_enemyCharacter);
            //enemy.AddComponent<AnimationController>();
            enemy.SetActive(false);
            WaitingRoomUI.Instance.SetLog("Load data nhan vat thanh cong");
            if (!SocketIOController.Instance.isReconnect)
            {
                WaitingRoomUI.Instance.SetLog("Tao timeout confirm load nhan vat thanh cong");
                this.PostEvent(EventID.CreateTimeoutConfirmLoadData);
            }
            if (SocketIOController.Instance.isReconnect)
            {
                this.PostEvent(EventID.ReconnectBattleScene);
            }
            WatingRoomController.Instance.state_waitingroom = STATEINWAITING.CONFIRM_LOADDATA;
        }
        else if (!Boolean.Parse(N["return"]))
        {
            Debug.Log("Do nothing");
        }
        // if Master create timeout for done B request
        // if Slave send done slave, create timeout for done A response

    }




   

    public void ApplyPropertyPlayer()
    {

     
        this.PostEvent(EventID.OnInitMECharacter);
       

    }
    public void ApplyPropertyEnemy()
    {
        this.PostEvent(EventID.OnInitEnemyCharacter);
    }

    public bool isPlayerCharacter(int _idChar)
    {
        if (_meCharacter._baseProperties.idHero == _idChar)
            return true;
       else
           return false;
    }

    ClassCharacter ConvertIdClassToClassName(int id)
    {
        ClassCharacter tempClass = ClassCharacter.None;
        switch (id)
        {
            case 1:
                tempClass = ClassCharacter.Assassin;
                break;
            case 2:
                tempClass = ClassCharacter.Paladin;
                break;
            case 3:
                tempClass = ClassCharacter.Cleric;
                break;
            case 4:
                tempClass = ClassCharacter.Sorceress;
                break;
            case 5:
                tempClass = ClassCharacter.Wizard;
                break;
            case 6:
                tempClass = ClassCharacter.Marksman;
                break;
            case 7:
                return ClassCharacter.Orc;
                break;
            case 8:
                tempClass = ClassCharacter.Barbarian;
                break;
        }
        return tempClass;
    }
}
