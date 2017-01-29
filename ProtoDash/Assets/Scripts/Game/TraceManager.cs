using UnityEngine;
using System.Collections.Generic;

namespace Dasher
{
	[RequireComponent(typeof(PastTraceManager))]
	public class TraceManager : MonoBehaviour
	{
		private class Trace
		{
			public GameObject obj;
			public TraceObject trace;
			public float timer;
		}
		
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
		private GameObject m_tracePrefab;
		[SerializeField]
		private Color m_traceColor;
		[SerializeField]
		private Material m_jumpMaterialBase;
		[SerializeField]
		private Material m_dashMaterialBase;
		[SerializeField]
		private Material m_deathMaterialBase;

		private int m_jumpCounter = 0;
		private int m_dashesCounter = 0;
		public int NbJumps { get { return m_jumpCounter; } }
		public int NbDashes { get { return m_dashesCounter; } }

		private PastTraceManager m_pastTraceManager;

		void Start()
		{
			var jumpMaterial = new Material(m_jumpMaterialBase);
			var dashMaterial = new Material(m_dashMaterialBase);
			var deathMaterial = new Material(m_deathMaterialBase);
			jumpMaterial.color = m_traceColor;
			dashMaterial.color = m_traceColor;
			deathMaterial.color = m_traceColor;

			for (int i = 0; i < tracePoolSize; ++i)
			{
				GameObject go = Instantiate<GameObject>(m_tracePrefab);
				go.transform.SetParent(transform);
				go.SetActive(false);
				Trace t = new Trace();
				t.obj = go;
				t.trace = go.GetComponent<TraceObject>();
				t.timer = 0;
				TraceList.Add(t);

				t.trace.SetMaterials(jumpMaterial, dashMaterial, deathMaterial);
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
				t.trace.SetJump();
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
				t.trace.SetDash();
				t.obj.transform.position = pos;
				t.obj.transform.localRotation = orientation;
				t.obj.transform.localScale = Vector3.zero;
				t.timer = popAnimationTime;

				TraceList[currentIndex] = t;
				currentAnimatedCount = currentAnimatedCount + 1;

				m_pastTraceManager.NotifyDash(t.obj.transform);
			}
		}

		public void NotifyDeath(Vector3 pos)
		{
			if (isActiveAndEnabled)
			{
				pos.z = -.5f;
				currentIndex++;
				currentIndex %= TraceList.Count;
				Trace t = TraceList[currentIndex];
				t.obj.SetActive(true);
				t.trace.SetDeath();
				t.obj.transform.position = pos;
				t.obj.transform.localScale = Vector3.zero;
				t.obj.transform.localRotation = Quaternion.identity;
				t.timer = popAnimationTime;

				TraceList[currentIndex] = t;
				currentAnimatedCount = currentAnimatedCount + 1;

				m_pastTraceManager.NotifyDeath(t.obj.transform);
			}
		}


	}
}
