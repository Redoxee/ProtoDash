﻿using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

	private static GameManager singleInstance;

	public static GameManager GetInstance()
	{
		return singleInstance;
	}

	enum GameState { MainMenu, InGame }
	GameState gameState = GameState.MainMenu;
	string currentLevel;

	void Awake()
	{
		//Check if instance already exists
		if (singleInstance == null)

			//if not, set instance to this
			singleInstance = this;

		//If instance already exists and it's not this:
		else if (singleInstance != this)

			//Then destroy this. This enforces our singleton pattern, meaning there can only ever be one instance of a GameManager.
			Destroy(gameObject);

		//Sets this to not be destroyed when reloading scene
		DontDestroyOnLoad(gameObject);
	}

	public void SwitchToHome()
	{
		SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
	}

	public void LaunchLevel(string levelName)
	{
		if (gameState != GameState.InGame)
		{
			SceneManager.LoadScene("GameSetupScene", LoadSceneMode.Single);
		}
		else
		{
			SceneManager.UnloadScene(currentLevel);
		}

		SceneManager.LoadScene(levelName, LoadSceneMode.Additive);
		currentLevel = levelName;
	}

	public void RelaunchLevel()
	{
		LaunchLevel(currentLevel);
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
}
