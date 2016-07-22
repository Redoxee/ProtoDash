﻿using UnityEngine;
using System.Collections;

public class GUIManager : MonoBehaviour {

	[SerializeField]
	private Character characterRef;

	private MainProcess gameManagerRef;

	[SerializeField]
	private Canvas mainCanvas;
	[SerializeField]
	private Canvas pauseCanvas;
	[SerializeField]
	private Canvas endLevelCanvas;

	// Use this for initialization
	void Start () {
		gameManagerRef = MainProcess.GetInstance();
		gameManagerRef.registerGUIManager(this);
	}

	void OnDisable()
	{
		gameManagerRef.unregisterGUI();
	}

	// Update is called once per frame
	void Update () {
	
	}

	public void PauseGame()
	{
		characterRef.PauseGame(true);
		mainCanvas.gameObject.SetActive(false);
		pauseCanvas.gameObject.SetActive(true);
	}

	public void ResumeGame()
	{
		characterRef.PauseGame(false);
		mainCanvas.gameObject.SetActive(true);
		pauseCanvas.gameObject.SetActive(false);
	}

	public void NotifyEndLevelReached()
	{
		characterRef.PauseGame(true);
		mainCanvas.gameObject.SetActive(false);
		endLevelCanvas.gameObject.SetActive(true);
	}

	public void GoHome()
	{
		mainCanvas.gameObject.SetActive(false);
		pauseCanvas.gameObject.SetActive(false);
		gameManagerRef.SwitchToHome();
	}

	public void NextLevel()
	{
		gameManagerRef.LaunchNextLevel();
	}

	public void RetryLevel()
	{
		gameManagerRef.RelaunchLevel();
	}
}
