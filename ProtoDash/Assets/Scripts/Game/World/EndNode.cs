using UnityEngine;
using System.Collections;

namespace Dasher
{
	public class EndNode : MonoBehaviour
	{
		const string c_animationTime = "_AnimationTime";
		const string c_closure = "_ClosureFactor";

		private float m_AnimationSpeed = 2.5f;

		private float m_currentTime = 0f;
		private float m_currentClosure = 0f;
		private float m_closureDuration = 1f;
		private bool m_isInClosure = false;

		private Material m_material;
		public Color CurrentColor { get { return m_material.color; } }


		[SerializeField]
		private Color m_ChampMissedColor = Color.white;

		void Awake()
		{
			m_material = GetComponent<Renderer>().material;
			m_material.SetFloat(c_closure, 0f);
		}

		void Start()
		{
			GameProcess gp = GameProcess.Instance;
			if (gp != null)
				gp.EndNode = this;
		}

		void Update()
		{
			float dt = Time.deltaTime;
			m_currentTime += dt;
			if (m_closureDuration == 1f)
			{
				m_material.SetFloat(c_animationTime, m_currentTime * m_AnimationSpeed);
			}
			if (m_isInClosure)
			{
				m_currentClosure += dt;
				float progression = Mathf.Min(m_currentClosure, 1f) / m_closureDuration;
				m_material.SetFloat(c_closure, progression);
				if (m_currentClosure >= 1f)
				{
					m_currentClosure = 1f;
					m_isInClosure = false;
				}
			}

		}

		void OnTriggerEnter2D(Collider2D col)
		{
			if (col.gameObject.tag == "Player")
			{
				GameProcess.Instance.NotifyEndLevelReached(gameObject);
			}
			m_isInClosure = true;
		}

		public void NotifyChampTimeMissed()
		{
			m_material.color = m_ChampMissedColor;
		}
	}
}
