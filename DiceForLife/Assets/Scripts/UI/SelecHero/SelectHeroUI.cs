using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class SelectHeroUI : MonoBehaviour {

    [SerializeField]
    private Button _closeBtn;
    // Use this for initialization
    private void OnEnable()
    {
        _closeBtn.onClick.AddListener(CloseThisDialog);
    }

    void CloseThisDialog()
    {
        this.gameObject.SetActive(false);
    }
}
