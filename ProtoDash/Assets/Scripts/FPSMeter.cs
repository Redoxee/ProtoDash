using UnityEngine;
using UnityEngine.UI;

public class FPSMeter : MonoBehaviour {

	private Text text;

	// Use this for initialization
	void Start () {
		text = GetComponent<Text>();
	}
	
	// Update is called once per frame
	void Update () {
		text.text = (1.0f / Time.deltaTime).ToString();
	}
}
