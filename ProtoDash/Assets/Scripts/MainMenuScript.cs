using UnityEngine;
using System.Collections;

public class MainMenuScript : MonoBehaviour {


	public void StartLevel(string levelName)
	{
		GameManager.GetInstance().LaunchLevel(levelName);
	}
}
