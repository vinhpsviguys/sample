using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

public class CharacterItemInGame : MonoBehaviour
{
    public static CharacterItemInGame Instance;
    private const string textAssetsSkillAssetBundle = "skilldata";
    private const string textAssetsAbnormalAssetBundle = "abnormalstatusdata";
    private const string textAssetMonsterCaimpaign = "monsterdatacaimpaign";
    private const string AssetMonsterPrefab = "monsterprefabcaimpaign";
    public string[] listClass = new string[] { "Barbarian", "Orc", "Marksman", "Wizard", "Sorceress", "Cleric", "Paladin", "Assassin" };
    public Dictionary<string, GameObject> _characterPrefabs = new Dictionary<string, GameObject>();
    public Dictionary<string, GameObject> _monstercaimPrefabs = new Dictionary<string, GameObject>();
    public Dictionary<string, Sprite> _characterAvatars = new Dictionary<string, Sprite>();

    public Sprite[] _listSpriteSkill;
    public Sprite[] _listSpriteEffect;
    public Sprite[] _avatarRect;
    public Sprite[] _avatarCircle;

    public List<TextAsset> _listTextAssetSkill;
    public List<TextAsset> _listTextAssetAbnormal;
    public List<TextAsset> _listTextAssetDataMonsterCaim;

    ReadXML _myDataSKillXML;
    ReadXML _myDataEffectXML;
    ReadXML _myDataCharacterXML;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            // If that is the case, we destroy other instances
            Destroy(gameObject);
        }
        _listTextAssetSkill = new List<TextAsset>();
        _listTextAssetAbnormal = new List<TextAsset>();
        _myDataSKillXML = new ReadXML("XMLFiles/SKill");
        _myDataEffectXML = new ReadXML("XMLFiles/Effects");
        _myDataCharacterXML = new ReadXML("XMLFiles/Characters");
        _listSpriteSkill = Resources.LoadAll<Sprite>("Textures/Skill");
        _listSpriteEffect = Resources.LoadAll<Sprite>("Textures/Buff");
    }


    public IEnumerator GetTextAssetsMonsterCaimpaign()
    {
        Debug.Log("get text assets monster caimpaign");
        for (int i = 1; i <= 5; i++)
        {
            yield return StartCoroutine(LoadingResourceController._instance.LoadAssetBundleTextAssetsAsync(textAssetMonsterCaimpaign, "monster_caim_" + i, value =>
            {
                LoadingResource.Instance.FakeProgress(i / 5f);
                if (value == null) Debug.LogError("Không load được data");
                _listTextAssetDataMonsterCaim.Add(value);
                if (i == 5) {
                    StartCoroutine(GetTextAssetsSkillFiles());
                }
            }));
        }
    }

    public IEnumerator GetTextAssetsSkillFiles()
    {
        Debug.Log("get text assets skill");
        for (int i = 1; i <= 80; i++)
        {
            yield return StartCoroutine(LoadingResourceController._instance.LoadAssetBundleTextAssetsAsync(textAssetsSkillAssetBundle, "Skill" + i, value =>
             {
                 LoadingResource.Instance.FakeProgress(i / 80f);
                 if (value == null) Debug.LogError("Không load được data");
                 _listTextAssetSkill.Add(value);
                 if (i == 80) { LoadingResource.Instance.LoadSkillToDictionary(); }
             }));
        }
    }

    public IEnumerator GetTextAssetsAbsFiles()
    {
        Debug.Log("get text assets abs");
        for (int i = 1; i <= 20; i++)
        {
            yield return StartCoroutine(LoadingResourceController._instance.LoadAssetBundleTextAssetsAsync(textAssetsAbnormalAssetBundle, "AS" + i, value =>
            {
                LoadingResource.Instance.FakeProgress(i / 20f);
                if (value == null) Debug.LogError("Không load được data");
                _listTextAssetAbnormal.Add(value);
                if (i == 20) { LoadingResource.Instance.LoadAbsToDictionary(); }
            }));
        }
    }


    public IEnumerator ExecuteGetMonsterResource()
    {
        int countLoadedPrefabs = 0;
        for (int i = 1; i <=1 ; i++)
        {
            yield return StartCoroutine(LoadingResourceController._instance.LoadAssetBundleObjectAsync("monsterprefabcaimpaign","Monster"+i, result =>
            {
                
                _monstercaimPrefabs.Add("Monster" + i, result);
                countLoadedPrefabs++;
                if (countLoadedPrefabs == 1) {
                    LoadingResource.Instance.ExecuteGetCharacterPrefabs();
                    }

            }));
        }
    }



    public void GetCharacterResource()
    {
        StartCoroutine(ExecuteGetCharacterResource());
    }


    IEnumerator ExecuteGetCharacterResource()
    {
        for (int i = 0; i < SplitDataFromServe._heroInits.Length; i++)
        {
            yield return StartCoroutine(LoadingResourceController._instance.LoadAssetBundleSpriteAsync("character", "Hero" + i, result =>
            {
                _characterAvatars.Add(listClass[i], result);
            }));
            yield return StartCoroutine(LoadingResourceController._instance.LoadAssetBundleObjectAsync(listClass[i].ToLower(), listClass[i], result =>
            {
                _characterPrefabs.Add(listClass[i], result);
            }));
        }
    }



    public String LoadModelCharacter(int id)
    {
        XmlNode _characterXml = _myDataCharacterXML.getDataByIndex(id - 1);
        return _characterXml.Attributes["pathmodel"].Value;
    }

    public String LoadPrefabsRangeAttack(int id)
    {
        XmlNode _characterXml = _myDataCharacterXML.getDataByIndex(id - 1);
        return _characterXml.Attributes["attackmile"].Value;
    }


    public EffectBuffCharacter LoadDataEffect(int id)
    {
        XmlNode _effectXml = _myDataEffectXML.getDataByIndex(id - 1);
        int _idEff = int.Parse(_effectXml.Attributes["id"].Value);
        EffectBuffType _typeEff = (EffectBuffType)System.Enum.Parse(typeof(EffectBuffType), _effectXml.Attributes["type"].Value);
        int _sourceEff = int.Parse(_effectXml.Attributes["source"].Value);
        string _nameEff = _effectXml.Attributes["name"].Value;
        string _desEff = _effectXml.Attributes["des"].Value + "abc";
        float _valueEff = float.Parse(_effectXml.Attributes["value"].Value);
        int _turnEff = int.Parse(_effectXml.Attributes["turn"].Value);
        return new EffectBuffCharacter(_idEff, _typeEff, _sourceEff, _nameEff, _desEff, _valueEff, _turnEff);

    }


    public Sprite GetIconEffect(int id)
    {
        XmlNode _myItemXml = _myDataEffectXML.getDataByIndex(id);
        Sprite _tempSprite = GetEffectSpriteByName(_myItemXml.Attributes["sprite"].Value);
        return _tempSprite;
    }

    public Sprite GetEffectSpriteByName(string name)
    {
        for (int i = 0; i < _listSpriteEffect.Length; i++)
        {
            if (_listSpriteEffect[i].name == name)
                return _listSpriteEffect[i];
        }
        return null;
    }



    public SkillCharacter LoadDataSkill(int initId, int id = 0)
    {
        string jsonData = PlayerPrefs.GetString(Constant.AllINIT);
        XmlNode _skillXml = _myDataSKillXML.getDataByIndex(initId - 1);


        ClassCharacter _classOfSkill = ClassCharacter.None;
        string name = "";
        string des = "";
        //string[] listIDeffectSkill = _skillXml.Attributes["effect"].Value.Split(',');
        int _actionPoint = 0;

        var N = SimpleJSON.JSON.Parse(jsonData);
        for (int i = 0; i < N["data"]["skill"].Count; i++)
        {
            if (initId == int.Parse(N["data"]["skill"][i]["idk"].Value))
            {
                _classOfSkill = (ClassCharacter)System.Enum.Parse(typeof(ClassCharacter), N["data"]["skill"][i]["forclass"].Value);
                name = N["data"]["skill"][i]["name"].Value;
                des = N["data"]["skill"][i]["description"].Value;
                _actionPoint = 5;
            }
        }

        return new SkillCharacter(id, initId, _classOfSkill, name, des, 1, 5, 0, 0, _actionPoint);
    }

    public string LoadSkillEffect(int id)
    {
        XmlNode _skillXml = _myDataSKillXML.getDataByIndex(id - 1);
        return _skillXml.Attributes["prefabeff"].Value;
    }

    public Sprite GetIconSkill(int id)
    {
        XmlNode _myItemXml = _myDataSKillXML.getDataByIndex(id - 1);
        Sprite _tempSprite = GetSkillSpriteByName(_myItemXml.Attributes["sprite"].Value);
        return _tempSprite;

    }

    public Sprite GetSkillSpriteByName(string name)
    {

        for (int i = 0; i < _listSpriteSkill.Length; i++)
        {
            if (_listSpriteSkill[i].name == name)
                return _listSpriteSkill[i];
        }
        return null;
    }

    public ActionHandle LoadActionHandle(List<ActionHandle> _listAct, int skillId)
    {

        foreach (ActionHandle act in _listAct)
        {

            if (act.idSkill == skillId)
            {

                return act;
            }
        }
        return null;
    }

}
