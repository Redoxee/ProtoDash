using UnityEngine;
using System.Collections;

public class CharacterScript : MonoBehaviour {

	[HideInInspector]
	public uint FloorCounter = 0;


	public void OnCollisionEnter(Collision col)
	{
		if (col.gameObject.tag == "Floor") {
			FloorCounter += 1;
		}
	}

	public void OnCollisionExit(Collision col)
	{
		if (col.gameObject.tag == "Floor")
		{
			FloorCounter -= 1;
		}
	}
}
