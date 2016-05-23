using UnityEngine;
using System.Collections;

public class CheapDashUI : MonoBehaviour {

	[SerializeField]
	private MainScript mScriptRef;

	[SerializeField]
	private float baseWidth = 5;

	private Vector3 originalPosition;

	void Start()
	{
		originalPosition = transform.position;
	}

	void FixedUpdate () {
		float p = 1.0f - Mathf.Max(mScriptRef.dashTimer,0) / mScriptRef.dashCoolDown;
		Vector3 newtScale = new Vector3( p * baseWidth, 1,1);
		transform.localScale = newtScale;

		transform.position = new Vector3(originalPosition.x + p * .5f * baseWidth, originalPosition.y, originalPosition.z);
	}
}
