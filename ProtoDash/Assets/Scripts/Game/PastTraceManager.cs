using UnityEngine;
using System.Collections.Generic;

namespace Dasher
{
	public enum TraceType {
		Jump,
		Dash,
		Death,
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

		public void NotifyDeath(Transform t)
		{
			m_currentTrace.AddPoint(TraceType.Death, new Vector2(t.position.x, t.position.y), 0);
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
		private GameObject m_traceObject = null;
		[SerializeField]
		Color m_traceColor = Color.black;
		[SerializeField]
		Material m_jumpMaterialBase = null;
		[SerializeField]
		Material m_dashMaterialBase = null;
		[SerializeField]
		Material m_deathMaterialBase = null;

		//private List<GameObject> m_spawnedObjects = new List<GameObject>();

		private void SetupFromPast()
		{

			var jumpMaterial = new Material(m_jumpMaterialBase);
			var dashMaterial = new Material(m_dashMaterialBase);
			var deathMaterial = new Material(m_deathMaterialBase);
			jumpMaterial.color = m_traceColor;
			dashMaterial.color = m_traceColor;
			deathMaterial.color = m_traceColor;

			int nbTrace = m_pastTrace.Length;
			for (int i = 0; i < nbTrace; ++i)
			{
				TracePoint tp = m_pastTrace.m_points[i];
				GameObject obj = Instantiate(m_traceObject);
				obj.transform.SetParent(transform);
				obj.transform.position = new Vector3(tp.position.x, tp.position.y, -1f);
				obj.transform.rotation = Quaternion.AngleAxis(tp.rotation,new Vector3(0,0,1));

				var traceObject = obj.GetComponent<TraceObject>();
				traceObject.SetMaterials(jumpMaterial, dashMaterial, deathMaterial);
				switch (tp.tType) {
					case TraceType.Jump:
						traceObject.SetJump();
						break;
					case TraceType.Dash:
						traceObject.SetDash();
						break;
					case TraceType.Death:
						traceObject.SetDeath();
						break;
				}
				//m_spawnedObjects.Add(obj);
			}
		}

		#endregion
	}
}
