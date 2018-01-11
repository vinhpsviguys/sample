using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InviteFriendUI : MonoBehaviour {

   

    public GameObject _inviteRewardPanel;

   
    public void OpenRewardPanel()
    {
        _inviteRewardPanel.SetActive(true);
    }

    public void CloseRewardPanel()
    {
        _inviteRewardPanel.SetActive(false);
    }
    public void CloseThisDialog()
    {
        this.gameObject.SetActive(false);
    }
}
