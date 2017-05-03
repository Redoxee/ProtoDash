using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Folicule : MonoBehaviour {
	public GameObject m_hairPrefab = null;
	public Transform m_childNode = null;

	public uint BodyCount = 4;

	public float StartDamp = .05f;
	public float DampDecreaseFactor = .5f;

	public float scaleDecreaseFactor = .8f;

	private void Awake()
	{
		var currentDamp = StartDamp;
		Transform currentTarget = m_childNode;
		var currentScale = Vector3.one * scaleDecreaseFactor;

		for (int i = 0; i < BodyCount; ++i)
		{
			var child = Instantiate(m_hairPrefab, currentTarget.position, currentTarget.rotation);
			var hair = child.GetComponentInChildren<HairBehavior>();
			hair.m_target = currentTarget;
			hair.m_dampLoose = currentDamp;
			hair.transform.localScale = currentScale;

			currentTarget = hair.m_childNode;
			currentDamp *= DampDecreaseFactor;
			currentScale *= scaleDecreaseFactor;
		}
	}

}
