using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PerformAction {
  WAIT=0,
  DICE,   
  PlAYERTURN,
  ENEMYTURN,
}

public enum TypeMatch
{
    BO1=0,
    BO2,
    BO3,
}

public class BattleSystemManager : MonoBehaviour {


    public static BattleSystemManager Instance;


    public TypeMatch typeMatch;
    public int countBattleInMatch = 0;

    public int winCountPlayer = 0;
    public int winCountEnemy = 0;
    public int turn = 0;
    public bool playerTurn = true;
    public bool callAIFuction = false;
    public bool endBattle = false;

    public PerformAction battleStates;
    // Use this for initialization

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            // If that is the case, we destroy other instances
            Destroy(gameObject);
        }

        // Here we save our singleton instance
        Instance = this;

        // Furthermore we make sure that we don't destroy between scenes (this is optional)
        DontDestroyOnLoad(gameObject);
    }

    void Start () {

        //CreateBattleInfo();
	}
	
    public void CreateBattleInfo()
    {
        typeMatch = TypeMatch.BO1;
        countBattleInMatch = 1;
        winCountEnemy = 0;
        winCountPlayer = 0;
        turn = 0;
        playerTurn = false ;
        endBattle = false;
        battleStates = PerformAction.WAIT;
        //this.PostEvent(EventID.CreateBattle);
    }
	// Update is called once per frame
	void Update () {
        switch (battleStates)
        {
            case (PerformAction.WAIT):
                break;
            case (PerformAction.DICE):
                //if (playerTurn == false)
                //    EnemyAutoRollDice();
                break;
            case (PerformAction.PlAYERTURN):

                break;
            case (PerformAction.ENEMYTURN):
                //if (callAIFuction==false)
                //{
                //    if (BattleSystemManager.Instance.endBattle == false)
                //    {
                //        CharacterManager.Instance._enemyCharacter.GetComponent<CharacterAI>().ChooseActionAI();
                //    }
                //    callAIFuction = true;
                //} 
                break;
        }
	}

    public void EnemyAutoRollDice()
    {
        
        this.PostEvent(EventID.OnRollingDice, BattleSystemManager.Instance.playerTurn);
        battleStates = PerformAction.WAIT;
    }
}
