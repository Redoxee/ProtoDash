using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Dasher
{
	public class LevelWallAnimator : MonoBehaviour
	{
		[SerializeField]
		LayoutElement m_layoutElement = null;
		Button m_button = null;
		Text m_mainText = null;
		Color m_colorOriginal;

		[SerializeField]
		AnimationCurve m_alphaCurve = AnimationCurve.Linear(0, 1, 1, 0);

		[SerializeField]
		AnimationCurve m_scaleCurve = AnimationCurve.Linear(0, 1, 1, 0);
		float m_startScaleHeight = 0f;

		[SerializeField]
		float m_duration = 1f;

		float m_timer = float.MaxValue;
		
		public bool IsAnimating { get { return m_timer < m_duration; } }

		Action m_endAnimationAction = null;

		static int s_colorId;

		public void Awake()
		{
			s_colorId = Shader.PropertyToID("_Color");

			var child = transform.GetChild(0);
			m_mainText = GetComponentInChildren<Text>();
			m_button = GetComponentInChildren<Button>();

			m_colorOriginal = m_mainText.color;


		} 

		public void StartAnimation(Action endAnimationAction = null)
		{
			m_timer = 0f;
			m_startScaleHeight = m_layoutElement.preferredHeight;
			m_button.interactable = false;
			m_endAnimationAction = endAnimationAction;
		}

		public void UpdateAnimation()
		{
			if (m_timer >= m_duration)
				return;

			m_timer += Time.deltaTime;
			var progression = Mathf.Clamp01(m_timer / m_duration);
			var alpha = m_alphaCurve.Evaluate(progression);

			m_colorOriginal.a = alpha;
			m_mainText.color = m_colorOriginal;

			m_layoutElement.preferredHeight = m_startScaleHeight * m_scaleCurve.Evaluate(progression);

			if (m_timer >= m_duration)
			{
				gameObject.SetActive(false);
				if (m_endAnimationAction != null)
					m_endAnimationAction();
			}
		}
	}
}