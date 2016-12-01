using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class AnimationFlashing : MonoBehaviour {
	[SerializeField]
	AnimationCurve m_fadeCurve = null;
	[SerializeField]
	AnimationCurve m_scaleCurve = null;
	[SerializeField]
	float m_duration = 1f;

	Image m_image = null;
	Color m_color = Color.white;

	float m_timer = 0f;


	public void StartAnimation()
	{
		m_image = GetComponent<Image>();
		m_color = m_image.color;

		m_timer = 0;
		m_color.a = m_fadeCurve.Evaluate(0f);
		m_image.color = m_color;
		transform.localScale = Vector3.one * m_scaleCurve.Evaluate(0f);
		enabled = true;
	}

	public void StopAnimation()
	{
		enabled = false;
		m_color.a = m_fadeCurve.Evaluate(1f);
		m_image.color = m_color;
		transform.localScale = Vector3.one * m_scaleCurve.Evaluate(1f);
	}

	void Update () {
		m_timer += Time.deltaTime;
		if (m_timer > m_duration)
		{
			StopAnimation();
		}
		var progression = Mathf.Clamp01(m_timer / m_duration);

		m_color.a = m_fadeCurve.Evaluate(progression);
		m_image.color = m_color;
		transform.localScale = Vector3.one * m_scaleCurve.Evaluate(progression);
	}
}
