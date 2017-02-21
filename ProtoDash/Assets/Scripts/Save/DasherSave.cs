// automatically generated by the FlatBuffers compiler, do not modify

namespace DasherSave
{

using System;
using FlatBuffers;

public sealed class FlatLevelSave : Table {
  public static FlatLevelSave GetRootAsFlatLevelSave(ByteBuffer _bb) { return GetRootAsFlatLevelSave(_bb, new FlatLevelSave()); }
  public static FlatLevelSave GetRootAsFlatLevelSave(ByteBuffer _bb, FlatLevelSave obj) { return (obj.__init(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public FlatLevelSave __init(int _i, ByteBuffer _bb) { bb_pos = _i; bb = _bb; return this; }

  public string LevelId { get { int o = __offset(4); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetLevelIdBytes() { return __vector_as_arraysegment(4); }
  public float BestTime { get { int o = __offset(6); return o != 0 ? bb.GetFloat(o + bb_pos) : (float)0.0f; } }
  public int NbTry { get { int o = __offset(8); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int NbComplete { get { int o = __offset(10); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }

  public static Offset<FlatLevelSave> CreateFlatLevelSave(FlatBufferBuilder builder,
      StringOffset LevelIdOffset = default(StringOffset),
      float BestTime = 0.0f,
      int NbTry = 0,
      int NbComplete = 0) {
    builder.StartObject(4);
    FlatLevelSave.AddNbComplete(builder, NbComplete);
    FlatLevelSave.AddNbTry(builder, NbTry);
    FlatLevelSave.AddBestTime(builder, BestTime);
    FlatLevelSave.AddLevelId(builder, LevelIdOffset);
    return FlatLevelSave.EndFlatLevelSave(builder);
  }

  public static void StartFlatLevelSave(FlatBufferBuilder builder) { builder.StartObject(4); }
  public static void AddLevelId(FlatBufferBuilder builder, StringOffset LevelIdOffset) { builder.AddOffset(0, LevelIdOffset.Value, 0); }
  public static void AddBestTime(FlatBufferBuilder builder, float BestTime) { builder.AddFloat(1, BestTime, 0.0f); }
  public static void AddNbTry(FlatBufferBuilder builder, int NbTry) { builder.AddInt(2, NbTry, 0); }
  public static void AddNbComplete(FlatBufferBuilder builder, int NbComplete) { builder.AddInt(3, NbComplete, 0); }
  public static Offset<FlatLevelSave> EndFlatLevelSave(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<FlatLevelSave>(o);
  }
};

public sealed class FlatSettings : Table {
  public static FlatSettings GetRootAsFlatSettings(ByteBuffer _bb) { return GetRootAsFlatSettings(_bb, new FlatSettings()); }
  public static FlatSettings GetRootAsFlatSettings(ByteBuffer _bb, FlatSettings obj) { return (obj.__init(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public FlatSettings __init(int _i, ByteBuffer _bb) { bb_pos = _i; bb = _bb; return this; }

  public bool LeftHandedMode { get { int o = __offset(4); return o != 0 ? 0!=bb.Get(o + bb_pos) : (bool)false; } }

  public static Offset<FlatSettings> CreateFlatSettings(FlatBufferBuilder builder,
      bool LeftHandedMode = false) {
    builder.StartObject(1);
    FlatSettings.AddLeftHandedMode(builder, LeftHandedMode);
    return FlatSettings.EndFlatSettings(builder);
  }

  public static void StartFlatSettings(FlatBufferBuilder builder) { builder.StartObject(1); }
  public static void AddLeftHandedMode(FlatBufferBuilder builder, bool LeftHandedMode) { builder.AddBool(0, LeftHandedMode, false); }
  public static Offset<FlatSettings> EndFlatSettings(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<FlatSettings>(o);
  }
};

public sealed class FlatGameSave : Table {
  public static FlatGameSave GetRootAsFlatGameSave(ByteBuffer _bb) { return GetRootAsFlatGameSave(_bb, new FlatGameSave()); }
  public static FlatGameSave GetRootAsFlatGameSave(ByteBuffer _bb, FlatGameSave obj) { return (obj.__init(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public FlatGameSave __init(int _i, ByteBuffer _bb) { bb_pos = _i; bb = _bb; return this; }

  public int Version { get { int o = __offset(4); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public string UserId { get { int o = __offset(6); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetUserIdBytes() { return __vector_as_arraysegment(6); }
  public FlatLevelSave GetLevelResults(int j) { return GetLevelResults(new FlatLevelSave(), j); }
  public FlatLevelSave GetLevelResults(FlatLevelSave obj, int j) { int o = __offset(8); return o != 0 ? obj.__init(__indirect(__vector(o) + j * 4), bb) : null; }
  public int LevelResultsLength { get { int o = __offset(8); return o != 0 ? __vector_len(o) : 0; } }
  public int TotalRuns { get { int o = __offset(10); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int TotalJumps { get { int o = __offset(12); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int TotalDashes { get { int o = __offset(14); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public string LastPlayedLevel { get { int o = __offset(16); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetLastPlayedLevelBytes() { return __vector_as_arraysegment(16); }
  public FlatSettings Settings { get { return GetSettings(new FlatSettings()); } }
  public FlatSettings GetSettings(FlatSettings obj) { int o = __offset(18); return o != 0 ? obj.__init(__indirect(o + bb_pos), bb) : null; }
  public int TotalDeaths { get { int o = __offset(20); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }

  public static Offset<FlatGameSave> CreateFlatGameSave(FlatBufferBuilder builder,
      int Version = 0,
      StringOffset UserIdOffset = default(StringOffset),
      VectorOffset levelResultsOffset = default(VectorOffset),
      int TotalRuns = 0,
      int TotalJumps = 0,
      int TotalDashes = 0,
      StringOffset LastPlayedLevelOffset = default(StringOffset),
      Offset<FlatSettings> SettingsOffset = default(Offset<FlatSettings>),
      int TotalDeaths = 0) {
    builder.StartObject(9);
    FlatGameSave.AddTotalDeaths(builder, TotalDeaths);
    FlatGameSave.AddSettings(builder, SettingsOffset);
    FlatGameSave.AddLastPlayedLevel(builder, LastPlayedLevelOffset);
    FlatGameSave.AddTotalDashes(builder, TotalDashes);
    FlatGameSave.AddTotalJumps(builder, TotalJumps);
    FlatGameSave.AddTotalRuns(builder, TotalRuns);
    FlatGameSave.AddLevelResults(builder, levelResultsOffset);
    FlatGameSave.AddUserId(builder, UserIdOffset);
    FlatGameSave.AddVersion(builder, Version);
    return FlatGameSave.EndFlatGameSave(builder);
  }

  public static void StartFlatGameSave(FlatBufferBuilder builder) { builder.StartObject(9); }
  public static void AddVersion(FlatBufferBuilder builder, int Version) { builder.AddInt(0, Version, 0); }
  public static void AddUserId(FlatBufferBuilder builder, StringOffset UserIdOffset) { builder.AddOffset(1, UserIdOffset.Value, 0); }
  public static void AddLevelResults(FlatBufferBuilder builder, VectorOffset levelResultsOffset) { builder.AddOffset(2, levelResultsOffset.Value, 0); }
  public static VectorOffset CreateLevelResultsVector(FlatBufferBuilder builder, Offset<FlatLevelSave>[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddOffset(data[i].Value); return builder.EndVector(); }
  public static void StartLevelResultsVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static void AddTotalRuns(FlatBufferBuilder builder, int TotalRuns) { builder.AddInt(3, TotalRuns, 0); }
  public static void AddTotalJumps(FlatBufferBuilder builder, int TotalJumps) { builder.AddInt(4, TotalJumps, 0); }
  public static void AddTotalDashes(FlatBufferBuilder builder, int TotalDashes) { builder.AddInt(5, TotalDashes, 0); }
  public static void AddLastPlayedLevel(FlatBufferBuilder builder, StringOffset LastPlayedLevelOffset) { builder.AddOffset(6, LastPlayedLevelOffset.Value, 0); }
  public static void AddSettings(FlatBufferBuilder builder, Offset<FlatSettings> SettingsOffset) { builder.AddOffset(7, SettingsOffset.Value, 0); }
  public static void AddTotalDeaths(FlatBufferBuilder builder, int TotalDeaths) { builder.AddInt(8, TotalDeaths, 0); }
  public static Offset<FlatGameSave> EndFlatGameSave(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<FlatGameSave>(o);
  }
  public static void FinishFlatGameSaveBuffer(FlatBufferBuilder builder, Offset<FlatGameSave> offset) { builder.Finish(offset.Value); }
};


}
