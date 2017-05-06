using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dasher
{
	public class HairBehavior : MonoBehaviour
	{

		public Transform m_childNode = null;

		public Transform m_target = null;

		public float m_dampLoose = 1f;

		Vector3 m_velocity;

		void Update()
		{
			transform.position = Vector3.SmoothDamp(transform.position, m_target.position, ref m_velocity, m_dampLoose);
		}


	}
}
