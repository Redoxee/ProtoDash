using UnityEngine;
using System.Diagnostics;

namespace Dasher
{
	public class PerfRuler : MonoBehaviour
	{


		int m_frameCount = 0;
		Stopwatch m_stopwatch= new Stopwatch();

		public void Initialize()
		{
			m_frameCount = 0;
		}

		public void StartRecord()
		{
			Initialize();
			m_stopwatch.Start();
		}

		public float StopRecord()
		{

			m_stopwatch.Stop();
			return GetMeanFPS();
		}

		public float GetMeanFPS()
		{
			if (m_frameCount > 0)
				return 1000f / (m_stopwatch.ElapsedMilliseconds / (float)m_frameCount);
			else
				return -1;
		}

		void Update()
		{
			m_frameCount++;
		}
	}
}
