using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dasher
{
	public class Folicule : MonoBehaviour {
		public GameObject m_hairPrefab = null;
		public Transform m_childNode = null;

		public uint BodyCount = 4;

		public float StartDamp = .05f;
		public float DampDecreaseFactor = .5f;

		public float scaleDecreaseFactor = .8f;

		[SerializeField]
		Character m_watched = null;
		float currentFacingSign = 0f;

		List<GameObject> m_childs = new List<GameObject>();

		private void Awake()
		{

			var currentDamp = StartDamp;
			Transform currentTarget = m_childNode.transform;
			var currentScale = Vector3.one * scaleDecreaseFactor;

			for (int i = 0; i < BodyCount; ++i)
			{
				var child = Instantiate(m_hairPrefab, currentTarget.position, currentTarget.rotation);
				var hair = child.GetComponentInChildren<HairBehavior>();
				hair.m_dampLoose = currentDamp;
				hair.transform.localScale = currentScale;
				hair.m_target = currentTarget;

				m_childs.Add(child);

				currentTarget = hair.m_childNode;
				currentDamp *= DampDecreaseFactor;
				currentScale *= scaleDecreaseFactor;
			}
		}

		private void Update()
		{
			var facing = m_watched.getFacingSign();
			if (currentFacingSign != facing)
			{
				currentFacingSign = facing;


				Vector3 s = transform.localScale;
				s.x = Mathf.Abs(s.x) * facing;
				transform.localScale = s;

				foreach (GameObject child in m_childs)
				{
					s = child.transform.localScale;
					s.x = Mathf.Abs(s.x) * facing;
					child.transform.localScale = s;
				}

			}
		}

		private void OnDestroy()
		{
			foreach (GameObject child in m_childs)
			{
				Destroy(child);
			}
		}
	}
}
