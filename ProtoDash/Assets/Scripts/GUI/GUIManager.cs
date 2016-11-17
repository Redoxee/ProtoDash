using UnityEngine;
using UnityEngine.UI;

using System;
using System.Collections.Generic;

namespace Dasher
{
	public class GUIManager : MonoBehaviour
	{
		private GameProcess m_gameProcess;

		[Header("Game UI")]
		[SerializeField]
		private GameCanvasHolder m_canvasHolder;


		private GameObject m_gameCanvas;
		private Text m_gameTimerText;
		private SmartGauge m_gauge;

		[Space]
		[Header("Pause")]
		[SerializeField]
		private GameObject m_pauseCanvas;
		[SerializeField]
		private Text m_pauseLevelLabelText;
		[SerializeField]
		private Text m_pauseCurrentTimeText;
		[SerializeField]
		private Text m_pauseParTimeText;

		[Space]
		[Header("End Level")]
		[SerializeField]
		private GameObject m_endCanvas;
		[SerializeField]
		private Text m_endLevelLabelText;
		[SerializeField]
		EndLevelGUI m_endLevelGUI = null;
		
		[Space]
		[Header("Death")]
		[SerializeField]
		private GameObject m_failCanvas;
		[SerializeField]
		private Text m_failLevelLabelText;

		[Space]
		[Header("Intro")]
		[SerializeField]
		private GameObject m_introCanvas = null;
		[SerializeField]
		private Text m_introLevelText = null;

		[Space]
		[Header("Debug")]
		private PerfRuler m_perf;
		[SerializeField]
		public Text m_debugFPS;

		void Awake()
		{
			bool isLeftHanded = MainProcess.Instance.DataManager.GetSettings().isLefthanded;
			var canvas = isLeftHanded ? m_canvasHolder.LeftCanvas:m_canvasHolder.RightCanvas;
			m_gameCanvas = canvas;
			m_gauge = canvas.GetComponentInChildren<SmartGauge>();
			m_gameTimerText = (isLeftHanded ? m_canvasHolder.LeftTimeText : m_canvasHolder.RightTimeText).GetComponentInChildren<Text>();
			(isLeftHanded ? m_canvasHolder.RightCanvas : m_canvasHolder.LeftCanvas).SetActive(false);
			
			m_perf = GetComponent<PerfRuler>();

			m_endLevelGUI.m_current.Initialize();
			m_endLevelGUI.m_best.Initialize();
			m_endLevelGUI.m_champ.Initialize();
		}

		void Start()
		{
			m_gameProcess = GameProcess.Instance;
			m_gameProcess.registerGUIManager(this);

			LevelData currentLevel = MainProcess.Instance.levelFlow.GetLevelData(GameProcess.CurrentLevelName);
			string levelLabel = currentLevel.GetLevelLabel();
			m_pauseLevelLabelText.text = levelLabel;
			m_endLevelLabelText.text = levelLabel;
			m_failLevelLabelText.text = levelLabel;
			m_introLevelText.text = levelLabel;
		}

		void OnDisable()
		{
			m_gameProcess.unregisterGUI();
		}

		public void ManualFixedUpdate()
		{
			if (m_gameCanvas.gameObject.activeSelf)
			{
				float time = m_gameProcess.GameTime.CurrentLevelTime;
				m_gameTimerText.text = time.ToString(TimeManager.c_timeDisplayFormat);
			}
		}

		public void ManualUpdate()
		{
			if (m_isEndLevel)
				Update_EndLevel();
		}

		public void PauseGame()
		{
			m_gameProcess.RequirePause();


			LevelData currentLevel = MainProcess.Instance.levelFlow.GetLevelData(GameProcess.CurrentLevelName);

			float time = m_gameProcess.GameTime.CurrentLevelTime;
			float parTime = currentLevel.parTime;

			m_pauseCurrentTimeText.text = "Current :\n" + time.ToString(TimeManager.c_timeDisplayFormat);
			m_pauseParTimeText.text = "Champ :\n" + parTime.ToString(TimeManager.c_timeDisplayFormat);

			m_gameCanvas.gameObject.SetActive(false);
			m_pauseCanvas.gameObject.SetActive(true);
		}

		public void ResumeGame()
		{
			m_pauseCanvas.gameObject.SetActive(false);
			m_gameCanvas.gameObject.SetActive(true);
			m_gameProcess.RequireResume();

		}

		public void NotifyLevelStart()
		{
			m_canvasHolder.gameObject.SetActive(true);
			m_gameCanvas.gameObject.SetActive(true);
			m_pauseCanvas.gameObject.SetActive(false);
			m_endCanvas.gameObject.SetActive(false);
			m_failCanvas.gameObject.SetActive(false);
			m_introCanvas.gameObject.SetActive(false);

			m_gauge.Initialize();
			m_perf.StartRecord();
		}

		bool m_isEndLevel = false;
		
		Dictionary<float, Action> m_endLevelEvents = null;
		List<float> m_endEventsKeys = null;
		int m_nextEventToShoot = 0;
		float m_endEventTimer = 0f;

		public void NotifyEndLevelReached(bool isNewBestTime, bool isNewparTime,float oldBestTime)
		{
			m_perf.StopRecord();
			m_debugFPS.text = m_perf.GetMeanFPS().ToString();
			m_gameProcess.RequirePause();
			m_gameCanvas.gameObject.SetActive(false);
			m_endCanvas.gameObject.SetActive(true);

			LevelData currentLevel = MainProcess.Instance.levelFlow.GetLevelData(GameProcess.CurrentLevelName);

			m_currentTime = m_gameProcess.GameTime.CurrentLevelTime;
			m_bestTime = currentLevel.currentBest;
			m_champTime = currentLevel.parTime;

			m_endLevelGUI.m_current.SetMainText("Time\n");
			m_endLevelGUI.m_best.SetMainText("Best\n");
			m_endLevelGUI.m_champ.SetMainText("Champ\n");

			SetEndLevelEvents();
			m_isEndLevel = true;
		}

		float m_currentTime, m_bestTime, m_champTime;


		void SetEndLevelEvents()
		{
			m_endLevelEvents = new Dictionary<float, Action>();
			m_endLevelEvents[.5f] = () => {
				m_endLevelGUI.m_current.FlashInText("Time\n" + m_currentTime.ToString(TimeManager.c_timeDisplayFormat));
			};
			m_endLevelEvents[.75f] = () => {
				m_endLevelGUI.m_best.FlashInText("Best\n" + m_bestTime.ToString(TimeManager.c_timeDisplayFormat));
			};
			m_endLevelEvents[1.0f] = () => {
				m_endLevelGUI.m_champ.FlashInText("Champ\n" + m_champTime.ToString(TimeManager.c_timeDisplayFormat));
			};

			m_endEventsKeys =  new List<float>(m_endLevelEvents.Count);
			var keys = m_endLevelEvents.Keys.GetEnumerator();
			while(keys.MoveNext())
			{
				m_endEventsKeys.Add(keys.Current);
			}
			m_endEventsKeys.Sort();
			m_nextEventToShoot = 0;
			m_endEventTimer = 0f;
		}

		void Update_EndLevel()
		{
			m_endLevelGUI.m_current.ManualUpdate();
			m_endLevelGUI.m_best.ManualUpdate();
			m_endLevelGUI.m_champ.ManualUpdate();

			if (m_nextEventToShoot < m_endEventsKeys.Count)
			{ 
				var prevTime = m_endEventTimer;
				m_endEventTimer += Time.deltaTime;
				while (shootNextEventIfNeeded(prevTime, m_endEventTimer))
				{
					if (m_nextEventToShoot >= m_endEventsKeys.Count)
					{
						break;
					}
				}
			}

		}

		bool shootNextEventIfNeeded(float prev,float current)
		{
			var eventTime = m_endEventsKeys[m_nextEventToShoot];
			if (eventTime > prev && eventTime < current)
			{
				m_endLevelEvents[eventTime]();
				m_nextEventToShoot++;
				return true;
			}
			return false;
		}

		public void NotifyIntermediateState()
		{
			m_gameCanvas.gameObject.SetActive(false);
		}

		public void NotifyDeath()
		{
			m_gameProcess.RequirePause();
			m_gameCanvas.gameObject.SetActive(false);
			m_failCanvas.gameObject.SetActive(true);
		}

		public void GoHome()
		{
			MainProcess.Instance.RequestSwitchToHome();
		}

		public void NextLevel()
		{
			MainProcess.Instance.RequestLaunchNextLevel();
		}

		public void RetryLevel()
		{
			MainProcess.Instance.RequestRelaunchLevel();
		}

		#region introState

		public void SetStateIntro()
		{
			m_introCanvas.gameObject.SetActive(true);
			m_gameCanvas.gameObject.SetActive(false);
			m_pauseCanvas.gameObject.SetActive(false);
			m_failCanvas.gameObject.SetActive(false);
			m_endCanvas.gameObject.SetActive(false);
		}

		#endregion
	}
}
