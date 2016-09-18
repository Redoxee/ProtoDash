using UnityEngine;
using System.Collections;

namespace Dasher
{
	public class FromLevelInfoBehaviour : StateMachineBehaviour {
		public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			MainProcess.Instance.MainMenu.NotifyFromLevelInfoAnimationEnded();
		}
	}
}
