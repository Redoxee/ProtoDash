using UnityEngine;
using System.Collections;

public class AnimatableScreen : MonoBehaviour
{
	public enum Position
	{
		Center,
		Left,
		Right,
		Up,
		Down
	}

	public float animatablePosition = 0f;
	public Position positionAtZero = Position.Center;
	public Position positionAtOne = Position.Down;

	private RectTransform rectTransform;

	void Start()
	{
		rectTransform = GetComponent<RectTransform>();
	}

	void Update()
	{
		Vector2 posAt0 = GetPosition(positionAtZero);
		Vector2 posAt1 = GetPosition(positionAtOne);
		rectTransform.anchoredPosition = Vector2.Lerp(posAt0, posAt1, animatablePosition);
	}

	Vector2 GetPosition(Position p)
	{
		switch (p)
		{
			case Position.Center: return Vector2.zero;
			case Position.Up: return new Vector2(0f, rectTransform.rect.height);
			case Position.Down: return new Vector2(0f, -rectTransform.rect.height);
			case Position.Left: return new Vector2(-rectTransform.rect.width, 0f);
			case Position.Right: return new Vector2(rectTransform.rect.width, 0f);
			default: return Vector2.zero;
		}
	}
}