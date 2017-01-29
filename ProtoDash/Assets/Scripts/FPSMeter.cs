using UnityEngine;
using UnityEngine.UI;

namespace Dasher
{
	public class FPSMeter : MonoBehaviour
	{

		private Text text;

		void Start()
		{
			text = GetComponent<Text>();
		}

		void Update()
		{
			text.text = (1.0f / Time.deltaTime).ToString();
		}
	}
}
