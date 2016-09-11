using UnityEngine;
using System.Collections;

namespace Dasher
{
	public class TransitionCanvasBehaviour : MonoBehaviour
	{

		public void OnTransitionIn()
		{
			MainProcess.Instance.OnTransitionIn();
		}

		public void OnTransitionOut()
		{
			MainProcess.Instance.OnTransitionOut();
		}
	}
}
