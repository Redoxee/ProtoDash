using UnityEngine;

namespace Dasher
{
	public class GameProcess : MonoBehaviour
	{
		private static GameProcess i_instance;
		public static GameProcess Instance { get {return i_instance;} }

		public static string CurrentLevelName {get { return MainProcess.Instance.CurrentLevel; }}


		private CameraScript m_CameraS;
		public void RegisterCamera(CameraScript cam)
		{
			m_CameraS = cam;
		}

		public void UnregisterCamera()
		{
			m_CameraS = null;
		}

		private TimeManager m_timeManager = new TimeManager();
		private float m_currentChampTime = 0f;
		public TimeManager GameTime { get { return m_timeManager;} }

		private bool m_initFrameWait = false;

		private PastTraceManager m_pastTraceManagerRef = null;

		#region Character
		private Character m_character;

		public void RegisterCharacter(Character character)
		{
			m_character = character;
		}

		public void UnregisterCharacter()
		{
			m_character = null;
		}

		public Character CurrentCharacter {get { return m_character; }}

		#endregion

		const float c_fullScreenScalerDepth = -1.5f;

		#region EndAnimations

		EndNode m_endNode = null;
		public EndNode EndNode { set { m_endNode = value; } }

		[SerializeField]
		GameObject m_fakeEndNode = null;
		FullScreenScaler m_endFakeScaller;
		#endregion

		#region MonoBehavior
		void Awake()
		{
			if (i_instance == null)
				i_instance = this;
			else
			{
				Debug.LogWarning("At least two game process at the same time");
				Destroy(gameObject);
			}

			InitStates();

			m_initFrameWait = true;

			m_endFakeScaller = m_fakeEndNode.GetComponent<FullScreenScaler>();
			m_fakeEndNode.SetActive(false);

			m_deathDummyScaler = m_deathDummy.GetComponent<FullScreenScaler>();
			m_deathDummy.SetActive(false);
		}

		void OnDestroy()
		{
			if (i_instance != this)
			{
				i_instance = null;
			}
		}

		void Update()
		{
			UpdateState();
			m_CameraS.ManualUpdate();

			if (m_GUIManager != null)
			{
				m_GUIManager.ManualUpdate();
			}
		}

		void FixedUpdate()
		{
			if (m_initFrameWait)
			{
				m_initFrameWait = false;
				LevelStart();
			}

			float prevTime = m_timeManager.CurrentLevelTime;
			m_timeManager.ManualFixedUpdate();
			float currentTime = m_timeManager.CurrentLevelTime;

			if (m_GUIManager != null)
			{
				m_GUIManager.ManualFixedUpdate();
			}
			FixedUpdateState();

			if (prevTime < m_currentChampTime && currentTime >= m_currentChampTime)
			{
				m_endNode.NotifyChampTimeMissed();
			}
		}

		void LateUpdate()
		{
			m_CameraS.ManualLateUpdate();
		}

		#endregion

		#region GUIManager

		private GUIManager m_GUIManager;
		public void registerGUIManager(GUIManager gui)
		{
			m_GUIManager = gui;
		}

		public void unregisterGUI()
		{
			m_GUIManager = null;
		}

		public GUIManager getCurrentGUI()
		{
			return m_GUIManager;
		}
		#endregion

		#region Level flow

		private void LevelStart()
		{
			if (m_GUIManager != null)
			{
				SetState(m_introState);
			}
			else
			{
				SetState(m_gamplayState);
			}

			if (CurrentLevelName != null)
			{
				LevelData currentLevelData = MainProcess.Instance.levelFlow.GetLevelData(CurrentLevelName);
				m_currentChampTime = currentLevelData.parTime;
			}
			else
			{
				m_currentChampTime = float.MaxValue;
			}
		}

		public void RequirePause()
		{
			m_character.Pause();
			m_timeManager.GameTimeFactor = 0f;
		}

		public void RequireResume()
		{
			m_character.Unpause();
			m_timeManager.GameTimeFactor = 1f;
		}
		
		private bool m_isNewbest = false;
		private bool m_isNewPar = false;
		private float m_oldBestTime = 0f;
		private bool m_isFirstCompletion = false;

		public void NotifyEndLevelReached(GameObject endNode)
		{
			m_timeManager.GameTimeFactor = 0f;
			MainProcess mp = MainProcess.Instance;
			SaveManager saveManager = mp.DataManager;

			float currentTime = m_timeManager.CurrentLevelTime;
			if (CurrentLevelName != null)
			{
				LevelData currentLevelData = mp.levelFlow.GetLevelData(CurrentLevelName);

				if(!saveManager.HasLevelBeenFinished(CurrentLevelName))
				{
					m_isFirstCompletion = true;
					mp.AnalyticsManager.NotifyNewLevelBeaten(CurrentLevelName, saveManager.GetLevelTryCount(CurrentLevelName), currentTime);
				}
				m_oldBestTime = saveManager.GetLevelTime(CurrentLevelName);
				if (currentTime < m_oldBestTime)
				{
					m_isNewbest = true;
					if (currentTime <= currentLevelData.parTime && m_oldBestTime > currentLevelData.parTime)
						m_isNewPar = true;
					currentLevelData.currentBest = currentTime;
					saveManager.SetLevelTime(CurrentLevelName, m_timeManager.CurrentLevelTime);

					saveManager.SetTraceForLevel(CurrentLevelName, m_pastTraceManagerRef.GetCurrentRecording());
					saveManager.SaveLevelTrace(CurrentLevelName);
				}
				saveManager.IncrementLestLevelPlayed();
				saveManager.NotifyEndRun(m_character.Traces.NbJumps, m_character.Traces.NbDashes);
				saveManager.Save();
			}
			
			SetState(m_outroCharacterAnimationState);
		}

		#endregion



		#region State machine
		delegate void D_StateMachineCallback();

		struct FSM_State
		{
			public D_StateMachineCallback d_begin;
			public D_StateMachineCallback d_update;
			public D_StateMachineCallback d_fixedUpdate;
			public D_StateMachineCallback d_end;

			public FSM_State(D_StateMachineCallback begin = null, D_StateMachineCallback update = null, D_StateMachineCallback fixedUpdate = null, D_StateMachineCallback end = null)
			{
				d_begin = begin;
				d_update = update;
				d_fixedUpdate = fixedUpdate;
				d_end = end;
			}
		}
		FSM_State m_currentState = new FSM_State();

		void SetState(FSM_State newState)
		{
			if (m_currentState.d_end != null)
				m_currentState.d_end();
			m_currentState = newState;
			if (m_currentState.d_begin != null)
				m_currentState.d_begin();
		}

		void UpdateState()
		{
			if (m_currentState.d_update != null)
				m_currentState.d_update();
		}

		void FixedUpdateState()
		{
			if (m_currentState.d_fixedUpdate != null)
				m_currentState.d_fixedUpdate();
		}

		#region intro state
		void Intro_begin()
		{
			if (m_GUIManager != null)
			{
				m_GUIManager.SetStateIntro();
			}
			GameTime.GameTimeFactor = 0;

			m_pastTraceManagerRef = transform.parent.GetComponentInChildren<PastTraceManager>();
			if (m_pastTraceManagerRef)
			{
				m_pastTraceManagerRef.Initialize();
			}

		}

		void Intro_update()
		{
			if (Input.GetMouseButtonDown(0))
			{
				SetState(m_gamplayState);
			}
		}

		FSM_State m_introState;

		#endregion

		#region gameplay state

		void Gameplay_begin()
		{
			if (m_GUIManager != null)
			{
				m_GUIManager.NotifyLevelStart();
			}
			m_timeManager.NotifyStartLevel();
			m_timeManager.GameTimeFactor = 1;
			MainProcess mp = MainProcess.Instance;

			if (mp.CurrentLevel != null)
			{
				mp.DataManager.NotifyLevelStarted(mp.CurrentLevel);
				mp.DataManager.Save();
			}

			m_deathFrameCounter = 0;
			m_deathZoneCounter = 0;

			m_character.NotifyGameStart();
		}

		void Gameplay_update()
		{
			m_character.ManualUpdate();
		}

		void Gameplay_fixedUpdate()
		{
			if (m_deathZoneCounter > 0)
			{
				if (m_deathFrameCounter > m_allowedDeathFrame && !m_character.IsInInvincibilityFrames())
				{
					OnDeath();
					return;
				}
				m_deathFrameCounter += 1;
			}
			else
			{
				m_deathFrameCounter = 0;
			}
			m_character.ManualFixedUpdate();
		}

		FSM_State m_gamplayState;

		private int m_deathZoneCounter = 0;
		private int m_deathFrameCounter = 0;
		private int m_allowedDeathFrame = 3;

		private GameObject m_lastDeathZoneTouched = null;
		private Vector3 m_firstDeathPosition = Vector3.zero;

		public void NotifyDeathZoneTouched(GameObject deadZone)
		{
			m_deathZoneCounter += 1;
			m_lastDeathZoneTouched = deadZone;

			if (m_deathZoneCounter == 1)
			{
				m_firstDeathPosition = m_character.transform.position;
			}
		}

		public void NotifyDeathZoneEmerged()
		{
			m_deathZoneCounter -= 1;
		}

		private Vector3 m_deathPositionTarget ;

		void computeDeathPosition()
		{

			var death = (m_lastDeathZoneTouched.transform.position);
			var deathScale = m_lastDeathZoneTouched.transform.localScale;
			var chara = m_firstDeathPosition;

			m_deathPositionTarget = FunctionUtils.Floor(chara);

			m_deathPositionTarget.x = Mathf.Max(m_deathPositionTarget.x, death.x);
			m_deathPositionTarget.x = Mathf.Min(m_deathPositionTarget.x, death.x + deathScale.x - 1);

			m_deathPositionTarget.y = Mathf.Max(m_deathPositionTarget.y, death.y);
			m_deathPositionTarget.y = Mathf.Min(m_deathPositionTarget.y, death.y + deathScale.y - 1);

			m_deathPositionTarget += new Vector3(.5f, .5f, 0f);
			m_firstDeathPosition = (m_deathPositionTarget + m_firstDeathPosition) / 2f;
		}

		private void OnDeath()
		{
			computeDeathPosition();
			SetState(m_dyingState);
			m_timeManager.GameTimeFactor = 0f;

			var dm = MainProcess.Instance.DataManager;
			if(CurrentLevelName != null && !dm.HasLevelBeenFinished(CurrentLevelName))
				dm.SetTraceForLevel(CurrentLevelName, m_pastTraceManagerRef.GetCurrentRecording());
		}
		#endregion

		#region Outro CharacterAnimation

		private float m_outroCharacterDuration = 1f;
		private float m_outroCharacterTimer = 0f;

		FSM_State m_outroCharacterAnimationState;

		private void OutroCharacter_begin()
		{
			var ep = m_endNode.transform.position;
			Vector2 endPosition = new Vector2(ep.x, ep.y);
			ep.z = c_fullScreenScalerDepth;
			m_fakeEndNode.transform.position = ep;

			m_character.NotifyEndLevel(endPosition);
			m_CameraS.NotifyEndGame(endPosition);
			m_outroCharacterTimer = 0f;


			if (m_GUIManager)
			{
				m_GUIManager.NotifyIntermediateState();
			}

		}
		private void OutroCharacter_update()
		{
			m_outroCharacterTimer += Time.deltaTime;
			if (m_outroCharacterTimer > m_outroCharacterDuration)
			{
				SetState(m_outroNodeAnimationState);
			}
			m_character.ManualUpdate();
		}
		private void OutroCharacter_FixedUpdate()
		{
			m_character.ManualFixedUpdate();
		}

		#endregion

		#region Outro end node animation
		private float m_outroNodeDuration = 1f;
		private float m_outroNodeTimer = 0f;

		FSM_State m_outroNodeAnimationState;

		private void OutroNode_begin()
		{
			m_outroNodeTimer = 0f;
			m_endNode.gameObject.SetActive(false);
			m_fakeEndNode.SetActive(true);
			m_character.gameObject.SetActive(false);

			var cs = MainProcess.Instance.m_colorScheme;
			m_endFakeScaller.StartColor = m_endNode.CurrentColor;
			m_endFakeScaller.EndColor = cs.EndLevelPanel;

		}

		private void OutroNode_Update()
		{
			m_outroNodeTimer += Time.deltaTime;
			m_endFakeScaller.SetProgression(m_outroNodeTimer / m_outroNodeDuration);

			if (m_outroNodeTimer > m_outroNodeDuration)
			{
				SetState(m_endState);
			}
		}

		private void OutroNode_fixedUpdate()
		{
			m_endFakeScaller.SetProgression(1f);
		}
		

		#endregion

		#region Dying
		FSM_State m_dyingState;

		float m_dyingDuration = 1.0f;
		float m_dyingTimer = 0f;

		[SerializeField]
		GameObject m_deathDummy = null;
		FullScreenScaler m_deathDummyScaler;


		private void Dying_Begin()
		{
			SaveManager dataManager = MainProcess.Instance.DataManager;
			dataManager.NotifyDeath(m_character.Traces.NbJumps, m_character.Traces.NbDashes);
			dataManager.Save();
			m_dyingTimer = 0f;
			m_CameraS.NotifyDeath(m_deathPositionTarget);
			m_character.NotifyDying(m_deathPositionTarget, m_firstDeathPosition);
			m_deathPositionTarget.z = c_fullScreenScalerDepth;
			m_deathDummy.transform.position = m_deathPositionTarget;
			m_deathDummy.SetActive(true);
			m_deathDummyScaler.SetProgression(0f);
			if(m_GUIManager != null)
				m_GUIManager.NotifyIntermediateState();
		}

		private void Dying_Update()
		{
			m_dyingTimer += Time.deltaTime;
			m_character.ManualUpdate();

			m_deathDummyScaler.SetProgression(m_dyingTimer / m_dyingDuration);

			if (m_dyingTimer >= m_dyingDuration)
			{
				SetState(m_deadState);
			}
		}

		private void Dying_FixedUpdate()
		{
			m_character.ManualFixedUpdate();
		}

		#endregion

		#region Dead
		#endregion
		FSM_State m_deadState;
		private void Dead_Begin()
		{
			if (m_GUIManager)
			{
				m_GUIManager.NotifyDeath();
			}
			else
			{
				FunctionUtils.Quit();
			}
		}
		#region End

		FSM_State m_endState;
		private void End_begin()
		{
			if (m_GUIManager)
			{
				m_GUIManager.NotifyEndLevelReached(m_isFirstCompletion, m_isNewbest, m_isNewPar, m_oldBestTime);
			}
			else
			{
				FunctionUtils.Quit();
			}
		}

		#endregion

		private void InitStates()
		{
			m_introState = new FSM_State(Intro_begin, Intro_update, null, null);
			m_gamplayState = new FSM_State(Gameplay_begin, Gameplay_update, Gameplay_fixedUpdate, null);
			m_outroCharacterAnimationState = new FSM_State(OutroCharacter_begin, OutroCharacter_update, OutroCharacter_FixedUpdate);
			m_outroNodeAnimationState = new FSM_State(OutroNode_begin, OutroNode_Update, OutroNode_fixedUpdate,null);
			m_endState = new FSM_State(End_begin);
			m_dyingState = new FSM_State(Dying_Begin,Dying_Update,Dying_FixedUpdate);
			m_deadState = new FSM_State(Dead_Begin);
		}

		#endregion
	}
}
