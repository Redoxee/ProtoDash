#define DEBUG_RAY

using UnityEngine;
namespace Dasher
{
	public class Character : MonoBehaviour
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
		State EndGame;
		State Dead;

		private void _InitializeStates()
		{
			Idle	= new State("Idle", _StartIdle, _GameplayIdle, _EndIdle);
			Jump	= new State("Jump", _StartJump, _GameplayJump, _EndJump);
			Dash	= new State("Dash", _StartDash, _GameplayDash, _EndDash);
			EndGame = new State("EndGame", _BeginEndGame, _UpdateEndGame, _EndEndGame);
			Dead	= new State("Dead"	, _BeginDead, _UpdateDead, _EndDead);
		}

		private void _SetState(State newState)
		{
			currentState.end();
			currentState = newState;
			newState.start();
		}
		[SerializeField]
		private InputManager m_inputManager = null;

		public bool isPaused = false;

		//[SerializeField]
		private Vector3 fakeGravity = new Vector3(0.0f, -60.0f, 0.0f);
		private float m_gravityFactor = 1f;

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
		private AnimationCurve dashCurve = null;
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
		public TraceManager Traces {  get { return traceManager; } }

		private Vector3 currentFacingVector;

		private bool canLateJump = false;
		private float jumpTimer = .0f;

		private Vector2 wallJumpVector;
		private Vector2 mirroredWallJumpVector;

		private float dashProgression = -1.0f;
		private Vector3 m_dashVector;
		private float dashAngle;
		private Quaternion dashRotation;
		private State currentState;

		private float dashTimer = 0.0f;

		//[SerializeField]
		private float upDashCost = 50f;
		//[SerializeField]
		private float diagonalUpDashCost = 50f;
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
			m_inputManager.Initialize();
		}

		void Start()
		{
			characterRB = GetComponent<Rigidbody2D>();


			body = transform.Find("Body").gameObject;
			beak = body.transform.Find("Beak").gameObject;
			bodyRenderer = body.GetComponent<Renderer>();

			originalBeakX = beak.transform.localPosition.x;

			currentFacingVector = new Vector3(1, 0, 0);
			_InitializeDashCosts();
			_InitializeStates();
			_InitializeWallJumpVectors();
			currentState = Idle;

			GameProcess gp = GameProcess.Instance;
			if (gp != null)
			{
				gp.RegisterCharacter(this);
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


			m_inputManager.ManualUpdate();
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
			newVelocity += fakeGravity * Time.fixedDeltaTime * m_gravityFactor;
			newVelocity = currentState.gameplay(newVelocity);
			characterRB.velocity = newVelocity;
			refillEnergy();
			updateDashInput();
			m_inputManager.ManualFixedUpdate();

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
				if (rt.collider && rt.distance < result.distance && rt.transform.tag == "Floor")
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
		#region States

		#region Idle
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

				if (m_inputManager.IsRequestingJump)
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

		#endregion

		#region Jump
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
				if (m_inputManager.IsRequestingJump)
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
					if (m_inputManager.IsRequestingJump)
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

			if (m_inputManager.IsRequestingJump)
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
		#endregion

		#region Dash

		private void updateDashInput()
		{
			if (m_cancelDash)
				return;

			if (dashTimer > 0)
			{
				dashTimer -= Time.fixedDeltaTime;
			}
			if (m_inputManager.IsRequestingDash)
			{
				if (dashTimer <= 0)
				{
					Vector3 dv = m_inputManager.GetDashVector();
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
						if (dCost <= currentEnergy)
						{
							m_dashVector = dv;
							dashAngle = Mathf.Acos(dv.x) * Mathf.Sign(dv.y) / Mathf.PI * 180;
							dashRotation = Quaternion.Euler(0, 0, dashAngle);
							currentEnergy = Mathf.Max(0, currentEnergy - dCost);
							_SetState(Dash);
							traceManager.NotifyDash(characterRB.transform.position, dashRotation);
						}
					}
				}
			}
		}

		/**
		* Dash
		**/

		private void _StartDash()
		{
			dashTimer = dashCoolDown;
			dashProgression = dashDuration;
			m_inputManager.NotifyBeginDash();

			if (m_dashVector.x > 0)
			{
				currentFacingVector.x = 1;
			}
			else if (m_dashVector.x < 0)
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
				currentVelocity = m_dashVector * v;
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
				currentVelocity = m_dashVector * dashCurve.Evaluate(1.0f) * dashMaxSpeed;
			}
			return currentVelocity;
		}

		private void _EndDash()
		{
			m_inputManager.NotifyEndDash();

			body.transform.localRotation = Quaternion.identity;
		}
		#endregion

		#region EndGame

		private bool m_cancelDash = false;

		private Vector2 m_endTargetPosition;
		private float m_endAttractionForce = 10f;

		private void _BeginEndGame()
		{
			m_gravityFactor = 0f;
			m_cancelDash = true;
		}

		private Vector2 _UpdateEndGame(Vector2 currentVelocity)
		{
			float dt = Time.deltaTime;
			Vector2 dir = m_endTargetPosition - new Vector2(transform.position.x, transform.position.y);
			float m = dir.magnitude;
			return (currentVelocity + Mathf.Pow(m * m_endAttractionForce, 2f) * dir.normalized * dt )* .9f;
		}

		private void _EndEndGame()
		{
			m_cancelDash = false;
		}


		public void NotifyEndLevel(Vector2 endPosition)
		{
			m_endTargetPosition = endPosition;
			_SetState(EndGame);
		}

		#endregion

		#region Dead

		void _BeginDead()
		{
			m_gravityFactor = 0f;
			m_cancelDash = true;
		}

		float m_deathSlowDownFactor = .8f;

		Vector2 _UpdateDead(Vector2 currentVelocity)
		{
			currentEnergy = 0f;
			return currentVelocity * m_deathSlowDownFactor;
		}

		void _EndDead()
		{
		}

		public void NotifyDying()
		{
			_SetState(Dead);
		}
		
		#endregion

		#endregion

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