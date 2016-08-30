using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

namespace Dasher
{
	public class MainProcess : Singleton<MainProcess>
	{

		protected MainProcess() { } // guarantee this will be always a singleton only - can't use the constructor!
		enum GameState { MainMenu, InGame }
		GameState m_gameState = GameState.MainMenu;

		[SerializeField]
		public LevelFlow levelFlow;

		private int m_currentLevelIndex = -1;
		public int CurrentLevelIndex {get { return m_currentLevelIndex; }}

		string currentLevelName;

		private Character m_character;

		private TimeManager m_timeManager = new TimeManager();
		public TimeManager GameTime { get { return m_timeManager;} }

		#region Character

		public void RegisterCharacter(Character character)
		{
			m_character = character;
		}

		public void UnregisterCharacter()
		{
			m_character = null;
		}

		#endregion

		#region MonoBehavior
		public void FixedUpdate()
		{
			if (m_gameState == GameState.InGame)
			{
				m_timeManager.ManualFixedUpdate();
				if (m_GUIManager != null)
					m_GUIManager.ManualFixedUpdate();
			}
		}
		#endregion

		#region Process navigation

		public void SwitchToHome()
		{
			SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
			m_gameState = GameState.MainMenu;
		}

		public void LaunchLevel(int index)
		{
			if (m_gameState != GameState.InGame)
			{
				SceneManager.LoadScene("GameSetupScene", LoadSceneMode.Single);
				m_gameState = GameState.InGame;
			}
			else
			{
				SceneManager.UnloadScene(currentLevelName);
			}
			currentLevelName = levelFlow.levelList[index].sceneName;

			m_currentLevelIndex = index;
			SceneManager.LoadScene(currentLevelName, LoadSceneMode.Additive);

			LevelStart();
		}

		public void RelaunchLevel()
		{
			LaunchLevel(m_currentLevelIndex);
		}

		public void LaunchNextLevel()
		{
			int levelIndex = m_currentLevelIndex + 1;
			if (levelIndex < 0 || levelIndex == levelFlow.levelList.Count)
			{
				SwitchToHome();
			}
			else
			{
				LaunchLevel(levelIndex);
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
