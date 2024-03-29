﻿//#define DASHER_NO_IAP

using UnityEngine;
using System.IO;
using System.Collections.Generic;
using FlatBuffers;
using DasherSave;
using System;
namespace Dasher
{
	public class SaveManager {
		public const int c_SaveVersion = 12;
		private DasherSavable m_savable = null;

		public const int c_storyUnlocked = 141418;
		public const int c_storyLocked = 49862;


		#region Save mecanics

		static string SavePath
		{
			get
			{
				string path = Application.persistentDataPath;
				if (path == "")
				{
					path = System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData);
				}
				return path;
			}
		}

		public static string S_GetSaveName()
		{
			return SavePath + "/dSave";
		}

		public static string S_GetLevelTraceFile(string lvlId)
		{
			return SavePath + "/Trace_" + lvlId;
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
					var level = new DasherSavable.LevelSavable();
					level.bestTime = lvlSave.BestTime;
					level.nbTry = lvlSave.NbTry;
					level.nbComplete = lvlSave.NbComplete;
					level.nbFail = lvlSave.NbFail;
					m_savable.m_levels[lvlSave.LevelId] = level;
					LevelData lvlData = flow.GetLevelData(lvlSave.LevelId);
					if (lvlData != null)
					{
						lvlData.currentBest = lvlSave.BestTime;
					}
				}

				m_savable.m_TotalRuns = save.TotalRuns;
				m_savable.m_TotalDeaths = save.TotalDeaths;
				m_savable.m_TotalDashes = save.TotalDashes;
				m_savable.m_TotalJumps = save.TotalJumps;

				m_savable.UserId = save.UserId;
				m_savable.m_LastLevelPlayed = save.LastPlayedLevel;
				if (save.Settings != null)
				{
					m_savable.m_settings.isLefthanded = save.Settings.LeftHandedMode;
					m_savable.m_settings.SelectedLanguage = save.Settings.CurrentLocaSelected;
				}
				m_savable.m_IsStoryQuestUnlocked = save.MainStoryUnlocked;

				ConvertSave(save.Version);
			}

			if(m_savable == null)
			{
				m_savable = new DasherSavable(0);
			}
			if (m_savable.UserId == null || m_savable.UserId == "")
			{
				m_savable.UserId = GenerateUserId();
			}
		}

		static string s_prefixId = null;
		public static string PrefixId { set { s_prefixId = value; } }

		public static string GenerateUserId()
		{
			string result = "Id";
			if (s_prefixId != null && s_prefixId != "")
			{
				result = s_prefixId + "_";
				s_prefixId = null;
			}

			result += DateTime.Now.Ticks.ToString("X");

			result += ((long)(Time.realtimeSinceStartup * Time.deltaTime)).ToString("X") ;
			Debug.Log(result);
			return result;
		}

		#region Convert Save
		void ConvertSave(int version)
		{
			if (m_savable != null)
			{
				if (version < 9)
				{
					ClearSave();
				}
			}
		}
		#endregion

		public void Save()
		{
			m_builder.Clear();


			var levelsEnumerator = m_savable.m_levels.GetEnumerator();
			var offsetTable = new Offset<FlatLevelSave>[m_savable.m_levels.Count];
			int i = 0;
			while (levelsEnumerator.MoveNext())
			{
				var current = levelsEnumerator.Current;
				var level = current.Value;
				var idOffset = m_builder.CreateString(current.Key);
				var lvlOffset = FlatLevelSave.CreateFlatLevelSave(m_builder, idOffset,level.bestTime,level.nbTry,level.nbComplete,level.nbFail);
				offsetTable[i++] = lvlOffset;
			}
			
			var userIdOffset = m_builder.CreateString(m_savable.UserId);
			string lastLevelPlayed = m_savable.m_LastLevelPlayed != null ? m_savable.m_LastLevelPlayed : "";
			var lastLevelPlayedOffset = m_builder.CreateString(lastLevelPlayed);
			var levelsOffset = FlatGameSave.CreateLevelResultsVector(m_builder, offsetTable);


			bool isLeftHanded = m_savable.m_settings.isLefthanded;
			int selectedLanguage = m_savable.m_settings.SelectedLanguage;
			var settingsOffset = FlatSettings.CreateFlatSettings(m_builder, m_savable.m_settings.isLefthanded,selectedLanguage);
	
			FlatGameSave.StartFlatGameSave(m_builder);
			FlatGameSave.AddVersion(m_builder,c_SaveVersion);
			FlatGameSave.AddUserId(m_builder, userIdOffset);
			FlatGameSave.AddLevelResults(m_builder, levelsOffset);

			FlatGameSave.AddTotalRuns(m_builder, m_savable.m_TotalRuns);
			FlatGameSave.AddTotalDeaths(m_builder, m_savable.m_TotalDeaths);
			FlatGameSave.AddTotalJumps(m_builder, m_savable.m_TotalJumps);
			FlatGameSave.AddTotalDashes(m_builder, m_savable.m_TotalDashes);
			FlatGameSave.AddLastPlayedLevel(m_builder, lastLevelPlayedOffset);
			FlatGameSave.AddSettings(m_builder, settingsOffset);
			FlatGameSave.AddMainStoryUnlocked(m_builder, m_savable.m_IsStoryQuestUnlocked);

			var gameOffset = FlatGameSave.EndFlatGameSave(m_builder);

			FlatGameSave.FinishFlatGameSaveBuffer(m_builder, gameOffset);

			using (var ms = new MemoryStream(m_builder.DataBuffer.Data, m_builder.DataBuffer.Position, m_builder.Offset))
			{
				File.WriteAllBytes(S_GetSaveName(), ms.ToArray());
				Debug.Log("data saved");
			}
		}

		public void SaveLevelTrace(string lvlId)
		{
			if (!m_traceDictionary.ContainsKey(lvlId))
				return;
			var trace = m_traceDictionary[lvlId];
			var fileName = S_GetLevelTraceFile(lvlId);

			m_builder.Clear();

			var idOffset = m_builder.CreateString(lvlId);

			var pointVectorOffset = new Offset<FlatTracePoint>[trace.Length];
			for (int i = 0; i < trace.Length; ++i)
			{
				var point = trace.m_points[i];
				var positionOffset = FlatVector2.CreateFlatVector2(m_builder,point.position.x, point.position.y);
				var pointType = (int)point.tType;
				pointVectorOffset[i] = FlatTracePoint.CreateFlatTracePoint(m_builder, pointType, positionOffset, point.rotation);
			}
			var pointOffs = FlatTraceSave.CreatePointsVector(m_builder,pointVectorOffset);
			var flatTrace = FlatTraceSave.CreateFlatTraceSave(m_builder, idOffset, pointOffs);
			FlatTraceSave.FinishFlatTraceSaveBuffer(m_builder, flatTrace);
			using (var ms = new MemoryStream(m_builder.DataBuffer.Data, m_builder.DataBuffer.Position, m_builder.Offset))
			{
				File.WriteAllBytes(fileName, ms.ToArray());
				Debug.Log("data saved");
			}
		}


		public bool LoadTrace(string lvlId,bool force = false)
		{
			if (m_traceDictionary.ContainsKey(lvlId) && !force)
				return true;

			string fileName = S_GetLevelTraceFile(lvlId);
			if (!File.Exists(fileName))
				return false;

			ByteBuffer bb = new ByteBuffer(File.ReadAllBytes(fileName));
			FlatTraceSave flatTrace = FlatTraceSave.GetRootAsFlatTraceSave(bb);

			TraceRecording newTrace = new TraceRecording();
			m_traceDictionary[lvlId] = newTrace;

			int nbPoint = flatTrace.PointsLength;
			for (int i = 0; i < nbPoint; ++i)
			{
				var flatPoint = flatTrace.GetPoints(i);
				newTrace.AddPoint((TraceType)flatPoint.TraceType, new Vector2(flatPoint.Position.X, flatPoint.Position.Y), flatPoint.Rotation);
			}

			return true;
		}

		public void ClearSave()
		{
			var info = new DirectoryInfo(Application.persistentDataPath);
			var fileEnum = info.GetFiles().GetEnumerator();
			while (fileEnum.MoveNext())
			{
				var file = (FileInfo)fileEnum.Current;
				File.Delete(file.FullName);
			}
			m_savable = new DasherSavable(0);
			m_traceDictionary = new Dictionary<string, TraceRecording>();

			var flow  = MainProcess.Instance.levelFlow;
			var levelEnum = flow.LevelList.GetEnumerator();
			while (levelEnum.MoveNext())
			{
				levelEnum.Current.currentBest = 0f;
			}
		}

		#endregion

		#region Interface

		public bool HasLevelBeenFinished(string levelId)
		{
			return m_savable.m_levels.ContainsKey(levelId) && m_savable.m_levels[levelId].bestTime > 0;
		}

		public float GetLevelTime(string levelId)
		{
			if (HasLevelBeenFinished(levelId))
			{
				return m_savable.m_levels[levelId].bestTime;
			}
			return float.MaxValue;
		}

		public int GetLevelTryCount(string levelId)
		{
			if (!m_savable.m_levels.ContainsKey(levelId))
				return 0;
			return m_savable.m_levels[levelId].nbTry;
		}

		public int GetLevelSuccessCount(string levelId)
		{
			if (!m_savable.m_levels.ContainsKey(levelId))
			{
				return 0;
			}
			return m_savable.m_levels[levelId].nbComplete;
		}

		public int GetLevelFailCount(string levelId)
		{
			if (!m_savable.m_levels.ContainsKey(levelId))
			{
				return 0;
			}
			return m_savable.m_levels[levelId].nbFail;
		}

		public void SetLevelTime(string levelId, float time)
		{

			DasherSavable.LevelSavable lvl = null;
			if (m_savable.m_levels.ContainsKey(levelId))
			{
				lvl = m_savable.m_levels[levelId];
			}
			else
			{
				lvl = new DasherSavable.LevelSavable();
				m_savable.m_levels[levelId] = lvl;
			}
			lvl.bestTime = time;
		}

		public void NotifyLevelStarted(string levelId)
		{
			m_savable.m_TotalRuns += 1;
			m_savable.m_LastLevelPlayed = levelId;


			DasherSavable.LevelSavable lvl = null;
			if (m_savable.m_levels.ContainsKey(levelId))
			{
				lvl = m_savable.m_levels[levelId];
			}
			else
			{
				lvl = new DasherSavable.LevelSavable();
				m_savable.m_levels[levelId] = lvl;
			}

			lvl.nbTry += 1;
		}

		public void NotifyEndRun(int nbJumps, int nbDashes, string levelId,bool isSuccess)
		{
			m_savable.m_TotalJumps += nbJumps;
			m_savable.m_TotalDashes += nbDashes;
			if (isSuccess)
			{
				m_savable.m_levels[levelId].nbComplete += 1;
			}
			else
			{
				m_savable.m_levels[levelId].nbFail += 1;
			}
		}

		public void NotifyDeath(int nbJumps, int nbDashes,string levelId)
		{
			NotifyEndRun(nbJumps,nbDashes, levelId, false);
			m_savable.m_TotalDeaths += 1;
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

		public int GetTotalDeaths()
		{
			return m_savable.m_TotalDeaths;
		}

		public string LastLevelPlayed { get { return m_savable.m_LastLevelPlayed; } }

		public Dictionary<string, TraceRecording> m_traceDictionary = new Dictionary<string, TraceRecording>();
		public TraceRecording GetTraceForLevel(string levelId)
		{
			if(m_traceDictionary.ContainsKey(levelId))
				return m_traceDictionary[levelId];
			if(LoadTrace(levelId))
				return m_traceDictionary[levelId];

			return null;
		} 

		public void SetTraceForLevel(string levelId, TraceRecording trace)
		{
			m_traceDictionary[levelId] = trace;
		}

		public DasherSettings GetSettings()
		{
			return m_savable.m_settings;
		}

		public void SetLeftHandedMode(bool isLeftHanded)
		{
			m_savable.m_settings.isLefthanded = isLeftHanded;
		}

		public LocaLanguage CurrentLanguage
		{
			get
			{
				if (m_savable.m_settings.SelectedLanguage >= 0)
					return (LocaLanguage)m_savable.m_settings.SelectedLanguage;
				return LocaObject.GetLocaFromSys();
			}

			set
			{
				m_savable.m_settings.SelectedLanguage = (int)value;
				Save();
			}
		}

		public void IncrementLastLevelPlayed()
		{
			var flow = MainProcess.Instance.levelFlow;
			
			var lastIndex = flow.GetLevelIndex(m_savable.m_LastLevelPlayed);
			var newIndex = (lastIndex + 1) % flow.GetLevelCount();
			m_savable.m_LastLevelPlayed = flow.LevelList[newIndex].sceneName;
		}

		public string UserId { get { return m_savable.UserId; } }

		public bool DoesProgressionAllowLevel(int lvlIndex)
		{
			if (lvlIndex < 1)
				return true;
			LevelFlow flow = MainProcess.Instance.levelFlow;
			if (flow.IsLevelFinished(lvlIndex))
				return true;
			var level = flow.GetLevelData(lvlIndex);
			if (level.world > ShopManager.c_storyBlockade)
			{
				if (!IsMainStoryUnlocked)
					return false;
			}
			if (lvlIndex >= LevelFlow.c_nbOriginalLevelNumber)
				if (!flow.IsOriginalLevelAllChamped())
					return false;
			if (flow.IsLevelFinished(lvlIndex - 1))
				return true;
			return false;
		}

		public bool DoesProgressionAllowLevel(string levelName)
		{
			LevelFlow flow = MainProcess.Instance.levelFlow;
			return DoesProgressionAllowLevel(flow.GetLevelIndex(levelName));
		}

		public bool IsMainStoryUnlocked
		{
			get
			{
#if !DASHER_NO_IAP
				return m_savable.m_IsStoryQuestUnlocked == c_storyUnlocked;
#else
				return true;
#endif
			}
			set { m_savable.m_IsStoryQuestUnlocked = value ? c_storyUnlocked : c_storyLocked; }
		}

#endregion
	}

	public class DasherSavable
	{
		public class LevelSavable {
			public float bestTime = -1f;
			public int nbTry = 0;
			public int nbComplete = 0;
			public int nbFail = 0;
		}
		public string UserId = null;

		public Dictionary<string, LevelSavable> m_levels;

		public int m_TotalRuns = 0;
		public int m_TotalDeaths = 0;
		public int m_TotalJumps = 0;
		public int m_TotalDashes = 0;

		public string m_LastLevelPlayed = null;

		public DasherSettings m_settings;

		public int m_IsStoryQuestUnlocked = SaveManager.c_storyLocked;

		public DasherSavable(int nbLevels)
		{
			m_levels = new Dictionary<string, LevelSavable>(nbLevels);
			m_settings = new DasherSettings();
		}
	}

	public struct DasherSettings
	{
		public bool isLefthanded;
		public int SelectedLanguage;

		public DasherSettings(bool leftHand = false, int selectedLanguage = -1)
		{
			isLefthanded = leftHand;
			SelectedLanguage = selectedLanguage;
		}
	}
}