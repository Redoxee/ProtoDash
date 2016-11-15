using UnityEngine;
using UnityEngine.UI;

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

		public void NotifyEndLevelReached(bool isNewBestTime, bool isNewparTime,float oldBestTime)
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

			m_endLevelGUI.m_current.SetMainText("Time\n" + time.ToString(TimeManager.c_timeDisplayFormat));
			m_endLevelGUI.m_best.SetMainText("Best\n" + bestTime.ToString(TimeManager.c_timeDisplayFormat));
			m_endLevelGUI.m_champ.SetMainText("Champ\n" + parTime.ToString(TimeManager.c_timeDisplayFormat));
			


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
