using UnityEngine;
using System.Collections;

namespace Dasher
{
	public class JumpParticlesManager : MonoBehaviour
	{
		[SerializeField]
		ExtensibleCircleManager m_extensibleCircle = null;
		[SerializeField]
		float m_animationDuration = 1f;
		[SerializeField]
		AnimationCurve m_extendCircleCurve = AnimationCurve.Linear(0, 0, 1, 1);
		[SerializeField]
		AnimationCurve m_colorCurve = AnimationCurve.Linear(0, 0, 1, 0);
		[SerializeField]
		Color m_colorStart = Color.white;
		[SerializeField]
		Color m_colorFull = Color.white;

		float m_timer = 0f;

		public void StartAnimation()
		{
			gameObject.SetActive(true);
			SetProgression(0f);
			m_timer = 0f;
		}

		public void manualUpdate()
		{
			m_timer += Time.deltaTime;

			float progression = m_timer / m_animationDuration;
			if (progression >= 1)
			{
				gameObject.SetActive(false);
			}
			else
			{
				SetProgression(Mathf.Clamp01(progression));
			}
		}

		private void SetProgression(float progression)
		{
			m_extensibleCircle.SetWidth(1f + m_extendCircleCurve.Evaluate(progression));
			m_extensibleCircle.SetColor(Color.Lerp(m_colorStart,m_colorFull,m_colorCurve.Evaluate(progression)));
		}
	}
}
