using UnityEngine;
using System.Collections.Generic;

namespace Dasher
{
	public class KeyBlock : MonoBehaviour
	{

		[SerializeField]
		GameObject m_mainGraphics = null;
		[SerializeField]
		GameObject m_sigil = null;
		[SerializeField]
		Collider2D m_collision = null;
		[SerializeField]
		ComposedStretchableCircle m_strechable = null;



		[Header("Animation")]
		[SerializeField]
		float m_duration = 1f;
		[SerializeField]
		AnimationCurve m_strechCurve = null;
		[SerializeField]
		float m_stretchFlipPoint = .5f;

		[SerializeField]
		AnimationCurve m_scaleCurve = null;
		[SerializeField]
		AnimationCurve m_sigilCurve = null;
		[SerializeField]
		AnimationCurve m_sigilAlphaCurve = null;

		Material m_sigilMaterial = null;
		Color m_sigilColor;
		const string c_stretchName = "_Stretch";
		const string c_thicknessName = "_Radius";

		[Header("BoxDeactivation")]
		[SerializeField]
		float m_boxDeactivation = .5f;
		[Header("Chain")]
		[SerializeField]
		float m_chainDelay = .75f;
		[SerializeField]
		List<KeyBlock> m_chainElements = null;

		float m_activationDelayTimer = 0f; 

		float m_timer = 0f;

		float m_targetLength = 0;

		void Awake()
		{
			Initialize();
		}

		void Initialize()
		{
			m_timer = m_duration + 1;
			m_activationDelayTimer = m_chainDelay + 1;

			var sigilRenderer = m_sigil.GetComponent<Renderer>();
			m_sigilMaterial = new Material(sigilRenderer.material);
			sigilRenderer.material = m_sigilMaterial;
			m_sigilColor = m_sigilMaterial.color;
		}

		void UpdateAnimation()
		{
			if (m_timer < m_duration)
			{
				var oldTime = m_timer;
				var oldProgression = m_timer / m_duration;
				m_timer += GameProcess.Instance.GameTime.GetGameDeltaTime();
				if (m_timer >= m_duration && m_activationDelayTimer > m_chainDelay)
				{
					gameObject.SetActive(false);
					return;
				}
				var progression = Mathf.Min(1,m_timer / m_duration);
				if (progression >= m_stretchFlipPoint && oldProgression < m_stretchFlipPoint)
				{
					m_strechable.Orient(m_end, m_start);
				}
				m_strechable.SetLength(m_strechCurve.Evaluate(progression) * m_targetLength);
				m_mainGraphics.transform.localScale = Vector3.one * m_scaleCurve.Evaluate(progression);

				m_sigilColor.a = m_sigilAlphaCurve.Evaluate(progression);
				m_sigilMaterial.color = m_sigilColor;
				m_sigilMaterial.SetFloat(c_stretchName, m_sigilCurve.Evaluate(progression));
			}
		}

		private void Update()
		{
			UpdateAnimation();
		}

		private void FixedUpdate()
		{
			if (m_activationDelayTimer < m_chainDelay)
			{
				var old = m_activationDelayTimer;
				m_activationDelayTimer += GameProcess.Instance.GameTime.GetGameFixedDeltaTime();
				if (old < m_boxDeactivation && m_activationDelayTimer >= m_boxDeactivation)
				{
					m_collision.enabled = false;
				}
				if (m_activationDelayTimer >= m_chainDelay)
				{
					var count = m_chainElements.Count;
					for (int i = 0; i < count; ++i)
					{
						m_chainElements[i].Activate(m_sigil.transform);
					}
				}
			}
		}

		Vector3 m_start;
		Vector3 m_end;

		public void Activate(Transform source)
		{
			m_start = source.transform.position;
			m_end = m_sigil.transform.position;
			m_start.z = m_end.z - .5f;
			m_end.z = m_start.z;
			m_strechable.gameObject.SetActive(true);
			m_strechable.Orient(m_start, m_end);
			m_targetLength = Vector3.Distance(m_start, m_end);
			m_timer = 0f;
			m_activationDelayTimer = 0f;
		}

#if UNITY_EDITOR

		private void OnDrawGizmos()
		{
			if (m_chainElements != null && m_chainElements.Count > 0)
			{
				var decal = m_chainElements[0].transform.localScale / 2;
				var source = m_sigil.transform.position;
				var tz = transform.position.z - 5;
				source.z = tz;
				for (int i = 0; i < m_chainElements.Count; ++i)
				{
					var target = m_chainElements[i].transform.position + decal;
					target.z = tz;
					Debug.DrawLine(source, target, Color.blue);
				}
			}
		}
#endif
	}
}
