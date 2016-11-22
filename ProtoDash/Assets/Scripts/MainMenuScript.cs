﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace Dasher
{
	public class MainMenuScript : MonoBehaviour
	{

		private static int c_a_select = Animator.StringToHash("LevelSelect");
		private static int c_a_info = Animator.StringToHash("LevelInfo");
		private static int c_a_settings = Animator.StringToHash("SettingsMenu");
		[SerializeField]
		private Animator m_menuAnimator = null;

		void Awake()
		{
			InitStates();
			InitializeWorlds();
		}

		public void StartLevel(int levelIndex)
		{
			MainProcess.Instance.RequestLevelLaunch(levelIndex);
		}

		#region FSM
		delegate void D_FSMCallback();
		struct FSM_State
		{
			public D_FSMCallback m_beging;
			public D_FSMCallback m_end;
			public D_FSMCallback m_lvlButtonCallback;
			public bool m_canGoSettings;

			public FSM_State(D_FSMCallback begin = null, D_FSMCallback end = null, D_FSMCallback lvl = null, bool canGoSettings = false)
			{
				m_beging = begin;
				m_end = end;
				m_lvlButtonCallback = lvl;
				m_canGoSettings = canGoSettings;
			}
		}

		private FSM_State m_currentState;

		private void SetState(FSM_State newState)
		{
			if (m_currentState.m_end != null)
				m_currentState.m_end();
			m_currentState = newState;
			if (m_currentState.m_beging != null)
				m_currentState.m_beging();
		}

		public void LevelButtonPressed()
		{
			if (m_currentState.m_lvlButtonCallback != null)
				m_currentState.m_lvlButtonCallback();
		}

		public void SettingsPressed()
		{
			if(m_currentState.m_canGoSettings)
				SetState(m_setingsState);
		}

		public void SettingsClosePressed()
		{
			SetState(m_introState);
		}

		#region Worlds
		[SerializeField]
		private Transform m_worldsParent = null;
		[SerializeField]
		private GameObject m_worldPrefab;

		public const string c_levelLabelPattern = "{0}-{1}";

		private bool m_isinitialized = false;
		private void InitializeWorlds()
		{
			if (m_isinitialized || MainProcess.Instance == null)
				return;

			MainProcess mp = MainProcess.Instance;
			mp.RegisterMainMenu(this);

			Material lightButtonMaterial = mp.m_colorScheme.LightButtond_Material;
			var levelFlow = mp.levelFlow;
			var structuredLevels = levelFlow.GetStructuredProgression();
			var worldEnumerator = structuredLevels.GetEnumerator();

			while (worldEnumerator.MoveNext())
			{
				GameObject worldObject = Instantiate<GameObject>(m_worldPrefab);
				worldObject.transform.SetParent(m_worldsParent, false);
				WorldHolder wh = worldObject.GetComponent<WorldHolder>();

				List<LevelData> levels = worldEnumerator.Current.Value;
				for (int lvl = 0; lvl < levels.Count; ++lvl)
				{
					GameObject btnObject = wh.AddLevelButton();

					string levelLabel = string.Format(c_levelLabelPattern, worldEnumerator.Current.Key, lvl + 1);
					Text labelObject = btnObject.GetComponentInChildren<Text>();

					labelObject.text = string.Format(levelLabel);

					Button levelButton = btnObject.GetComponent<Button>();
					int levelIndex = lvl;
					levelButton.onClick.AddListener(() => { OnLevelPressed(levels[levelIndex], levelLabel); });

					Image buttonImage = btnObject.GetComponent<Image>();
					buttonImage.material = lightButtonMaterial;
				}
			}

			Button playButton = m_levelInfo.PlayButton.GetComponent<Button>();
			playButton.onClick.RemoveAllListeners();
			playButton.onClick.AddListener(() => { OnLevelPlayPressed(); });

			ScrollRect scrollRect = m_worldsParent.GetComponentInParent<ScrollRect>();
			scrollRect.verticalNormalizedPosition = 0;

			string currentLevel = mp.DataManager.LastLevelPlayed;
			if (currentLevel == null)
			{
				currentLevel = levelFlow.LevelList[0].sceneName;
			}

			var cl = levelFlow.GetWorldAndRankPosition(currentLevel);
			QuickStartLevelDisplay.text = string.Format(c_levelLabelPattern, cl.Key, cl.Value);

			m_isinitialized = true;
		}

		void Update()
		{
			if (!m_isinitialized)
			{
				InitializeWorlds();
			}
		}

		void OnDisable()
		{
			m_isinitialized = false;
			MainProcess.Instance.UnregisterMainMenu();
		}

		#endregion

		#region States
		private void InitStates()
		{
			m_introState = new FSM_State(null, null, Intro_levelPressed,true);
			m_levelSelectState = new FSM_State(null, null, LvlSelect_levelPressed);
			m_setingsState = new FSM_State(BeginSettings, EndSettings);
			SetState(m_introState);
		}

		#region Intro
		[SerializeField]
		private Text QuickStartLevelDisplay = null;

		public void OnQuickStartPressed()
		{
			MainProcess mp = MainProcess.Instance;
			SaveManager save = mp.DataManager;
			string levelToStart = save.LastLevelPlayed;
			int levelIndex = 0;
			if (levelToStart != null)
			{
				levelIndex = mp.levelFlow.GetLevelIndex(levelToStart);
			}
			StartLevel(levelIndex);
		}

		FSM_State m_introState;
		private void Intro_levelPressed()
		{
			m_menuAnimator.SetBool(c_a_select, true);
			SetState(m_levelSelectState);
		}

		public void OnStatsPressed()
		{
			MainProcess.Instance.RequestSwitchToStats();
		}

		#endregion
		#region LevelSelect
		FSM_State m_levelSelectState;
		private void LvlSelect_levelPressed()
		{
			m_menuAnimator.SetBool(c_a_select, false);
			SetState(m_introState);
		}


		[SerializeField]
		LevelInfoHolder m_levelInfo = null;
		
		private LevelData m_currentLevelDisplayed = null;
		private LevelData m_levelToDisplay = null;
		private string m_levelNameToDisplay = null;
		public void OnLevelPressed(LevelData lvl, string levelName = "0-0")
		{
			if (m_currentLevelDisplayed == null)
			{
				m_menuAnimator.SetBool(c_a_info, true);
				SetUpLevelInfo(lvl, levelName);
			}
			else if (m_currentLevelDisplayed == lvl)
			{
				m_menuAnimator.SetBool(c_a_info, false);
				m_currentLevelDisplayed = null;
			}
			else
			{
				m_menuAnimator.SetBool(c_a_info, false);
				m_levelToDisplay = lvl;
				m_levelNameToDisplay = levelName;
			}
		}

		public void NotifyFromLevelInfoAnimationEnded()
		{
			if (m_levelToDisplay != null)
			{
				SetUpLevelInfo(m_levelToDisplay, m_levelNameToDisplay);
				m_levelToDisplay = null;
				m_menuAnimator.SetBool(c_a_info, true);
			}
		}

		const string c_bestTimePattern = "Best : {0}";
		const string c_parTimePattern = "Champ : {0}";

		private void SetUpLevelInfo(LevelData lvl, string levelName)
		{
			m_currentLevelDisplayed = lvl;

			m_levelInfo.LevelTitle.text = levelName;

			m_levelInfo.ParTime.text = string.Format(c_parTimePattern, lvl.parTime.ToString(TimeManager.c_timeDisplayFormat));

			m_levelInfo.BestTime.text = string.Format(c_bestTimePattern, (Mathf.Max(lvl.currentBest,0f)).ToString(TimeManager.c_timeDisplayFormat));

		}

		private void OnLevelPlayPressed()
		{
			if (m_currentLevelDisplayed != null)
			{
				MainProcess.Instance.RequestLevelLaunch(m_currentLevelDisplayed);
			}
		}

		#endregion

		#region Settings
		FSM_State m_setingsState;

		private void BeginSettings()
		{
			m_menuAnimator.SetBool(c_a_settings, true);
		}

		private void EndSettings()
		{
			m_menuAnimator.SetBool(c_a_settings, false);
		}

		#endregion
		#endregion

		#endregion
	}
}
