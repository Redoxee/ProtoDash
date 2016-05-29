using UnityEngine;
using UnityEngine.UI;
using Assets;

public class HUD : MonoBehaviour {

	[SerializeField]
	private Image dashBar;
	[SerializeField]
	private MainScript mainScript;

	private float baseLength;

	private float displayedDash = 1.0f;

	void Start()
	{
		baseLength = dashBar.rectTransform.sizeDelta.x;
	}

	void Update()
	{
		float prog = mainScript.currentEnergy / mainScript.maxEnergyPoints;
		displayedDash = prog;
		dashBar.rectTransform.sizeDelta = new Vector2(baseLength * displayedDash, dashBar.rectTransform.sizeDelta.y);
	}
}
