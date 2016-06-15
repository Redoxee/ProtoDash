﻿//#define DEBUG_RAY

using UnityEngine;
using System.Collections;

public class MainScript : MonoBehaviour {
	private delegate Vector2 gameplayDelegate(Vector2 currentVelocity);
	private delegate void startStateDelegate();
	private delegate void endStateDelegate();
	struct State {
		public State(string name,startStateDelegate d1, gameplayDelegate d2, endStateDelegate d3)
		{
			start = d1;
			gameplay = d2;
			end = d3;
			this.name = name;

		}
		public startStateDelegate start;
		public gameplayDelegate gameplay;
		public endStateDelegate end;
		public string name;
	};


	State Idle;
	State Jump;
	State Dash;

	[SerializeField]
	private MonoBehaviour mainCharacter;
	[SerializeField]
	private Camera mainCamera;
	
	[SerializeField]
	private float propulsionImpulse = 0.5f;
	[SerializeField]
	private float maxPropulsion = 15.0f;

	[SerializeField]
	private float jumpInputFloorDelay = .125f;
	[SerializeField]
	private float jumpForce = 15.0f;
	[SerializeField]
	private float earlyJumpDistance = .65f;
	[SerializeField]
	private float lateJumpDuration = .25f;
	[SerializeField]
	private float wallJumpAngle = 55.0f;
	[SerializeField]
	private float wallJumpForce = 15.0f;


	[SerializeField]
	private float airBreakFactor = .85f;
	[SerializeField]
	private float wallSlideSpeed = 5.0f;

	[SerializeField]
	private float airPropulsion = 0.125f;

	[SerializeField]
	private float magnetRadius = .6f;
	[SerializeField]
	private float magnetForce = .5f;

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
	[SerializeField]
	private float dashCoolDown = 0.5f;


	[SerializeField]
	public float maxEnergyPoints = 100.0f;
	[SerializeField]
	private float floorEnergyPointsRecovery = 200.0f;
	[SerializeField]
	private float wallEnergyRecoveryPoints = 80.0f;
	[SerializeField]
	private float airEnergyRecoveryPoints = 30.0f;
	
	[HideInInspector]
	public float currentEnergy = 100.0f;

	private Rigidbody2D characterRB;
	[SerializeField]
	private GameObject beak;

	private float screenRatio;

	private bool isMouseDown = false;
	private bool isMouseUp = false;
	private bool isMousePressed = false;
	private bool isSweeping = false;
	private Vector3 currentFacingVector;

	private bool hasStartedFloorInput = false;

	private float floorButtonDownTimer;
	private bool canLateJump = false;
	private float jumpTimer = .0f;

	private Vector2 wallJumpVector;
	private Vector2 mirroredWallJumpVector;
	private bool hasAirBreak;

	private float squareSwipeInputTrigger;
	private float dashProgression = -1.0f;
	private Vector3 dashVector;
	private State currentState;

	private Vector3 tapPosition;
	private float dashTimer = 0.0f;

	//[SerializeField]
	private float upDashCost = 75.0f;
	//[SerializeField]
	private float diagonalUpDashCost = 60.0f;
	//[SerializeField]
	private float lateralDashCost = 50.0f;
	//[SerializeField]
	private float diagonalDownDashCost = 40.0f;
	//[SerializeField]
	private float downDashCost = 0.0f;
	private float[] energyCostTable = new float[11];



	private float probDistance = 2.0f;
	private float skinDistance = .65f;
	[SerializeField]
	private bool isTouchingDown = false;
	[SerializeField]
	private bool isTouchingRight = false;
	[SerializeField]
	private bool isTouchingLeft = false;
	private bool isInMagnetRight = false;
	private bool isInMagnetLeft = false;
	private bool isInEarlyJumpRange = false;

	private float originalBeakX = 0.0f;

	private void _InitializeStates()
	{
		Idle = new State("Idle", _StartIdle, _GameplayIdle, _EndIdle);
		Jump = new State("Jump", _StartJump, _GameplayJump, _EndJump);
		Dash = new State("Dash", _StartDash, _GameplayDash, _EndDash);
	}

	private void _InitializeDashCosts()
	{
		energyCostTable[0] = 0;
		energyCostTable[1] = lateralDashCost;
		energyCostTable[2] = lateralDashCost;
		energyCostTable[3] = 0;
		energyCostTable[4] = upDashCost;
		energyCostTable[5] = diagonalUpDashCost;
		energyCostTable[6] = diagonalUpDashCost;
		energyCostTable[7] = 0;
		energyCostTable[8] = downDashCost;
		energyCostTable[9] = diagonalDownDashCost;
		energyCostTable[10] = diagonalDownDashCost;
	}

	private void _InitializeWallJumpVectors()
	{
		float a = wallJumpAngle / 360.0f * Mathf.PI * 2.0f;
		wallJumpVector = new Vector2(Mathf.Cos(a), Mathf.Sin(a));
		mirroredWallJumpVector = wallJumpVector; mirroredWallJumpVector.x *= -1;
	}

	void Start() {
		squareSwipeInputTrigger = swipeInputDistance * swipeInputDistance;

		characterRB = mainCharacter.GetComponent<Rigidbody2D>();

		originalBeakX = beak.transform.localPosition.x;

		Rect cameraRect = mainCamera.pixelRect;
		if (cameraRect.width < cameraRect.height)
		{
			screenRatio = 1.0f / cameraRect.width;
		}
		else
		{
			screenRatio = 1.0f / cameraRect.height;
		}

		currentFacingVector = new Vector3(1, 0, 0);
		_InitializeDashCosts();
		_InitializeStates();
		_InitializeWallJumpVectors();
		currentState = Idle;
	}

	private void _SetState(State newState)
	{
		currentState.end();
		currentState = newState;
		newState.start();
	}

	void Update()
	{
		if (Input.GetMouseButtonDown(0))
		{
			isMouseDown = true;
		}
		isMousePressed = Input.GetMouseButton(0);
		if (Input.GetMouseButtonUp(0))
		{
			isMouseUp = true;
		}
		Vector3 sv = tapPosition - Input.mousePosition;
		sv.x *= screenRatio;
		sv.y *= screenRatio;
		isSweeping = squareSwipeInputTrigger < sv.sqrMagnitude;

		updateBeak();
	}

	void FixedUpdate() {
		updateRayCasts();

		Vector3 newVelocity = characterRB.velocity;

		newVelocity = currentState.gameplay(newVelocity);

		characterRB.velocity = newVelocity;
		refillEnergy();
		updateDashInput();
		isMouseDown = false;
		isMouseUp = false;
	}

	private void updateRayCasts()
	{
		isTouchingDown = false;
		isTouchingLeft = false;
		isTouchingRight = false;
		isInEarlyJumpRange = false;
		isInMagnetLeft = false;
		isInMagnetRight = false;
		RaycastHit2D rayDown = Physics2D.Raycast(characterRB.position, Vector2.down, probDistance);
		RaycastHit2D rayRight = Physics2D.Raycast(characterRB.position, Vector2.right, probDistance);
		RaycastHit2D rayLeft = Physics2D.Raycast(characterRB.position, Vector2.left, probDistance);
#if DEBUG_RAY
		Debug.DrawRay(characterRB.position, Vector2.down * probDistance, Color.red);
		Debug.DrawRay(characterRB.position, Vector2.right * probDistance, Color.red);
		Debug.DrawRay(characterRB.position, Vector2.left * probDistance, Color.red);
#endif
		if (rayDown.collider)
		{
			if (rayDown.distance <= earlyJumpDistance)
			{
				isInEarlyJumpRange = true;
				if (rayDown.distance <= skinDistance)
				{
					isTouchingDown = true;
				}
			}
		}
		if (rayRight.collider)
		{
			if (rayRight.distance <= magnetRadius)
			{
				isInMagnetRight = true;
				if (rayRight.distance <= skinDistance)
				{
					isTouchingRight = true;
				}
			}
		}
		if (rayLeft.collider)
		{
			if (rayLeft.distance <= magnetRadius)
			{
				isInMagnetLeft = true;
				if (rayLeft.distance <= skinDistance)
				{
					isTouchingLeft = true;
				}
			}
		}
	}

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
						currentEnergy = Mathf.Max(0, currentEnergy - dCost);

						if (dashDirection == 8) // dashDirection down
						{
							currentFacingVector.x *= -1;
						}

						_SetState(Dash);
					}
				}
			}
			else
			{
				tapPosition = mp;
			}
		}
	}

	private void refillEnergy()
	{
		float refillRate = airEnergyRecoveryPoints;
		if (isTouchingDown)// characterS.downCollision)
			refillRate = floorEnergyPointsRecovery;
		else if (isTouchingRight || isTouchingLeft)// characterS.rightCollision || characterS.leftCollision)
			refillRate = wallEnergyRecoveryPoints;
		currentEnergy = Mathf.Min(maxEnergyPoints, currentEnergy + refillRate * Time.fixedDeltaTime);
	}

	private void updateBeak()
	{
		float bPos = originalBeakX * currentFacingVector.x;
		Vector3 cPos = beak.transform.localPosition;
		if (cPos.x != bPos)
		{
			cPos.x = bPos;
			beak.transform.localPosition = cPos;
		}
	}

	/**
	* Idle
	**/

	private void _StartIdle()
	{
		floorButtonDownTimer = -1;
		hasStartedFloorInput = false;
		canLateJump = false;
	}
	private Vector2 _GameplayIdle(Vector2 currentVelocity) {
		if (isTouchingDown)// characterS.downCollision)
		{

			if (isMouseDown)
			{
				floorButtonDownTimer = 0;
				hasStartedFloorInput = true;
			}
			else if (isMouseUp && hasStartedFloorInput)
			{
				currentVelocity.y = jumpForce;
				_SetState(Jump);
			}
			else if (isMousePressed && hasStartedFloorInput)
			{
				floorButtonDownTimer += Time.fixedDeltaTime;
				if (floorButtonDownTimer >= jumpInputFloorDelay)
				{
					currentVelocity.y = jumpForce;
					_SetState(Jump);
				}
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

	/**
	* Jump
	**/
	private void _StartJump()
	{
		hasAirBreak = false;
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
				return currentVelocity;
			}
		}
		if (canLateJump)
		{
			jumpTimer += Time.fixedDeltaTime;
			if (jumpTimer < lateJumpDuration)
			{
				if (isMouseDown || isMousePressed || isMouseUp)
				{
					currentVelocity.y = jumpForce;
					_SetState(Jump);
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
				return currentVelocity;
			}
			else if (!hasAirBreak)
			{
				hasAirBreak = true;
				currentVelocity *= airBreakFactor;
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

	/**
	* Dash
	**/

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
		tapPosition = Input.mousePosition;
	}


	public string getState()
	{
		return currentState.name;
	}

	public float getFacingSign()
	{
		return currentFacingVector.x;
	}
}
