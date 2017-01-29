using UnityEngine;
using System.Collections;

namespace Dasher
{
	public class TraceObject : MonoBehaviour
	{

		[SerializeField]
		private GameObject JumpTrace = null;
		[SerializeField]
		private GameObject DashTrace = null;
		[SerializeField]
		private GameObject DeathTrace = null;

		public void SetJump()
		{
			JumpTrace.SetActive(true);
			DashTrace.SetActive(false);
			DeathTrace.SetActive(false);
		}
		public void SetDash()
		{
			JumpTrace.SetActive(false);
			DashTrace.SetActive(true);
			DeathTrace.SetActive(false);
		}
		public void SetDeath()
		{
			JumpTrace.SetActive(false);
			DashTrace.SetActive(false);
			DeathTrace.SetActive(true);
		}

		public void SetMaterials(Material jump,Material dash, Material death)
		{
			JumpTrace.GetComponent<Renderer>().material = jump;
			DashTrace.GetComponentInChildren<Renderer>().material = dash;
			DeathTrace.GetComponent<Renderer>().material = death;
		}
	}
}
