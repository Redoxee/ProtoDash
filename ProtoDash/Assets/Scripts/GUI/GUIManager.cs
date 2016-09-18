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
		private Canvas m_gameCanvas;
		[SerializeField]
		private Text m_gameTimerText;

		[SerializeField]
		private SmartGauge m_gauge;

		[SerializeField]
		private Canvas m_pauseCanvas;

		[SerializeField]
		private Text m_pauseLevelLabelText;

		[SerializeField]
		private Canvas m_endCanvas;

		[SerializeField]
		private Text m_endLevelLabelText;
		[SerializeField]
		private Text m_endTimerText;
		[SerializeField]
		private Text m_endBestTimerText;

		[SerializeField]
		private Canvas m_failCanvas;

		[SerializeField]
		private Text m_failLevelLabelText;
		[SerializeField]
		private Text m_failTimerText;

		[SerializeField]
		private Canvas m_introCanvas = null;
		//[SerializeField]
		//private Text m_introText = null;


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
			if (m_gameCanvas.gameObject.activeSelf)
			{
				float time = m_gameProcess.GameTime.CurrentLevelTime;
				m_gameTimerText.text = time.ToString(TimeManager.c_timeDisplayFormat);
			}
		}

		public void PauseGame()
		{
			m_gameProcess.RequirePause();

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

			string levelLabel = "Level - " + (MainProcess.Instance.CurrentLevelIndex + 1); // TODO : use string builder or something
			m_pauseLevelLabelText.text = levelLabel;
			m_endLevelLabelText.text = levelLabel;
			m_failLevelLabelText.text = levelLabel;

			m_gauge.Initialize();
		}

		public void NotifyEndLevelReached()
		{
			m_gameProcess.RequirePause();
			m_gameCanvas.gameObject.SetActive(false);
			m_endCanvas.gameObject.SetActive(true);

			float time = m_gameProcess.GameTime.CurrentLevelTime;
			m_endTimerText.text = time.ToString(TimeManager.c_timeDisplayFormat);

			float bestTime = MainProcess.Instance.DataManager.GetLevelTime(GameProcess.CurrentLevelName);
			m_endBestTimerText.text = bestTime.ToString(TimeManager.c_timeDisplayFormat);
		}

		public void NotifyDeathZoneTouched()
		{
			m_gameProcess.RequirePause();
			m_gameCanvas.gameObject.SetActive(false);
			m_failCanvas.gameObject.SetActive(true);

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
