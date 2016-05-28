﻿using UnityEngine;
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
	private Vector2 wallJumpVector = new Vector2(1, 1);
	[SerializeField]
	private float wallJumpForce = 15.0f;

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
	public float maxEnergyPoints = 100.0f;
	[SerializeField]
	private float floorEnergyPointsRecovery = 75.0f;
	[SerializeField]
	private float airEnergyRecoveryPoints = 50.0f;
	[SerializeField]
	private float dashEnergyCost = 100.0f;
	
	[HideInInspector]
	public float currentEnergy = 100.0f;

	private Rigidbody characterRB;
	private CharacterScript characterS;

	private float screenRatio;

	private bool isMouseDown = false;
	private bool isMousePressed = false;
	private bool isSweeping = false;
	
	private Vector3 floorTapPosition;

	private Vector3 currentFacingVector;
	private bool hasAirBreak;
	private Vector2 mirrorWallJumpVector;
	private float squareSwipeInputTrigger;
	private float dashProgression = -1.0f;
	private Vector3 dashVector;
	private State currentState;

	private Vector3 tapPosition;

	private void _InitializeStates()
	{
		Idle = new State(_StartIdle, _GameplayIdle, _EndIdle);
		Jump = new State(_StartJump, _GameplayJump, _EndJump);
		Dash = new State(_StartDash, _GameplayDash, _EndDash);

		Rect cameraRect = mainCamera.pixelRect;
		if (cameraRect.width < cameraRect.height)
		{
			screenRatio = 1.0f / cameraRect.width;
		}
		else
		{
			screenRatio = 1.0f / cameraRect.height;
		}

		wallJumpVector.Normalize();
		mirrorWallJumpVector = new Vector2(-wallJumpVector.x, wallJumpVector.y);

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

	void Update()
	{
		if (Input.GetMouseButtonDown(0))
		{
			isMouseDown = true;
		}
		isMousePressed = Input.GetMouseButton(0);

		Vector3 sv = tapPosition - Input.mousePosition;
		sv.x *= screenRatio;
		sv.y *= screenRatio;
		isSweeping = squareSwipeInputTrigger < sv.sqrMagnitude;
	}

	void FixedUpdate() {

		Vector3 newVelocity = characterRB.velocity;

		newVelocity = currentState.gameplay(newVelocity);
		characterRB.velocity = newVelocity;
		updateDashInput();
		isMouseDown = false;
		characterS.notifyColisionConsumed();
	}

	private void updateDashInput()
	{
		Vector3 mp = Input.mousePosition;
		if (isMouseDown)
		{
			tapPosition = Input.mousePosition;
		}
		else if (isMousePressed)
		{
			if (isSweeping && dashEnergyCost <= currentEnergy)
			{
				currentEnergy = Mathf.Max(0, currentEnergy - dashEnergyCost);
				Vector3 swipePosition = (mp - tapPosition).normalized;
				SetDashVector(swipePosition);
				_SetState(Dash);
			}
		}
	}

	/**
	* Idle
	**/

	private void _StartIdle()
	{
	}
	private Vector2 _GameplayIdle(Vector2 currentVelocity) {
		if (currentEnergy < maxEnergyPoints)
		{
			currentEnergy = Mathf.Min(currentEnergy + floorEnergyPointsRecovery * Time.fixedDeltaTime,maxEnergyPoints);
		}
		if (characterS.downCollision)
		{
			float d = currentFacingVector.x * propulsionImpulse;
			currentVelocity.x = Mathf.Clamp(currentVelocity.x + d, -maxPropulsion, maxPropulsion);

			if (isMouseDown)
			{
				currentVelocity.y += jumpImpulse;
			}

			//if (characterS.rightCollision)
			//{
			//	currentFacingVector.x = -1;
			//}
			//else if (characterS.leftCollision)
			//{
			//	currentFacingVector.x = 1;
			//}
		}
		else
		{
			_SetState(Jump);
		}
		return currentVelocity;
	}

	private void _EndIdle()
	{
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
		dashVector.Normalize();
	}
	/**
	* Jump
	**/
	private void _StartJump()
	{
		hasAirBreak = false;
	}

	private Vector2 _GameplayJump(Vector2 currentVelocity)
	{
		if (currentEnergy < maxEnergyPoints)
		{
			currentEnergy = Mathf.Min(currentEnergy + airEnergyRecoveryPoints * Time.fixedDeltaTime, maxEnergyPoints);
		}
		if (isMouseDown)
		{
			if (!hasAirBreak)
			{
				hasAirBreak = true;
				currentVelocity *= airBreakFactor;
			}
			if (characterS.leftCollision)
			{
				currentVelocity = wallJumpVector * wallJumpForce;
				currentFacingVector.x =  1;
			}
			else if (characterS.rightCollision)
			{
				currentVelocity = mirrorWallJumpVector * wallJumpForce;
				currentFacingVector.x = -1;
			}
		}
		if (characterS.downCollision)
		{
			_SetState(Idle);
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
			if (characterS.downCollision)
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
}
