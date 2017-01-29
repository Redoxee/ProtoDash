using UnityEngine;
using UnityEngine.UI;

using System.Text.RegularExpressions;

namespace Dasher
{
	public class ResetSavePopup : MonoBehaviour
	{
		public InputField m_customId = null;

		public void OnConfirm()
		{
			var customId = m_customId.text;
			if (customId != null)
			{
				customId = Regex.Replace(customId, "[^\\w\\._]", "");
				customId = customId.Trim();
			}
			if (customId != null && customId != "")
			{
				SaveManager.PrefixId = customId;
			}

			var data = MainProcess.Instance.DataManager;
			data.ClearSave();
			MainProcess.Instance.RelaunchGame();
			gameObject.SetActive(false);
		}

		public void OnCancel()
		{
			gameObject.SetActive(false);
		}
	}
}
