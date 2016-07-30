using UnityEngine;
using System.Collections;

public class GUIManager : MonoBehaviour {
	
	private Character characterRef;

	private MainProcess gameManagerRef;

	[SerializeField]
	private Canvas mainCanvas;
	[SerializeField]
	private Canvas gameCanvas;
	[SerializeField]
	private Canvas pauseCanvas;
	[SerializeField]
	private Canvas endLevelCanvas;

	private Animator guiAnimator;


	// Use this for initialization
	void Start () {
		gameManagerRef = MainProcess.GetInstance();
		gameManagerRef.registerGUIManager(this);

		guiAnimator = mainCanvas.GetComponent<Animator>();
	}

	public void NotifyCharacterStart(Character c)
	{
		characterRef = c;
	}

	public void notifyCharacterDisable(Character character)
	{
		characterRef = null;
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
		//gameCanvas.gameObject.SetActive(false);
		//pauseCanvas.gameObject.SetActive(true);
		
		guiAnimator.SetTrigger("GoToPause");
		characterRef.PauseGame(true);
	}

	public void ResumeGame()
	{
		guiAnimator.SetTrigger("ResumeGame");
		//gameCanvas.gameObject.SetActive(true);
		//pauseCanvas.gameObject.SetActive(false);

	}

	public void OnResumeAnimationEnd()
	{
		characterRef.PauseGame(false);
	}

	public void NotifyEndLevelReached()
	{
		characterRef.PauseGame(true);
		gameCanvas.gameObject.SetActive(false);
		endLevelCanvas.gameObject.SetActive(true);
	}

	public void GoHome()
	{
		gameCanvas.gameObject.SetActive(false);
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
