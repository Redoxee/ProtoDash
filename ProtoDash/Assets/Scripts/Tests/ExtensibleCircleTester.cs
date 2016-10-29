using UnityEngine;
using System.Collections;
using Dasher;

public class ExtensibleCircleTester : MonoBehaviour {
	public ExtensibleCircleManager manager = null;

	float time = 0f;
	void Update()
	{
		time += Time.deltaTime;
		manager.SetWidth(2f + Mathf.Sin(time));
	}
}
