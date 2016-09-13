using UnityEngine;
using UnityEngine.UI;

namespace Dasher
{
	public class GUIManager : MonoBehaviour
	{
		private GameProcess m_gameProcess;

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
		private Text m_endBestTimerText;

		[SerializeField]
		private Canvas failCanvas;

		[SerializeField]
		private Text m_failLevelLabelText;
		[SerializeField]
		private Text m_failTimerText;

		void Start()
		{
			m_gameProcess = GameProcess.Instance;
			m_gameProcess.registerGUIManager(this);
		}

		void OnDisable()
		{
			m_gameProcess.unregisterGUI();
		}

		public void ManualFixedUpdate()
		{
			if (gameCanvas.gameObject.activeSelf)
			{
				float time = m_gameProcess.GameTime.CurrentLevelTime;
				m_gameTimerText.text = time.ToString(TimeManager.c_timeDisplayFormat);
			}
		}

		public void PauseGame()
		{
			m_gameProcess.RequirePause();

			gameCanvas.gameObject.SetActive(false);
			pauseCanvas.gameObject.SetActive(true);
		}

		public void ResumeGame()
		{
			pauseCanvas.gameObject.SetActive(false);
			gameCanvas.gameObject.SetActive(true);
			m_gameProcess.RequireResume();

		}

		public void NotifyLevelStart()
		{
			gameCanvas.gameObject.SetActive(true);
			pauseCanvas.gameObject.SetActive(false);
			endLevelCanvas.gameObject.SetActive(false);
			failCanvas.gameObject.SetActive(false);

			string levelLabel = "Level - " + (MainProcess.Instance.CurrentLevelIndex + 1); // TODO : use string builder or something
			m_pauseLevelLabelText.text = levelLabel;
			m_endLevelLabelText.text = levelLabel;
			m_failLevelLabelText.text = levelLabel;

			m_gauge.Initialize();
		}

		public void NotifyEndLevelReached()
		{
			m_gameProcess.RequirePause();
			gameCanvas.gameObject.SetActive(false);
			endLevelCanvas.gameObject.SetActive(true);

			float time = m_gameProcess.GameTime.CurrentLevelTime;
			m_endTimerText.text = time.ToString(TimeManager.c_timeDisplayFormat);

			float bestTime = MainProcess.Instance.DataManager.GetLevelTime(GameProcess.CurrentLevelName);
			m_endBestTimerText.text = bestTime.ToString(TimeManager.c_timeDisplayFormat);
		}

		public void NotifyDeathZoneTouched()
		{
			m_gameProcess.RequirePause();
			gameCanvas.gameObject.SetActive(false);
			failCanvas.gameObject.SetActive(true);

			float time = m_gameProcess.GameTime.CurrentLevelTime;
			m_failTimerText.text = time.ToString(TimeManager.c_timeDisplayFormat);
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
	}
}
