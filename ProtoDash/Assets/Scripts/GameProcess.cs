using UnityEngine;

namespace Dasher
{
	public class GameProcess : MonoBehaviour
	{
		private static GameProcess i_instance;
		public static GameProcess Instance { get {return i_instance;} }

		public static string CurrentLevelName {get { return MainProcess.Instance.CurrentLevel; }}

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

		public void NotifyEndLevelReached()
		{
			m_timeManager.GameTimeFactor = 0f;

			SaveManager saveManager = MainProcess.Instance.DataManager;

			float currentTime = m_timeManager.CurrentLevelTime;
			if (currentTime < saveManager.GetLevelTime(CurrentLevelName))
			{
				saveManager.SetLevelTime(CurrentLevelName, m_timeManager.CurrentLevelTime);
				saveManager.Save();
			}

			if (m_GUIManager)
			{
				m_GUIManager.NotifyEndLevelReached();
			}
			else
			{
				FunctionUtils.Quit();
			}
		}

		public void NotifyDeathZoneTouched()
		{
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

			m_character.NotifyGameStart();
		}

		void Gameplay_update()
		{
			m_character.ManualUpdate();
		}

		void Gameplay_fixedUpdate()
		{
			m_character.ManualFixedUpdate();
		}

		FSM_State m_gamplayState;

		#endregion 

		private void InitStates()
		{
			m_gamplayState = new FSM_State(Gameplay_begin, Gameplay_update, Gameplay_fixedUpdate, null);
			m_introState = new FSM_State(Intro_begin, Intro_update, null, null);
		}

		#endregion
	}
}
