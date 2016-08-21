using UnityEngine;
using System.Collections;

namespace Dasher
{
	public class DeathZoneMaterialUpdater : MonoBehaviour
	{

		[SerializeField]
		private Material deathZoneMaterial;
		[SerializeField]
		private Color color1;
		[SerializeField]
		private Color color2;
		[SerializeField]
		private float frequence = .5f;

		private float currentTimer = 0f;

		void Update()
		{
			currentTimer += Time.deltaTime;
			float p = Mathf.Sin(currentTimer * frequence) * .5f + .5f;
			deathZoneMaterial.color = color1 * p + color2 * (1f - p);
		}
	}
}
