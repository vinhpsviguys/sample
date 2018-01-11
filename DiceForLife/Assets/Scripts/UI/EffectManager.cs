using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class EffectManager : MonoBehaviour {

    public static EffectManager Instance;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            // If that is the case, we destroy other instances
            Destroy(gameObject);
        }

        // Here we save our singleton instance
        Instance = this;

        // Furthermore we make sure that we don't destroy between scenes (this is optional)
        //DontDestroyOnLoad(gameObject);
    }


    public GameObject CreateEffect(string path, Transform _transform)
    {
        Debug.Log("path effect " + path);
        GameObject effect = Instantiate(Resources.Load(path) as GameObject, _transform.position,Quaternion.identity);
        return effect;
    }

    public void EffectMoving(GameObject effect, float x, float duration)
    {
        effect.transform.DOMoveX(x, duration).OnComplete(() => DestroyEffectObj(effect));
    }
    public void DestroyEffectObj(GameObject obj)
    {
        Destroy(obj.gameObject);
    }
}
