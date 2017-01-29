using UnityEngine;
using System.Collections.Generic;

namespace Dasher
{
	public class Key : MonoBehaviour
	{
		[SerializeField]
		GameObject m_background = null;

		[SerializeField]
		GameObject m_sigil = null;

		[SerializeField]
		Collider2D m_trigger = null;

		[SerializeField]
		float m_rotationSpeed = 50f;

		[SerializeField]
		List<KeyBlock> m_activables = new List<KeyBlock>();

		[Header("Animation")]
		[SerializeField]
		AnimationCurve m_branchLengthAnimation = null;
		[SerializeField]
		AnimationCurve m_branchThiknessAnimation = null;
		[SerializeField]
		AnimationCurve m_alphaCurve = null;

		Material m_sigilMaterial = null;
		const string c_stretchName = "_Stretch";
		const string c_thicknessName = "_Radius";

		Color m_baseColor;

		float m_duration = 1f;
		float m_timer = 0f;

		void Initialize()
		{
			m_duration = Mathf.Max(
				m_branchLengthAnimation.keys[m_branchLengthAnimation.length - 1].time,
				m_branchThiknessAnimation.keys[m_branchThiknessAnimation.length - 1].time
				);
			m_timer = m_duration + 1;

			var sigilRenderer = m_sigil.GetComponent<MeshRenderer>();
			m_sigilMaterial = new Material(sigilRenderer.material);
			sigilRenderer.material = m_sigilMaterial;

			m_baseColor = m_sigilMaterial.color;

			m_sigilMaterial.SetFloat(c_stretchName, m_branchLengthAnimation.Evaluate(0));
			m_sigilMaterial.SetFloat(c_thicknessName, m_branchThiknessAnimation.Evaluate(0));
		}


		void StartAnimation()
		{
			m_timer = 0;
		}

		void UpdateAnimation()
		{
			if (m_timer < m_duration)
			{
				var oldTimer = m_timer;
				TimeManager tm = GameProcess.Instance.GameTime;

				m_timer += tm.GetGameDeltaTime();
				var progression = Mathf.Min(m_timer, m_duration);
				m_sigilMaterial.SetFloat(c_stretchName, m_branchLengthAnimation.Evaluate(progression));
				m_sigilMaterial.SetFloat(c_thicknessName, m_branchThiknessAnimation.Evaluate(progression));
				m_baseColor.a = m_alphaCurve.Evaluate(progression);
				m_sigilMaterial.color = m_baseColor;

				if (m_timer >= m_duration)
				{
					gameObject.SetActive(false);
				}
			}
		}

		private void Awake()
		{
			Initialize();
		}

#if UNITY_EDITOR
		[SerializeField]
		bool m_forceActivation = false;
#endif

		private void Update()
		{
#if UNITY_EDITOR
			if (m_forceActivation)
			{
				Activate();
				m_forceActivation = true;
			}
#endif

			m_sigil.transform.rotation *= Quaternion.AngleAxis(Time.deltaTime * m_rotationSpeed, Vector3.forward);
			UpdateAnimation();
		}

		void Activate()
		{
			m_trigger.enabled = false;
			StartAnimation();

			int ac = m_activables.Count;
			for (int i = 0; i < ac; ++i)
			{
				m_activables[i].Activate(transform);
			}
		}

		private void OnTriggerEnter2D(Collider2D collision)
		{
			Activate();
		}

#if UNITY_EDITOR

		private void OnDrawGizmos()
		{
			if (m_activables != null && m_activables.Count > 0)
			{
				var decal = m_activables[0].transform.localScale / 2;
				var source = m_sigil.transform.position;
				var tz = transform.position.z - 5;
				source.z = tz;
				for (int i = 0; i < m_activables.Count; ++i)
				{
					var target = m_activables[i].transform.position + decal;
					target.z = tz;
					Debug.DrawLine(source, target, Color.blue);
				}
			}
		}
#endif
	}
}
