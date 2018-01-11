using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScriptTest : MonoBehaviour
{
    [SerializeField]
    private GameObject _characterPopup, _bagPopup, _selectHeroPopup,_upgradePopup;

    public void BtnClick(int id)
    {
        if (id == 0) _characterPopup.SetActive(true);
        else if (id == 1) _bagPopup.SetActive(true);
        else if (id == 2) _selectHeroPopup.SetActive(true);
        else if (id == 3) _upgradePopup.SetActive(true);
        else if (id == 4)
        {
            Application.LoadLevel("FindMatch");
        }
    }
}
