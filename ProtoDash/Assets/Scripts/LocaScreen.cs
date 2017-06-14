using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace Dasher
{
	public class LocaScreen : MonoBehaviour
	{

		[SerializeField]
		Text m_homeButtonText = null;

		List<LocaScreenElements> m_items = new List<LocaScreenElements>();

		public void RegisterItem(LocaScreenElements item)
		{
			if (!m_items.Contains(item))
			{
				m_items.Add(item);
			}
		}

		private void Start()
		{
			RefreshItems();
		}

		public void OnLocaPressed(LocaLanguage lang)
		{

			var currentLoca = MainProcess.Instance.Localization.CurrentLoca;
			if (currentLoca == lang)
				return;

			MainProcess.Instance.Localization.CurrentLoca = lang;
			MainProcess.Instance.DataManager.CurrentLanguage = lang;
			RefreshItems();
		}

		public void RefreshItems()
		{

			var currentLoca = MainProcess.Instance.Localization.CurrentLoca;

			foreach (LocaScreenElements items in m_items)
			{
				items.Refresh(currentLoca);
			}

			m_homeButtonText.text = MainProcess.Instance.Localization.GetText(39);
		}

		public void OnHomePressed()
		{
			MainProcess.Instance.RequestSwitchToHome();
		}
	}
}