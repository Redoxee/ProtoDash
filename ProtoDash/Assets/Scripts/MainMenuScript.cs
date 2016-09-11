using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace Dasher
{
	public class MainMenuScript : MonoBehaviour
	{

		private List<GameObject> LevelButons;

		[SerializeField]
		GameObject LevelButonTemplate;

		private const string BASE_LEVEL_LABEL = "Level - ";

		public void Start()
		{
			MainProcess mp = MainProcess.Instance;


			LevelButons = new List<GameObject>(mp.levelFlow.levelList.Count);
			Button btn = LevelButonTemplate.GetComponent<Button>();
			btn.onClick.AddListener(delegate { StartLevel(0); });
			Text label = LevelButonTemplate.transform.Find("Label").GetComponent<Text>();
			label.text = BASE_LEVEL_LABEL + 1;
			LevelButons.Add(LevelButonTemplate);

			for (int i = 1; i < mp.levelFlow.levelList.Count; ++i)
			{
				int copy = i; // forcing i into the scope

				GameObject lb = Instantiate<GameObject>(LevelButonTemplate);
				lb.transform.SetParent(LevelButonTemplate.transform.parent);

				btn = lb.GetComponent<Button>();

				btn.onClick.AddListener(() => {
					StartLevel(copy);
				});

				label = lb.transform.Find("Label").GetComponent<Text>();
				label.text = BASE_LEVEL_LABEL + (i + 1);

				LevelButons.Add(lb);
			}

		}

		public void StartLevel(int levelIndex)
		{
			MainProcess.Instance.LaunchLevel(levelIndex);
		}
	}
}
