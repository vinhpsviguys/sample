using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class WaitingPanelScript : MonoBehaviour
{
    [SerializeField]
    private Transform _imgCirle;
    private Image _myImg;
    public static WaitingPanelScript _instance;

    float timeStartLoading = 0f;
    private GameObject[] dontdestroyObj;
    bool isShow = false;
    private float _maxTimeWait = 30f;
    private void Awake()
    {
        _myImg = GetComponent<Image>();
    }
    private void OnEnable()
    {
        timeStartLoading = 0f;
        isShow = false;
        _myImg.color = new Color32(0, 0, 0, 5);
        _imgCirle.gameObject.SetActive(false);
    }
    void LateUpdate()
    {
        timeStartLoading += Time.deltaTime;
        if (!isShow)
        {
            if (timeStartLoading > 1)
            {
                isShow = true;
                _myImg.color = new Color32(0, 0, 0, 200);
                _imgCirle.gameObject.SetActive(true);
            }
        }
        else _imgCirle.Rotate(0, 0, 3);

        if (timeStartLoading > _maxTimeWait && !SocketIOController.sfs.mySocket.IsOpen)
        {
            ShowWaiting(false);
            if (dontdestroyObj == null)
                dontdestroyObj = GameObject.FindGameObjectsWithTag("DontDestroyObject");

            foreach (GameObject obj in dontdestroyObj)
            {
                Destroy(obj);
            }
            dontdestroyObj = null;
            SceneManager.LoadScene("Loading");
            //Application.LoadLevel("Loading");
            //Application.Quit();
        }
        else if (timeStartLoading > _maxTimeWait && SocketIOController.sfs.mySocket.IsOpen && WatingRoomController.Instance != null && WatingRoomController.Instance.state_waitingroom != STATEINWAITING.NONE && !SocketIOController.Instance.isReconnect)
        {
            WaitingRoomUI.Instance.CancelFind();
            ShowWaiting(false);
        }
        else if (timeStartLoading > _maxTimeWait && SocketIOController.sfs.mySocket.IsOpen && WatingRoomController.Instance == null)
        {
            Debug.Log("waing room controller " + WatingRoomController.Instance);
            BattleSceneUI.Instance.BackToMainMenu();
            ShowWaiting(false);
        }
        else if (timeStartLoading > _maxTimeWait && SocketIOController.sfs.mySocket.IsOpen && WatingRoomController.Instance != null && WatingRoomController.Instance.state_waitingroom == STATEINWAITING.NONE && SocketIOController.Instance.isReconnect)
        {
            SocketIOController.Instance.isReconnect = false;
            WaitingRoomUI.Instance.ClearIdRoom();
            ShowWaiting(false);
        }
    }
    internal void ShowWaiting(bool isShow)
    {
        this.gameObject.SetActive(isShow);
    }
}
