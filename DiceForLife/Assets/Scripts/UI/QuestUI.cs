﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestUI : MonoBehaviour {

	public void CloseThisDialog()
    {
        this.gameObject.SetActive(false);
    }
}