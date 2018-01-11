using AssetBundles;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class LoadingResourceController : MonoBehaviour
{
    private string myServer = "http://45.32.106.62/upload/";
    public static LoadingResourceController _instance;
    private bool isUseBAM = false;
    public bool isLoaded = false;

    public IDictionary<string, AssetBundle> _myBundles;
    private AssetBundle _tempAssetBundle;

    void Awake()
    {
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(this.gameObject);
            //this.gameObject.tag = "DontDestroyObject";
            if (Application.platform == RuntimePlatform.WindowsEditor)
            {
                isUseBAM = true;
                myServer += "Android/";
            }

            else if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer || Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.OSXEditor)

            {
                if (Application.platform == RuntimePlatform.Android)
                {
                    myServer += "Android/";
                }
                else if (Application.platform == RuntimePlatform.WindowsPlayer)
                {
                    myServer += "Windows/";

                } else if (Application.platform == RuntimePlatform.OSXEditor) {
                    myServer += "Android/";

                }
                isUseBAM = false;
            }
            StartCoroutine(Initialize());
        }
        else DestroyImmediate(this.gameObject);
    }
    void Start()
    {
        //StartCoroutine(Initialize());
        _myBundles = new Dictionary<string, AssetBundle> { };

    }
    protected IEnumerator Initialize()
    {
        if (isUseBAM)
        {
            AssetBundleManager.logMode = AssetBundleManager.LogMode.JustErrors;
#if DEVELOPMENT_BUILD || UNITY_EDITOR
            AssetBundleManager.SetDevelopmentAssetBundleServer();
#else
        AssetBundleManager.SetSourceAssetBundleURL(myServer);
#endif
            var request = AssetBundleManager.Initialize();
            if (request != null)
                yield return StartCoroutine(request);
        }
    }

    internal IEnumerator LoadAssetBundleSpriteAsync(string assetBundleName, string assetName, System.Action<Sprite> result)
    {
        if (isUseBAM)
        {
            AssetBundleLoadAssetOperation request = AssetBundleManager.LoadAssetAsync(assetBundleName, assetName, typeof(Texture2D));
            if (request == null)
            {
                Debug.Log("request null");
                result(null);
                yield return null;
            }
            else
            {
                yield return StartCoroutine(request);
                var iconTexture = request.GetAsset<Texture2D>();
                var iconRect = new Rect(0, 0, iconTexture.width, iconTexture.height);
                var iconSprite = Sprite.Create(iconTexture, iconRect, new Vector2(.5f, .5f));
                if (iconSprite == null) Debug.Log("Không load được");
                result(iconSprite);
            }
        }
        else
        {
            _tempAssetBundle = null;
            foreach (var assetBundle in _myBundles)
            {
                if (assetBundle.Key == assetBundleName)//nếu đã load rồi
                {
                    _tempAssetBundle = assetBundle.Value;
                    break;
                }
            }
            if (_tempAssetBundle == null)
            {
                var www = WWW.LoadFromCacheOrDownload(myServer + assetBundleName, 5);
                yield return www;
                if (!string.IsNullOrEmpty(www.error))
                {
                    Debug.Log(www.error);
                    result(null);
                    yield return null;
                }
                _tempAssetBundle = www.assetBundle;
                _myBundles.Add(assetBundleName, _tempAssetBundle);
            }
            var iconTexture = _tempAssetBundle.LoadAsset<Texture2D>(assetName);
            var iconRect = new Rect(0, 0, iconTexture.width, iconTexture.height);
            var iconSprite = Sprite.Create(iconTexture, iconRect, new Vector2(.5f, .5f));
            result(iconSprite);
        }
    }

    internal IEnumerator LoadAssetBundleObjectAsync(string assetBundleName, string assetName, System.Action<GameObject> result)
    {
        if (isUseBAM)
        {
            AssetBundleLoadAssetOperation request = AssetBundleManager.LoadAssetAsync(assetBundleName, assetName, typeof(GameObject));
            if (request == null)
            {
                Debug.Log("request null");
                result(null);
                yield return null;
            }
            yield return StartCoroutine(request);
            var objectLoaded = request.GetAsset<GameObject>();
            result(objectLoaded);
        }
        else
        {
            _tempAssetBundle = null;
            foreach (var assetBundle in _myBundles)
            {
                if (assetBundle.Key == assetBundleName)//nếu đã load rồi
                {
                    _tempAssetBundle = assetBundle.Value;
                    break;
                }
            }
            //if (_tempAssetBundle == null)
            //{
            //    var www = WWW.LoadFromCacheOrDownload(myServer + Application.platform + "/" + assetBundleName, 5);
            //    yield return www;
            //    if (!string.IsNullOrEmpty(www.error))
            //    {
            //        Debug.Log(www.error);
            //        result(null);
            //        yield return null;
            //    }
            //    _tempAssetBundle = www.assetBundle;
            //    _myBundles.Add(assetBundleName, _tempAssetBundle);
            //}
            var objectLoaded = _tempAssetBundle.LoadAsset<GameObject>(assetName);
            result(objectLoaded);
        }
    }
    internal IEnumerator LoadAssetBundleTextAssetsAsync(string assetBundleName, string assetName, System.Action<TextAsset> result)
    {
        if (isUseBAM)
        {
            AssetBundleLoadAssetOperation request = AssetBundleManager.LoadAssetAsync(assetBundleName, assetName, typeof(TextAsset));
            if (request == null)
            {

                result(null);
                yield return null;
            }
            yield return StartCoroutine(request);
            var dataXML = request.GetAsset<TextAsset>();
            result(dataXML);
        }
        else
        {
            _tempAssetBundle = null;
            foreach (var assetBundle in _myBundles)
            {
                if (assetBundle.Key == assetBundleName)//nếu đã load rồi
                {
                    _tempAssetBundle = assetBundle.Value;
                    break;
                }
            }
            //if (_tempAssetBundle == null)
            //{
            //    var www = WWW.LoadFromCacheOrDownload(myServer + Application.platform + "/" + assetBundleName, 5);
            //    yield return www;
            //    if (!string.IsNullOrEmpty(www.error))
            //    {
            //        Debug.Log(www.error);
            //        result(null);
            //        yield return null;
            //    }
            //    _tempAssetBundle = www.assetBundle;
            //    _myBundles.Add(assetBundleName, _tempAssetBundle);
            //}
            TextAsset asset = _tempAssetBundle.LoadAsset<TextAsset>(assetName);
            Debug.Log(asset.name);
            result(asset);
        }
    }

    internal IEnumerator LoadAssetBundleAsync(string assetBundleName, System.Action<bool> result)
    {

        if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer || Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.OSXEditor)

        {
            _tempAssetBundle = null;
            foreach (var assetBundle in _myBundles)
            {
                if (assetBundle.Key == assetBundleName)//nếu đã load rồi
                {
                    result(true);
                    yield return null;
                }
            }
            if (_tempAssetBundle == null)
            {
                UnityWebRequest www = UnityWebRequest.GetAssetBundle(myServer + assetBundleName);

                yield return www.Send();

                if (www.isError)
                {
                    Debug.Log(www.error);
                    result(false);
                    yield return null;
                }
                else
                {
                    _tempAssetBundle = ((DownloadHandlerAssetBundle)www.downloadHandler).assetBundle;
                    //PlayerPrefs.SetInt(Constant.VERSION, 5);
                }
                //}
                //else
                //{
                //    var www1 = WWW.LoadFromCacheOrDownload(myServer + Application.platform + "/" + assetBundleName, 5);

                //    if (!string.IsNullOrEmpty(www1.error))
                //    {
                //        Debug.Log(www1.error);
                //        result(false);
                //        yield return null;
                //    }
                //    _tempAssetBundle = www1.assetBundle;
                //}
            }
            _myBundles.Add(assetBundleName, _tempAssetBundle);
            result(true);
        }
    }

    internal IEnumerator DownloadAndCache(string assetBundleName, System.Action<bool> result)
    {

        if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer || Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.OSXEditor)

        {
            _tempAssetBundle = null;
            foreach (var assetBundle in _myBundles)
            {
                if (assetBundle.Key == assetBundleName)//nếu đã load rồi
                {
                    result(true);
                    yield return null;
                }
            }
            if (_tempAssetBundle == null)
            {
                while (!Caching.ready)
                {
                    yield return null;
                }

                // if you want to always load from server, can clear cache first
                //        Caching.CleanCache();

                // get current bundle hash from server, random value added to avoid caching

                UnityWebRequest www = UnityWebRequest.Get(myServer + assetBundleName + ".manifest?r=" + (UnityEngine.Random.value * 9999999));
                StartCoroutine(LoadingResource.Instance.LoadDataProgress(www));
                //Debug.Log("Loading manifest:" + myServer + assetBundleName + ".manifest");

                // wait for load to finish
                yield return www.Send();

                // if received error, exit
                if (www.isError == true)
                {
                    Debug.LogError("www error:" + www.error);
                    yield break;
                }

                // create empty hash string
                Hash128 hashString = (default(Hash128));// new Hash128(0, 0, 0, 0);

                // check if received data contains 'ManifestFileVersion'
                if (www.downloadHandler.text.Contains("ManifestFileVersion"))
                {
                    // extract hash string from the received data, should add some error checking here
                    var hashRow = www.downloadHandler.text.ToString().Split("\n".ToCharArray())[5];
                    hashString = Hash128.Parse(hashRow.Split(':')[1].Trim());

                    if (hashString.isValid == true)
                    {
                        if (Caching.IsVersionCached(myServer + assetBundleName, hashString) == true)
                        {
                            //Debug.Log("Bundle with this hash is already cached!");
                        }
                        else
                        {
                            Debug.Log("No cached version founded for this hash..");
                        }
                    }
                    else
                    {
                        // invalid loaded hash, just try loading latest bundle
                        Debug.LogError("Invalid hash:" + hashString);
                        yield break;
                    }

                }
                else
                {
                    Debug.LogError("Manifest doesn't contain string 'ManifestFileVersion': " + myServer + assetBundleName + ".manifest");
                    yield break;
                }

                // now download the actual bundle, with hashString parameter it uses cached version if available
                www = UnityWebRequest.GetAssetBundle(myServer + assetBundleName + "?r=" + (UnityEngine.Random.value * 9999999), hashString, 0);
                StartCoroutine(LoadingResource.Instance.LoadDataProgress(www));
                // wait for load to finish
                yield return www.Send();

                if (www.isError)
                {
                    Debug.Log(www.error);
                    result(false);
                    yield return null;
                }
                else
                {
                    _tempAssetBundle = ((DownloadHandlerAssetBundle)www.downloadHandler).assetBundle;
                    //PlayerPrefs.SetInt(Constant.VERSION, 5);
                }

                //_tempAssetBundle.Unload(false);
            }

            _myBundles.Add(assetBundleName, _tempAssetBundle);
            result(true);
        }
    }
}
