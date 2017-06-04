using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Dasher
{
	[RequireComponent(typeof(Text))]
	public class SimpleTextLocalizer : MonoBehaviour
	{
		[SerializeField]
		int m_locaKey;

		void Start()
		{
			SetupLoca();
		}

		public void SetupLoca()
		{
			if (MainProcess.Instance == null)
				return;
			var loca = MainProcess.Instance.Localization;
			Text text = GetComponent<Text>();
			text.text = loca.GetText(m_locaKey);
		}

	}
}