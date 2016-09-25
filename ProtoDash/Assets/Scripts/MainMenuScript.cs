using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace Dasher
{
	public class MainMenuScript : MonoBehaviour
	{

		private const string c_a_select = "LevelSelect";
		private const string c_a_info = "LevelInfo";
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

			public FSM_State(D_FSMCallback begin = null, D_FSMCallback end = null, D_FSMCallback lvl = null)
			{
				m_beging = begin;
				m_end = end;
				m_lvlButtonCallback = lvl;
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

			var structuredLevels = mp.levelFlow.GetStructuredProgression();
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
			m_introState = new FSM_State(null, null, Intro_levelPressed);
			m_levelSelectState = new FSM_State(null, null, LvlSelect_levelPressed);
			SetState(m_introState);
		}

		#region Intro

		FSM_State m_introState;
		private void Intro_levelPressed()
		{
			m_menuAnimator.SetBool(c_a_select, true);
			SetState(m_levelSelectState);
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

			m_levelInfo.BestTime.text = string.Format(c_bestTimePattern, lvl.currentBest.ToString(TimeManager.c_timeDisplayFormat));

		}

		private void OnLevelPlayPressed()
		{
			if (m_currentLevelDisplayed != null)
			{
				MainProcess.Instance.RequestLevelLaunch(m_currentLevelDisplayed);
			}
		}

		#endregion
		#endregion

		#endregion
	}
}
