using UnityEngine;
using System.Collections.Generic;

namespace Dasher
{
	public class TraceManager : MonoBehaviour
	{

		private struct Trace
		{
			public GameObject obj;
			public SpriteRenderer sprite;
			public float timer;
		}

		[SerializeField]
		GameObject traceObject;
		[SerializeField]
		int tracePoolSize = 50;

		[SerializeField]
		private float popAnimationTime = 1;
		[SerializeField]
		private AnimationCurve popAnimationCurve;

		private List<Trace> TraceList = new List<Trace>();
		private int currentIndex = 0;
		private int currentAnimatedCount = 0;

		[SerializeField]
		private Sprite JumpSprite;
		[SerializeField]
		private Sprite DashSprite;

		// Use this for initialization
		void Start()
		{

			for (int i = 0; i < tracePoolSize; ++i)
			{
				GameObject go = Instantiate<GameObject>(traceObject);
				go.transform.SetParent(transform);
				go.SetActive(false);
				Trace t = new Trace();
				t.obj = go;
				t.sprite = go.GetComponent<SpriteRenderer>();
				t.timer = 0;
				TraceList.Add(t);
			}

		}


		void Update()
		{
			int c = currentAnimatedCount;
			float dt = GameProcess.Instance.GameTime.GetGameDeltaTime();
			for (int i = c; i > 0; --i)
			{
				int index = currentIndex - i + 1;
				if (index < 0)
				{
					index += TraceList.Count;
				}
				Trace t = TraceList[index];

				t.timer -= dt;

				TraceList[index] = t;
				float progression = Mathf.Clamp(1.0f - t.timer / popAnimationTime, 0.0f, 1.0f);
				t.obj.transform.localScale = Vector3.one * popAnimationCurve.Evaluate(progression);
				if (t.timer <= 0)
				{
					currentAnimatedCount--;
				}
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
			t.obj.transform.localScale = Vector3.zero;
			t.timer = popAnimationTime;

			TraceList[currentIndex] = t;
			currentAnimatedCount = currentAnimatedCount + 1;
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
			t.obj.transform.localScale = Vector3.zero;
			t.timer = popAnimationTime;

			TraceList[currentIndex] = t;
			currentAnimatedCount = currentAnimatedCount + 1;
		}

	}
}
