using UnityEngine;
using System.Collections;

namespace Dasher
{
	public class TimeManager 
	{
		public const string c_timeDisplayFormat = "000.00";

		private float m_gameTimeFactor = 1f;
		public float GameTimeFactor {get {return m_gameTimeFactor;} set { m_gameTimeFactor = value; } }

		public float GetGameDeltaTime()
		{
			return Time.deltaTime * m_gameTimeFactor;
		}

		public float GetGameFixedDeltaTime()
		{
			return Time.fixedDeltaTime * m_gameTimeFactor;
		}

		private float currentLevelTime = 0f;
		public float CurrentLevelTime { get { return currentLevelTime; } }

		public void NotifyStartLevel()
		{
			currentLevelTime = 0f;
		}

		public void ManualFixedUpdate()
		{
			currentLevelTime += GetGameFixedDeltaTime();
		}
	}
}