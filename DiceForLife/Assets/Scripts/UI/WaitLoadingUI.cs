using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaitLoadingUI : MonoBehaviour {

    public static WaitLoadingUI Instance;

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
        DontDestroyOnLoad(gameObject);
    }




}
