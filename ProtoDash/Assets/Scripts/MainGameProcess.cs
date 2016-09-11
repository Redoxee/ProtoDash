using UnityEngine;

namespace Dasher
{
	public class MainGameProcess : MonoBehaviour
	{
		private static MainGameProcess i_instance;
		public static MainGameProcess Instance { get {return i_instance;} }

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
			m_initFrameWait = true;
		}

		void OnDestroy()
		{
			if (i_instance != this)
			{
				i_instance = null;
			}
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
				m_GUIManager.NotifyLevelStart();
			}
			m_timeManager.NotifyStartLevel();
			m_timeManager.GameTimeFactor = 1;

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
	}
}
