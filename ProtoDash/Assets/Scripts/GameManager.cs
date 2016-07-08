using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

	enum GameState { MainMenu, InGame}

	GameState gameState = GameState.MainMenu;

	// Use this for initialization
	void Start () {
		DontDestroyOnLoad(this);

		
	}
	
	// Update is called once per frame
	void Update () {
		if (gameState == GameState.MainMenu)
		{
			if (Input.GetMouseButtonDown(0))
			{
				switchToGame();
			}
		}
	}

	void switchToGame()
	{
		SceneManager.LoadScene("MainScene",LoadSceneMode.Single);
		gameState = GameState.InGame;
	}
}
