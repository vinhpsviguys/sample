using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class ScrollviewDontDestroy : MonoBehaviour {

    public static ScrollviewDontDestroy Instance;
    public Transform scrollViewLog;
    public Transform parentLog;
    public Object textLog;
    public Button _closeDialog;
    bool isShow = true;
    private void Awake()
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
    }
    // Use this for initialization
    void Start () {
		
	}
	
    public void ShowHideLog()
    {
        if (isShow)
        {
            isShow = false;
            scrollViewLog.DOLocalMoveX(2000f, 0.1f);
        } else
        {
            isShow = true;
            scrollViewLog.DOLocalMoveX(960f, 0.1f);
        }
    }

    public void SetLog(string content)
    {
        GameObject _log = Instantiate(textLog, Vector3.zero, Quaternion.identity) as GameObject;
        _log.transform.parent = parentLog;
        _log.transform.localScale = Vector3.one;
        _log.GetComponent<Text>().text = content;
    }
}
