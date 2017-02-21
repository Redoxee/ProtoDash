using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dasher
{
	public class GoHomeScript : MonoBehaviour
	{

		public void GoHome()
		{
			MainProcess.Instance.RequestSwitchToHome();
		}
	}
}
