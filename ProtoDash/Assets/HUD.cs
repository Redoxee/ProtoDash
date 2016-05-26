using UnityEngine;
using UnityEngine.UI;
using Assets;

public class HUD : MonoBehaviour {

	[SerializeField]
	private Image dashBar;
	[SerializeField]
	private MainScript mainScript;

	private float baseLength;

	private float displayedDash = 0.0f;

	void Start()
	{
		baseLength = dashBar.rectTransform.sizeDelta.x;
	}

	void Update()
	{
		float prog = 1.0f - mainScript.dashTimer / mainScript.dashCoolDown;
		displayedDash = FuctionUtils.damping(.05f, displayedDash, prog, Time.deltaTime);
		dashBar.rectTransform.sizeDelta = new Vector2(baseLength * displayedDash, dashBar.rectTransform.sizeDelta.y);
	}
}
