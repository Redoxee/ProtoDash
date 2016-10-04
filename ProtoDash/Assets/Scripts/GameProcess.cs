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
		public TimeManager GameTime { get { return m_timeManager;} }

		private bool m_initFrameWait = false;

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
		}

		void FixedUpdate()
		{
			if (m_initFrameWait)
			{
				m_initFrameWait = false;
				LevelStart();
			}
			m_timeManager.ManualFixedUpdate();
			if (m_GUIManager != null)
			{
				m_GUIManager.ManualFixedUpdate();
			}
			FixedUpdateState();
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

		public void NotifyEndLevelReached(Vector2 endPosition)
		{
			m_timeManager.GameTimeFactor = 0f;
			SaveManager saveManager = MainProcess.Instance.DataManager;

			float currentTime = m_timeManager.CurrentLevelTime;
			if (CurrentLevelName != null)
			{
				LevelData currentLevelData = MainProcess.Instance.levelFlow.GetLevelData(CurrentLevelName);

				if (currentTime < saveManager.GetLevelTime(CurrentLevelName))
				{
					m_isNewbest = true;
					if (currentTime < currentLevelData.parTime)
						m_isNewPar = true;
					currentLevelData.currentBest = currentTime;
					saveManager.SetLevelTime(CurrentLevelName, m_timeManager.CurrentLevelTime);
				}
				saveManager.NotifyEndRun(m_character.Traces.NbJumps, m_character.Traces.NbDashes);
				saveManager.Save();
			}

			m_character.NotifyEndLevel(endPosition);
			m_CameraS.NotifyEndGame(endPosition);
			SetState(m_outroState);
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
			MainProcess.Instance.DataManager.NotifyLevelStarted();


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
				if (m_deathFrameCounter > m_allowedDeathFrame)
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

		public void NotifyDeathZoneTouched()
		{
			m_deathZoneCounter += 1;
		}

		public void NotifyDeathZoneEmerged()
		{
			m_deathZoneCounter -= 1;
		}

		private void OnDeath()
		{
			SetState(m_deathState);
			m_timeManager.GameTimeFactor = 0f;
			if (m_GUIManager)
			{
				m_GUIManager.NotifyDeathZoneTouched();
			}
			else
			{
				FunctionUtils.Quit();
			}
		}
		#endregion

		#region Outro

		private float m_outroDuration = 1.5f;
		private float m_outroTimer = 0f;

		FSM_State m_outroState;

		private void Outro_begin()
		{
			m_outroTimer = 0f;
		}
		private void Outro_update()
		{
			m_outroTimer += Time.deltaTime;
			if (m_outroTimer > m_outroDuration)
			{
				SetState(m_endState);
			}
			m_character.ManualUpdate();
		}
		private void Outro_FixedUpdate()
		{
			m_character.ManualFixedUpdate();
		}

		#endregion

		#region Death
		FSM_State m_deathState;
		private void Death_Begin()
		{
			SaveManager dataManager = MainProcess.Instance.DataManager;
			dataManager.NotifyEndRun(m_character.Traces.NbJumps, m_character.Traces.NbDashes);
			dataManager.Save();
		}

		#endregion

		#region End

		FSM_State m_endState;
		private void End_begin()
		{
			if (m_GUIManager)
			{
				m_GUIManager.NotifyEndLevelReached(m_isNewbest,m_isNewPar);
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
			m_outroState = new FSM_State(Outro_begin, Outro_update, Outro_FixedUpdate);
			m_endState = new FSM_State(End_begin);
			m_deathState = new FSM_State(Death_Begin);
		}

		#endregion
	}
}
