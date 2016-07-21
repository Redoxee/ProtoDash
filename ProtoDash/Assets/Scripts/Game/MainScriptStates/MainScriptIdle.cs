﻿using UnityEngine;
using System.Collections;

public partial class MainScript {

	/**
	* Idle
	**/

	private void _StartIdle()
	{
		canLateJump = false;
	}

	private Vector2 _GameplayIdle(Vector2 currentVelocity)
	{
		if (isTouchingDown)
		{

			if (isMouseDown)
			{
				currentVelocity.y = jumpForce;
				_SetState(Jump);
				traceManager.NotifyJump(characterRB.transform.position);
			}

			float d = currentFacingVector.x * propulsionImpulse;
			currentVelocity.x = Mathf.Clamp(currentVelocity.x + d, -maxPropulsion, maxPropulsion);
		}
		else
		{
			canLateJump = true;
			_SetState(Jump);
		}
		return currentVelocity;
	}

	private void _EndIdle()
	{
	}

}