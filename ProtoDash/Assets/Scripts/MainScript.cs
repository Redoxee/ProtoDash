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
	private float turnBackTapTime = .5f;
	[SerializeField]
	private float propulsionImpulse = 0.5f;
	[SerializeField]
	private float maxPropulsion = 15.0f;

	[SerializeField]
	private float airBreakFactor = .85f;
	[SerializeField]
	private float wallSlideSpeed = 5.0f;

	[SerializeField]
	private float airPropulsion = 1.0f;

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
	private float wallEnergyRecoveryPoints = 100.0f;
	[SerializeField]
	private float airEnergyRecoveryPoints = 50.0f;
	
	[HideInInspector]
	public float currentEnergy = 100.0f;

	private Rigidbody characterRB;
	private CharacterScript characterS;

	private float screenRatio;

	private bool isMouseDown = false;
	private bool isMouseUp = false;
	private bool isMousePressed = false;
	private bool isSweeping = false;
	private Vector3 currentFacingVector;

	private float floorButtonDownTimer;
	

	private bool hasAirBreak;
	private float squareSwipeInputTrigger;
	private float dashProgression = -1.0f;
	private Vector3 dashVector;
	private State currentState;

	private Vector3 tapPosition;
	private float dashTimer = 0.0f;

	private float upDashCost = 75.0f;
	private float diagonalUpDashCost = 40.0f;
	private float lateralDashCost = 40.0f;
	private float diagonalDownDashCost = 25.0f;
	private float downDashCost = 0.0f;

	private float[] energyCostTable = new float[11];

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

	void Start() {
		squareSwipeInputTrigger = swipeInputDistance * swipeInputDistance;

		characterRB = mainCharacter.GetComponent<Rigidbody>();
		characterS = mainCharacter.GetComponent<CharacterScript>();

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
	}

	void FixedUpdate() {

		Vector3 newVelocity = characterRB.velocity;

		newVelocity = currentState.gameplay(newVelocity);

		characterRB.velocity = newVelocity;
		refillEnergy();
		updateDashInput();
		isMouseDown = false;
		isMouseUp = false;
		characterS.notifyColisionConsumed();
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
				bool dashingIntoWall = (dv.x > 0 && characterS.rightCollision) || (dv.x < 0 && characterS.leftCollision) && ! characterS.downCollision;
				if (dv.sqrMagnitude > 0 && !dashingIntoWall)
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
		if (characterS.downCollision)
			refillRate = floorEnergyPointsRecovery;
		else if (characterS.rightCollision || characterS.leftCollision)
			refillRate = wallEnergyRecoveryPoints;
		currentEnergy = Mathf.Min(maxEnergyPoints, currentEnergy + refillRate * Time.fixedDeltaTime);
	}

	/**
	* Idle
	**/

	private void _StartIdle()
	{
		floorButtonDownTimer = -1;
	}
	private Vector2 _GameplayIdle(Vector2 currentVelocity) {
		if (characterS.downCollision)
		{

			if (isMouseDown)
			{
				floorButtonDownTimer = turnBackTapTime;
			}
			else if (isMouseUp)
			{
				if (floorButtonDownTimer >= 0)
				{
					currentFacingVector.x *= -1;
				}
			}else if (floorButtonDownTimer > 0)
			{
				floorButtonDownTimer -= Time.fixedDeltaTime;
			}

			float d = currentFacingVector.x * propulsionImpulse;
			currentVelocity.x = Mathf.Clamp(currentVelocity.x + d, -maxPropulsion, maxPropulsion);
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
	}

	private Vector2 _GameplayJump(Vector2 currentVelocity)
	{

		if (isMouseDown)
		{

			if (!hasAirBreak)
			{
				hasAirBreak = true;
				currentVelocity *= airBreakFactor;
			}
		}

		if (characterS.leftCollision || characterS.rightCollision)
		{
			currentVelocity.y = Mathf.Max(currentVelocity.y, -wallSlideSpeed);
		}

		if (characterS.downCollision && currentVelocity.y <= 0)
		{
			_SetState(Idle);
		}
		else
		{
			bool hasMagneted = false;
			Vector2 magnetVector = Vector2.zero;
			if (magnetRadius > .0f)
			{
				if (Physics.Raycast(characterRB.position, Vector3.right, magnetRadius))
				{
					magnetVector.x += magnetForce;
					hasMagneted = true;
				}
				else if (Physics.Raycast(characterRB.position, Vector3.left, magnetRadius))
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


	public string getState()
	{
		return currentState.name;
	}
}
