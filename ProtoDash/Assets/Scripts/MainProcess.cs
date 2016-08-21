using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

namespace Dasher
{
	public class MainProcess : Singleton<MainProcess>
	{

		protected MainProcess() { } // guarantee this will be always a singleton only - can't use the constructor!
		/*
		#region singleton
		private static MainProcess singleInstance;

		public static MainProcess GetInstance()
		{
			return singleInstance;
		}
		void Awake()
		{
			//Check if instance already exists
			if (singleInstance == null)

				//if not, set instance to this
				singleInstance = this;

			//If instance already exists and it's not this:
			else if (singleInstance != this)
			{
				//Then destroy this. This enforces our singleton pattern, meaning there can only ever be one instance of a MainProcess.
				Destroy(gameObject);
				return;
			}
			//Sets this to not be destroyed when reloading scene
			DontDestroyOnLoad(gameObject);

		}
	#endregion
	*/
		enum GameState { MainMenu, InGame }
		GameState gameState = GameState.MainMenu;

		[SerializeField]
		public LevelFlow levelFlow;
		private int currentLevelIndex = -1;
		string currentLevelName;


		public void SwitchToHome()
		{
			SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
		}

		public void LaunchLevel(int index)
		{
			if (gameState != GameState.InGame)
			{
				SceneManager.LoadScene("GameSetupScene", LoadSceneMode.Single);
			}
			else
			{
				SceneManager.UnloadScene(currentLevelName);
			}
			currentLevelName = levelFlow.levelList[index].sceneName;

			currentLevelIndex = index;
			SceneManager.LoadScene(currentLevelName, LoadSceneMode.Additive);
		}

		public void RelaunchLevel()
		{
			LaunchLevel(currentLevelIndex);
		}

		public void LaunchNextLevel()
		{
			int levelIndex = currentLevelIndex + 1;
			if (levelIndex < 0 || levelIndex == levelFlow.levelList.Count)
			{
				SwitchToHome();
			}
			else
			{
				LaunchLevel(levelIndex);
			}
		}

		private GUIManager currentGUI;
		public void registerGUIManager(GUIManager gui)
		{
			currentGUI = gui;
		}
		public void unregisterGUI()
		{
			currentGUI = null;
		}

		public GUIManager getCurrentGUI()
		{
			return currentGUI;
		}

		public void NotifyCharacterStart(Character character)
		{
			if (currentGUI)
				currentGUI.NotifyCharacterStart(character);
		}

		public void NotifyCharacterDisable(Character character)
		{
			if (currentGUI)
				currentGUI.notifyCharacterDisable(character);
		}

		public void NotifyEndLevelReached()
		{
			if (currentGUI)
			{
				currentGUI.NotifyEndLevelReached();
			}
			else
			{
				FunctionUtils.Quit();
			}
		}

		public void NotifyDeathZoneTouched()
		{
			if (currentGUI)
			{
				currentGUI.NotifyDeathZoneTouched();
			}
			else
			{
				FunctionUtils.Quit();
			}
		}
	}
}
