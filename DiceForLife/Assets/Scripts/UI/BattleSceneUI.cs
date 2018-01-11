using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Xml;
using DG.Tweening;

public class BattleSceneUI : MonoBehaviour {

    public static BattleSceneUI Instance;

    public Vector2 _meCharacterPos;
    public Text _meCharacterName;
    public Text _meCharacterHP;
    public Text _meLevel;
    public Text _meCharacterActionPoint;
    public Image heathBarPlayer;
    public Image actionPointBarPlayer;
    public Image avatarMe;

    public Vector2 _enemyCharacterPos;
    public Text _enemyCharacterName;
    public Text _enemyCharacterHP;
    public Text _enemyLevel;
    public Text _enemyCharacterActionPoint;
    public Image heathBarEnemy;
    public Image actionPointBarEnemy;
    public Image avatarEnemy;


    public Text TypeOfMatch;
    public Sprite[] winloseSprite;
   
    public Text TurnOfPlayerName;
    public Text TimeCountText;
    public Text PlayerPos;
    public Text StatusTurn;
    public Text NumberSkill;
    public Text OnlineDetect;

    public GameObject DiceDialog;
    public Text InfoDiceDialog;
    public Button DiceBtn;

    public GameObject DialogInfo, SurrenderPanel;

    public GameObject EffectPossitiveSkillMe, EffectNegativeSkillMe;
    public GameObject EffectPossitiveSkillEnemy, EffectNegativeSkillEnemy;
    public GameObject ListSkillPassive;


    public Sprite[] _listSpriteSkill;
    public Sprite[] _listSpriteEffect;
    public Sprite[] _boundEffect;

    public GameObject _panelSkill;
    public Button _startOrderAction;


    public GameObject _panelLoading;

    public GameObject _gameOverPanel;

    public GameObject _effectParentMe,_effectParentEnemy;

    public GameObject _meBuffPanel, _enemyBuffPanel;
    public GameObject _dice1, _dice2, _dice3;
    public Button _gameOverOk,_surrenderBtn, _leaveGameBtn, _resumeBtn, _toggleMeBuffBtn,_toggleEnemyBuffBtn;

    bool isShowMeBuff = false;
    bool isShowEnemyBuff = false;
    bool isSocketOff = false;
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
        //DontDestroyOnLoad(gameObject);


        _listSpriteSkill = Resources.LoadAll<Sprite>("Textures/Skill");
        _listSpriteEffect= Resources.LoadAll<Sprite>("Textures/Buff");


        DiceBtn.onClick.AddListener(CallEventRollDice);
        _startOrderAction.onClick.AddListener(CallEventStartOrderAction);
        _gameOverOk.onClick.AddListener(BackToMainMenu);
        _leaveGameBtn.onClick.AddListener(SurrenderAndLeaveGame);
        _resumeBtn.onClick.AddListener(ResumeGame);
        _surrenderBtn.onClick.AddListener(DisplaySurrenderPanel);
        _toggleMeBuffBtn.onClick.AddListener(ShowHideMeBuffPanel);
        _toggleEnemyBuffBtn.onClick.AddListener(ShowHideEnemyBuffPanel);
    }

    private void Start()
    {
        isSocketOff = false;
        _gameOverPanel.SetActive(false);
    }
    private void OnEnable()
    {
        _meBuffPanel.transform.DOLocalMoveX(-250f-960f, 0f);
        _enemyBuffPanel.transform.DOLocalMoveX(0f+960f, 0f);
    }
    private void Update()
    {
        if(!SocketIOController.sfs.mySocket.IsOpen && !isSocketOff)
        {
            isSocketOff = true;
            WaitingPanelScript._instance.ShowWaiting(true);
        }
    }

    void ShowHideMeBuffPanel()
    {
        if (!isShowMeBuff)
        {
            _meBuffPanel.transform.DOLocalMoveX(0 - 960f, 0f).SetEase(Ease.InOutElastic).OnComplete(() => { isShowMeBuff = true; });
        } else
        {
            _meBuffPanel.transform.DOLocalMoveX(-250f - 960f, 0f).SetEase(Ease.InOutElastic).OnComplete(()=> { isShowMeBuff = false; });
        }
    }
    void ShowHideEnemyBuffPanel()
    {
        if (!isShowEnemyBuff)
        {
            _enemyBuffPanel.transform.DOLocalMoveX(-250f + 960f, 0f).SetEase(Ease.InOutElastic).OnComplete(() => { isShowEnemyBuff = true; });
        }
        else
        {
            _enemyBuffPanel.transform.DOLocalMoveX(0f + 960f, 0f).SetEase(Ease.InOutElastic).OnComplete(() => { isShowEnemyBuff = false; });
        }
    }

    public void CallEventRollDice()
    {
       
        this.PostEvent(EventID.OnRollingDice);
         //BattleSystemManager.Instance.battleStates = PerformAction.WAIT;
        if (DiceBtn.interactable==true)
        {
            DiceBtn.interactable = false;
        }
    }

    public void CallEventStartOrderAction()
    {
         this.PostEvent(EventID.OnCharacterOrderAction);
    }

    public void ShowDialog(string name,string des, Vector3 pos)
    {
       
        DialogInfo.SetActive(true);
        DialogInfo.transform.localPosition = pos;
        DialogInfo.transform.GetChild(0).GetComponent<Text>().text = name;
        DialogInfo.transform.GetChild(1).GetComponent<Text>().text = des;
    }
    public void HideDialog()
    {
        DialogInfo.SetActive(false);
    }


    public void UpdateDisplayBar(Image bar, float currentValue, float maxValue)
    {
        bar.fillAmount = currentValue / maxValue;
        if (currentValue / maxValue >= 1)
        {
            bar.fillAmount = 1;
        }
    }


    public IEnumerator WaitToBackMainMenu()
    {
        Destroy(GameObject.Find("EffectManger"));
        Destroy(GameObject.Find("BattleManagerSystem"));
        //GameObject tempObj = CharacterManager.Instance._meCharacter.gameObject;
        if (CharacterManager.Instance._meCharacter != null)
        {
            Destroy(CharacterManager.Instance._meCharacter.gameObject);
            CharacterManager.Instance._meCharacter = null;
        }
        if (CharacterManager.Instance._enemyCharacter != null)
        {
            Destroy(CharacterManager.Instance._enemyCharacter.gameObject);
            CharacterManager.Instance._enemyCharacter = null;

        }
        Destroy(CharacterManager.Instance);
        Destroy(BattleSceneController.Instance);
        yield return new WaitForSeconds(0.5f);
        SceneLoader._instance.LoadScene(3);
        //Application.LoadLevel(3);


    }



    public void BackToMainMenu()
    {
        StartCoroutine(WaitToBackMainMenu());
    }

    void DisplaySurrenderPanel()
    {
        SurrenderPanel.SetActive(true);
    }

    void SurrenderAndLeaveGame()
    {
        this.PostEvent(EventID.LeaveRoom);
        BackToMainMenu();
    }
    void ResumeGame()
    {
        SurrenderPanel.SetActive(false);
    }
}
