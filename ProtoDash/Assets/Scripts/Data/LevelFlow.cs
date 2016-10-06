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
		[NonSerialized]
		public float currentBest = 0f;

		[NonSerialized]
		private const string c_levelNamePattern = "{0}-{1}";
		[NonSerialized]
		public int indexInWorld = -1;

		public string GetLevelLabel()
		{
			return string.Format(c_levelNamePattern, world, indexInWorld);
		}
	}

	public class LevelFlow : ScriptableObject
	{
		public const int c_nbLevelInWorld = 6;

		[SerializeField]
		public List<LevelData> levelList;

		public LevelData GetLevelData(string levelName)
		{
			for (int i = 0; i < levelList.Count; ++i)
			{
				if (levelList[i].sceneName == levelName)
					return levelList[i];
			}
			return null;
		}

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

				lvl.indexInWorld = m_structuredLevelFlow[lvl.world].Count;
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

		public int GetLevelCount()
		{
			return levelList.Count;
		}

		public bool IsLevelFinished(int levelIndex)
		{
			return levelList[levelIndex].currentBest > 0;
		}

		public bool IsLevelChamp(int levelIndex)
		{
			LevelData lvl = levelList[levelIndex];
			return lvl.currentBest > 0 && lvl.currentBest < lvl.parTime;
		}
	}
}