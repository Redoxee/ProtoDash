using UnityEngine;
using UnityEngine.Analytics;
using System.Collections;
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

		private const string c_key_userId		= "DeviceId";
		private const string c_key_deviceName	= "DeviceName";
		private const string c_key_gameVersion	= "GameVersion";
		private const string c_key_levelName	= "LevelName";
		private const string c_key_nbTry		= "NbTry";
		private const string c_key_lvlTime		= "Time";

		#region New level beaten
		const string c_newBestScore_event = "NewBestScore";
		private Dictionary<string, object> m_newLevelDictionary;

		private const string c_nbTry_key = "NbTry";
		private const string c_levelTime_key = "Time";

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

#if (UNITY_ANDROID || UNITY_IOS) && !UNITY_EDITOR
			MainProcess.Instance.StartCoroutine(SendRestRequest(levelId, nbTry, time));
#else
				Debug.Log("skip tracking");
#endif
		}

		private const string c_levelRecordUrl = "http://antonroy.fr/Dasher/";
		private IEnumerator SendRestRequest(string levelId, int nbTry, float time)
		{
			var dataManager = MainProcess.Instance.DataManager;
			if (!dataManager.UserId.Contains("Anton"))
			{
				WWWForm content = new WWWForm();

				//content.headers[c_level_key] = levelId;
				//content.headers[c_key_userId] = dataManager.UserId;
				//content.headers[c_key_gameVersion] = MainProcess.Instance.buildData.GetVersionLabel();
				//content.headers[c_nbTry_key] = nbTry.ToString();
				//content.headers[c_levelTime_key] = time.ToString();
				//content.headers[c_key_deviceName] = SystemInfo.deviceModel;

				content.AddField(c_key_userId, dataManager.UserId);
				content.AddField(c_key_deviceName, SystemInfo.deviceModel);
				content.AddField(c_key_gameVersion, MainProcess.Instance.buildData.GetVersionLabel());
				content.AddField(c_key_levelName, levelId);
				content.AddField(c_key_nbTry, nbTry);
				content.AddField(c_key_lvlTime, time.ToString());


				WWW www = new WWW(c_levelRecordUrl, content);
				yield return www;
				Debug.Log("HAVE RESULTS");
				//.. process results from WWW request here...
				if (www.error != null)
				{
					Debug.Log("Erro: " + www.error);
				}
				else
				{
					Debug.Log("All OK");
				}
			}
		}

		#endregion
	}
}
