using UnityEngine;
using UnityEngine.UI;

namespace Dasher
{
	public class SettingsMenu : MonoBehaviour {

		[SerializeField]
		private DasherToggleButton m_leftHandedMode = null;

		[SerializeField]
		private Button m_deleteSaveButton = null;

		[SerializeField]
		private GameObject m_deleteSavePopup = null;

		[SerializeField]
		private GameObject m_secretDeleteSavePopup = null;

		void Awake()
		{
			m_secretDeleteSavePopup.SetActive(false);
			m_deleteSavePopup.SetActive(false);
			enabled = false;

			MainProcess mp = MainProcess.Instance;
			if (mp != null)
			{
				DasherSettings setting = mp.DataManager.GetSettings();
				m_leftHandedMode.SetOn(setting.isLefthanded);
			}
		}

		public void OnLeftHandToggle()
		{
			var data = MainProcess.Instance.DataManager;
			data.SetLeftHandedMode(m_leftHandedMode.Toggle());
			data.Save();
		}



		public void OnDeleteSavePressed()
		{
			m_deleteSavePopup.SetActive(true);
		}

		public void DeleteSave()
		{
			var data = MainProcess.Instance.DataManager;
			data.ClearSave();
			MainProcess.Instance.RelaunchGame();
		}

		void DisplayCustomDeleteSavePopup()
		{
			m_secretDeleteSavePopup.SetActive(true);
		}

		const float c_deleteLongPressDuration = 5;
		float m_deletePressTimer = -1f;

		public void DeletePointerDown()
		{
			enabled = true;
			m_deletePressTimer = 0f;
		}
		public void DeletePointerUp()
		{
			if (enabled && m_deletePressTimer >= 0)
			{
				if (m_deletePressTimer >= c_deleteLongPressDuration)
				{
					DisplayCustomDeleteSavePopup();
				}
				else
				{
					OnDeleteSavePressed();
				}
			}
		}

		public void DeletePointerExit()
		{
			enabled = false;
			m_deletePressTimer = -1;
		}

		void Update()
		{
			m_deletePressTimer += Time.deltaTime;
			//if (m_deletePressTimer >= c_deleteLongPressDuration)
			//{
			//	DisplayCustomDeleteSavePopup();
			//}
		}

		public void CancelDeleteSave()
		{
			m_deleteSavePopup.SetActive(false);
		}
	}
}
