using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace Dasher
{
	public class MainMenuScript : MonoBehaviour
	{
		private const string BASE_LEVEL_LABEL = "Level - ";

		private const string c_a_select = "LevelSelect";
		private const string c_a_info = "LevelInfo";
		[SerializeField]
		private Animator m_menuAnimator = null;

		void Awake()
		{
			InitStates();
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

		#endregion
		#endregion

		#endregion
	}
}
