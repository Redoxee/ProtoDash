using UnityEngine;
using UnityEngine.UI;

namespace Dasher
{
	public class GUIManager : MonoBehaviour
	{
		private MainProcess m_mainProcess;

		[SerializeField]
		private Canvas mainCanvas;
		[SerializeField]
		private Canvas gameCanvas;
		[SerializeField]
		private Text m_gameTimerText;

		[SerializeField]
		private SmartGauge m_gauge;

		[SerializeField]
		private Canvas pauseCanvas;

		[SerializeField]
		private Text m_pauseLevelLabelText;

		[SerializeField]
		private Canvas endLevelCanvas;

		[SerializeField]
		private Text m_endLevelLabelText;
		[SerializeField]
		private Text m_endTimerText;

		[SerializeField]
		private Canvas failCanvas;

		[SerializeField]
		private Text m_failLevelLabelText;
		[SerializeField]
		private Text m_failTimerText;


		void Awake()
		{
			Debug.Log("GUI Awake");
		}

		void Start()
		{
			m_mainProcess = MainProcess.Instance;
			m_mainProcess.registerGUIManager(this);
		}

		void OnDisable()
		{
			m_mainProcess.unregisterGUI();
		}

		public void ManualFixedUpdate()
		{
			if (gameCanvas.gameObject.activeSelf)
			{
				float time = m_mainProcess.GameTime.CurrentLevelTime;
				m_gameTimerText.text = time.ToString(TimeManager.c_timeDisplayFormat);
			}
		}

		public void PauseGame()
		{
			m_mainProcess.RequirePause();

			gameCanvas.gameObject.SetActive(false);
			pauseCanvas.gameObject.SetActive(true);
		}

		public void ResumeGame()
		{
			pauseCanvas.gameObject.SetActive(false);
			gameCanvas.gameObject.SetActive(true);
			m_mainProcess.RequireResume();

		}

		public void NotifyLevelStart()
		{
			Debug.Log("GUILevelStart");
			gameCanvas.gameObject.SetActive(true);
			pauseCanvas.gameObject.SetActive(false);
			endLevelCanvas.gameObject.SetActive(false);
			failCanvas.gameObject.SetActive(false);

			string levelLabel = "Level - " + (m_mainProcess.CurrentLevelIndex + 1); // TODO : use string builder or something
			m_pauseLevelLabelText.text = levelLabel;
			m_endLevelLabelText.text = levelLabel;
			m_failLevelLabelText.text = levelLabel;

			m_gauge.Initialize();
		}

		public void NotifyEndLevelReached()
		{
			m_mainProcess.RequirePause();
			gameCanvas.gameObject.SetActive(false);
			endLevelCanvas.gameObject.SetActive(true);

			float time = m_mainProcess.GameTime.CurrentLevelTime;
			m_endTimerText.text = time.ToString(TimeManager.c_timeDisplayFormat);
		}

		public void NotifyDeathZoneTouched()
		{
			m_mainProcess.RequirePause();
			gameCanvas.gameObject.SetActive(false);
			failCanvas.gameObject.SetActive(true);

			float time = m_mainProcess.GameTime.CurrentLevelTime;
			m_failTimerText.text = time.ToString(TimeManager.c_timeDisplayFormat);
		}

		public void GoHome()
		{
			m_mainProcess.SwitchToHome();
		}

		public void NextLevel()
		{
			m_mainProcess.LaunchNextLevel();
		}

		public void RetryLevel()
		{
			m_mainProcess.RelaunchLevel();
		}
	}
}
