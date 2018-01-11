using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneLoader : MonoBehaviour
{
    public static SceneLoader _instance;
    private bool loadScene = false;

    [SerializeField]
    private Text loadingText;
    [SerializeField]
    private Image imgProgess;
    [SerializeField]
    private GameObject loadingPanel;
    [SerializeField]
    private WaitingPanelScript _waitingPanel;
    private float loadingProgess = 0;

    void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            WaitingPanelScript._instance = _waitingPanel;
            DontDestroyOnLoad(this.gameObject);
            SceneManager.sceneLoaded += SceneManager_sceneLoaded;
        }
        else DestroyImmediate(this.gameObject);
    }

    private void SceneManager_sceneLoaded(Scene arg0, LoadSceneMode arg1)
    {
        loadingPanel.SetActive(false);
    }

    void Update()
    {
        loadingText.color = new Color(loadingText.color.r, loadingText.color.g, loadingText.color.b, Mathf.PingPong(Time.time, 1));
    }

    public void LoadScene(int idScene)
    {
        StartCoroutine(LoadNewScene(idScene));
    }

    public IEnumerator LoadNewScene(int idSene)
    {
        loadingPanel.SetActive(true);
        loadingProgess = 0;
        imgProgess.fillAmount = 0;

        AsyncOperation async = SceneManager.LoadSceneAsync(idSene);

        while (async.progress < 0.9f)
        {
            loadingProgess = async.progress;
            imgProgess.fillAmount = loadingProgess;
            yield return null;
        }
        yield return new WaitForEndOfFrame();
        //loadingPanel.SetActive(false);
    }
}