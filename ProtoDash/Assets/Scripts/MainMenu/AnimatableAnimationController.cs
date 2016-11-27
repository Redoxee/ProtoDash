using UnityEngine;
using System.Collections;

namespace Dasher
{
	public class AnimatableAnimationController : StateMachineBehaviour
	{
		public AnimatableUI m_animatable;

		public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			m_animatable.enabled = true;
		}

		public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			m_animatable.enabled = false;
			m_animatable.SetAnimationPosition(1);
		}
	}
}
