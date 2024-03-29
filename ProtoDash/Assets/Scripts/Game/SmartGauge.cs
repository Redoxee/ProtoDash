﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace Dasher
{
	public class SmartGauge : MonoBehaviour {

		[SerializeField]
		private Image m_mainSprite;

		Character m_character;
		public bool m_isRight;

		[SerializeField]
		private GameObject m_cost1;
		private Image m_costImage1;
		private AnimatedEnergyCost m_animated1;

		[SerializeField]
		private GameObject m_cost2;
		private Image m_costImage2;
		private AnimatedEnergyCost m_animated2;

		[SerializeField]
		private GameObject m_flashObject;
		private Material m_flashMaterial;
		[SerializeField]
		private float m_flashDuration = 1.5f;
		[SerializeField]
		private AnimationCurve m_flashCurve = null;
		private float m_flashTimer = 0f;

		private Image m_currentCostImage;
		private AnimatedEnergyCost m_currentAnimated;

		private float m_previousFill = 1f;

		private float m_height = 1f;

		public void Initialize()
		{
			m_previousFill = 1f;
			RectTransform rt = (RectTransform)transform;
			m_height = rt.rect.height;

			m_costImage1 = m_cost1.GetComponent<Image>();
			m_animated1 = m_cost1.GetComponent<AnimatedEnergyCost>();

			m_costImage2 = m_cost2.GetComponent<Image>();
			m_animated2 = m_cost2.GetComponent<AnimatedEnergyCost>();

			Vector3 offset = m_animated1.m_animationOffset;
			offset.x = Mathf.Abs(offset.x) * (m_isRight ? -1 : 1);
			m_animated1.m_animationOffset = offset;
			m_animated2.m_animationOffset = offset;

			m_currentCostImage = m_costImage1;
			m_currentAnimated = m_animated1;

			m_character = GameProcess.Instance.CurrentCharacter;
			m_animated1.Reset();
			m_animated2.Reset();

			m_flashMaterial = m_flashObject.GetComponent<Image>().material;
			m_flashObject.SetActive(false);
			m_flashTimer = m_flashDuration + 1f;
		}

		void Update() {
			if (m_character)
			{
				float currentFill = m_character.currentEnergy / m_character.maxEnergyPoints;

				RectTransform rt = m_mainSprite.rectTransform;
				Vector3 mainScale = rt.localScale;
				mainScale.y = currentFill;
				rt.localScale = mainScale;

				if (currentFill < m_previousFill)
				{
					Vector3 costScale = m_currentCostImage.rectTransform.localScale;
					Vector3 costPos = m_currentCostImage.rectTransform.localPosition;

					costScale.y = m_previousFill - currentFill;
					costPos.y = currentFill * m_height;
					costPos.x = 0;
					m_currentCostImage.rectTransform.localPosition = costPos;
					m_currentCostImage.rectTransform.localScale = costScale;

					m_currentAnimated.StartAnimation();
					SwapCostObjects();
				}

				m_previousFill = currentFill;

				UpdateFlash();
			}
		}

		private void SwapCostObjects()
		{
			if (m_currentCostImage == m_costImage2)
			{
				m_currentCostImage = m_costImage1;
				m_currentAnimated = m_animated1;
			}
			else
			{
				m_currentCostImage = m_costImage2;
				m_currentAnimated = m_animated2;
			}
		}

		public void StartFlash()
		{
			m_flashTimer = 0f;
			m_flashObject.SetActive(true);
		}

		public void EndFlash()
		{
			m_flashObject.SetActive(false);
		}

		void UpdateFlash()
		{
			if (m_flashTimer < m_flashDuration)
			{
				m_flashTimer += Time.deltaTime;
				var progression =  Mathf.Clamp01(m_flashTimer/m_flashDuration);
				m_flashMaterial.color = new Color(1f, 1f, 1f, m_flashCurve.Evaluate(progression));
				if (m_flashTimer >= m_flashDuration)
				{
					EndFlash();
				}
			}
		}
	}
}