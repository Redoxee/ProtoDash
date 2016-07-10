using UnityEngine;
using System.Collections;

public class GUIManager : MonoBehaviour {

	[SerializeField]
	private MainScript mainScriptRef;

	private GameManager gameManagerRef;

	[SerializeField]
	private Canvas mainCanvas;
	[SerializeField]
	private Canvas pauseCanvas;

	// Use this for initialization
	void Start () {
		gameManagerRef = GameManager.GetInstance();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void PauseGame()
	{
		mainScriptRef.PauseGame(true);
		mainCanvas.gameObject.SetActive(false);
		pauseCanvas.gameObject.SetActive(true);
	}

	public void ResumeGame()
	{
		mainScriptRef.PauseGame(false);
		mainCanvas.gameObject.SetActive(true);
		pauseCanvas.gameObject.SetActive(false);
	}

	public void GoHome()
	{
		mainCanvas.gameObject.SetActive(false);
		pauseCanvas.gameObject.SetActive(false);
		gameManagerRef.SwitchToHome();
	}

	public void RetryLevel()
	{
		gameManagerRef.RelaunchLevel();
	}
}
