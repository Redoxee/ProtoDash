using UnityEngine;
using System.Collections;

namespace Dasher
{
	public class BackMenu : MonoBehaviour
	{

		public void OnHomePressed()
		{
			MainProcess.Instance.RequestSwitchToHome();
		}
	}
}
