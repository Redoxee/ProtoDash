using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

namespace Dasher
{
	public enum GameStates {
		Error,
		Intro,
		MainMenu,
		InGame,
		Stats,
		Credits,
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
		public BuildData buildData;
	
		//[SerializeField]
		//public ColorScheme m_colorScheme;

		[SerializeField]
		Camera m_transitionCamera;

		[SerializeField]
		GameObject InputInterceptionLayer;
		[SerializeField]
		Animator m_transitionAnimator;

		private DasherAnalyticsManager m_AnalyticsManager;
		public DasherAnalyticsManager AnalyticsManager { get { return m_AnalyticsManager; } }

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
			m_AnalyticsManager = new DasherAnalyticsManager();

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
		const string c_statsScreen = "StatsScreen";
		const string c_creditsScreen = "Credits";
		string m_currenLevelScene = null;
		int m_currentLevelIndex = -1;

		private void UnloadCurrentAdditionalScene(bool keepLevel = false)
		{
			if (m_gameState == GameStates.InGame)
			{
				SceneManager.UnloadScene(c_gameSetupScene);
				if (!keepLevel)
				{
					SceneManager.UnloadScene(m_currenLevelScene);
				}
			}

			if (m_gameState == GameStates.MainMenu)
			{
				SceneManager.UnloadScene(c_mainMenuScene);
			}

			if (m_gameState == GameStates.Stats)
			{
				SceneManager.UnloadScene(c_statsScreen);
			}

			if (m_gameState == GameStates.Credits)
			{
				SceneManager.UnloadScene(c_creditsScreen);
			}
		}


		public void SwitchToHome()
		{
			UnloadCurrentAdditionalScene();

			SceneManager.LoadScene(c_mainMenuScene, LoadSceneMode.Additive);
			SetState(GameStates.MainMenu);
		}

		public void LaunchLevel(int index)
		{
			UnloadCurrentAdditionalScene();
			
			SceneManager.LoadScene(c_gameSetupScene, LoadSceneMode.Additive);
			SetState(GameStates.InGame);
			
			m_currenLevelScene = levelFlow.LevelList[index].sceneName;

			m_currentLevelIndex = index;
			SceneManager.LoadScene(m_currenLevelScene, LoadSceneMode.Additive);
		}

		public void LaunchLevel(LevelData level)
		{
			int i = 0;
			int count = levelFlow.GetLevelCount();
			for (; i < count; ++i)
			{
				if (levelFlow.LevelList[i] == level)
				{
					break;
				}
			}
			LaunchLevel(i);
		}

		public void RelaunchLevel()
		{
			LaunchLevel(m_currentLevelIndex);
		}

		public void LaunchNextLevel()
		{
			int levelIndex = m_currentLevelIndex + 1;
			if (levelIndex < 0 || levelIndex == levelFlow.GetLevelCount())
			{
				SwitchToStatsScreen(); 
			}
			else
			{
				LaunchLevel(levelIndex);
			}
		}

		public string CurrentLevel { get { return m_currenLevelScene; } }
		public int CurrentLevelIndex { get { return m_currentLevelIndex; } }

		public void RelaunchGame()
		{
			SceneManager.LoadScene(0);
		}
		#endregion

		#region Save

		private SaveManager m_saveManager = null;
		public SaveManager DataManager { get { return m_saveManager; } }

		#endregion

		#region StatsScreen

		public void SwitchToStatsScreen()
		{
			UnloadCurrentAdditionalScene();
			SetState(GameStates.Stats);
			SceneManager.LoadScene(c_statsScreen, LoadSceneMode.Additive);
		}

		#endregion

		#region Credits Screen

		public void SwitchToCreditsScreen()
		{
			UnloadCurrentAdditionalScene();
			SetState(GameStates.Credits);
			SceneManager.LoadScene(c_creditsScreen, LoadSceneMode.Additive);
		}
		
		#endregion

		#region States
		public void SetState(GameStates newState)
		{
			if (newState != GameStates.Intro)
			{
				m_transitionCamera.gameObject.SetActive(false);
			}
			m_gameState = newState;
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
			//Debug.Log("Transition In");
			if (m_transitionCallback != null)
			{
				m_delayedAction = m_transitionCallback;
				//m_transitionCallback();
				m_transitionCallback = null;
			}
		}

		public void OnTransitionOut()
		{
			//Debug.Log("Transition Out");
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
		
		public void RequestLevelLaunch(LevelData level)
		{
			m_transitionCamera.gameObject.SetActive(true);
			RequestTransition(() => {
				LaunchLevel(level);
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

		public void RequestSwitchToStats()
		{
			m_transitionCamera.gameObject.SetActive(true);
			RequestTransition(() => {
				SwitchToStatsScreen();
				m_transitionAnimator.SetBool(c_transitionBool, false);
				m_transitionCamera.gameObject.SetActive(false);
			});
		}

		public void RequestSwitchToCredits()
		{
			m_transitionCamera.gameObject.SetActive(true);
			RequestTransition(() => {
				SwitchToCreditsScreen();
				m_transitionAnimator.SetBool(c_transitionBool, false);
				m_transitionCamera.gameObject.SetActive(false);
			});
		}

		#endregion

		#region Main menu reference

		MainMenuScript m_mainMenuRef = null;
		public void RegisterMainMenu(MainMenuScript mm)
		{
			if (m_mainMenuRef != null)
			{
				Debug.LogWarning("Flow error : trying to register main menu script several time !");
			}
			m_mainMenuRef = mm;
		}

		public void UnregisterMainMenu()
		{
			if (m_mainMenuRef == null)
			{
				Debug.LogWarning("unregistering Main menu while no main menu registered !");
			}
			m_mainMenuRef = null;
		}

		public MainMenuScript MainMenu { get { return m_mainMenuRef; } }

		#endregion

		#region Feedback
		const string c_feedbackEmail = "antonmakegames@gmail.com";

		public static void SimpleFeedback()
		{
			SendFeedback("I have some feedback on Dasher", "");
		}

		static void SendFeedback(string header, string body = "")
		{
			string subject = EscapeURL(header);
			body = EscapeURL(body);
			Application.OpenURL("mailto:" + c_feedbackEmail + "?subject=" + subject + "&body=" + body);
		}

		static string EscapeURL(string url)
		{
			return WWW.EscapeURL(url).Replace("+", "%20");
		}

		#endregion
	}
}
