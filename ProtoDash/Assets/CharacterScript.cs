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

	public bool downCollision = false;
	public bool upCollision = false;
	public bool rightCollision = false;
	public bool leftCollision = false;

	public void OnCollisionStay(Collision col)
	{
		foreach(ContactPoint c in col.contacts)
		{
			Vector2 norm = (new Vector2(c.normal.x, c.normal.y)).normalized;
			if (norm.x > .5)
			{
				leftCollision = true;
			}
			else if (norm.x < -.5)
			{
				rightCollision = true;
			}
			if (norm.y > .5)
			{
				downCollision = true;
			}
			else if (norm.y < -.5)
			{
				upCollision = true;
			}
		}
	}

	public void notifyColisionConsumed()
	{
		leftCollision = false;
		rightCollision = false;
		upCollision = false;
		downCollision = false;
	}
	public void OnCollisionExit(Collision col)
	{
		if (col.gameObject.tag == "Floor")
		{
			FloorCounter -= 1;
		}
	}
}
