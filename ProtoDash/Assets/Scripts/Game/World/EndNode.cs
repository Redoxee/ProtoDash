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
			float dt = GameProcess.Instance.GameTime.GetGameDeltaTime();
			transform.rotation *= Quaternion.AngleAxis(rotationSpeed * dt, Vector3.forward);
		}

		void OnTriggerEnter2D(Collider2D col)
		{
			if (col.gameObject.tag == "Player")
			{
				GameProcess.Instance.NotifyEndLevelReached();
			}
		}
	}
}
