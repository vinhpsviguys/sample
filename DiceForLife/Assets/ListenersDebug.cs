using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ListenersDebug : MonoBehaviour {

    public static ListenersDebug Instance;
    public Transform content;
    public Transform scrollViewLog;
    public UnityEngine.Object _textLog;
    bool isShow = true;

    // Use this for initialization

    void Start () {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            ShowHideLog();
        }
        else
        {
            // If that is the case, we destroy other instances
            Destroy(gameObject);
        }
    }

    public void debugListeners(string str)
    {
        if (UnityMainThreadDispatcher.Instance() != null)
        UnityMainThreadDispatcher.Instance().Enqueue(()=> {
            foreach (Transform child in content)
            {
                Destroy(child.gameObject);
            }

            GameObject _log = Instantiate(_textLog, Vector3.zero, Quaternion.identity) as GameObject;
            _log.transform.parent = content;
            _log.transform.localScale = Vector3.one;
            _log.GetComponent<Text>().text = str;
        });
        
    }

    public void ShowHideLog()
    {
        if (isShow)
        {
            isShow = false;
            scrollViewLog.DOLocalMoveX(-2000f, 0.1f);
        }
        else
        {
            isShow = true;
            scrollViewLog.DOLocalMoveX(-360f, 0.1f);
        }
    }
}
