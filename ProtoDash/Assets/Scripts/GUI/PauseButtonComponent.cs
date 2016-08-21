using UnityEngine;
using System.Collections;

namespace Dasher
{
	public class PauseButtonComponent : MonoBehaviour
	{

		[SerializeField]
		GUIManager GUIManagerRef;

		public void OnResumeAnimationEnd()
		{
			GUIManagerRef.OnResumeAnimationEnd();
		}

		// Use this for initialization
		void Start()
		{

		}

		// Update is called once per frame
		void Update()
		{

		}
	}
}
