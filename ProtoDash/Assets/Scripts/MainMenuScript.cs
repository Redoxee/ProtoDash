using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace Dasher
{
	public class MainMenuScript : MonoBehaviour
	{

		private static int c_a_select = Animator.StringToHash("LevelSelect");
		private static int c_a_settings = Animator.StringToHash("SettingsMenu");
		private static int c_a_instantLevelSelect = Animator.StringToHash("InstantLevelSelect");
		[SerializeField]
		private Animator m_menuAnimator = null;

		void Awake()
		{
			InitStates();
			Initialize();
			InitColors();
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
		private Material m_levelNormalMaterial = null;
		[SerializeField]
		private Material m_levelChampMaterial = null;

		public const string c_levelLabelPattern = "{0}-{1}";

		#region Initialization

		private bool m_isinitialized = false;
		private void Initialize()
		{
			if (m_isinitialized || MainProcess.Instance == null)
				return;

			MainProcess mp = MainProcess.Instance;
			mp.RegisterMainMenu(this);

			var levelFlow = mp.levelFlow;
			var dataManager = mp.DataManager;
			var structuredLevels = levelFlow.GetStructuredProgression();
			var worldEnumerator = structuredLevels.GetEnumerator();

			#region World creation
			while (worldEnumerator.MoveNext())
			{
				GameObject worldObject = Instantiate<GameObject>(m_worldPrefab);
				worldObject.transform.SetParent(m_worldsParent, false);
				WorldHolder wh = worldObject.GetComponent<WorldHolder>();

				List<LevelData> levels = worldEnumerator.Current.Value;
				for (int lvl = 0; lvl < levels.Count; ++lvl)
				{
					GameObject btnObject = wh.AddLevelButton();
					var objectDescriptor = btnObject.GetComponentInChildren<LevelListButton>();
					string levelLabel = string.Format(c_levelLabelPattern, worldEnumerator.Current.Key, lvl + 1);

					objectDescriptor.MainLabel.text = string.Format(levelLabel);

					Button levelButton = btnObject.GetComponent<Button>();
					int levelIndex = lvl;
					levelButton.onClick.AddListener(() => { OnLevelPressed(levels[levelIndex], levelLabel); });

					Image buttonImage = btnObject.GetComponent<Image>();

					bool isUnlocked = true;
#if !DASHER_DEMO
					if (dataManager != null)
					{
						isUnlocked = dataManager.DoesProgressionAllowLevel(levels[levelIndex].sceneName);
						if (levelFlow.IsLevelChamp(levels[levelIndex].sceneName))
						{
							Image border = btnObject.GetComponent<Image>();
							border.material = m_levelChampMaterial;
							objectDescriptor.ChampLabel.gameObject.SetActive(true);
						}
						else
						{
							objectDescriptor.ChampLabel.gameObject.SetActive(false);
						}
					}
#endif
					if (isUnlocked)
					{
						objectDescriptor.DisableImage.SetActive(false);
					}
				}
			}
			#endregion

			ScrollRect scrollRect = m_worldsParent.GetComponentInParent<ScrollRect>();
			scrollRect.verticalNormalizedPosition = 0;

			var level = levelFlow.GetMostInterestingLevel();
			string levelName = level.sceneName;

			
			QuickStartLevelDisplay.text = string.Format(c_levelLabelPattern, level.world + 1, level.indexInWorld + 1);

			m_lightLevelButton.Initialize();
			OnLevelPressed(level, string.Format(c_levelLabelPattern, level.world + 1, level.indexInWorld + 1));

			InitColors();

			m_isinitialized = true;
		}
		
		#region Colors
		void InitColors()
		{
			MainProcess mp = MainProcess.Instance;
			if (mp == null)
				return;
			LevelFlow lf = mp.levelFlow;
			SaveManager sm = mp.DataManager;

			var worldIndex = lf.GetMostInterestingLevel().world;
			var dresser = mp.WorldDresser;
			var dress = dresser.GetDressForWorld(worldIndex);
			dress.ColorSetter.ApplyColors();
		}

		#endregion
		#endregion

		void Update()
		{
			if (!m_isinitialized)
			{
				Initialize();
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

					if (lvl.IsLevelChamp)
					{
						m_lightLevelButton.SetMainMaterial(m_levelChampMaterial);
					}
					else {
						m_lightLevelButton.SetMainMaterial(m_levelNormalMaterial);
					}
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

		#region Shortcuts
		public void InstantLevelSelection()
		{
			m_menuAnimator.SetTrigger(c_a_instantLevelSelect);
			m_menuAnimator.SetBool(c_a_select, true);
			SetState(m_levelSelectState);
		}
		#endregion

		#region Feedbacks

		public void FeedbackRequest()
		{
			MainProcess.SimpleFeedback();
		}

		#endregion

		#endregion
	}
}
