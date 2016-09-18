using UnityEngine;
using System;
using System.Collections.Generic;
namespace Dasher
{
	[Serializable]
	public class LevelData
	{
		public UnityEngine.Object sceneObject;
		public string sceneName;
		public int world;
		public float parTime;
	}

	public class LevelFlow : ScriptableObject
	{
		[SerializeField]
		public List<LevelData> levelList;


		private Dictionary<int, List<LevelData>> m_structuredLevelFlow = null;
		private void BuildStructuredData()
		{
			m_structuredLevelFlow = new Dictionary<int, List<LevelData>>();
			for (int i = 0; i < levelList.Count; ++i)
			{
				LevelData lvl = levelList[i];
				if (!m_structuredLevelFlow.ContainsKey(lvl.world))
				{
					m_structuredLevelFlow[lvl.world] = new List<LevelData>();
				}
				m_structuredLevelFlow[lvl.world].Add(lvl);
			}
		}
		public Dictionary<int, List<LevelData>> GetStructuredProgression()
		{
			if (m_structuredLevelFlow == null)
			{
				BuildStructuredData();
			}
			return m_structuredLevelFlow;
		}
	}
}