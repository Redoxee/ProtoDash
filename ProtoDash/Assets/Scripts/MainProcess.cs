﻿using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

namespace Dasher
{
	public enum GameStates {
		Error = 0,
		Intro = 1,
		MainMenu = 2,
		InGame = 3
	}

	public class MainProcess : MonoBehaviour
	{
		const string c_mainScene = "MainScene";

		private static MainProcess m_instance;
		public static MainProcess Instance { get { return m_instance; } }

		GameStates m_gameState = GameStates.MainMenu;

		[SerializeField]
		public LevelFlow levelFlow;
	
		[SerializeField]
		public ColorScheme m_colorScheme;

		[SerializeField]
		Camera m_transitionCamera;

		[SerializeField]
		GameObject InputInterceptionLayer;
		[SerializeField]
		Animator m_transitionAnimator;

		#region Monobehaviour

		void Awake()
		{
			m_gameState = GameStates.Intro;
			if (m_instance == null)
			{
				m_instance = this;
			}
			else
			{
				Destroy(gameObject);
				return;
			}

			m_saveManager = new SaveManager();

			m_colorScheme.SetColors();

			if (SceneManager.GetActiveScene().name != c_mainScene)
			{
				Scene mainScene = SceneManager.GetSceneByName(c_mainScene);
				SceneManager.SetActiveScene(mainScene);
			}
			
			if (SceneManager.sceneCount == 1)
			{
				SwitchToHome();
			}
		}

		void Update()
		{
			if (m_delayedAction != null)
			{
				m_delayedAction();
				m_delayedAction = null;
			}
		}

		#endregion

		#region Process navigation

		const string c_mainMenuScene = "MainMenu";
		const string c_gameSetupScene = "GameSetupScene";

		string m_currenLevelScene = null;
		int m_currentLevelIndex = -1;

		public void SwitchToHome()
		{
			if (m_gameState == GameStates.InGame)
			{
				SceneManager.UnloadScene(c_gameSetupScene);
				SceneManager.UnloadScene(m_currenLevelScene);
			}
			SceneManager.LoadScene(c_mainMenuScene, LoadSceneMode.Additive);
			SetState(GameStates.MainMenu);
		}

		public void LaunchLevel(int index)
		{
			if (m_gameState == GameStates.MainMenu)
			{
				SceneManager.UnloadScene(c_mainMenuScene);
			}
			if (m_gameState == GameStates.InGame)
			{
				SceneManager.UnloadScene(c_gameSetupScene);
				if(m_currenLevelScene != null)
				{
					SceneManager.UnloadScene(m_currenLevelScene);
				}
			}
			
			SceneManager.LoadScene(c_gameSetupScene, LoadSceneMode.Additive);
			SetState(GameStates.InGame);
			
			m_currenLevelScene = levelFlow.levelList[index].sceneName;

			m_currentLevelIndex = index;
			SceneManager.LoadScene(m_currenLevelScene, LoadSceneMode.Additive);
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
				SwitchToHome(); // TODO Recap screen
			}
			else
			{
				LaunchLevel(levelIndex);
			}
		}

		public string CurrentLevel { get { return m_currenLevelScene; } }
		public int CurrentLevelIndex { get { return m_currentLevelIndex; } }
		#endregion

		#region Save

		private SaveManager m_saveManager = null;
		public SaveManager DataManager { get { return m_saveManager; } }

		#endregion

		#region States
		public void SetState(GameStates newState)
		{
			if (newState != GameStates.Intro)
			{
				m_gameState = newState;
				m_transitionCamera.gameObject.SetActive(false);
			}
		}
		#endregion

		#region Transitions
		private const string c_transitionBool = "InTransition";

		public delegate void TransitionCallback();

		TransitionCallback m_transitionCallback;
		TransitionCallback m_endTransitionCallback;

		TransitionCallback m_delayedAction = null;

		public void OnTransitionIn()
		{
			Debug.Log("Transition In");
			if (m_transitionCallback != null)
			{
				m_delayedAction = m_transitionCallback;
				//m_transitionCallback();
				m_transitionCallback = null;
			}
		}

		public void OnTransitionOut()
		{
			Debug.Log("Transition Out");
			InputInterceptionLayer.SetActive(false);
			if (m_endTransitionCallback != null)
			{
				m_delayedAction = m_endTransitionCallback;
				//m_endTransitionCallback();
				m_endTransitionCallback = null;
			}
		}

		public void RequestTransition(TransitionCallback inCallback = null, TransitionCallback outCallback = null)
		{
			InputInterceptionLayer.SetActive(true);
			m_transitionCallback = inCallback;
			m_endTransitionCallback = outCallback;
			m_transitionAnimator.SetBool(c_transitionBool, true);
		}

		public void RequestLevelLaunch(int index)
		{
			m_transitionCamera.gameObject.SetActive(true);
			RequestTransition(() => {
				LaunchLevel(index);
				m_transitionAnimator.SetBool(c_transitionBool, false);
				m_transitionCamera.gameObject.SetActive(false);
			});
		}

		public void RequestLaunchNextLevel()
		{
			m_transitionCamera.gameObject.SetActive(true);
			RequestTransition(() => {
				LaunchNextLevel();
				m_transitionAnimator.SetBool(c_transitionBool, false);
				m_transitionCamera.gameObject.SetActive(false);
			});
		}

		public void RequestRelaunchLevel()
		{
			m_transitionCamera.gameObject.SetActive(true);
			RequestTransition(() => {
				RelaunchLevel();
				m_transitionAnimator.SetBool(c_transitionBool, false);
				m_transitionCamera.gameObject.SetActive(false);
			});
		}

		public void RequestSwitchToHome()
		{
			m_transitionCamera.gameObject.SetActive(true);
			RequestTransition(() => {
				SwitchToHome();
				m_transitionAnimator.SetBool(c_transitionBool, false);
				m_transitionCamera.gameObject.SetActive(false);
			});
		}
		#endregion
	}
}
