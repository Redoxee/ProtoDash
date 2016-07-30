using UnityEngine;
using System.Collections;

public class PauseButtonComponent : MonoBehaviour {

	[SerializeField]
	GUIManager GUIManagerRef;

	public void OnResumeAnimationEnd()
	{
		GUIManagerRef.OnResumeAnimationEnd();
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
