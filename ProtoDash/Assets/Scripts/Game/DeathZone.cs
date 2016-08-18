using UnityEngine;
using System.Collections;

public class DeathZone : MonoBehaviour {


	void OnTriggerEnter2D(Collider2D col)
	{
		if (col.gameObject.tag == "Player")
		{
			MainProcess.GetInstance().getCurrentGUI().NotifyDeathZoneTouched();
		}
	}
}
