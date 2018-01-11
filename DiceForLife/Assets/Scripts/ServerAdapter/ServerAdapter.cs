using SimpleJSON;
using System.Collections;
using UnityEngine;

public class ServerAdapter
{
    private static string serverURL = "http://45.32.106.62/";

    private static string DataReturn(string input)
    {
        var N = JSON.Parse(input);
        if (N == null)
        {
            return "Error Code: Wrong struct";
        }
        bool returnValue = bool.Parse(N["return"].Value);        // versionString will be a string containing "1.0"
        if (returnValue)
            return N["data"].ToString();
        else
            return "Error Code:" + N["dataerror"].Value;
    }

    public static IEnumerator CheckServer(System.Action<string> result)
    {
        WWW www = new WWW(serverURL + "/api/initload/getversion");
        yield return www;
        if (!string.IsNullOrEmpty(www.error))
        {
            result("Error Code: " + www.error);
        }
        else
        {
            result(DataReturn(www.text));
        }
    }

    public static IEnumerator Login(string username, string password, string namePlayer, string deviceID, int typeOS, System.Action<string> result)
    {
        WWWForm form = new WWWForm();
        form.AddField("username", username);
        form.AddField("password", password);
        form.AddField("name", namePlayer);
        form.AddField("deviceid", deviceID);
        form.AddField("os", typeOS);
        // Upload to a cgi script
        WWW w = new WWW(serverURL + "/api/user/loginplayer", form);
        yield return w;
#if UNITY_EDITOR
        Debug.Log(string.Format("LOGIN! username:{0} password:{1} device:{2} name:{3} os:{4}", username, password, deviceID, namePlayer, typeOS));
#endif
        if (!string.IsNullOrEmpty(w.error))
        {
            result("Error Code: " + w.error);
        }
        else
        {
            result(DataReturn(w.text));
        }
    }
    public static IEnumerator SignUpAccount(string username, string password, string deviceID, System.Action<string> result)
    {
        WaitingPanelScript._instance.ShowWaiting(true);
        WWWForm form = new WWWForm();
        form.AddField("email", username);
        form.AddField("password", password);
        form.AddField("deviceid", deviceID);
        // Upload to a cgi script
        WWW w = new WWW(serverURL + "/api/user/linkedplayer", form);
        yield return w;
#if UNITY_EDITOR
        Debug.Log(string.Format("SIGN UP! email:{0} password:{1} deviceid:{2}", username, password, deviceID));
#endif
        WaitingPanelScript._instance.ShowWaiting(false);
        if (!string.IsNullOrEmpty(w.error))
        {
            result("Error Code: " + w.error);
        }
        else
        {
            result(DataReturn(w.text));
        }
    }
    public static IEnumerator SwitchAccount(string username, string password, string deviceID, System.Action<string> result)
    {
        WWWForm form = new WWWForm();
        form.AddField("username", username);
        form.AddField("password", password);
        form.AddField("deviceid", deviceID);

        WWW w = new WWW(serverURL + "/api/user/switchplayer", form);
        yield return w;
#if UNITY_EDITOR
        Debug.Log(string.Format("SWITCH ACCOUNT! username:{0} password:{1} deviceid:{2}", username, password, deviceID));
#endif
        if (!string.IsNullOrEmpty(w.error))
        {
            result("Error Code: " + w.error);
        }
        else
        {
            result(DataReturn(w.text));
        }
    }
    public static IEnumerator ChangePassword(string email, string oldpassword, string newpassword, System.Action<string> result)
    {
        WWWForm form = new WWWForm();
        form.AddField("email", email);
        form.AddField("oldpassword", oldpassword);
        form.AddField("newpassword", newpassword);

        WWW w = new WWW(serverURL + "/api/user/changepassplayer", form);
        yield return w;
#if UNITY_EDITOR
        Debug.Log(string.Format("CHANGE PASSWORD! email:{0} oldpassword:{1} newpassword:{2}", email, oldpassword, newpassword));
#endif
        if (!string.IsNullOrEmpty(w.error))
        {
            result("Error Code: " + w.error);
        }
        else
        {
            result(DataReturn(w.text));
        }
    }

    public static IEnumerator LoadInitData(System.Action<string> result)
    {
        WWWForm form = new WWWForm();
        form.AddField("", "");
        WWW w = new WWW(Constant.urlRequest + "/api/initload/allinit", form);
        yield return w;
        if (!string.IsNullOrEmpty(w.error))
        {
            result("Error Code: " + w.error);
        }
        else
        {
            result(DataReturn(w.text));
        }
    }

    public static IEnumerator LoadShopInit(System.Action<string> result)
    {
        WWWForm form = new WWWForm();
        form.AddField("", "");
        WWW w = new WWW(Constant.urlRequest + "/api/shop/listshop", form);
        yield return w;
        if (!string.IsNullOrEmpty(w.error))
        {
            result("Error Code: " + w.error);
        }
        else
        {
            result(DataReturn(w.text));
        }
    }

    public static IEnumerator LoadEquippedData(System.Action<string> result)
    {
        WWWForm form = new WWWForm();
        form.AddField("", "");
        WWW w = new WWW(Constant.urlRequest + "/api/initload/equipped", form);
        yield return w;
        if (!string.IsNullOrEmpty(w.error))
        {
            result("Error Code: " + w.error);
        }
        else
        {
            result(DataReturn(w.text));
        }
    }

    public static IEnumerator CheckNameCreateHero(string tempName, System.Action<string> result)
    {
        WWWForm form = new WWWForm();
        form.AddField("name", tempName);
        WWW www = new WWW(serverURL + "/api/user/checkname", form);
        yield return www;
#if UNITY_EDITOR
        Debug.Log("Result Check name Hero: " + www.text);
#endif
        yield return www;
        if (!string.IsNullOrEmpty(www.error))
        {
            result("Error Code: " + www.error);
        }
        else
        {
            result(DataReturn(www.text));
        }
    }
    public static IEnumerator ExecuteCreateHero(int idHeroSelected, string tempName, System.Action<string> result)
    {
        WWWForm form = new WWWForm();
        form.AddField("idcode", SplitDataFromServe._myAccount.idcode);
        form.AddField("idu", SplitDataFromServe._myAccount.idu);
        form.AddField("idih", idHeroSelected);
        form.AddField("name", tempName);
        WWW www = new WWW(serverURL + "/api/user/chosehero", form);
        yield return www;

#if UNITY_EDITOR
        Debug.Log("Result Create Hero: " + www.text);
#endif
        yield return www;
        if (!string.IsNullOrEmpty(www.error))
        {
            result("Error Code: " + www.error);
        }
        else
        {
            result(DataReturn(www.text));
        }
    }


    public static IEnumerator LoadDetailHero(string idcode, int idh, System.Action<string> result)
    {
        WWWForm form = new WWWForm();
        form.AddField("idh", idh);
        form.AddField("idcode", idcode);
        // Upload to a cgi script
        WWW w = new WWW(serverURL + "/api/hero/detail", form);
#if UNITY_EDITOR
        Debug.Log(string.Format("LOAD DETAIL! idh:{0} idcode:{1}", idh, idcode));
#endif
        yield return w;
        if (!string.IsNullOrEmpty(w.error))
        {
            result("Error Code: " + w.error);
        }
        else
        {
            result(DataReturn(w.text));
        }
    }



    public static IEnumerator ChangeName(string newName, string idCode, int idHero, System.Action<string> result)
    {
        WaitingPanelScript._instance.ShowWaiting(true);
        WWWForm form = new WWWForm();

        form.AddField("idh", idHero);
        form.AddField("idcode", idCode);
        form.AddField("name", newName);
        // Upload to a cgi script
        WWW w = new WWW(serverURL + "/api/hero/changename", form);
        yield return w;

        WaitingPanelScript._instance.ShowWaiting(false);
        if (!string.IsNullOrEmpty(w.error))
        {
            result("Error Code: " + w.error);
        }
        else
        {
            result(DataReturn(w.text));
        }
    }
    public static IEnumerator InitData(System.Action<string> result)
    {
        WWWForm form = new WWWForm();
        form.AddField("deviceid", "");
        // Upload to a cgi script
        WWW w = new WWW(serverURL + "/api/initload/allinit", form);
        yield return w;
        if (!string.IsNullOrEmpty(w.error))
        {
            result("Error Code: " + w.error);
        }
        else
        {
            result(DataReturn(w.text));
        }
    }
    public static IEnumerator ListSkillOfHero(int _idh, string _idcode, System.Action<string> result)
    {
        WWWForm form = new WWWForm();
        form.AddField("idh", _idh);
        form.AddField("idcode", _idcode);
        // Upload to a cgi script
        WWW w = new WWW(serverURL + "/api/hero/listskill", form);
        yield return w;
#if UNITY_EDITOR
        Debug.Log(string.Format("LOAD SKILL! idh:{0} idcode:{1}", _idh, _idcode));
#endif
        if (!string.IsNullOrEmpty(w.error))
        {
            result("Error Code: " + w.error);
        }
        else
        {
            result(DataReturn(w.text));
        }
    }


    public static IEnumerator EquipSkill(int _idh, string _idcode, int _idhk, System.Action<string> result)
    {
        WaitingPanelScript._instance.ShowWaiting(true);
        WWWForm form = new WWWForm();
        form.AddField("idh", _idh);
        form.AddField("idcode", _idcode);
        form.AddField("idhk", _idhk);
        // Upload to a cgi script
        WWW w = new WWW(serverURL + "/api/hero/usekill", form);
        yield return w;

        WaitingPanelScript._instance.ShowWaiting(false);
        if (!string.IsNullOrEmpty(w.error))
        {
            result("Error Code: " + w.error);
        }
        else
        {
            result(DataReturn(w.text));
        }
    }
    public static IEnumerator UnEquipSkill(int _idh, string _idcode, int _idhk, System.Action<string> result)
    {
        WaitingPanelScript._instance.ShowWaiting(true);
        WWWForm form = new WWWForm();
        form.AddField("idh", _idh);
        form.AddField("idcode", _idcode);
        form.AddField("idhk", _idhk);
        // Upload to a cgi script
        WWW w = new WWW(serverURL + "/api/hero/removeskill", form);
        yield return w;
        WaitingPanelScript._instance.ShowWaiting(false);
        if (!string.IsNullOrEmpty(w.error))
        {
            result("Error Code: " + w.error);
        }
        else
        {
            result(DataReturn(w.text));
        }
    }
    public static IEnumerator LearnSkill(int _idh, string _idcode, int _idk, string _type, int _skillPoint, System.Action<string> result)
    {
        WaitingPanelScript._instance.ShowWaiting(true);
        WWWForm form = new WWWForm();
        form.AddField("idh", _idh);
        form.AddField("idcode", _idcode);
        form.AddField("idk", _idk);
        form.AddField("type", _type);
        form.AddField("sp", _skillPoint);
        // Upload to a cgi script
        WWW w = new WWW(serverURL + "/api/hero/addskill", form);
        yield return w;
        WaitingPanelScript._instance.ShowWaiting(false);
        if (!string.IsNullOrEmpty(w.error))
        {
            result("Error Code: " + w.error);
        }
        else
        {
            result(DataReturn(w.text));
        }
    }
    public static IEnumerator UpLevelSkill(int _idh, string _idcode, int _idhk, int _levelcurrent, int _sprequire, System.Action<string> result)
    {
        WaitingPanelScript._instance.ShowWaiting(true);
        WWWForm form = new WWWForm();
        form.AddField("idh", _idh);
        form.AddField("idcode", _idcode);
        form.AddField("idhk", _idhk);
        form.AddField("levelcurrent", _levelcurrent);
        form.AddField("sp", _sprequire);
        // Upload to a cgi script
        WWW w = new WWW(serverURL + "/api/hero/uplevelskill", form);
        yield return w;
        WaitingPanelScript._instance.ShowWaiting(false);
        if (!string.IsNullOrEmpty(w.error))
        {
            result("Error Code: " + w.error);
        }
        else
        {
            result(DataReturn(w.text));
        }
    }

    public static IEnumerator ListItemInShop(System.Action<string> result)
    {
        WWWForm form = new WWWForm();
        form.AddField("", "");
        // Upload to a cgi script
        WWW w = new WWW(serverURL + "/api/shop/listshop", form);
        yield return w;
        if (!string.IsNullOrEmpty(w.error))
        {
            result("Error Code: " + w.error);
        }
        else
        {
            result(DataReturn(w.text));
        }
    }
    public static IEnumerator BuyItemInShop(int _idh, int _idcode, int _iditem, int _qtt, System.Action<string> result)
    {
        WWWForm form = new WWWForm();
        form.AddField("idh", _idh);
        form.AddField("idcode", _idcode);
        form.AddField("ids", _iditem);
        form.AddField("quantity", _qtt);
        // Upload to a cgi script
        WWW w = new WWW(serverURL + "/api/shop/addtiemshop", form);
        yield return w;
        if (!string.IsNullOrEmpty(w.error))
        {
            result("Error Code: " + w.error);
        }
        else
        {
            result(DataReturn(w.text));
        }
    }

    public static IEnumerator GetVersion(System.Action<string> result)
    {
        WWW www = new WWW(serverURL + "/api/initload/getversion");
        yield return www;
        if (!string.IsNullOrEmpty(www.error))
        {
            result("Error Code: " + www.error);
        }
        else
        {
            result(DataReturn(www.text));
        }
    }

    public static IEnumerator GetListPet(int _idh, int _idcode, System.Action<string> result)
    {
        WWWForm form = new WWWForm();
        form.AddField("idh", _idh);
        form.AddField("idcode", _idcode);
        WWW www = new WWW(serverURL + "/api/pet/listpet", form);
        yield return www;
        if (!string.IsNullOrEmpty(www.error))
        {
            result("Error Code: " + www.error);
        }
        else
        {
            result(DataReturn(www.text));
        }
    }
    public static IEnumerator AddPet(int _idh, int _idcode, int _idip, System.Action<string> result)
    {
        WWWForm form = new WWWForm();
        form.AddField("idh", _idh);
        form.AddField("idcode", _idcode);
        form.AddField("idip", _idip);
        WWW www = new WWW(serverURL + "/api/pet/addpet", form);
        yield return www;
        if (!string.IsNullOrEmpty(www.error))
        {
            result("Error Code: " + www.error);
        }
        else
        {
            result(DataReturn(www.text));
        }
    }

    public static IEnumerator AddCustomValue(int _idh, string _idcode, string type, int _qtt, System.Action<string> result)
    {

        WWWForm form = new WWWForm();
        form.AddField("idh", _idh);
        form.AddField("idcode", _idcode);
        form.AddField("type", type);
        form.AddField("quantity", _qtt);
        WWW www = new WWW(serverURL + "/api/hero/addcustom", form);
        yield return www;
        if (!string.IsNullOrEmpty(www.error))
        {
            result("Error Code: " + www.error);
        }
        else
        {
            result(DataReturn(www.text));
        }
    }

    public static IEnumerator ReduceCustomValue(int _idh, string _idcode, string type, int _qtt, System.Action<string> result)
    {

        WWWForm form = new WWWForm();
        form.AddField("idh", _idh);
        form.AddField("idcode", _idcode);
        form.AddField("type", type);
        form.AddField("quantity", _qtt);
        WWW www = new WWW(serverURL + "/api/hero/reduceturn", form);
        yield return www;
        if (!string.IsNullOrEmpty(www.error))
        {
            result("Error Code: " + www.error);
        }
        else
        {
            result(DataReturn(www.text));
        }
    }

    public static IEnumerator GetRewardFromMonster(int _idh, string _idcode, int _levelMonster, int _hpMonster, System.Action<string> result)
    {
        WaitingPanelScript._instance.ShowWaiting(true);
        WWWForm form = new WWWForm();
        form.AddField("idh", _idh);
        form.AddField("idcode", _idcode);
        form.AddField("level", _levelMonster);
        form.AddField("hp", _hpMonster);
        WWW www = new WWW(serverURL + "/api/fight/finishcampaign", form);
        yield return www;
        WaitingPanelScript._instance.ShowWaiting(false);
        if (!string.IsNullOrEmpty(www.error))
        {
            result("Error Code: " + www.error);
        }
        else
        {
            result(DataReturn(www.text));
        }
    }

    public static IEnumerator BuyItemInShop(int _idh, string _idcode, int _ids, int _qtt, System.Action<string> result)
    {
        WaitingPanelScript._instance.ShowWaiting(true);
        WWWForm form = new WWWForm();
        form.AddField("idh", _idh);
        form.AddField("idcode", _idcode);
        form.AddField("ids", _ids);
        form.AddField("quantity", _qtt);
        WWW www = new WWW(serverURL + "/api/shop/addtiemshop", form);
        yield return www;
#if UNITY_EDITOR
        Debug.Log("idh:" + _idh + " | idcode:" + _idcode + " | ids:" + _ids + " | quantity:" + _qtt);
        Debug.Log(www.text);
#endif
        WaitingPanelScript._instance.ShowWaiting(false);
        if (!string.IsNullOrEmpty(www.error))
        {
            result("Error Code: " + www.error);
        }
        else
        {

            result(DataReturn(www.text));
        }
    }

    public static IEnumerator UseItemInShop(int idh, string idcode, int idht, int quantity, System.Action<string> result)
    {
        WaitingPanelScript._instance.ShowWaiting(true);
        WWWForm form = new WWWForm();
        form.AddField("idh", idh);
        form.AddField("idcode", idcode);
        form.AddField("idht", idht);
        form.AddField("quantity", quantity);
        WWW www = new WWW(serverURL + "/api/items/useitem", form);
        yield return www;
#if UNITY_EDITOR
        Debug.Log("**USE ITEM** idh:" + idh + " | idcode:" + idcode + " | idht:" + idht + " | quantity:" + quantity);
        Debug.Log(www.text);
#endif
        WaitingPanelScript._instance.ShowWaiting(false);
        if (!string.IsNullOrEmpty(www.error))
        {
            result("Error Code: " + www.error);
        }
        else
        {

            result(DataReturn(www.text));
        }
    }

    public static IEnumerator SellItem(int _idh, string _idcode, int _idht, int _qtt, System.Action<string> result)
    {
        WaitingPanelScript._instance.ShowWaiting(true);
        WWWForm form = new WWWForm();
        form.AddField("idh", _idh);
        form.AddField("idcode", _idcode);
        form.AddField("idht", _idht);
        form.AddField("quantity", _qtt);
        WWW www = new WWW(serverURL + "/api/items/sellitem", form);
        yield return www;
        WaitingPanelScript._instance.ShowWaiting(false);
        if (!string.IsNullOrEmpty(www.error))
        {
            result("Error Code: " + www.error);
        }
        else
        {

            result(DataReturn(www.text));
        }
    }
    public static IEnumerator SellGem(int _idh, string _idcode, int _idhg, int _qtt, System.Action<string> result)
    {
        WaitingPanelScript._instance.ShowWaiting(true);
        WWWForm form = new WWWForm();
        form.AddField("idh", _idh);
        form.AddField("idcode", _idcode);
        form.AddField("idhg", _idhg);
        form.AddField("quantity", _qtt);
        WWW www = new WWW(serverURL + "/api/gem/sellgem", form);
        yield return www;
#if UNITY_EDITOR
        Debug.Log("idh:" + _idh + " | idcode:" + _idcode + " | idhg:" + _idhg);
        Debug.Log(www.text);
#endif
        WaitingPanelScript._instance.ShowWaiting(false);
        if (!string.IsNullOrEmpty(www.error))
        {
            result("Error Code: " + www.error);
        }
        else
        {

            result(DataReturn(www.text));
        }
    }
    public static IEnumerator SellEquipment(int _idh, string _idcode, int _ide, System.Action<string> result)
    {
        WaitingPanelScript._instance.ShowWaiting(true);
        WWWForm form = new WWWForm();
        //Debug.Log(_idh);
        form.AddField("idh", _idh);
        form.AddField("idcode", _idcode);
        form.AddField("ide", _ide);
        //Debug.Log(_ide);
        WWW www = new WWW(serverURL + "/api/equipped/sellequipped", form);
        yield return www;
        WaitingPanelScript._instance.ShowWaiting(false);

        if (!string.IsNullOrEmpty(www.error))
        {
            result("Error Code: " + www.error);
        }
        else
        {
            result(DataReturn(www.text));
        }
    }

    public static IEnumerator ExecuteRemoveItemOnHero(TypeEquipmentCharacter _typeItem, int idItem, System.Action<string> result)
    {
        WaitingPanelScript._instance.ShowWaiting(true);
        WWWForm form = new WWWForm();
        form.AddField("idh", CharacterInfo._instance._baseProperties.idHero);
        form.AddField("idcode", CharacterInfo._instance._baseProperties.idCodeHero);
        switch (_typeItem)
        {
            case TypeEquipmentCharacter.Weapon:
                form.AddField("ide_weapon", 0);
                break;
            case TypeEquipmentCharacter.Head:
                form.AddField("ide_head", 0);
                break;
            case TypeEquipmentCharacter.Shield:
                form.AddField("ide_shield", 0);
                break;
            case TypeEquipmentCharacter.OffhandWeapon:
                form.AddField("ide_shield", 0);
                break;
            case TypeEquipmentCharacter.Gloves:
                form.AddField("ide_gloves", 0);
                break;
            case TypeEquipmentCharacter.Boots:
                form.AddField("ide_boots", 0);
                break;
            case TypeEquipmentCharacter.Torso:
                form.AddField("ide_torso", 0);
                break;
            case TypeEquipmentCharacter.Belt:
                form.AddField("ide_belt", 0);
                break;
            case TypeEquipmentCharacter.Leg:
                form.AddField("ide_leg", 0);
                break;
            case TypeEquipmentCharacter.Ring:
                form.AddField("ide_ring", 0);
                break;
            case TypeEquipmentCharacter.Amulet:
                form.AddField("ide_amulet", 0);
                break;
            case TypeEquipmentCharacter.Buff:
                form.AddField("ide_buff", 0);
                form.AddField("un_buff", idItem);
                //Debug.LogError("Buff không được phép tháo");
                break;
            case TypeEquipmentCharacter.Avatar:
                //form.AddField("ide_avatar", 0);
                //form.AddField("un_avatar", idItem);
                Debug.LogError("Avatar không được phép tháo");
                //Debug.Log(string.Format("idh| {0} idcode| {1} un_avatar|{2}", SplitDataFromServe._heroCurrentPLay.idh, SplitDataFromServe._heroCurrentPLay.idcode, idItem));
                break;
        }
        // Upload to a cgi script
        WWW www = new WWW(serverURL + "/api/hero/wearweapon", form);
        yield return www;
        WaitingPanelScript._instance.ShowWaiting(false);
        if (!string.IsNullOrEmpty(www.error))
        {
            result("Error Code: " + www.error);
        }
        else
        {
            result(DataReturn(www.text));
        }

    }
    public static IEnumerator ExecuteChangeEquipment(EquipmentItem _tempItemEquip, System.Action<string> result)
    {
        WaitingPanelScript._instance.ShowWaiting(true);
        WWWForm form = new WWWForm();
        form.AddField("idh", SplitDataFromServe._heroCurrentPLay.idh);
        form.AddField("idcode", SplitDataFromServe._heroCurrentPLay.idcode);
        //#if UNITY_EDITOR
        //        Debug.Log(_tempItemEquip.typeItem);
        //        Debug.Log(FittingRoomController.lastIDSelected);
        //        Debug.Log(CharacterInfo._instance._baseProperties._classCharacter);
        //#endif
        switch (_tempItemEquip.typeItem)
        {
            case TypeEquipmentCharacter.Weapon:
                if (CharacterInfo._instance._baseProperties._classCharacter != ClassCharacter.Assassin && CharacterInfo._instance._baseProperties._classCharacter != ClassCharacter.Wizard)
                {
                    Debug.Log("fuck yeah");
                    form.AddField("ide_weapon", _tempItemEquip.idItem);
                }
                else
                {
                    if (FittingRoomController.lastIDSelected == 2)
                    {
                        Debug.Log("add vao slot shield " + _tempItemEquip.idItem);
                        form.AddField("ide_shield", _tempItemEquip.idItem);
                    }
                    else
                    {
                        form.AddField("ide_weapon", _tempItemEquip.idItem);
                    }
                }
                break;
            case TypeEquipmentCharacter.Head:
                form.AddField("ide_head", _tempItemEquip.idItem);
                break;
            case TypeEquipmentCharacter.Shield:
                form.AddField("ide_shield", _tempItemEquip.idItem);
                break;
            case TypeEquipmentCharacter.OffhandWeapon:
                form.AddField("ide_shield", _tempItemEquip.idItem);
                break;
            case TypeEquipmentCharacter.Gloves:
                form.AddField("ide_gloves", _tempItemEquip.idItem);
                break;
            case TypeEquipmentCharacter.Boots:
                form.AddField("ide_boots", _tempItemEquip.idItem);
                break;
            case TypeEquipmentCharacter.Torso:
                form.AddField("ide_torso", _tempItemEquip.idItem);
                break;
            case TypeEquipmentCharacter.Belt:
                form.AddField("ide_belt", _tempItemEquip.idItem);
                break;
            case TypeEquipmentCharacter.Leg:
                form.AddField("ide_leg", _tempItemEquip.idItem);
                break;
            case TypeEquipmentCharacter.Ring:
                form.AddField("ide_ring", _tempItemEquip.idItem);
                break;
            case TypeEquipmentCharacter.Amulet:
                form.AddField("ide_amulet", _tempItemEquip.idItem);
                break;
            case TypeEquipmentCharacter.Buff:
                form.AddField("ide_buff", _tempItemEquip.idItem);
                form.AddField("un_buff", _tempItemEquip.idItem);
                //Debug.Log(string.Format("idh| {0} idcode| {1} un_avatar|{2}", SplitDataFromServe._heroCurrentPLay.idh, SplitDataFromServe._heroCurrentPLay.idcode, _tempItemEquip.idItem));

                break;
            case TypeEquipmentCharacter.Avatar:
                //form.AddField("ide_avatar", _tempItemEquip.idItem);
                //form.AddField("un_avatar", _tempItemEquip.idItem);
                Debug.LogError("avatar không tháo và mặc được!");
                break;
        }
        // Upload to a cgi script
        WWW www = new WWW(serverURL + "/api/hero/wearweapon", form);
        yield return www;
        WaitingPanelScript._instance.ShowWaiting(false);

        if (!string.IsNullOrEmpty(www.error))
        {
            result("Error Code: " + www.error);
        }
        else
        {
            result(DataReturn(www.text));
        }
    }

    public static IEnumerator UpgradeReinforceEquipment(int _idh, string _idcode, int _ide, int _idhg, int _idht, System.Action<string> result)
    {
        WaitingPanelScript._instance.ShowWaiting(true);
        //Debug.Log("idh:" + _idh + " |idcode:" + _idcode + " |ide:" + _ide + " |idhg:" + _idhg + " |_idht:" + _idht);
        WWWForm form = new WWWForm();
        form.AddField("idh", _idh);
        form.AddField("idcode", _idcode);
        form.AddField("ide", _ide);
        form.AddField("idhg", _idhg);
        if (_idht != 0)
            form.AddField("idht", _idht);
        WWW www = new WWW(serverURL + "/api/gem/upgradeequipped", form);
        yield return www;
        WaitingPanelScript._instance.ShowWaiting(false);
        if (!string.IsNullOrEmpty(www.error))
        {
            result("Error Code: " + www.error);
        }
        else
        {
            result(DataReturn(www.text));
        }
    }
    public static IEnumerator UpgradeReinforceAvatar(int _idh, string _idcode, int idha, int _idhg, int _idht, System.Action<string> result)
    {
        WaitingPanelScript._instance.ShowWaiting(true);
        //Debug.Log("idh:" + _idh + " |idcode:" + _idcode + " |idha:" + idha + " |idhg:" + _idhg + " |_idht:" + _idht);
        WWWForm form = new WWWForm();
        form.AddField("idh", _idh);
        form.AddField("idcode", _idcode);
        form.AddField("idha", idha);
        form.AddField("idhg", _idhg);
        if (_idht != 0)
            form.AddField("idht", _idht);
        WWW www = new WWW(serverURL + "/api/gem/upgradeavatar", form);
        yield return www;
        WaitingPanelScript._instance.ShowWaiting(false);
        //Debug.Log(www.text);
        if (!string.IsNullOrEmpty(www.error))
        {
            result("Error Code: " + www.error);
        }
        else
        {
            result(DataReturn(www.text));
        }
    }
    public static IEnumerator UpgradeReinforceBook(int _idh, string _idcode, int idbf, int _idhg, int _idht, System.Action<string> result)
    {
        WaitingPanelScript._instance.ShowWaiting(true);
        //Debug.Log("idh:" + _idh + " |idcode:" + _idcode + " |idbf:" + idbf + " |idhg:" + _idhg + " |_idht:" + _idht);
        WWWForm form = new WWWForm();
        form.AddField("idh", _idh);
        form.AddField("idcode", _idcode);
        form.AddField("idbf", idbf);
        form.AddField("idhg", _idhg);
        if (_idht != 0)
            form.AddField("idht", _idht);
        WWW www = new WWW(serverURL + "/api/gem/upgradebuff", form);
        yield return www;
        WaitingPanelScript._instance.ShowWaiting(false);
        //Debug.Log(www.text);
        if (!string.IsNullOrEmpty(www.error))
        {
            result("Error Code: " + www.error);
        }
        else
        {
            result(DataReturn(www.text));
        }
    }
    public static IEnumerator UpgradeAddingRunestone(int _idh, string _idcode, int _ide, int _idhg, System.Action<string> result)
    {
        WaitingPanelScript._instance.ShowWaiting(true);
        WWWForm form = new WWWForm();
        form.AddField("idh", _idh);
        form.AddField("idcode", _idcode);
        form.AddField("ide", _ide);
        form.AddField("idhg", _idhg);
        WWW www = new WWW(serverURL + "/api/gem/uppropertiesequipped", form);
        yield return www;
#if UNITY_EDITOR
        Debug.Log("idh:" + _idh + " |idcode:" + _idcode + " |ide:" + _ide + " |idhg:" + _idhg);
        Debug.Log(www.text);
#endif
        WaitingPanelScript._instance.ShowWaiting(false);
        if (!string.IsNullOrEmpty(www.error))
        {
            result("Error Code: " + www.error);
        }
        else
        {
            result(DataReturn(www.text));
        }
    }
    public static IEnumerator UpgradeAddingRunestoneAvatar(int idh, string idcode, int idha, int idhg, System.Action<string> result)
    {
        WaitingPanelScript._instance.ShowWaiting(true);
        WWWForm form = new WWWForm();
        form.AddField("idh", idh);
        form.AddField("idcode", idcode);
        form.AddField("idha", idha);
        form.AddField("idhg", idhg);
        WWW www = new WWW(serverURL + "/api/gem/uppropertiesavatar", form);
        yield return www;
#if UNITY_EDITOR
        Debug.Log("idh:" + idh + " |idcode:" + idcode + " |idha:" + idha + " |idhg:" + idhg);
        Debug.Log(www.text);
#endif
        WaitingPanelScript._instance.ShowWaiting(false);
        if (!string.IsNullOrEmpty(www.error))
        {
            result("Error Code: " + www.error);
        }
        else
        {
            result(DataReturn(www.text));
        }
    }
    public static IEnumerator UpgradeCombineRunestone(int _idh, string _idcode, int _idhg, int _quantity, int _gold, System.Action<string> result)
    {
        WaitingPanelScript._instance.ShowWaiting(true);
        WWWForm form = new WWWForm();
        form.AddField("idh", _idh);
        form.AddField("idcode", _idcode);
        form.AddField("idhg", _idhg);
        form.AddField("quantity", _quantity);
        form.AddField("gold", _gold);
        WWW www = new WWW(serverURL + "/api/gem/upgradegem", form);
        yield return www;
        WaitingPanelScript._instance.ShowWaiting(false);
#if UNITY_EDITOR
        Debug.Log("idh:" + _idh + " |idcode:" + _idhg + " |idhg:" + _idhg + " |quantity:" + _quantity + "|gold" + _gold);
        Debug.Log(www.text);
#endif
        if (!string.IsNullOrEmpty(www.error))
        {
            result("Error Code: " + www.error);
        }
        else
        {
            result(DataReturn(www.text));
        }
    }
    public static IEnumerator UpgradeDismantleEquipment(int idh, string idcode, int ide, System.Action<string> result)
    {
        WaitingPanelScript._instance.ShowWaiting(true);
        //Debug.Log("idh:" + idh + " | idcode:" + idcode + " | ide:" + ide);
        WWWForm form = new WWWForm();
        form.AddField("idh", idh);
        form.AddField("idcode", idcode);
        form.AddField("ide", ide);
        WWW www = new WWW(serverURL + "/api/gem/disequipped", form);
        yield return www;
        WaitingPanelScript._instance.ShowWaiting(false);
        //Debug.Log(www.text);
        if (!string.IsNullOrEmpty(www.error))
        {
            result("Error Code: " + www.error);
        }
        else
        {
            result(DataReturn(www.text));
        }
    }
    public static IEnumerator AddAttributePoint(int idh, string idcode, int point, string type, System.Action<string> result)
    {
        WaitingPanelScript._instance.ShowWaiting(true);
        //Debug.Log("idh:" + idh + " | idcode:" + idcode + " | ide:" + ide);
        WWWForm form = new WWWForm();
        form.AddField("idh", idh);
        form.AddField("idcode", idcode);
        switch (type)
        {
            case "strength":
                form.AddField("strength", point);
                break;
            case "intelligence":
                form.AddField("intelligence", point);
                break;
            case "vitality":
                form.AddField("vitality", point);
                break;

        }
        WWW www = new WWW(serverURL + "/api/hero/addpoint", form);
        yield return www;
        WaitingPanelScript._instance.ShowWaiting(false);
        //Debug.Log(www.text);
        if (!string.IsNullOrEmpty(www.error))
        {
            result("Error Code: " + www.error);
        }
        else
        {
            result(DataReturn(www.text));
        }
    }
}
