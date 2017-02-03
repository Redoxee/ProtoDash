using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dasher
{
	public class DashRefill : MonoBehaviour
	{
		[SerializeField]
		AnimatedCross m_animated = null;

		[SerializeField]
		float m_disableDuration = 5f;

		float m_timer = 0f;

		void Start()
		{
			m_timer = m_disableDuration + 1f;
		}

		void OnTriggerEnter2D(Collider2D collision)
		{
			if(m_timer >= m_disableDuration)
				ActivatePowerUp();
		}

		void ActivatePowerUp()
		{
			GameProcess.Instance.NotifyDashRefill();

			m_timer = 0f;
			m_animated.FadeOut();
		}

		void FixedUpdate()
		{
			m_timer += GameProcess.Instance.GameTime.GetGameFixedDeltaTime();
			if (m_timer >= m_disableDuration)
			{
				Reactivate();
			}
		}

		void Reactivate()
		{
			m_animated.FadeIn();
		}
	}
}
