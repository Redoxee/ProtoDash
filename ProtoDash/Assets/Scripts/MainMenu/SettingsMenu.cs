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
		private GameObject m_deletSavePopup = null;

		void Awake()
		{
			m_deletSavePopup.SetActive(false);
			enabled = false;

			DasherSettings setting = MainProcess.Instance.DataManager.GetSettings();
			m_leftHandedMode.SetOn(setting.isLefthanded);

			
		}

		public void OnLeftHandToggle()
		{
			var data = MainProcess.Instance.DataManager;
			data.SetLeftHandedMode(m_leftHandedMode.Toggle());
			data.Save();
		}



		public void OnDeleteSavePressed()
		{
			var data = MainProcess.Instance.DataManager;
			data.ClearSave();
			MainProcess.Instance.RelaunchGame();
		}

		void DisplayCustomDeleteSavePopup()
		{
			m_deletSavePopup.SetActive(true);
		}

		const float c_deleteLongPressDuration = 10f;
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
	}
}
