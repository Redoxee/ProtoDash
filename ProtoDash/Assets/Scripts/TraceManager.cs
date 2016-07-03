using UnityEngine;
using System.Collections.Generic;

public class TraceManager : MonoBehaviour {

	[SerializeField]
	private GameObject JumpTraceHolder;
	[SerializeField]
	private GameObject DashTraceHolder;

	private List<GameObject> JumpObjectList = new List<GameObject>();
	private int CurrentJumpIndex = -1;

	private List<GameObject> DashObjectList = new List<GameObject>();
	private int CurrentDashIndex = -1;

	// Use this for initialization
	void Start () {

		for (int i = 0; i < JumpTraceHolder.transform.childCount; ++i)
		{
			GameObject go = JumpTraceHolder.transform.GetChild(i).gameObject;
			go.SetActive(false);
			JumpObjectList.Add(go);
		}

		for (int i = 0; i < DashTraceHolder.transform.childCount; ++i)
		{
			GameObject go = DashTraceHolder.transform.GetChild(i).gameObject;
			go.SetActive(false);
			DashObjectList.Add(go);
		}
	}


	public void NotifyJump(Vector3 pos)
	{
		CurrentJumpIndex++;
		CurrentJumpIndex %= JumpObjectList.Count;

		GameObject trace = JumpObjectList[CurrentJumpIndex];
		trace.SetActive(true);
		trace.transform.position = pos;
	}

	public void NotifyDash(Vector3 pos, Quaternion orientation)
	{
		CurrentDashIndex++;
		CurrentDashIndex %= DashObjectList.Count;

		GameObject trace = DashObjectList[CurrentDashIndex];
		trace.SetActive(true);
		trace.transform.position = pos;
		trace.transform.localRotation = orientation;
	}
	
}
