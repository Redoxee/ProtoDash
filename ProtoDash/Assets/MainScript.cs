using UnityEngine;
using System.Collections;

public class MainScript : MonoBehaviour {
	private delegate Vector2 gameplayDelegate(Vector2 currentVelocity);
	private delegate void startStateDelegate();
	private delegate void endStateDelegate();
	struct State {
		public State(startStateDelegate d1, gameplayDelegate d2, endStateDelegate d3)
		{
			start = d1;
			gameplay = d2;
			end = d3;
		}
		public startStateDelegate start;
		public gameplayDelegate gameplay;
		public endStateDelegate end;
	};


	State Idle;
	State Jump;
	State Dash;

	[SerializeField]
	private MonoBehaviour mainCharacter;
	[SerializeField]
	private Camera mainCamera;

	[SerializeField]
	private float jumpImpulse = 5;
	[SerializeField]
	private float propulsionImpulse = 0.5f;
	[SerializeField]
	private float maxPropulsion = 15.0f;
	
	[SerializeField]
	private float airBreakFactor = .85f;
	[SerializeField]
	private float swipeInputDistance = .25f;
	[SerializeField]
	private float swipeDeadZone = .4f;
	[SerializeField]
	private AnimationCurve dashCurve;
	[SerializeField]
	private float dashMaxSpeed = 17.0f;
	[SerializeField]
	private float dashDuration = .7f;

	private Rigidbody characterRB;
	private CharacterScript characterS;
	private Rect cameraRect;
	
	private Vector3 floorTapPosition;
	private bool hasFloortaped;
	private float tapTimer;

	private Vector3 currentFacingVector;
	private Vector3 airTapPosition;
	private bool hasAirBreak;
	private bool hasReleaseAirTouch;
	private bool hasDashed = false;
	private float squareSwipeInputTrigger;
	private float dashTimer = -1.0f;
	private Vector3 dashVector;
	private State currentState;

	private void _InitializeStates()
	{
		Idle = new State(_StartIdle, _GameplayIdle, _EndIdle);
		Jump = new State(_StartJump, _GameplayJump, _EndJump);
		Dash = new State(_StartDash, _GameplayDash, _EndDash);

		cameraRect = mainCamera.pixelRect;

		currentFacingVector = new Vector3(1, 0, 0);
		currentState = Idle;
	}

	void Start() {
		squareSwipeInputTrigger = swipeInputDistance * swipeInputDistance;

		characterRB = mainCharacter.GetComponent<Rigidbody>();
		characterS = mainCharacter.GetComponent<CharacterScript>();

		_InitializeStates();
	}

	private void _SetState(State newState)
	{
		currentState.end();
		currentState = newState;
		newState.start();
	}

	void FixedUpdate() {

		Vector3 newVelocity = characterRB.velocity;

		newVelocity = currentState.gameplay(newVelocity);

		characterRB.velocity = newVelocity;

	}

	private bool IsSwipping(Vector3 startPoint, Vector3 endPoint)
	{
		Vector3 sv = endPoint - startPoint;
		sv.x /= cameraRect.width;
		sv.y /= cameraRect.height;
		return squareSwipeInputTrigger < sv.sqrMagnitude; 
	}

	/**
	* Idle
	**/

	private void _StartIdle()
	{
		hasDashed = false;
	}
	private Vector2 _GameplayIdle(Vector2 currentVelocity) {

		if (characterS.FloorCounter > 0)
		{
			float d = currentFacingVector.x * propulsionImpulse;
			currentVelocity.x = Mathf.Clamp(currentVelocity.x + d, -maxPropulsion, maxPropulsion);

			if (Input.GetMouseButtonUp(0))
			{
				currentVelocity.y += jumpImpulse;
			}
			else
			{
				if (Input.GetMouseButtonDown(0))
				{
					hasFloortaped = true;
					floorTapPosition = Input.mousePosition;
				}
				else if (Input.GetMouseButton(0))
				{
					if (IsSwipping(floorTapPosition,Input.mousePosition))
					{
						Vector3 swipeVector = (Input.mousePosition - floorTapPosition);

						swipeVector.Normalize();
						floorTapPosition = Input.mousePosition;
						/***
						if (swipeVector.x > swipeDeadZone)
						{
							currentFacingVector.x = 1;
						}
						else if (swipeVector.x < -swipeDeadZone)
						{
							currentFacingVector.x = -1;
						}
						**/
						SetDashVector(swipeVector);
						_SetState(Dash);
					}
				}
			}
		}else
		{
			_SetState(Jump);
		}
		return currentVelocity;
	}

	private void _EndIdle()
	{
		hasFloortaped = false;
	}

	private void SetDashVector(Vector3 inVec)
	{
		dashVector = new Vector3(0, 0, 0);
		if (inVec.x > swipeDeadZone)
		{
			dashVector.x += 1;
		}
		else if (inVec.x < -swipeDeadZone)
		{
			dashVector.x -= 1;
		}
		if (inVec.y > swipeDeadZone)
		{
			dashVector.y += 1;
		}
		else if (inVec.y < -swipeDeadZone)
		{
			dashVector.y -= 1;
		}
	}
	/**
	* Jump
	**/
	private void _StartJump()
	{
		hasAirBreak = false;
		hasReleaseAirTouch = false;
	}

	private Vector2 _GameplayJump(Vector2 currentVelocity)
	{

		if (hasAirBreak)
		{
			if (IsSwipping(airTapPosition,Input.mousePosition) && !hasDashed)
			{
				Vector3 swipeVector = (Input.mousePosition - airTapPosition);
				swipeVector.Normalize();
				SetDashVector(swipeVector);

				_SetState(Dash);
			}
			if (Input.GetMouseButtonUp(0))
			{
				hasReleaseAirTouch = true;
			}
		}
		if (Input.GetMouseButtonDown(0) && !hasAirBreak)
		{
			hasAirBreak = true;
			airTapPosition = Input.mousePosition;
			currentVelocity *= airBreakFactor;
		}
		else if (Input.GetMouseButtonUp(0))
		{
		}

		if (characterS.FloorCounter > 0)
		{
			_SetState(Idle);
			hasAirBreak = false;
		}
		return currentVelocity;
	}

	private void _EndJump()
	{

	}

	/**
	* Dash
	**/

	private void _StartDash()
	{
		hasDashed = true;
		dashTimer = dashDuration;
		if (dashVector.x > 0)
		{
			currentFacingVector.x = 1;
		}
		else if (dashVector.x < 0)
		{
			currentFacingVector.x = -1;
		}
	}

	private Vector2 _GameplayDash(Vector2 currentVelocity)
	{
		float dt = Time.fixedDeltaTime;
		dashTimer -= dt;
		if (dashTimer > 0)
		{
			float progression = 1.0f - dashTimer / dashDuration;
			float v = dashCurve.Evaluate(progression) * dashMaxSpeed;
			currentVelocity = dashVector * v;
		}
		else
		{
			if (characterS.FloorCounter > 0)
			{
				_SetState(Idle);
			} else
			{
				_SetState(Jump);
 			}
			currentVelocity = dashVector * dashCurve.Evaluate(1.0f) * dashMaxSpeed;
		}
		return currentVelocity;
	}

	private void _EndDash()
	{
	}
}
