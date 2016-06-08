using UnityEngine;
using System.Collections;

public class CharacterScript : MonoBehaviour {

	public bool downCollision = false;
	public bool upCollision = false;
	public bool rightCollision = false;
	public bool leftCollision = false;

	public void OnCollisionStay2D(Collision2D col)
	{
		foreach(ContactPoint2D c in col.contacts)
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
}
