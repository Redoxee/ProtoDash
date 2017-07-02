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
			return string.Format(c_levelNamePattern, world + 1, indexInWorld);
		}

		public bool IsLevelChamp { get {
				return currentBest > 0 && currentBest < parTime;
			} }
		public bool HasBeenFinished { get { return currentBest > 0; } }
	}

	public class LevelFlow : ScriptableObject
	{
		public const int c_nbLevelInWorld = 6;
		public const int c_nbOriginalLevelNumber = 60;
		public const int c_nbOriginalWorld = 10;

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

		public LevelData GetLevelData(int levelIndex)
		{
			return m_levelList[levelIndex];
		}

		private void InitializeWorldInLevels()
		{
			int count = m_levelList.Count;
			for (int i = 0; i < count; ++i)
			{
				m_levelList[i].world = i / c_nbLevelInWorld;
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
			if (!IsOriginalLevelAllChamped())
				return c_nbOriginalLevelNumber;
			return m_levelList.Count;
		}

		public bool IsLevelFinished(int levelIndex)
		{
			return m_levelList[levelIndex].HasBeenFinished;
		}

		public bool IsLevelChamp(int levelIndex)
		{
			LevelData lvl = m_levelList[levelIndex];
			return lvl.currentBest > 0 && lvl.currentBest < lvl.parTime;
		}

		public bool IsLevelChamp(string levelName)
		{
			return IsLevelChamp(GetLevelIndex(levelName));
		}

		public bool IsOriginalLevelAllChamped()
		{
			for (int i = 0; i < c_nbOriginalLevelNumber; ++i)
			{
				if (!IsLevelChamp(i))
					return false;
			}
			return true;
		}


		public int GetLevelIndex(string levelName)
		{
			return m_levelList.FindIndex((LevelData data) => { return data.sceneName == levelName; });
		}

		public KeyValuePair<int,int> GetWorldAndRankPosition(string levelName)
		{
			if (levelName == null)
				return new KeyValuePair<int, int>(0, 0);

			var wc = m_structuredLevelFlow.Count;
			for (int i = 0; i < wc; ++i)
			{
				var world = i + 1;
				int lc = m_structuredLevelFlow[world].Count;
				for (int level = 0; level < lc; ++level)
				{
					var lvl = m_structuredLevelFlow[world][level];
					if (lvl.sceneName == levelName)
					{
						return new KeyValuePair<int, int>(world, level + 1);
					}
				}
			}
			return new KeyValuePair<int, int>(-1,-1);
		}

		public LevelData GetMostInterestingLevel()
		{
			SaveManager saveManager = MainProcess.Instance.DataManager;
			LevelData result = m_levelList[0];
			var levelCount = m_levelList.Count;
			var levelIndex = 0;
			while(levelIndex < levelCount && saveManager.DoesProgressionAllowLevel(levelIndex))
			{
				result = m_levelList[levelIndex];
				if (!result.HasBeenFinished)
				{
					break;
				}
				levelIndex += 1;
			}
			if (levelIndex >= levelCount && result.HasBeenFinished)
			{
				string name = saveManager.LastLevelPlayed;
				result = GetLevelData(name);
			}
			if (result == null)
			{
				result = m_levelList[0];
			}

			return result;
		}
	}
}