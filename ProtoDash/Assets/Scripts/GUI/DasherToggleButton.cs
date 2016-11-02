using UnityEngine;
using UnityEngine.UI;

namespace Dasher
{
	public class DasherToggleButton : MonoBehaviour
	{
		[SerializeField]
		Color m_activatedColor = Color.yellow;
		[SerializeField]
		Color m_deactiveColor = Color.white;

		[Multiline]
		[SerializeField]
		string m_activeLabel = "on";
		[Multiline]
		[SerializeField]
		string m_deactiveLabel = "off";

		private bool m_state = false;
		public bool State {get {return m_state;} }

		private Material m_borderMaterial;
		private Text m_text;

		void Awake()
		{
			var img = GetComponent<Image>();
			m_borderMaterial = new Material(img.material);
			img.material = m_borderMaterial;
			m_text = GetComponentInChildren<Text>();
			SetOn(false,true);
		}

		public void SetOn(bool newState = true, bool force = false)
		{
			if (newState != m_state || force)
			{
				m_borderMaterial.color = newState ? m_activatedColor : m_deactiveColor;
				m_text.text = newState ? m_activeLabel : m_deactiveLabel;
				m_state = newState;
			}
		}

		public bool Toggle() {
			SetOn(!m_state);
			return m_state;
		}

	}
}
