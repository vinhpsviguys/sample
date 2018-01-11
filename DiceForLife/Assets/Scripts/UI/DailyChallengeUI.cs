using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DailyChallengeUI : MonoBehaviour {


    public Button _monsterChalBtn, _bossChalBtn, _pvpChalBtn;

    public GameObject _goldChalPanel,_battleInfoPanel,_rewardBattlePanel,_portalBattleScenes;
    public Button _closeChallengePanel;

    private void OnEnable()
    {
        _monsterChalBtn.onClick.AddListener(OpenChallengePanel);
        _bossChalBtn.onClick.AddListener(OpenChallengePanel);
        _pvpChalBtn.onClick.AddListener(OpenChallengePanel);
        _closeChallengePanel.onClick.AddListener(CloseChallengePanel);
    }
    private void OnDisable()
    {
        _monsterChalBtn.onClick.RemoveListener(OpenChallengePanel);
        _bossChalBtn.onClick.RemoveListener(OpenChallengePanel);
        _pvpChalBtn.onClick.RemoveListener(OpenChallengePanel);
        _closeChallengePanel.onClick.RemoveListener(CloseChallengePanel);
    }


    public void StartBattleChallengeMonster()
    {
        _portalBattleScenes.SetActive(true);
    }

    public void OpenRewardBattlePanel()
    {
        _rewardBattlePanel.SetActive(true);
    }

    public void OpenInfoMonsterChallenge()
    {
        _battleInfoPanel.SetActive(true);
    }

    void OpenChallengePanel()
    {
        _goldChalPanel.SetActive(true);
    }

    public void CloseRewardBattlePanel()
    {
        _rewardBattlePanel.SetActive(false);
    }

    public void CloseBattleInfoPanel()
    {
        _battleInfoPanel.SetActive(false);
    }

    public void CloseChallengePanel()
    {
        _goldChalPanel.SetActive(false);
    }

    public void CloseThisDialog()
    {
        this.gameObject.SetActive(false);
    }
}
