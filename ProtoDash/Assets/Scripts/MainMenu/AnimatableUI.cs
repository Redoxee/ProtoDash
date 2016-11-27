using UnityEngine;
using UnityEngine.UI;

public class AnimatableUI : MonoBehaviour {

	private float prevPos = -1;
	public float animatablePosition;

	private RectTransform rTransform;

	public Vector2 minAnchor0;
	public Vector2 minAnchor1;
	public Vector2 maxAnchor0;
	public Vector2 maxAnchor1;

	public Vector2 relativPosition0;
	public Vector2 relativPosition1;

	void Start () {
		rTransform = transform as RectTransform;
		
	}
	
	void Update () {
		if (prevPos != animatablePosition)
		{
			prevPos = animatablePosition;

			rTransform.anchorMin = Vector2.Lerp(minAnchor0, minAnchor1, animatablePosition);
			rTransform.anchorMax = Vector2.Lerp(maxAnchor0, maxAnchor1, animatablePosition);
			rTransform.anchoredPosition = Vector2.Lerp(relativPosition0, relativPosition1, animatablePosition);
		}
	}

	public void SetAnimationPosition(float p)
	{
		rTransform.anchorMin = Vector2.Lerp(minAnchor0, minAnchor1, p);
		rTransform.anchorMax = Vector2.Lerp(maxAnchor0, maxAnchor1, p);
		rTransform.anchoredPosition = Vector2.Lerp(relativPosition0, relativPosition1, p);
	}
}
