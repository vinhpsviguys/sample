using DG.Tweening;
using Spine.Unity;
using UnityEngine;
using UnityEngine.UI;
using CoreLib;


public class PlayerShowFittingRoom : MonoBehaviour
{
    public SkeletonAnimation skeletonAnimation;
    [SerializeField]
    private Button _closeBtn;

    [SerializeField]
    private Button btnInforOff, btnFittingOff;
    [SerializeField]
    private GameObject btnInforOn, btnFittingOn;

    [SerializeField] private Transform _leftObject;
    [SerializeField]
    private GameObject _inforPanel, _fittingPanel, _bagHeroPanel;
    [SerializeField]
    private GameObject _skillContentPanel, _skillInfoObj;

    [SerializeField]
    private Text _nameCharacterText, _fightingPowerTxt, _expText, _vitalityTxt, _strengthTxt, _intellText;
    [SerializeField]
    private Text _classCharacterNameText, _heathText, _physicDmgText, _physicDefText, _magicDmgText, _magicDefText;


    private GameObject tempHero = null;

    private void Awake()
    {
        Constants.init();
    }

  
    private void Start()
    {
        tempHero = Instantiate(CharacterItemInGame.Instance._characterPrefabs[CharacterInfo._instance._baseProperties._classCharacter.ToString()]);
        tempHero.transform.SetParent(_leftObject);
        tempHero.transform.localPosition = new Vector3(-495f, -238f, 0f);
        skeletonAnimation = tempHero.GetComponent<SkeletonAnimation>();
        tempHero.GetComponent<MeshRenderer>().sortingOrder = 3;
    }

    void OnEnable()
    {

        btnInforOff.onClick.AddListener(ActiveInforPanel);
        btnFittingOff.onClick.AddListener(ActiveFittingPanel);
        _closeBtn.onClick.AddListener(CloseThisDialog);

        ActiveInforPanel();
    }

    private void OnDisable()
    {
        //Destroy(tempHero);
        btnInforOff.onClick.RemoveListener(ActiveInforPanel);
        btnFittingOff.onClick.RemoveListener(ActiveFittingPanel);
        _closeBtn.onClick.RemoveListener(CloseThisDialog);
    }

    private void ActiveInforPanel()
    {
        _inforPanel.SetActive(true);
        _fittingPanel.SetActive(false);
        btnInforOn.SetActive(true);
        btnFittingOn.SetActive(false);

        UpdateTxtValueProperties();

        _leftObject.gameObject.SetActive(true);
        _leftObject.localPosition = new Vector3(-1000, 0);
        _leftObject.DOLocalMoveX(0, 0.35f).SetEase(Ease.OutBack);
        _bagHeroPanel.SetActive(false);
    }
    private void ActiveFittingPanel()
    {
        _inforPanel.SetActive(false);
        _fittingPanel.SetActive(true);
        btnInforOn.SetActive(false);
        btnFittingOn.SetActive(true);

        _leftObject.gameObject.SetActive(false);
        _bagHeroPanel.SetActive(true);
    }

    void CloseThisDialog()
    {
        this.gameObject.SetActive(false);
    }

    public void UpdateTxtValueProperties()
    {
        CharacterPlayer _tempPlayer = new CharacterPlayer();
        CharacterPlayer.LoadCharacterPlayer(_tempPlayer);
        NewCharacterStatus _tempNewCharacter = new NewCharacterStatus(_tempPlayer);
        _tempNewCharacter.loadDynamicIndexToMid();
        _nameCharacterText.text = CharacterInfo._instance._baseProperties.name.ToString();
        _fightingPowerTxt.text = CharacterInfo._instance._baseProperties.AttributePoint.ToString();
        _expText.text = CharacterInfo._instance._baseProperties.Level.ToString();
        _vitalityTxt.text = "" + (float)_tempNewCharacter.getMidIndex(Indexes.vit_na);
        _strengthTxt.text = "" + (float)_tempNewCharacter.getMidIndex(Indexes.str_na);
        _intellText.text = "" + (float)_tempNewCharacter.getMidIndex(Indexes.int_na);

       
        _heathText.text = ""+(float)_tempNewCharacter.getMidIndex(Indexes.max_hp_na);
        _physicDmgText.text = ""+Mathf.RoundToInt(((float)_tempNewCharacter.getMidIndex(Indexes.min_pda_na) + (float)_tempNewCharacter.getMidIndex(Indexes.max_pda_na)) * 0.5f);
         //Debug.Log( _tempNewCharacter.getMidIndex(Indexes.min_pda_na) + " - " + _tempNewCharacter.getMidIndex(Indexes.max_pda_na));
        _physicDefText.text = "" + Mathf.RoundToInt(((float)_tempNewCharacter.getMidIndex(Indexes.min_pde_na) + (float) _tempNewCharacter.getMidIndex(Indexes.max_pde_na)) * 0.5f);
        //_tempNewCharacter.getMidIndex(Indexes.min_pde_na) + " - " + _tempNewCharacter.getMidIndex(Indexes.max_pde_na);
        _magicDmgText.text = "" + Mathf.RoundToInt(((float)_tempNewCharacter.getMidIndex(Indexes.min_mda_na) + (float) _tempNewCharacter.getMidIndex(Indexes.max_mda_na)) * 0.5f);
        // _tempNewCharacter.getMidIndex(Indexes.min_mda_na) + " - " + _tempNewCharacter.getMidIndex(Indexes.max_mda_na);
        _magicDefText.text = "" + Mathf.RoundToInt(((float)_tempNewCharacter.getMidIndex(Indexes.min_mde_na) + (float)_tempNewCharacter.getMidIndex(Indexes.max_mde_na)) * 0.5f);
        //_tempNewCharacter.getMidIndex(Indexes.min_mde_na) + " - " + _tempNewCharacter.getMidIndex(Indexes.max_mde_na);
    }

    public void AddAttributePoint(int id)
    {
        if (CharacterInfo._instance._baseProperties.AttributePoint >= 1)
        {
            switch (id)
            {
                case 0:
                    StartCoroutine(ServerAdapter.AddAttributePoint(CharacterInfo._instance._baseProperties.idHero, CharacterInfo._instance._baseProperties.idCodeHero, 1, "vitality", result =>
                    {
                        if (result.StartsWith("Error"))
                        {
                            Debug.Log("Do nothing");
                        }
                        else
                        {
                            CharacterInfo._instance._baseProperties.AttributePoint--;
                            CharacterInfo._instance._baseProperties.Vitality++;
                            UpdateTxtValueProperties();
                        }
                    }));
                    break;
                case 1:
                    StartCoroutine(ServerAdapter.AddAttributePoint(CharacterInfo._instance._baseProperties.idHero, CharacterInfo._instance._baseProperties.idCodeHero, 1, "strength", result =>
                    {
                        if (result.StartsWith("Error"))
                        {
                            Debug.Log("Do nothing");
                        }
                        else
                        {
                            CharacterInfo._instance._baseProperties.AttributePoint--;
                            CharacterInfo._instance._baseProperties.Strength++;
                            UpdateTxtValueProperties();
                        }
                    }));
                    break;
                case 2:
                    StartCoroutine(ServerAdapter.AddAttributePoint(CharacterInfo._instance._baseProperties.idHero, CharacterInfo._instance._baseProperties.idCodeHero, 1, "intelligence", result =>
                    {
                        if (result.StartsWith("Error"))
                        {
                            Debug.Log("Do nothing");
                        }
                        else
                        {
                            CharacterInfo._instance._baseProperties.AttributePoint--;
                            CharacterInfo._instance._baseProperties.Intelligence++;
                            UpdateTxtValueProperties();
                        }
                    }));
                    break;
            }
        }
    }
}
