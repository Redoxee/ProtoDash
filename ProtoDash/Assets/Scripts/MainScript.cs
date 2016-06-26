#define DEBUG_RAY

using UnityEngine;
using Assets;

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

	//[SerializeField]
	private Vector3 fakeGravity = new Vector3(0.0f, -60.0f,0.0f);
	
	[SerializeField]
	private float propulsionImpulse = 0.5f;
	[SerializeField]
	private float maxPropulsion = 15.0f;
	
	[SerializeField]
	private int idleJumpFarmeCount = 1;
	[SerializeField]
	private float jumpDeadZone = .05f;

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
	[SerializeField]
	private GameObject body;

	private float screenRatio;

	private bool isMouseDown = false;
	private bool isMouseUp = false;
	private bool isMousePressed = false;
	private bool isSweeping = false;
	private Vector3 currentFacingVector;

	private bool hasStartedFloorInput = false;
	private Vector3 mouseDownPosition;
	private int currentFrameCount = 0;

	private bool canLateJump = false;
	private float jumpTimer = .0f;

	private Vector2 wallJumpVector;
	private Vector2 mirroredWallJumpVector;

	private float squareSwipeInputTrigger;
	private float dashProgression = -1.0f;
	private Vector3 dashVector;
	private float dashAngle;
	private Quaternion dashRotation;
	private State currentState;

	private Vector3 tapPosition;
	private float dashTimer = 0.0f;

	//[SerializeField]
	private float upDashCost = 75.0f;
	//[SerializeField]
	private float diagonalUpDashCost = 65.0f;
	//[SerializeField]
	private float lateralDashCost = 40.0f;
	//[SerializeField]
	private float diagonalDownDashCost = 30.0f;
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


	private float squishDampingFactor = .15f;
	private float currentSquishX = 1.0f;
	private float currentSquishY = 1.0f;

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
		updateSquish(Time.deltaTime);
		updateBeak();
	}

	void FixedUpdate() {
		updateRayCasts();

		Vector3 newVelocity = characterRB.velocity;

		newVelocity += fakeGravity * Time.fixedDeltaTime;
		newVelocity = currentState.gameplay(newVelocity);
		characterRB.velocity = newVelocity;
		refillEnergy();
		updateDashInput();
		isMouseDown = false;
		isMouseUp = false;
	}

	const uint  NB_RAYCHECK = 3;
	const float RAY_RANGE = .3f;
	private RaycastHit2D _MultipleRayCasts(Vector2 direction, Vector2 normal)
	{
		RaycastHit2D result = new RaycastHit2D();
		result.distance = Mathf.Infinity;
		for (int i = 0; i < NB_RAYCHECK; ++i)
		{
			RaycastHit2D rt = Physics2D.Raycast(characterRB.position + normal * (i - NB_RAYCHECK / 2) * RAY_RANGE, direction, probDistance);
#if DEBUG_RAY
			Vector2 start = characterRB.position + normal * (i - NB_RAYCHECK / 2) * RAY_RANGE;
			Debug.DrawRay(start, direction * probDistance, Color.red);
#endif
			if (rt.collider && rt.distance < result.distance)
			{
				result = rt;
			}
		}
		return result;
	}


	private void updateRayCasts()
	{
		isTouchingDown = false;
		isTouchingLeft = false;
		isTouchingRight = false;
		isInEarlyJumpRange = false;
		isInMagnetLeft = false;
		isInMagnetRight = false;
		RaycastHit2D rayDown = _MultipleRayCasts(Vector2.down,Vector2.right);
		RaycastHit2D rayRight = _MultipleRayCasts(Vector2.right,Vector2.up);
		RaycastHit2D rayLeft = _MultipleRayCasts(Vector2.left, Vector2.up);
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
						dashAngle = Mathf.Acos(dv.x) * Mathf.Sign(dv.y) / Mathf.PI * 180;
						dashRotation = Quaternion.Euler(0, 0, dashAngle);
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

	private void updateSquish(float dt)
	{
		currentSquishX = FuctionUtils.damping(squishDampingFactor, currentSquishX, 1.0f, dt);
		currentSquishY = FuctionUtils.damping(squishDampingFactor, currentSquishY, 1.0f, dt);
		body.transform.localScale = new Vector3(currentSquishX, currentSquishY, 1.0f);
	}

	/**
	* Idle
	**/

	private void _StartIdle()
	{
		hasStartedFloorInput = false;
		canLateJump = false;
	}

	private Vector2 _GameplayIdle(Vector2 currentVelocity) {
		if (isTouchingDown)
		{

			if (isMouseDown)
			{
				hasStartedFloorInput = true;
				mouseDownPosition = Input.mousePosition;
				currentFrameCount = idleJumpFarmeCount;
			}
			else if (isMouseUp && hasStartedFloorInput)
			{
				currentVelocity.y = jumpForce;
				_SetState(Jump);
			}
			else if (isMousePressed && hasStartedFloorInput && currentFrameCount > 0)
			{
				if (Vector3.Distance(mouseDownPosition, Input.mousePosition) <= jumpDeadZone)
				{
					currentVelocity.y = jumpForce;
					_SetState(Jump);
				}
				else
				{
					mouseDownPosition = Input.mousePosition;
				}
				currentFrameCount -= 1;
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
				currentSquishX = .85f;
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

		body.transform.localRotation = Quaternion.identity;
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
