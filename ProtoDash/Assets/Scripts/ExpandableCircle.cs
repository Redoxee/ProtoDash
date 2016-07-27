using UnityEngine;
using System.Collections;

public class ExpandableCircle : MonoBehaviour {

	private delegate void StateFunction(float dt); 
	
	[SerializeField]
	private float animationDuration = 3f;
	[SerializeField]
	private float shiftLength = 3f;
	[SerializeField]
	private AnimationCurve animationCurve;
	[SerializeField]
	private AnimationCurve fadeCurve;
	[SerializeField]
	private Color objectColor = Color.white;
	
	private GameObject childObject;

	private Renderer rendererRef;
	private Material objectMaterial;
	private Vector3 originalPosition;
	private float animationTimer = 0f;
	private StateFunction currentState = null;

	private float m_width = 1f;
	public float Width
	{
		get { return m_width; }
		set
		{
			m_width = value;
			Vector3 t = childObject.transform.localScale;
			t.x = value / t.y;
			childObject.transform.localScale = t;
			objectMaterial.SetFloat("_Ratio", t.y / t.x);
		}
	}

	public void Start()
	{
		childObject = transform.GetChild(0).gameObject;
		rendererRef = childObject.GetComponent<Renderer>();
		objectMaterial = rendererRef.material;
		originalPosition = childObject.transform.localPosition;

		objectColor.a = 0f;
		rendererRef.enabled = false;
		objectMaterial.SetColor("_Color", objectColor);
	}

	public void Update()
	{

		if (currentState != null)
		{
			currentState(Time.deltaTime);

			objectColor.a = fadeCurve.Evaluate(animationTimer / animationDuration);
			objectMaterial.SetColor("_Color", objectColor);
		}
	}

	public void StartAnimation()
	{
		animationTimer = 0f;
		rendererRef.enabled = true;
		childObject.transform.localPosition = originalPosition;
		currentState = UpdateExpand;
	}

	private void UpdateExpand(float dt)
	{
		animationTimer += dt;
		float progression = Mathf.Clamp(animationCurve.Evaluate(animationTimer / animationDuration) * 2f,0f,1f);
		Width = 1f + progression * shiftLength;
		if (animationTimer > animationDuration * .5)
			currentState = UpdateRetract;
	}

	private void UpdateRetract(float dt)
	{
		animationTimer += dt;
		float progression = Mathf.Clamp((animationCurve.Evaluate(animationTimer / animationDuration) * 2f) - 1f, 0f, 1f);
		Width = 1f + (1f - progression) * shiftLength;

		childObject.transform.localPosition = originalPosition + Vector3.right * shiftLength * progression;

		if (animationTimer > animationDuration)
		{
			currentState = null;
			rendererRef.enabled = false;
		}
	}
}
