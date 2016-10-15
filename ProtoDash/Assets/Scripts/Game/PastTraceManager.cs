using UnityEngine;
using System.Collections.Generic;

namespace Dasher
{
	public enum TraceType {
		Jump,
		Dash,
	}

	public struct TracePoint
	{
		public TraceType tType;
		public Vector2 position;
		public float rotation;
	}

	public class TraceRecording
	{
		public const ushort c_TraceAllocated = 200;
		private int m_length = 0;
		public int Length { get { return m_length; } }
		public TracePoint[] m_points = new TracePoint[c_TraceAllocated];
		public void AddPoint(TraceType t, Vector2 p, float r)
		{
			m_points[m_length].tType = t;
			m_points[m_length].position = p;
			m_points[m_length].rotation = r;
			m_length = (m_length + 1) % c_TraceAllocated;
		}
	}

	public class PastTraceManager : MonoBehaviour
	{
		TraceRecording m_pastTrace = null;
		TraceRecording m_currentTrace = null;
		//bool m_recordingStarted = false;

		#region Recording
		public void StartRecording()
		{
			m_currentTrace = new TraceRecording();
			//m_recordingStarted = true;
		}

		public void NotifyJump(Transform t)
		{
			m_currentTrace.AddPoint(TraceType.Jump, new Vector2(t.position.x, t.position.y), 0);
		}

		public void NotifyDash(Transform t)
		{
			var dir = t.rotation * Vector3.right;
			float a = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
			m_currentTrace.AddPoint(TraceType.Dash, new Vector2(t.position.x, t.position.y),a);
		}

		public TraceRecording GetCurrentRecording()
		{
			return m_currentTrace;
		}

		#endregion

		#region Past
		public void Initialize()
		{
			MainProcess mp = MainProcess.Instance;
			TraceRecording pastTrace = mp.DataManager.GetTraceForLevel(mp.CurrentLevel);
			if (pastTrace != null)
			{
				m_pastTrace = pastTrace;
				SetupFromPast();
			}
		}

		[SerializeField]
		private GameObject TracePrefab = null;
		[SerializeField]
		private Sprite m_jumpSprite = null;
		[SerializeField]
		private Sprite m_dashSprite = null;

		private List<GameObject> m_spawnedObjects = new List<GameObject>();

		private void SetupFromPast()
		{
			int nbTrace = m_pastTrace.Length;
			for (int i = 0; i < nbTrace; ++i)
			{
				TracePoint tp = m_pastTrace.m_points[i];
				GameObject obj = Instantiate(TracePrefab);
				obj.transform.SetParent(transform);
				obj.transform.position = new Vector3(tp.position.x, tp.position.y, transform.position.z);
				obj.transform.rotation = Quaternion.AngleAxis(tp.rotation,new Vector3(0,0,1));
				Sprite tex = (tp.tType == TraceType.Jump) ? m_jumpSprite : m_dashSprite;

				SpriteRenderer renderer = obj.GetComponent<SpriteRenderer>();
				renderer.sprite = tex;
			}
		}

		#endregion
	}
}
