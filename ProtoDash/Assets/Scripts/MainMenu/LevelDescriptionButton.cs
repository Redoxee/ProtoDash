using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace Dasher
{
	public class LevelDescriptionButton : MonoBehaviour
	{
		[Header("Components")]
		[SerializeField]
		Text m_title = null;
		[SerializeField]
		Text m_bestTime = null;
		[SerializeField]
		Text m_champTime = null;
		[SerializeField]
		Image m_flashImage = null;
		Color m_flashColor = Color.white;
		[SerializeField]
		AnimationFlashing m_playButtonFlash = null;

		[Header("Parameters")]
		[SerializeField]
		float m_flashDuration = 1f;
		[SerializeField]
		AnimationCurve m_flashCurve;
		[Range(0f, 1f)]
		[SerializeField]
		float m_peakPercentage = .5f;

		float m_flashTimer = 0f;
		float m_lastProgression = 0f;
		Action m_peakAction = null;
		
		Image m_mainImage = null;

		public void Initialize()
		{
			m_flashColor = m_flashImage.color;
			m_mainImage = GetComponent<Image>();
			enabled = false;
		}

		public void FlashInInfo(string title, string leftMessage, string rightMessage)
		{
			enabled = true;
			m_flashTimer = 0f;
			m_lastProgression = 0f;
			if (m_peakPercentage > 0f)
			{
				m_peakAction = () => { SetTexts(title, leftMessage, rightMessage); };
			}
			else
			{
				SetTexts(title, leftMessage, rightMessage);
			}

			m_playButtonFlash.StartAnimation();
		}

		public void SetTexts(string title, string leftMessage, string rightMessage)
		{
			m_title.text = title;
			m_bestTime.text = leftMessage;
			m_champTime.text = rightMessage;
		}

		public void SetMainMaterial(Material mat)
		{
			m_mainImage.material = mat;
		}

		public void ManualUpdate()
		{
			if (enabled)
			{
				m_flashTimer += Time.deltaTime;
				var progression = Mathf.Clamp01(m_flashTimer / m_flashDuration);

				if (m_lastProgression < m_peakPercentage && progression >= m_peakPercentage && m_peakAction != null)
				{
					m_peakAction();
					m_peakAction = null;
				}
				m_lastProgression = progression;

				m_flashColor.a = m_flashCurve.Evaluate(progression);
				m_flashImage.color = m_flashColor;

				if (m_flashTimer > m_flashDuration)
				{
					enabled = false;
				}
			}
		}
	}
}
