using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class MainMenuScript : MonoBehaviour {

	private List<GameObject> LevelButons;

	[SerializeField]
	GameObject LevelButonTemplate;


	public void Start()
	{
		GameManager gm = GameManager.GetInstance();

		

		LevelButons = new List<GameObject>(gm.Levels.Count);
		Button btn = LevelButonTemplate.GetComponent<Button>();
		btn.onClick.AddListener(delegate { StartLevel(gm.Levels[0]); });
		Text label = LevelButonTemplate.transform.Find("Label").GetComponent<Text>();
		label.text = gm.Levels[0];
		LevelButons.Add(LevelButonTemplate);

		for (int i = 1; i < gm.Levels.Count; ++i)
		{
			GameObject lb = Instantiate<GameObject>(LevelButonTemplate);
			lb.transform.SetParent(LevelButonTemplate.transform.parent);

			btn = lb.GetComponent<Button>();
			string target = gm.Levels[i];
			btn.onClick.AddListener(delegate { StartLevel(target); });

			label = lb.transform.Find("Label").GetComponent<Text>();
			label.text = gm.Levels[i];

			LevelButons.Add(lb);
		} 

	}

	public void StartLevel(string levelName)
	{
		GameManager.GetInstance().LaunchLevel(levelName);
	}
	
}
