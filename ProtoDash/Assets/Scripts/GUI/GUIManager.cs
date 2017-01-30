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
		private GameCanvasHolder m_canvasHolder = null;


		private GameObject m_gameCanvas;
		private Text m_gameTimerText;
		private SmartGauge m_gauge;
		private GameObject m_pauseButton = null;
		private Rect m_pauseRect;

		[Space]
		[Header("Pause")]
		[SerializeField]
		private GameObject m_pauseCanvas = null;
		[SerializeField]
		private Text m_pauseLevelLabelText = null;
		[SerializeField]
		private Text m_pauseCurrentTimeText = null;
		[SerializeField]
		private Text m_pauseParTimeText = null;

		[Space]
		[Header("End Level")]
		[SerializeField]
		private GameObject m_endCanvas = null;
		[SerializeField]
		private Text m_endLevelLabelText = null;
		[SerializeField]
		EndLevelGUI m_endLevelGUI = null;
		[SerializeField]
		Color m_endLevelBackGround = new Color(0.006896503f, 0.0045977007f, 0.2735632f, a: 0.9411765f);
		public Color EndLevelBackground { get { return m_endLevelBackGround; } }
		
		[Space]
		[Header("Death")]
		[SerializeField]
		private GameObject m_failCanvas = null;
		[SerializeField]
		private Text m_failLevelLabelText = null;

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
			m_pauseButton = m_gameCanvas.GetComponentInChildren<Button>().gameObject;

			Vector2 ScreenSize = new Vector2(Screen.width, Screen.height);
			RectTransform pTransform = (m_pauseButton.transform as RectTransform);
			Rect buttonRect = pTransform.rect;
			
			float originx = ScreenSize.x * pTransform.anchorMax.x - pTransform.pivot.x * buttonRect.width  + pTransform.anchoredPosition.x;
			float originy = ScreenSize.y * pTransform.anchorMax.y - pTransform.pivot.y * buttonRect.height + pTransform.anchoredPosition.y;

			m_pauseRect = new Rect(originx,originy,buttonRect.width,buttonRect.height);

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
#if !DASHER_DEMO
			m_gameCanvas.gameObject.SetActive(true);

			//GameProcess.Instance.CurrentCharacter.InputManager.RegisterButton(m_pauseButton.transform as RectTransform);
			GameProcess.Instance.CurrentCharacter.InputManager.RegisterButton(m_pauseRect);
#endif
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
		float m_currentTime, m_champTime;

		public void NotifyEndLevelReached(bool isFirstTime, bool isNewBestTime, bool isNewparTime,float oldBestTime)
		{
			m_perf.StopRecord();
			m_debugFPS.text = m_perf.GetMeanFPS().ToString();
			m_gameProcess.RequirePause();
			m_gameCanvas.gameObject.SetActive(false);
			m_endCanvas.gameObject.SetActive(true);

			LevelData currentLevel = MainProcess.Instance.levelFlow.GetLevelData(GameProcess.CurrentLevelName);

			m_currentTime = m_gameProcess.GameTime.CurrentLevelTime;

			m_champTime = currentLevel.parTime;

			m_endLevelGUI.m_current.SetMainText("Time\n" + m_currentTime.ToString(TimeManager.c_timeDisplayFormat));
			m_endLevelGUI.m_best.SetMainText("Best\n" + oldBestTime.ToString(TimeManager.c_timeDisplayFormat));
			m_endLevelGUI.m_champ.SetMainText("Champ\n" + m_champTime.ToString(TimeManager.c_timeDisplayFormat));

			var bestTimeDifference = m_currentTime - oldBestTime ;
			var isChampTime = m_currentTime <= currentLevel.parTime;

			if (!isFirstTime)
			{
				m_endLevelGUI.m_best.SetAdditionalText(bestTimeDifference.ToString(TimeManager.c_diffDisplayFormat));
			}
			else
			{
				m_endLevelGUI.m_best.SetAdditionalText("New Best");
				m_endLevelGUI.m_best.SetMainText(m_currentTime.ToString(TimeManager.c_timeDisplayFormat));

				m_endLevelGUI.m_current.SetAdditionalText("First time!");
				m_endLevelGUI.m_current.SetMainText("Finished!");

				m_endLevelGUI.m_current.SetBackBorderState(TimeDisplayCapsule.CapsuleSuccessState.Good);
				m_endLevelGUI.m_best.SetBackBorderState(TimeDisplayCapsule.CapsuleSuccessState.Good);
			}

			if (!isFirstTime)
			{
				if (isNewBestTime)
				{
					m_endLevelGUI.m_current.SetAdditionalText("New Best!");
					m_endLevelGUI.m_current.SetBackBorderState(TimeDisplayCapsule.CapsuleSuccessState.Good);
					m_endLevelGUI.m_best.SetBackBorderState(TimeDisplayCapsule.CapsuleSuccessState.Good);
				}
				else if (bestTimeDifference > 0)
				{
					m_endLevelGUI.m_current.SetBackBorderState(TimeDisplayCapsule.CapsuleSuccessState.Neutral);
					m_endLevelGUI.m_best.SetBackBorderState(TimeDisplayCapsule.CapsuleSuccessState.Bad);
				}
				else
				{
					m_endLevelGUI.m_current.SetBackBorderState(TimeDisplayCapsule.CapsuleSuccessState.Neutral);
					m_endLevelGUI.m_best.SetBackBorderState(TimeDisplayCapsule.CapsuleSuccessState.Neutral);
				}
			}

			m_endLevelGUI.m_champ.SetAdditionalText("You're a Champ!");
			if (isChampTime)
			{
				m_endLevelGUI.m_champ.SetBackBorderState(TimeDisplayCapsule.CapsuleSuccessState.Good);
			}
			else
			{
				m_endLevelGUI.m_champ.SetBackBorderState(TimeDisplayCapsule.CapsuleSuccessState.Neutral);
			}

			var displayChampTime = isChampTime || (oldBestTime < currentLevel.parTime);

			SetEndLevelEvents(isFirstTime,isNewBestTime, displayChampTime);
			m_isEndLevel = true;
		}

		void SetEndLevelEvents(bool isFirstTime, bool isNewBestTime, bool displayChampTime)
		{

			var ts = .01f;
			var timeGap = .25f;
			var sg = .03f;

			m_endLevelEvents = new Dictionary<float, Action>();
			m_endLevelEvents[ts] = () => {
				m_endLevelGUI.m_current.StartFlash();
			};
			ts += timeGap;

			m_endLevelEvents[ts] = () => {
				m_endLevelGUI.m_best.StartFlash();
			};
			ts += timeGap;

			m_endLevelEvents[ts] = () => {
				m_endLevelGUI.m_champ.StartFlash();
			};
			ts += timeGap * 2f;
			if (isNewBestTime)
			{
				m_endLevelEvents[ts - sg] = () =>
				{
					if (m_endLevelGUI.m_current.CurrentState == TimeDisplayCapsule.CapsuleSuccessState.Good)
					{
						m_endLevelGUI.m_current.StartSplash();
					}
				};

				m_endLevelEvents[ts] = () => {
					m_endLevelGUI.m_current.StartSlide();
				};
				ts += timeGap;
			}
			
			m_endLevelEvents[ts - sg] = () =>
			{
				if (m_endLevelGUI.m_best.CurrentState == TimeDisplayCapsule.CapsuleSuccessState.Good)
				{
					m_endLevelGUI.m_best.StartSplash();
				}
			};

			m_endLevelEvents[ts] = () =>
			{
				m_endLevelGUI.m_best.StartSlide();
			};
			ts += timeGap;

			if (displayChampTime)
			{
				m_endLevelEvents[ts - sg] = () =>
				{
					if (m_endLevelGUI.m_champ.CurrentState == TimeDisplayCapsule.CapsuleSuccessState.Good)
					{
						m_endLevelGUI.m_champ.StartSplash();
					}
				};
				m_endLevelEvents[ts] = () =>
				{
					m_endLevelGUI.m_champ.StartSlide();
				};
				ts += timeGap;
			}

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

		public void GoToLevelSelect()
		{
			MainProcess.Instance.RequestSwitchToLevelSelect();
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
