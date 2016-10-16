using UnityEngine;
using UnityEngine.Analytics;
using System.Collections.Generic;

namespace Dasher
{
	public class DasherAnalyticsManager {


		private MainProcess m_mainProcess = null;

		public DasherAnalyticsManager()
		{
			m_mainProcess = MainProcess.Instance;
			 
			InitNewLevel();
		}

		private const string c_version_key = "BuildVersion";
		private const string c_level_key = "LevelId";

		#region New level beaten
		const string c_newBestScore_event = "NewBestScore";
		private Dictionary<string, object> m_newLevelDictionary;

		private const string c_nbTry_key = "NbTry";
		private const string c_levelTime_key = "Level time";

		private void InitNewLevel()
		{
			m_newLevelDictionary = new Dictionary<string, object>();
			m_newLevelDictionary[c_version_key] = m_mainProcess.buildData.GetVersionLabel();
			m_newLevelDictionary[c_level_key] = "NoLevel";
			m_newLevelDictionary[c_nbTry_key] = 0;
			m_newLevelDictionary[c_levelTime_key] = -1f;
		}



		public void NotifyNewLevelBeaten(string levelId, int nbTry, float time)
		{
			m_newLevelDictionary[c_level_key] = levelId;
			m_newLevelDictionary[c_nbTry_key] = nbTry;
			m_newLevelDictionary[c_levelTime_key] = time;

			//foreach (var v in m_newLevelDictionary)
			//{
			//	Debug.Log(v.Key + " : " + v.Value.ToString());
			//}
			Analytics.CustomEvent(c_newBestScore_event, m_newLevelDictionary);
		}
		#endregion
	}
}
