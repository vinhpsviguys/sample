using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CoreLib;

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

    // Use this for initialization
    void Start () {
        me = CharacterManager.Instance._meCharacter;
        me.loadDictionaries(SplitDataFromServe._heroSkill, SplitDataFromServe._enemySkill, SplitDataFromServe._heroAbs);
        monster = CharacterManager.Instance._enemyCharacter;
        monster.loadDictionaries(SplitDataFromServe._enemySkill, SplitDataFromServe._heroSkill, SplitDataFromServe._heroAbs);

        CreateBattleSceneUI();
        SetupLogicGame();
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
        defStatus = logic.getStatusByPlayerID(2);
        Debug.Log("Start");

    }


    // Update is called once per frame
    void Update () {
		
	}
}
