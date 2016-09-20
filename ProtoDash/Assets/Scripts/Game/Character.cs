﻿#define DEBUG_RAY

using UnityEngine;
namespace Dasher
{
	public partial class Character : MonoBehaviour
	{
		private delegate Vector2 gameplayDelegate(Vector2 currentVelocity);
		private delegate void startStateDelegate();
		private delegate void endStateDelegate();
		struct State
		{
			public State(string name, startStateDelegate d1, gameplayDelegate d2, endStateDelegate d3)
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

		private void _InitializeStates()
		{
			Idle = new State("Idle", _StartIdle, _GameplayIdle, _EndIdle);
			Jump = new State("Jump", _StartJump, _GameplayJump, _EndJump);
			Dash = new State("Dash", _StartDash, _GameplayDash, _EndDash);
		}

		private void _SetState(State newState)
		{
			currentState.end();
			currentState = newState;
			newState.start();
		}

		[SerializeField]
		private Camera mainCamera;

		public bool isPaused = false;

		//[SerializeField]
		private Vector3 fakeGravity = new Vector3(0.0f, -60.0f, 0.0f);

		[SerializeField]
		private float propulsionImpulse = 0.5f;
		[SerializeField]
		private float maxPropulsion = 15.0f;

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

		private GameObject beak;

		private GameObject body;

		private Renderer bodyRenderer;

		[SerializeField]
		private TraceManager traceManager;

		private float screenRatio;

		private bool isMouseDown = false;
		//private bool isMouseUp = false;
		private bool isMousePressed = false;
		private bool isSweeping = false;
		private Vector3 currentFacingVector;

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


		private float squishDampingFactor = .075f;
		private float currentSquishX = 1.0f;
		private float currentSquishY = 1.0f;

		private Vector3 savedVelocity = Vector3.zero;

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

		public void NotifyGameStart()
		{
			tapPosition = Input.mousePosition;
		}

		void Start()
		{
			squareSwipeInputTrigger = swipeInputDistance * swipeInputDistance;

			characterRB = GetComponent<Rigidbody2D>();


			body = transform.Find("Body").gameObject;
			beak = body.transform.Find("Beak").gameObject;
			bodyRenderer = body.GetComponent<Renderer>();

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

			GameProcess mp = GameProcess.Instance;
			if (mp != null)
			{
				mp.RegisterCharacter(this);
			}
		}

		public void Disable()
		{
			GameProcess mp = GameProcess.Instance;
			if (mp != null)
			{
				mp.UnregisterCharacter();
			}
		}

		public void ManualUpdate()
		{
			if (isPaused)
			{
				return;
			}

			if (Input.GetMouseButtonDown(0))
			{
				isMouseDown = true;
			}
			isMousePressed = Input.GetMouseButton(0);
			Vector3 sv = tapPosition - Input.mousePosition;
			sv.x *= screenRatio;
			sv.y *= screenRatio;
			isSweeping = squareSwipeInputTrigger < sv.sqrMagnitude;
			updateSquish(Time.deltaTime);
			updateBeak();
		}

		public void ManualFixedUpdate()
		{

			if (isPaused)
			{
				return;
			}

			updateRayCasts();

			Vector3 newVelocity = characterRB.velocity;

			newVelocity += fakeGravity * Time.fixedDeltaTime;
			newVelocity = currentState.gameplay(newVelocity);
			characterRB.velocity = newVelocity;
			refillEnergy();
			updateDashInput();
			isMouseDown = false;
			//isMouseUp = false;

			bodyRenderer.material.SetFloat("_Progression", currentEnergy / maxEnergyPoints);
		}

		const uint NB_RAYCHECK = 3;
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
			RaycastHit2D rayDown = _MultipleRayCasts(Vector2.down, Vector2.right);
			RaycastHit2D rayRight = _MultipleRayCasts(Vector2.right, Vector2.up);
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
			currentSquishX = FunctionUtils.damping(squishDampingFactor, currentSquishX, 1.0f, dt);
			currentSquishY = FunctionUtils.damping(squishDampingFactor, currentSquishY, 1.0f, dt);
			body.transform.localScale = new Vector3(currentSquishX, currentSquishY, 1.0f);
		}

		public float getFacingSign()
		{
			return currentFacingVector.x;
		}

		public void Pause()
		{
			if (!isPaused)
			{
				savedVelocity = characterRB.velocity;
				characterRB.isKinematic = true;
				isPaused = true;
			}
			
		}

		public void Unpause()
		{
			if (isPaused)
			{
				characterRB.isKinematic = false;
				characterRB.velocity = savedVelocity;
				isPaused = false;
			}
		}
	}
}