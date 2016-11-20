using UnityEngine;
using UnityEngine.UI;

using System;

namespace Dasher
{
	public class TimeDisplayCapsule : MonoBehaviour
	{
		[Header("Components")]
		[SerializeField]
		GameObject m_mainText = null;
		Text m_mainTextText;

		[SerializeField]
		GameObject m_additionalText = null;
		Text m_additionalTextText;

		[SerializeField]
		GameObject m_frontPanel = null;

		[SerializeField]
		GameObject m_backPanel = null;

		[SerializeField]
		GameObject m_flashPanel = null;
		Image m_flashImage = null;
		Color m_flashImageColor = Color.white;
		Transform m_flashTransform = null;

		[Header("Flash")]
		[SerializeField]
		float m_flashDuration = 1f;
		float m_flashTimer = 0f;

		[SerializeField]
		[Range(0,1)]
		float m_hitTime = .5f;

		[SerializeField]
		AnimationCurve m_flashCurve = null;

		[SerializeField]
		AnimationCurve m_flashSplosion = null;
		[SerializeField]
		float m_flashSplosionFactor = 2f;

		Action m_impactAction = null;

		[Header("Slide")]
		[SerializeField]
		float m_slideAnimationDuration = 1f;
		float m_slideAnimationTimer = 0f;

		[SerializeField]
		AnimationCurve m_slideAnimationCurve = null;

		[Header("Colors")]
		[SerializeField]
		Material m_badMaterial = null;

		[SerializeField]
		Material m_neutralMaterial = null;

		[SerializeField]
		Material m_goodMaterial = null;


		Image m_frontBorderImage = null;
		Image m_backBorderImage = null;

		float m_slideEndOffset = 0f;

		public void SetMainText(string message)
		{
			m_mainTextText.text = message;
		}

		public void SetAdditionalText(string message)
		{
			m_additionalTextText.text = message;
		}

		public void SetTexts(string messageMain,string messageAdditional)
		{
			SetMainText(messageMain);
			SetAdditionalText(messageAdditional);
		}

		#region Flash

		public void StartFlash(Action impactAction = null)
		{
			m_flashTimer = 0f;
			m_flashImage.enabled = true;
			m_flashImageColor.a = 0f;
			m_flashImage.color = m_flashImageColor;

			m_impactAction = impactAction;
			m_flashTransform.localScale = Vector3.one;
		}

		public void EndFlash()
		{
			m_flashImage.enabled = false;
			m_flashTimer = m_flashDuration + 1f;

			m_flashTransform.localScale = Vector3.one;
		}
		#endregion

		#region Slide

		public void HideBackPanel()
		{
			EndSlideAnimation();
			m_backPanel.gameObject.SetActive(false);
		}

		public void StartSlide()
		{
			m_slideAnimationTimer = 0f;
			m_backPanel.transform.localPosition = new Vector3(0, 0, 0);
			m_backPanel.gameObject.SetActive(true);
		}

		public void EndSlideAnimation()
		{
			m_slideAnimationTimer = m_slideAnimationTimer + 1f;
		}

		#endregion


		public void Initialize()
		{
			m_mainTextText = m_mainText.GetComponent<Text>();
			m_additionalTextText = m_additionalText.GetComponent<Text>();
			m_flashImage = m_flashPanel.GetComponent<Image>();
			m_flashImageColor = m_flashImage.color;
			m_flashTransform = m_flashPanel.transform;

			m_slideEndOffset = m_backPanel.transform.localPosition.y;

			EndFlash();

			HideBackPanel();

			m_frontBorderImage = m_frontPanel.GetComponent<Image>();
			m_backBorderImage = m_backPanel.GetComponent<Image>();
		}

		public void ManualUpdate()
		{
			float dt = Time.deltaTime;
			if (m_flashTimer < m_flashDuration)
			{
				var wasUnder = (m_flashTimer / m_flashDuration < m_hitTime);
				m_flashTimer += dt;
				var isUpper = (m_flashTimer / m_flashDuration >= m_hitTime);
				float progression = m_flashTimer / m_flashDuration;
				m_flashImageColor.a = m_flashCurve.Evaluate(progression);
				m_flashImage.color = m_flashImageColor;

				m_flashTransform.localScale = Vector3.one * (1f + m_flashSplosion.Evaluate(progression) * m_flashSplosionFactor);

				if (wasUnder && isUpper && m_impactAction != null)
				{
					m_impactAction();
				}

				if (m_flashTimer >= m_flashDuration)
				{
					EndFlash();
				}
			}

			if (m_slideAnimationTimer < m_slideAnimationDuration)
			{
				m_slideAnimationTimer += dt;

				float p = Mathf.Clamp01(m_slideAnimationTimer / m_slideAnimationDuration);
				p = m_slideAnimationCurve.Evaluate(p);
				var pos = m_backPanel.transform.localPosition;
				pos.y = m_slideEndOffset * p;
				m_backPanel.transform.localPosition = pos; 

				if( m_slideAnimationTimer >= m_slideAnimationDuration)
				{
					EndSlideAnimation();
				}
			}
		}

		#region SuccessState

		public enum CapsuleSuccessState {
			Bad,
			Neutral,
			Good
		}

		void SetBorderMaterial(Image target, CapsuleSuccessState state = CapsuleSuccessState.Neutral)
		{
			Material appliedMaterial = null;
			switch (state)
			{
				case CapsuleSuccessState.Bad:
					appliedMaterial = m_badMaterial;
					break;
				case CapsuleSuccessState.Neutral:
					appliedMaterial = m_neutralMaterial;
					break;
				case CapsuleSuccessState.Good:
					appliedMaterial = m_goodMaterial;
					break;
			}
			target.material = appliedMaterial;
		}

		public void SetFrontBorderState(CapsuleSuccessState state)
		{
			SetBorderMaterial(m_frontBorderImage, state);
		}

		public void SetBackBorderState(CapsuleSuccessState state)
		{
			SetBorderMaterial(m_backBorderImage, state);
		}

		#endregion
	}
}
