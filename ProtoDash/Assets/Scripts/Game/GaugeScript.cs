using UnityEngine;
using UnityEngine.UI;

namespace Dasher
{
	public class GaugeScript : MonoBehaviour
	{
		private Character m_character;

		private Material gaugeMaterial;

		void Start()
		{
			gaugeMaterial = GetComponent<Image>().material;
			m_character = MainGameProcess.Instance.CurrentCharacter;
		}

		void Update()
		{
			gaugeMaterial.SetFloat("_GaugeProgression", m_character.currentEnergy / m_character.maxEnergyPoints);
		}
	}
}
