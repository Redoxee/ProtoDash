using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

namespace Dasher
{
	public class StatsScreen : MonoBehaviour {

		[SerializeField]
		GameObject m_itemPrefab = null;

		const string c_noStats = "No stats yet!\nFinish a level and come back";
		const string c_LevelMessage = "You've beaten {0} out of {1} levels";
		const string c_AllLevelComplete = "Congratulation !\nYou've beaten all the levels";
		const string c_ChampMessage = "You are a champ on {0} levels";
		const string c_ChampAllLevels = "Congratulation !\nYou are a champ on all levels!";
		const string c_TotalTime = "Cumulated best Time : {0}";
		const string c_NbRunLaunched = "Level launched : {0}";
		const string c_NbJumps = "Number of jumps : {0}";
		const string c_NbDash = "Number of dashes : {0}";


		[SerializeField]
		GameObject m_container = null;

		private LevelFlow m_levelFlow;

		private int m_nbStats = 0;

		private List<GameObject> m_allEntries = new List<GameObject>();

		private void Setup()
		{
			m_allEntries.Clear();
			m_nbStats = 0;
			int levelCount = m_levelFlow.GetLevelCount();
			int levelFinished = 0, levelChamp = 0;

			float totalTime = 0f;

			for (int i = 0; i < levelCount; ++i)
			{
				if (m_levelFlow.IsLevelFinished(i))
				{
					levelFinished++;
					if (m_levelFlow.IsLevelChamp(i))
					{
						levelChamp ++;
					}
					totalTime += m_levelFlow.LevelList[i].currentBest;
				}
			}

			if (levelFinished > 0)
			{
				if (levelFinished == levelCount)
				{
					CreateItem(string.Format(c_AllLevelComplete));

					CreateItem(string.Format(c_TotalTime, totalTime.ToString(TimeManager.c_timeDisplayFormat)));
				}
				else
				{
					CreateItem(string.Format(c_LevelMessage, levelFinished, levelCount));
				}


				if (levelChamp > 0)
				{
					if (levelChamp == levelCount)
					{
						CreateItem(c_ChampAllLevels);
					}
					else
					{
						CreateItem(string.Format(c_ChampMessage, levelChamp));
					}
				}
			}

			SaveManager datas = MainProcess.Instance.DataManager;
			int totalRuns = datas.GetTotalRuns();
			if (totalRuns > 0)
			{
				CreateItem(string.Format(c_NbRunLaunched, totalRuns));
			}
			int totalJumps = datas.GetTotalJumps();
			if (totalJumps > 0)
			{
				CreateItem(string.Format(c_NbJumps, totalJumps));
			}
			int totalDashes = datas.GetTotalDashes();
			if (totalDashes > 0)
			{
				CreateItem(string.Format(c_NbDash, totalDashes));
			}
			

			if (m_nbStats == 0)
			{
				CreateItem(c_noStats);
			}
		}

		GameObject CreateItem(string message)
		{
			m_nbStats++;
			GameObject item = Instantiate(m_itemPrefab);
			item.transform.SetParent(m_container.transform, false);
			Text text = item.GetComponentInChildren<Text>();
			text.text = message;

			m_allEntries.Add(item);
			return item;
		}

		void Start() {
			m_levelFlow = MainProcess.Instance.levelFlow;

			Setup();
		}

		public void OnHomePressed()
		{
			MainProcess.Instance.RequestSwitchToHome();
		}

		#region Animation
		[SerializeField]
		float m_initialDelay = .5f;
		[SerializeField]
		float m_interval = .5f;

		float m_antimationTimer = 0f;
		void Update()
		{

			var prevIndex = GetInterval(m_antimationTimer);
			m_antimationTimer += Time.deltaTime;
			var currentIndex = GetInterval(m_antimationTimer);

			if (prevIndex != currentIndex)
			{
				var entryCount = m_allEntries.Count;
				if (currentIndex < entryCount)
				{
					AnimationFlashing anim = m_allEntries[currentIndex].GetComponentInChildren<AnimationFlashing>();
					anim.StartAnimation();
				}
				else
				{
					enabled = false;
				}
			}
		}

		int GetInterval(float ts)
		{
			ts -= m_initialDelay;
			if(ts < 0 )
				return -1;
			int result = (int)(ts / m_interval);
			return result;
		}
		
		#endregion
	}
}
