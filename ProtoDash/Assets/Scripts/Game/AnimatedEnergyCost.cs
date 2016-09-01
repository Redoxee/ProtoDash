using UnityEngine;
using UnityEngine.UI;

namespace Dasher
{
	public class AnimatedEnergyCost : MonoBehaviour
	{

		private float m_animationDuration = 1f;
		private Vector3 m_animationOffset = new Vector3(-1, 0, 0) * 60;
		[SerializeField]
		private AnimationCurve m_animationCurve;

		private float m_animationTimer = -1f;
		private Vector3 m_animationStartPos;
		private Color m_originalCostColor;

		private Material m_material;

		void Start()
		{
			Image img = GetComponent<Image>();
			m_material = new Material(img.material);
			img.material = m_material;
			m_originalCostColor = m_material.color;
		}

		public void Reset()
		{
			gameObject.SetActive(false);
		}

		public void StartAnimation()
		{
			gameObject.SetActive(true);

			m_animationTimer = m_animationDuration;
			m_animationStartPos = transform.localPosition;
		}

		void Update()
		{
			if (m_animationTimer > 0)
			{
				m_animationTimer -= Time.deltaTime;
				float prog = 1f - m_animationTimer / m_animationDuration;
				prog = m_animationCurve.Evaluate(prog);
				m_originalCostColor.a = prog;
				m_material.color = m_originalCostColor;
				transform.localPosition = m_animationStartPos + m_animationOffset * (1f - prog);

				if (m_animationTimer <= 0)
				{
					gameObject.SetActive(false);
				}
			}
		}
	}
}
