using UnityEngine;
using System.Collections;

public class TutorialAnimator : MonoBehaviour {

	[SerializeField]
	float animationTime = 2f;
	[SerializeField]
	private AnimationCurve scaleAnimation;

	[SerializeField]
	private bool xOnly;

	private Vector3 originalScale;

	private float animationTimer;

	// Use this for initialization
	void Start () {
		originalScale = transform.localScale;
		animationTimer = 0f;
	}
	
	// Update is called once per frame
	void Update () {
		animationTimer = (animationTimer + Time.deltaTime) % animationTime;
		Vector3 nextScale;
		if (!xOnly)
		{
			nextScale = originalScale * scaleAnimation.Evaluate(animationTimer / animationTime);
		}
		else
		{
			nextScale = originalScale;
			nextScale.x *= scaleAnimation.Evaluate(animationTimer / animationTime);
		}
		gameObject.transform.localScale = nextScale;
	}
}
