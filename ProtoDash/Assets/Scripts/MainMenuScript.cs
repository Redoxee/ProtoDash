using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace Dasher
{
	public class MainMenuScript : MonoBehaviour
	{

		private static int c_a_select = Animator.StringToHash("LevelSelect");
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
			public bool m_canGoSettings;

			public FSM_State(D_FSMCallback begin = null, D_FSMCallback end = null, bool canGoSettings = false)
			{
				m_beging = begin;
				m_end = end;
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
			Intro_levelPressed();
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
		private GameObject m_worldPrefab = null;
		[SerializeField]
		private Material m_levelChampMaterial = null;

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
			var dataManager = mp.DataManager;
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

					bool isUnlocked = true;
#if !DASHER_DEMO
					if (dataManager != null)
					{
						isUnlocked = dataManager.DoesProgressionAllowLevel(levels[levelIndex].sceneName);
						if (levelFlow.IsLevelChamp(levels[levelIndex].sceneName))
						{
							Image border = btnObject.GetComponent<Image>();
							border.material = m_levelChampMaterial;
						}
					}
#endif
					if (isUnlocked)
					{
						var disableImage = btnObject.GetComponentInChildren<LevelListButton>();
						disableImage.DisableImage.SetActive(false);
					}
				}
			}

			ScrollRect scrollRect = m_worldsParent.GetComponentInParent<ScrollRect>();
			scrollRect.verticalNormalizedPosition = 0;

			string currentLevel = mp.DataManager.LastLevelPlayed;
			if (currentLevel == null)
			{
				currentLevel = levelFlow.LevelList[0].sceneName;
			}

			var cl = levelFlow.GetWorldAndRankPosition(currentLevel);
			QuickStartLevelDisplay.text = string.Format(c_levelLabelPattern, cl.Key, cl.Value);

			m_lightLevelButton.Initialize();
			OnLevelPressed(structuredLevels[1][0], string.Format(c_levelLabelPattern, 1, 1));

			m_isinitialized = true;
		}

		void Update()
		{
			if (!m_isinitialized)
			{
				InitializeWorlds();
			}
			m_lightLevelButton.ManualUpdate();
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
			m_introState = new FSM_State(null, null,true);
			m_levelSelectState = new FSM_State(null, null);
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

		[SerializeField]
		LevelDescriptionButton m_lightLevelButton = null;

		private LevelData m_currentLevelDisplayed = null;

		public void OnLevelPressed(LevelData lvl, string levelName = "0-0")
		{
			//if (m_currentLevelDisplayed != lvl)
			{
				var dataManager = MainProcess.Instance.DataManager;
#if !DASHER_DEMO
				if (dataManager.DoesProgressionAllowLevel(lvl.sceneName))
#endif
				{
					m_currentLevelDisplayed = lvl;
					string title = string.Format("PLAY : {0}", levelName);
					var bestTime = (Mathf.Max(lvl.currentBest, 0f)).ToString(TimeManager.c_timeDisplayFormat);
					var champTime = (Mathf.Max(lvl.parTime, 0f)).ToString(TimeManager.c_timeDisplayFormat);
					string best = string.Format("best\n{0}", bestTime);
					string champ = string.Format("champ\n{0}", champTime);
					m_lightLevelButton.FlashInInfo(title, best, champ);
				}
			}
		}

		public void NotifyLevelBackPressed()
		{
			m_menuAnimator.SetBool(c_a_select, false);
			SetState(m_introState);
		}
		
		public void OnLevelPlayPressed()
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

		public void OnCreditsPressed()
		{
			MainProcess.Instance.RequestSwitchToCredits();
		}

#endregion
#endregion

#endregion
	}
}
