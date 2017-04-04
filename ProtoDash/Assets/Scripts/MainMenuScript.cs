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

		public void OnLevelWallPressed()
		{
			ShopManager.Instance.PurchaseMainQuest(OnMainQuestPurchased);
			//m_levelWallAnimator.StartAnimation(UnlockNextStoryLevelIfNeeded);
		}

		void OnMainQuestPurchased()
		{
			m_levelWallAnimator.StartAnimation(UnlockNextStoryLevelIfNeeded);
		}

		#region Worlds
		[SerializeField]
		private Transform m_worldsParent = null;
		[SerializeField]
		private GameObject m_worldPrefab = null;
		[SerializeField]
		private GameObject m_storyWallPrefab = null;
		[SerializeField]
		private Material m_levelNormalMaterial = null;
		[SerializeField]
		private Material m_levelChampMaterial = null;

		public const string c_levelLabelPattern = "{0}-{1}";


		public LevelWallAnimator m_levelWallAnimator;
		#region Initialization

		private bool m_isinitialized = false;

		private void Initialize()
		{
			if (m_isinitialized || MainProcess.Instance == null)
				return;

			MainProcess mp = MainProcess.Instance;
			mp.RegisterMainMenu(this);

			var levelFlow = mp.levelFlow;

			PopulateWorld(mp);

			StartCoroutine(ScrollWorldTo(1));

			var level = levelFlow.GetMostInterestingLevel();

			string currentLevelLabel = level.GetLevelLabel();

			QuickStartLevelDisplay.text = currentLevelLabel;

			m_lightLevelButton.Initialize();
			OnLevelPressed(level, currentLevelLabel);

			InitColors();
			m_isinitialized = true;
		}

		System.Collections.IEnumerator ScrollWorldTo(float y)
		{
			yield return new WaitForEndOfFrame();
			ScrollRect scrollRect = m_worldsParent.GetComponentInParent<ScrollRect>();
			scrollRect.verticalNormalizedPosition = y;
		}

		List<List<LevelListButton>> m_storyLevelButtons;

		void PopulateWorld(MainProcess mp)
		{

			var levelFlow = mp.levelFlow;
			var dataManager = mp.DataManager;
			var structuredLevels = levelFlow.GetStructuredProgression();
			var worldEnumerator = structuredLevels.GetEnumerator();

			var shopManager = mp.ShopManager;
			var wallIndex = ShopManager.c_storyBlockade;

			#region World creation

			m_storyLevelButtons = new List<List<LevelListButton>>();
			foreach (var currentWorld in structuredLevels)
			{
				GameObject worldObject = Instantiate<GameObject>(m_worldPrefab);
				worldObject.transform.SetParent(m_worldsParent, false);
				WorldHolder wh = worldObject.GetComponent<WorldHolder>();

				var buttonList = new List<LevelListButton>();
				m_storyLevelButtons.Add(buttonList); 

				var currentWorldIndex = currentWorld.Key;

				List<LevelData> levels = currentWorld.Value;
				for (int levelIndex = 0; levelIndex < levels.Count; ++levelIndex)
				{
					var lvlData = levels[levelIndex];
					GameObject btnObject = wh.AddLevelButton();
					var levelButtonDesc = btnObject.GetComponentInChildren<LevelListButton>();
					buttonList.Add(levelButtonDesc);
					string levelLabel = lvlData.GetLevelLabel();

					levelButtonDesc.bindedLevel = lvlData;
					levelButtonDesc.MainLabel.text = string.Format(levelLabel);

					Button levelButton = btnObject.GetComponent<Button>();
					var capturedIndex = levelIndex;
					levelButton.onClick.AddListener(() => { OnLevelPressed(lvlData, levelLabel); });
					
					bool isUnlocked = true;
#if !DASHER_DEMO
					if (dataManager != null)
					{
						isUnlocked = dataManager.DoesProgressionAllowLevel(lvlData.sceneName);
						if (levelFlow.IsLevelChamp(lvlData.sceneName))
						{
							Image border = btnObject.GetComponent<Image>();
							border.material = m_levelChampMaterial;
							levelButtonDesc.ChampLabel.gameObject.SetActive(true);
						}
						else
						{
							levelButtonDesc.ChampLabel.gameObject.SetActive(false);
						}
					}
#endif
					if (isUnlocked)
					{
						levelButtonDesc.DisableImage.SetActive(false);
					}
				}

				if (currentWorldIndex -1 == wallIndex && !dataManager.IsMainStoryUnlocked)
				{
					var wallInstance = Instantiate(m_storyWallPrefab);
					wallInstance.transform.SetParent(m_worldsParent, false);
					Button wallButton = wallInstance.GetComponentInChildren<Button>();
					wallButton.onClick.AddListener(OnLevelWallPressed);
					m_levelWallAnimator = wallInstance.GetComponent<LevelWallAnimator>();
				}
			}
			#endregion
		}

		void UnlockNextStoryLevelIfNeeded()
		{
			ShopManager sm = ShopManager.Instance;
			var pervLevel = m_storyLevelButtons[ShopManager.c_storyBlockade][m_storyLevelButtons[ShopManager.c_storyBlockade].Count - 1];
			var nextLevel = m_storyLevelButtons[ShopManager.c_storyBlockade + 1][0];
			LevelFlow levelFlow = MainProcess.Instance.levelFlow;
			if (pervLevel.bindedLevel.HasBeenFinished)
			{
				nextLevel.DisableImage.SetActive(false);
			}
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

			if (m_levelWallAnimator != null && m_levelWallAnimator.IsAnimating)
			{
				m_levelWallAnimator.UpdateAnimation();
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
