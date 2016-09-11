using UnityEngine;
using System.Collections;

namespace Dasher
{
	public class EndNode : MonoBehaviour
	{
		[SerializeField]
		private float rotationSpeed = 1.0f;


		// Update is called once per frame
		void Update()
		{
			transform.rotation *= Quaternion.AngleAxis(rotationSpeed * Time.deltaTime, Vector3.forward);
		}

		void OnTriggerEnter2D(Collider2D col)
		{
			if (col.gameObject.tag == "Player")
			{
				MainGameProcess.Instance.NotifyEndLevelReached();
			}
		}
	}
}
