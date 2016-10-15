using UnityEngine;
using System.Collections.Generic;

namespace Dasher
{
	[RequireComponent(typeof(PastTraceManager))]
	public class TraceManager : MonoBehaviour
	{

		private struct Trace
		{
			public GameObject obj;
			public SpriteRenderer sprite;
			public float timer;
		}

		[SerializeField]
		GameObject traceObject = null;
		[SerializeField]
		int tracePoolSize = 50;

		[SerializeField]
		private float popAnimationTime = 1;
		[SerializeField]
		private AnimationCurve popAnimationCurve = AnimationCurve.Linear(0,0,1,1);

		private List<Trace> TraceList = new List<Trace>();
		private int currentIndex = 0;
		private int currentAnimatedCount = 0;

		[SerializeField]
		private Sprite JumpSprite = null;
		[SerializeField]
		private Sprite DashSprite = null;

		private int m_jumpCounter = 0;
		private int m_dashesCounter = 0;
		public int NbJumps { get { return m_jumpCounter; } }
		public int NbDashes { get { return m_dashesCounter; } }

		private PastTraceManager m_pastTraceManager;

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

			m_jumpCounter = 0;
			m_dashesCounter = 0;

			m_pastTraceManager = GetComponent<PastTraceManager>();
			m_pastTraceManager.StartRecording();
		}


		void Update()
		{
			int c = currentAnimatedCount;
			float dt = Time.deltaTime;
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
			m_jumpCounter += 1;
			if (isActiveAndEnabled)
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

				m_pastTraceManager.NotifyJump(t.obj.transform);
			}
		}

		public void NotifyDash(Vector3 pos, Quaternion orientation)
		{
			m_dashesCounter += 1;
			if (isActiveAndEnabled)
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

				m_pastTraceManager.NotifyDash(t.obj.transform);
			}
		}

	}
}
