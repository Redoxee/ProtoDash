using UnityEngine;
using UnityEngine.UI;

namespace Dasher
{
	public class SettingsMenu : MonoBehaviour {

		[SerializeField]
		private Toggle m_leftHandedMode = null;

		void Awake()
		{
			DasherSettings setting = MainProcess.Instance.DataManager.GetSettings();

			m_leftHandedMode.isOn = setting.isLefthanded;
			m_leftHandedMode.onValueChanged.AddListener(OnLeftHandedChanged);
		}

		void OnLeftHandedChanged(bool isOn)
		{
			var data = MainProcess.Instance.DataManager;
			data.SetLeftHandedMode(isOn);
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
