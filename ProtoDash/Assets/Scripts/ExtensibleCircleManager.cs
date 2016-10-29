using UnityEngine;
using System.Collections.Generic;

namespace Dasher
{
	public class ExtensibleCircleManager : MonoBehaviour
	{
		Transform[] m_circles;

		[SerializeField]
		Material m_referenceMaterial = null;
		int m_ratioId;

		Material m_circleMaterial;

		void Initialize()
		{
			m_ratioId = Shader.PropertyToID("_Ratio");

			m_circleMaterial = new Material(m_referenceMaterial);

			int cCount = transform.childCount;
			m_circles = new Transform[cCount];
			for (int i = 0; i < cCount; ++i)
			{
				var circle = transform.GetChild(i);
				m_circles[i] = circle.transform;
				var renderer = circle.GetComponentInChildren<Renderer>();
				renderer.material = m_circleMaterial;
			}
			SetWidth(1);
		}

		void Awake()
		{
			Initialize();
		}

		public void SetColor(Color col)
		{
			m_circleMaterial.color = col;
		}

		public void SetWidth(float width)
		{
			var newScale = new Vector3(width, 1f, 1f);
			for (int i = 0; i < m_circles.Length; ++i)
			{
				m_circles[i].transform.localScale = newScale;
			}
			m_circleMaterial.SetFloat(m_ratioId, 1f / width);
		}
	}
}
