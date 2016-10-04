using UnityEngine;
using System.IO;
using System.Collections.Generic;
using FlatBuffers;
using DasherSave;
using System;
namespace Dasher
{
	public class SaveManager {
		public const string c_SaveVersion = "0.00.002";
		private DasherSavable m_savable;

		#region Save mecanics

		public static string S_GetSaveName()
		{
			return Application.persistentDataPath + "/dSave";
		}

		private FlatBufferBuilder m_builder;

		public SaveManager()
		{
			m_builder = new FlatBufferBuilder(1);

			if (File.Exists(S_GetSaveName()))
			{
				ByteBuffer bb = new ByteBuffer(File.ReadAllBytes(S_GetSaveName()));
				FlatGameSave save = FlatGameSave.GetRootAsFlatGameSave(bb);

				m_savable = new DasherSavable(save.LevelResultsLength);
				LevelFlow flow = MainProcess.Instance.levelFlow;
				for (int i = 0; i < save.LevelResultsLength; ++i)
				{
					FlatLevelSave lvlSave = save.GetLevelResults(i);
					m_savable.m_levels[lvlSave.LevelId] = lvlSave.BestTime;
					LevelData lvlData = flow.GetLevelData(lvlSave.LevelId);
					if (lvlData != null)
					{
						lvlData.currentBest = lvlSave.BestTime;
					}
				}

				m_savable.m_TotalRuns = save.TotalRuns;
				m_savable.m_TotalDashes = save.TotalDashes;
				m_savable.m_TotalJumps = save.TotalJumps;
			}
			else
			{
				m_savable = new DasherSavable(0);
			}
		}

		public void Save()
		{
			m_builder.Clear();


			var levelsEnumerator = m_savable.m_levels.GetEnumerator();
			var offsetTable = new Offset<FlatLevelSave>[m_savable.m_levels.Count];
			int i = 0;
			while (levelsEnumerator.MoveNext())
			{
				var value = levelsEnumerator.Current;
				var idOffset = m_builder.CreateString(value.Key);
				var lvlOffset = FlatLevelSave.CreateFlatLevelSave(m_builder, idOffset, value.Value);
				offsetTable[i++] = lvlOffset;
			}

			var versionOffset = m_builder.CreateString(c_SaveVersion);
			var levelsOffset = FlatGameSave.CreateLevelResultsVector(m_builder, offsetTable);
			FlatGameSave.StartFlatGameSave(m_builder);

			FlatGameSave.AddVersion(m_builder, versionOffset);
			FlatGameSave.AddLevelResults(m_builder, levelsOffset);

			FlatGameSave.AddTotalRuns(m_builder, m_savable.m_TotalRuns);
			FlatGameSave.AddTotalJumps(m_builder, m_savable.m_TotalJumps);
			FlatGameSave.AddTotalDashes(m_builder, m_savable.m_TotalDashes);

			var gameOffset = FlatGameSave.EndFlatGameSave(m_builder);

			FlatGameSave.FinishFlatGameSaveBuffer(m_builder, gameOffset);

			using (var ms = new MemoryStream(m_builder.DataBuffer.Data, m_builder.DataBuffer.Position, m_builder.Offset))
			{
				File.WriteAllBytes(S_GetSaveName(), ms.ToArray());
				Debug.Log("data saved");
			}
		}

		#endregion

		#region Interface

		public bool HasLevelBeenDone(string levelId)
		{
			return m_savable.m_levels.ContainsKey(levelId);
		}

		public float GetLevelTime(string levelId)
		{
			if (HasLevelBeenDone(levelId))
			{
				return m_savable.m_levels[levelId];
			}
			return float.MaxValue;
		}

		public void SetLevelTime(string levelId, float time)
		{
			m_savable.m_levels[levelId] = time;
		}

		public void NotifyLevelStarted()
		{
			m_savable.m_TotalRuns += 1;
		}

		public void NotifyEndRun(int nbJumps, int nbDashes)
		{
			m_savable.m_TotalJumps += nbJumps;
			m_savable.m_TotalDashes += nbDashes;
		}

		public int GetTotalRuns()
		{
			return m_savable.m_TotalRuns;
		}

		public int GetTotalJumps()
		{
			return m_savable.m_TotalJumps;
		}

		public int GetTotalDashes()
		{
			return m_savable.m_TotalDashes;
		}

		#endregion
	}

	public class DasherSavable
	{
		public Dictionary<string, float> m_levels;

		public int m_TotalRuns = 0;
		public int m_TotalJumps = 0;
		public int m_TotalDashes = 0;

		public DasherSavable(int nbLevels)
		{
			m_levels = new Dictionary<string, float>(nbLevels);
		}
	}
}