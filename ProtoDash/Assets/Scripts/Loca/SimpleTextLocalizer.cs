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
		int m_locaKey = -1;

		public int LocaKey { get{ return m_locaKey; } set { m_locaKey = value; } }

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