using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

namespace Dasher
{
	public class MainProcessInstaciator : MonoBehaviour {
		const string c_mainScene = "MainScene";

		[SerializeField]
		private GameStates m_requiredState;

		private bool m_waitForSetup = false;

		void Awake()
		{
			if (MainProcess.Instance == null)
			{
				SceneManager.LoadScene(c_mainScene, LoadSceneMode.Additive);
				SceneManager.SetActiveScene(SceneManager.GetSceneByName(c_mainScene));
				m_waitForSetup = true;
			}

			if (!m_waitForSetup)
			{
				Destroy(gameObject);
			}
		}

		void Update()
		{

			if(m_requiredState != GameStates.Error)
			{
				MainProcess.Instance.SetState(m_requiredState);
			}
			Destroy(gameObject);
		}
	}
}
