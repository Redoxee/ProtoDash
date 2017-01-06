using UnityEngine;
using System.Collections;

namespace Dasher
{
	public class Key : MonoBehaviour
	{
		[SerializeField]
		GameObject m_background = null;

		[SerializeField]
		GameObject m_mainGraphics = null;

		[SerializeField]
		float m_rotationSpeed = 50f;

		private void Update()
		{
			m_mainGraphics.transform.rotation *= Quaternion.AngleAxis(Time.deltaTime * m_rotationSpeed, Vector3.forward);
		}
	}
}
