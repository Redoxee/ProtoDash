using UnityEngine.UI;
using UnityEngine;

namespace Dasher
{
	public class LocaScreenElements : MonoBehaviour
	{
		[SerializeField]
		Image m_border = null;
		[SerializeField]
		Color m_on = Color.white;
		[SerializeField]
		Color m_off = Color.black;
		[SerializeField]
		LocaLanguage m_loca = LocaLanguage.English;

		[SerializeField]
		LocaScreen m_locaScreenRef = null;
		[SerializeField]
		AnimationFlashing m_flash = null;


		private void Awake()
		{
			m_locaScreenRef.RegisterItem(this);
		}

		public void OnPressed()
		{
			m_locaScreenRef.OnLocaPressed(m_loca);
			m_flash.StartAnimation();
		}

		public void Refresh(LocaLanguage currentLoca)
		{
			Color color = (currentLoca == m_loca) ? m_on : m_off;
			m_border.color = color;
		}
	}
}
