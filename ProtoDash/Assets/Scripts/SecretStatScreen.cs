using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Dasher
{
	public class SecretStatScreen : MonoBehaviour
	{
		[SerializeField]
		Text m_textDisplay = null;

		bool m_isInitialized = false;

		// Use this for initialization
		void Start()
		{
			m_isInitialized = false;
			ManualStart();
		}

		private void Update()
		{
			ManualStart();
		}

		void ManualStart()
		{
			if (MainProcess.Instance == null || m_isInitialized)
				return;
			m_isInitialized = true;

			var levelFlow = MainProcess.Instance.levelFlow;
			var lvlList = levelFlow.LevelList;
			var saveManager = MainProcess.Instance.DataManager;

			var displayBuilder = new System.Text.StringBuilder();
			for (int i = lvlList.Count - 1; i > -1 ; --i)
			{
				var level = lvlList[i];
				var lvlId = level.sceneName;
				displayBuilder.Append(level.GetLevelLabel()).Append("\n");
				displayBuilder.Append(level.sceneName.Substring(0, 3)).Append("\n");
				var isLevelUnlocked = saveManager.DoesProgressionAllowLevel(i);
				if (isLevelUnlocked)
				{
					var tryCount = saveManager.GetLevelTryCount(lvlId);
					var completeCount	= saveManager.GetLevelSuccessCount(lvlId);
					var failCount		= saveManager.GetLevelFailCount(lvlId);
					var bestTime = saveManager.GetLevelTime(lvlId);
					if (tryCount < 1)
						bestTime = -1;
					displayBuilder.AppendFormat("Tries {0}, Success {1}, Fail {2}, Best {3}",tryCount,completeCount,failCount,bestTime.ToString("##.###")).Append("\n");
				}
				else
				{
					displayBuilder.Append("Locked\n");
				}

				displayBuilder.Append("---\n\n");
			}

			string endResult = displayBuilder.ToString();
			m_textDisplay.text = endResult;
			//GUIUtility.systemCopyBuffer = endResult;

		}

		public void SendStats()
		{
			MainProcess.SendFeedback("Here are my stats", m_textDisplay.text);
		}
	}
}