using UnityEngine;
using System.Collections;

namespace Dasher
{
	public partial class Character
	{

		/**
		* Jump
		**/
		private void _StartJump()
		{

			currentSquishY = .8f;
			jumpTimer = .0f;
		}

		private Vector2 _GameplayJump(Vector2 currentVelocity)
		{


			if (isTouchingRight || isTouchingLeft)
			{
				currentVelocity.y = Mathf.Max(currentVelocity.y, -wallSlideSpeed);
				if (isMouseDown)
				{
					currentVelocity = isTouchingLeft ? wallJumpVector : mirroredWallJumpVector;
					currentFacingVector.x = isTouchingLeft ? 1 : -1;
					currentVelocity *= wallJumpForce;
					currentSquishX = .55f;
					canLateJump = false;
					traceManager.NotifyJump(characterRB.transform.position);
					return currentVelocity;
				}
			}
			if (canLateJump)
			{
				jumpTimer += Time.fixedDeltaTime;
				if (jumpTimer < lateJumpDuration)
				{
					if (isMouseDown)
					{
						currentVelocity.y = jumpForce;
						_SetState(Jump);
						traceManager.NotifyJump(characterRB.transform.position);
						return currentVelocity;
					}
				}
				else
				{
					canLateJump = false;
				}
			}

			if (isTouchingDown && currentVelocity.y <= 0)
			{
				_SetState(Idle);
				return currentVelocity;
			}
			if (isMouseDown)
			{
				if (currentVelocity.y < 0 && isInEarlyJumpRange)
				{
					currentVelocity.y = jumpForce;
					_SetState(Jump);
					traceManager.NotifyJump(characterRB.transform.position);
					return currentVelocity;
				}
			}


			{
				bool hasMagneted = false;
				Vector2 magnetVector = Vector2.zero;
				if (magnetRadius > .0f)
				{
					if (isInMagnetRight)
					{
						magnetVector.x += magnetForce;
						hasMagneted = true;
					}
					if (isInMagnetLeft)
					{
						magnetVector.x -= magnetForce;
						hasMagneted = true;
					}
					currentVelocity += magnetVector;
				}
				if (!hasMagneted)
				{
					float d = currentFacingVector.x * airPropulsion;
					if (Mathf.Abs(currentVelocity.x + d) < maxPropulsion)
					{
						currentVelocity.x = currentVelocity.x + d;
					}
				}
			}
			return currentVelocity;
		}

		private void _EndJump()
		{
			canLateJump = false;
		}
	}
}
