using UnityEngine;
using UnityEngine.UI;

public class CheckPlatform : MonoBehaviour {

	// Use this for initialization
	void Start () {
        this.gameObject.GetComponent<Text>().text = Application.platform.ToString();
	}
}
