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
		[NonSerialized]
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

		private bool m_isInitialized = false;
		private void Initialize(bool force = false)
		{
			if (m_isInitialized || force)
				return;
			InitializeWorldInLevels();
			BuildStructuredData();
		}

		void Awake()
		{
			Initialize();
		}

		[SerializeField]
		private List<LevelData> m_levelList = null;
		public List<LevelData> LevelList { get { Initialize(); return m_levelList; } }

		public LevelData GetLevelData(string levelName)
		{
			int count = LevelList.Count;
			for (int i = 0; i < count; ++i)
			{
				if (m_levelList[i].sceneName == levelName)
					return m_levelList[i];
			}
			return null;
		}

		private void InitializeWorldInLevels()
		{
			int count = m_levelList.Count;
			for (int i = 0; i < count; ++i)
			{
				m_levelList[i].world = i / c_nbLevelInWorld + 1;
			}
		}

		private Dictionary<int, List<LevelData>> m_structuredLevelFlow = null;
		private void BuildStructuredData()
		{
			m_structuredLevelFlow = new Dictionary<int, List<LevelData>>();
			int count = m_levelList.Count;
			for (int i = 0; i < count; ++i)
			{
				LevelData lvl = m_levelList[i];
				int worldIndex = i / c_nbLevelInWorld + 1;
				if (!m_structuredLevelFlow.ContainsKey(worldIndex))
				{
					m_structuredLevelFlow[worldIndex] = new List<LevelData>();
				}
				m_structuredLevelFlow[worldIndex].Add(lvl);

				lvl.indexInWorld = m_structuredLevelFlow[worldIndex].Count;
			}
		}

		public Dictionary<int, List<LevelData>> GetStructuredProgression()
		{
			Initialize();
			return m_structuredLevelFlow;
		}

		public int GetLevelCount()
		{
			return m_levelList.Count;
		}

		public bool IsLevelFinished(int levelIndex)
		{
			return m_levelList[levelIndex].currentBest > 0;
		}

		public bool IsLevelChamp(int levelIndex)
		{
			LevelData lvl = m_levelList[levelIndex];
			return lvl.currentBest > 0 && lvl.currentBest < lvl.parTime;
		}
	}
}