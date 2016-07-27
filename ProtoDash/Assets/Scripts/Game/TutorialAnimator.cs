using UnityEngine;
using System.Collections.Generic;

public class TutorialAnimator : MonoBehaviour {

	[SerializeField]
	float animationTime = 2f;
	[SerializeField]
	private AnimationCurve scaleAnimation;

	[SerializeField]
	private bool xOnly;

	[SerializeField]
	private List<ExpandableCircle> animatedCircles;
	[SerializeField]
	private float animatedTimeStart = 0f;

	private bool hasNotifiedCircles = false;

	private Vector3 originalScale;

	private float animationTimer;

	// Use this for initialization
	void Start () {
		originalScale = transform.localScale;
		animationTimer = 0f;
	}
	
	// Update is called once per frame
	void Update () {
		animationTimer = (animationTimer + Time.deltaTime);
		if (animationTimer > animationTime)
		{
			hasNotifiedCircles = false;
			animationTimer %= animationTime;
		}
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

		if (animationTimer >= animatedTimeStart && !hasNotifiedCircles)
		{
			for (int i = 0; i < animatedCircles.Count; ++i)
			{
				animatedCircles[i].StartAnimation();
			}
			hasNotifiedCircles = true;
		}
	}
}
