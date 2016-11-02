using UnityEngine;
using UnityEngine.UI;

namespace Dasher
{
	public class SettingsMenu : MonoBehaviour {

		[SerializeField]
		private DasherToggleButton m_leftHandedMode = null;

		void Awake()
		{
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
	}
}
