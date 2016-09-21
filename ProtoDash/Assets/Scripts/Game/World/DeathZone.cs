using UnityEngine;
using System.Collections;

namespace Dasher
{
	public class DeathZone : MonoBehaviour
	{


		void OnTriggerEnter2D(Collider2D col)
		{
			if (col.gameObject.tag == "Player")
			{
				GameProcess.Instance.NotifyDeathZoneTouched();
			}
		}

		void OnTriggerExit2D(Collider2D col)
		{
			if (col.gameObject.tag == "Player")
			{
				GameProcess.Instance.NotifyDeathZoneEmerged();
			}
		}
	}
}
