using UnityEngine;
using UnityEngine.UI;

namespace Dasher
{
	public class GUIManager : MonoBehaviour
	{
		private GameProcess m_gameProcess;
		
		[Header("Game UI")]
		[SerializeField]
		private Canvas m_gameCanvas;
		[SerializeField]
		private Text m_gameTimerText;
		[SerializeField]
		private SmartGauge m_gauge;

		[Space]
		[Header("Pause")]
		[SerializeField]
		private Canvas m_pauseCanvas;
		[SerializeField]
		private Text m_pauseLevelLabelText;
		[SerializeField]
		private Text m_pauseCurrentTimeText;
		[SerializeField]
		private Text m_pauseParTimeText;

		[Space]
		[Header("End Level")]
		[SerializeField]
		private Canvas m_endCanvas;
		[SerializeField]
		private Text m_endLevelLabelText;
		[SerializeField]
		private GameObject m_endTimer;
		private Text m_endTimerText;
		[SerializeField]
		private GameObject m_endBestTime;
		private Text m_endBestTimeText;
		[SerializeField]
		private GameObject m_endParTime;
		private Text m_endParTimeText;

		[Space]
		[Header("Death")]
		[SerializeField]
		private Canvas m_failCanvas;
		[SerializeField]
		private Text m_failLevelLabelText;

		[Space]
		[Header("Intro")]
		[SerializeField]
		private Canvas m_introCanvas = null;
		[SerializeField]
		private Text m_introLevelText = null;

		[Space]
		[Header("Debug")]
		private PerfRuler m_perf;
		[SerializeField]
		public Text m_debugFPS;

		void Awake()
		{
			m_endTimerText = m_endTimer.GetComponentInChildren<Text>();
			m_endBestTimeText = m_endBestTime.GetComponentInChildren<Text>();
			m_endParTimeText = m_endParTime.GetComponentInChildren<Text>();
			m_perf = GetComponent<PerfRuler>();
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
			m_gameCanvas.gameObject.SetActive(true);
			m_pauseCanvas.gameObject.SetActive(false);
			m_endCanvas.gameObject.SetActive(false);
			m_failCanvas.gameObject.SetActive(false);
			m_introCanvas.gameObject.SetActive(false);

			m_gauge.Initialize();
			m_perf.StartRecord();
		}

		public void NotifyEndLevelReached(bool isNewBestTime, bool isNewparTime)
		{
			m_perf.StopRecord();
			m_debugFPS.text = m_perf.GetMeanFPS().ToString();
			m_gameProcess.RequirePause();
			m_gameCanvas.gameObject.SetActive(false);
			m_endCanvas.gameObject.SetActive(true);

			LevelData currentLevel = MainProcess.Instance.levelFlow.GetLevelData(GameProcess.CurrentLevelName);

			float time = m_gameProcess.GameTime.CurrentLevelTime;
			float bestTime = currentLevel.currentBest;
			float parTime = currentLevel.parTime;

			m_endTimerText.text = "Current :\n" + time.ToString(TimeManager.c_timeDisplayFormat);
			m_endBestTimeText.text = "Best :\n" + bestTime.ToString(TimeManager.c_timeDisplayFormat);
			m_endParTimeText.text = "Champ :\n" + parTime.ToString(TimeManager.c_timeDisplayFormat);
			if (bestTime <= parTime)
			{
				m_endParTime.SetActive(false);
			}


		}

		public void NotifyDeathZoneTouched()
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
