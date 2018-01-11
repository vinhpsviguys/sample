using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;


public class ChatUI : MonoBehaviour
{
    public RectTransform boundChat;
    public Button _worldChatBtn;
    public Button _guildChatBtn;
    public Button _recuitChatBtn;

    public Button _settingBtn, _closeBtn;

    public Sprite[] _buttonImgState;

    public GameObject _worldChatPanel, _guildChatPanel, _recuitChatPanel;

    bool minimize = false;


    private Color32 _txtOffColor = new Color(255f / 255f, 232f / 255f, 170f / 255f, 255f / 255f);
    private Color32 _txtOnColor = new Color(97f / 255f, 58f / 255f, 222f / 255f, 255f / 255f);

    private void OnEnable()
    {
        boundChat.localPosition = new Vector3(-1310f, boundChat.localPosition.y);
        boundChat.DOLocalMoveX(-307f, 0.25f);
        _worldChatBtn.onClick.AddListener(ShowWorldChat);
        _guildChatBtn.onClick.AddListener(ShowGuildChat);
        _recuitChatBtn.onClick.AddListener(ShowRecuitChat);
        _closeBtn.onClick.AddListener(CloseThisDialog);
        ShowWorldChat();
    }
    private void OnDisable()
    {
        _worldChatBtn.onClick.RemoveListener(ShowWorldChat);
        _guildChatBtn.onClick.RemoveListener(ShowGuildChat);
        _recuitChatBtn.onClick.RemoveListener(ShowRecuitChat);
        _closeBtn.onClick.RemoveListener(CloseThisDialog);
    }
    // Update is called once per frame
    void Update()
    {

    }

    void ShowWorldChat()
    {
        _worldChatBtn.image.sprite = _buttonImgState[0];
        _worldChatBtn.transform.GetChild(0).GetComponent<Text>().color = _txtOnColor;
        _guildChatBtn.image.sprite = _buttonImgState[1];
        _guildChatBtn.transform.GetChild(0).GetComponent<Text>().color = _txtOffColor;
        _recuitChatBtn.image.sprite = _buttonImgState[1];
        _recuitChatBtn.transform.GetChild(0).GetComponent<Text>().color = _txtOffColor;
        _worldChatPanel.SetActive(true);
        _guildChatPanel.SetActive(false);
        _recuitChatPanel.SetActive(false);
    }
    void ShowGuildChat()
    {
        _worldChatBtn.image.sprite = _buttonImgState[1];
        _worldChatBtn.transform.GetChild(0).GetComponent<Text>().color = _txtOffColor;
        _guildChatBtn.image.sprite = _buttonImgState[0];
        _guildChatBtn.transform.GetChild(0).GetComponent<Text>().color = _txtOnColor;
        _recuitChatBtn.image.sprite = _buttonImgState[1];
        _recuitChatBtn.transform.GetChild(0).GetComponent<Text>().color = _txtOffColor;
        _worldChatPanel.SetActive(false);
        _guildChatPanel.SetActive(true);
        _recuitChatPanel.SetActive(false);
    }
    void ShowRecuitChat()
    {
        _worldChatBtn.image.sprite = _buttonImgState[1];
        _worldChatBtn.transform.GetChild(0).GetComponent<Text>().color = _txtOffColor;
        _guildChatBtn.image.sprite = _buttonImgState[1];
        _guildChatBtn.transform.GetChild(0).GetComponent<Text>().color = _txtOffColor;
        _recuitChatBtn.image.sprite = _buttonImgState[0];
        _recuitChatBtn.transform.GetChild(0).GetComponent<Text>().color = _txtOnColor;
        _worldChatPanel.SetActive(false);
        _guildChatPanel.SetActive(false);
        _recuitChatPanel.SetActive(true);
    }
    public void ControlSizeChatPanel()
    {
        if (!minimize)
        {
            boundChat.localPosition = new Vector3(-307, -270, 0);
            boundChat.sizeDelta = new Vector2(1306, 540);
            minimize = true;
        }
        else
        {
            boundChat.localPosition = new Vector3(-307, 0, 0);
            boundChat.sizeDelta = new Vector2(1306, 1080);
            minimize = false;
        }

    }
    void CloseThisDialog()
    {
        boundChat.DOLocalMoveX(-1310f, 0.25f).OnComplete(OnCompleteMoveOut);
    }
    void OnCompleteMoveOut()
    {
        this.gameObject.SetActive(false);

    }
}
