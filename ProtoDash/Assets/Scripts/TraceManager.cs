using UnityEngine;
using System.Collections.Generic;

public class TraceManager : MonoBehaviour {

	private struct Trace
	{
		public GameObject obj;
		public SpriteRenderer sprite;
		public float timer;
	}

	[SerializeField]
	private GameObject JumpTraceHolder;
	[SerializeField]
	private GameObject DashTraceHolder;

	[SerializeField]
	private float animationTime = 1;
	
	private List<Trace> TraceList = new List<Trace>();
	private int currentIndex = 0;

	[SerializeField]
	private Sprite JumpSprite;
	[SerializeField]
	private Sprite DashSprite;

	// Use this for initialization
	void Start () {

		for (int i = 0; i < transform.childCount; ++i)
		{
			GameObject go = transform.GetChild(i).gameObject;
			go.SetActive(false);
			Trace t = new Trace();
			t.obj = go;
			t.sprite = go.GetComponent<SpriteRenderer>();
			t.timer = 0;
			TraceList.Add(t);
		}
		
	}


	public void NotifyJump(Vector3 pos)
	{
		currentIndex++;
		currentIndex %= TraceList.Count;

		Trace t = TraceList[currentIndex];
		t.obj.SetActive(true);
		t.sprite.sprite = JumpSprite;
		t.obj.transform.position = pos;
	}

	public void NotifyDash(Vector3 pos, Quaternion orientation)
	{
		currentIndex++;
		currentIndex %= TraceList.Count;

		Trace t = TraceList[currentIndex];
		t.obj.SetActive(true);
		t.sprite.sprite = DashSprite;
		t.obj.transform.position = pos;
		t.obj.transform.localRotation = orientation;
	}
	
}
