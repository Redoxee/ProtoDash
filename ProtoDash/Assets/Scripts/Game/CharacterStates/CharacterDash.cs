using UnityEngine;
using System.Collections;
namespace Dasher
{
	public partial class Character
	{

		private void updateDashInput()
		{
			Vector3 mp = Input.mousePosition;
			if (dashTimer > 0)
			{
				dashTimer -= Time.fixedDeltaTime;
			}
			if (isMouseDown)
			{
				tapPosition = mp;
			}
			else if (isMousePressed)
			{
				if (dashTimer <= 0)
				{
					Vector3 dv = getDashVectorFromSwipe((mp - tapPosition).normalized);
					if (dv.sqrMagnitude > 0)
					{
						dv.Normalize();
						System.UInt16 dashDirection = 0x0;
						if (dv.x > 0)
							dashDirection |= 0x1;
						else if (dv.x < 0)
							dashDirection |= 0x2;
						if (dv.y > 0)
							dashDirection |= 0x4;
						else if (dv.y < 0)
							dashDirection |= 0x8;
						float dCost = energyCostTable[dashDirection];
						if (isSweeping && dCost <= currentEnergy)
						{
							dashVector = dv;
							dashAngle = Mathf.Acos(dv.x) * Mathf.Sign(dv.y) / Mathf.PI * 180;
							dashRotation = Quaternion.Euler(0, 0, dashAngle);
							currentEnergy = Mathf.Max(0, currentEnergy - dCost);

							//if (dashDirection == 8) // dashDirection down
							//{
							//	currentFacingVector.x *= -1;
							//}

							_SetState(Dash);
							traceManager.NotifyDash(characterRB.transform.position, dashRotation);
						}
					}
				}
				else
				{
					tapPosition = mp;
				}
			}
		}

		/**
		* Dash
		**/

		private Vector3 getDashVectorFromSwipe(Vector3 sw)
		{
			Vector3 res = new Vector3(0, 0, 0);
			if (sw.x > swipeDeadZone)
			{
				res.x += 1;
			}
			else if (sw.x < -swipeDeadZone)
			{
				res.x -= 1;
			}
			if (sw.y > swipeDeadZone)
			{
				res.y += 1;
			}
			else if (sw.y < -swipeDeadZone)
			{
				res.y -= 1;
			}
			return res;
		}

		private void _StartDash()
		{
			dashTimer = dashCoolDown;
			dashProgression = dashDuration;
			if (dashVector.x > 0)
			{
				currentFacingVector.x = 1;
			}
			else if (dashVector.x < 0)
			{
				currentFacingVector.x = -1;
			}

			body.transform.localRotation = dashRotation;
			currentSquishY = .35f;
		}

		private Vector2 _GameplayDash(Vector2 currentVelocity)
		{
			float dt = Time.fixedDeltaTime;
			dashProgression -= dt;
			if (dashProgression > 0)
			{
				float progression = 1.0f - dashProgression / dashDuration;
				float v = dashCurve.Evaluate(progression) * dashMaxSpeed;
				currentVelocity = dashVector * v;
			}
			else
			{
				if (isTouchingDown)
				{
					_SetState(Idle);
				}
				else
				{
					_SetState(Jump);
				}
				currentVelocity = dashVector * dashCurve.Evaluate(1.0f) * dashMaxSpeed;
			}
			return currentVelocity;
		}

		private void _EndDash()
		{
			tapPosition = Input.mousePosition;

			body.transform.localRotation = Quaternion.identity;
		}
	}
}