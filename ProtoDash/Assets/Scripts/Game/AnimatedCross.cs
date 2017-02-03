using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dasher {
	public class AnimatedCross : MonoBehaviour {
		float m_rotationSpeed = 50f;
		Material m_crossMaterial;
		Color m_originalColor = Color.white;

		const string c_stretch = "_Stretch";
		const string c_radius = "_Radius";
		
		[SerializeField]
		float m_animationFadeDuration = 1f;
		[SerializeField]
		AnimationCurve m_strechCurve = null;
		[SerializeField]
		AnimationCurve m_radiusCurve = null;
		[SerializeField]
		AnimationCurve m_alphaCurve = null;

		float m_timer = -1f;
		float m_animationDirection = -1f;

#if UNITY_EDITOR
		public bool m_forceFadeIn = false;
		public bool m_forceFadeOut = false;
#endif

		void Start()
		{
			Initialize();
		}

		void Initialize()
		{
			Renderer renderer = GetComponent<Renderer>();
			m_crossMaterial = new Material(renderer.material);
			renderer.material = m_crossMaterial;
			m_originalColor = m_crossMaterial.color;

		}

		public void FadeIn()
		{
			m_animationDirection = -1f;
			m_timer = Mathf.Min(m_timer, m_animationFadeDuration);
		}

		public void FadeOut()
		{
			m_animationDirection = 1f;
			m_timer = Mathf.Max(0f, m_timer);
		}

		void UpdateAnimation(float dt)
		{
			if ((m_timer < m_animationFadeDuration && m_animationDirection > 0) 
				|| (m_timer > 0 && m_animationDirection < 0))
			{

				m_timer += dt * m_animationDirection;

				var progression = Mathf.Clamp01(m_timer / m_animationFadeDuration);
				SetFade(progression);

			}
		}

		void SetFade(float progression)
		{
			m_originalColor.a = m_alphaCurve.Evaluate(progression);
			m_crossMaterial.color = m_originalColor;

			m_crossMaterial.SetFloat(c_stretch, m_strechCurve.Evaluate(progression));
			m_crossMaterial.SetFloat(c_radius, m_radiusCurve.Evaluate(progression));
		}

		void Update()
		{
			float dt = GameProcess.Instance.GameTime.GetGameDeltaTime();
			UpdateAnimation(dt);
			transform.rotation *= Quaternion.AngleAxis(dt * m_rotationSpeed, Vector3.forward);

#if UNITY_EDITOR
			if(m_forceFadeIn)
				FadeIn();
			if (m_forceFadeOut)
				FadeOut();
			m_forceFadeIn = false;
			m_forceFadeOut = false;
#endif
		}
	}
}
