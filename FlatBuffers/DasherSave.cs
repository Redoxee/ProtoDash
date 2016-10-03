// automatically generated by the FlatBuffers compiler, do not modify

namespace DasherSave
{

using System;
using FlatBuffers;

public sealed class LevelSave : Table {
  public static LevelSave GetRootAsLevelSave(ByteBuffer _bb) { return GetRootAsLevelSave(_bb, new LevelSave()); }
  public static LevelSave GetRootAsLevelSave(ByteBuffer _bb, LevelSave obj) { return (obj.__init(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public LevelSave __init(int _i, ByteBuffer _bb) { bb_pos = _i; bb = _bb; return this; }

  public string LevelId { get { int o = __offset(4); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetLevelIdBytes() { return __vector_as_arraysegment(4); }
  public float BestTime { get { int o = __offset(6); return o != 0 ? bb.GetFloat(o + bb_pos) : (float)0.0f; } }

  public static Offset<LevelSave> CreateLevelSave(FlatBufferBuilder builder,
      StringOffset LevelIdOffset = default(StringOffset),
      float BestTime = 0.0f) {
    builder.StartObject(2);
    LevelSave.AddBestTime(builder, BestTime);
    LevelSave.AddLevelId(builder, LevelIdOffset);
    return LevelSave.EndLevelSave(builder);
  }

  public static void StartLevelSave(FlatBufferBuilder builder) { builder.StartObject(2); }
  public static void AddLevelId(FlatBufferBuilder builder, StringOffset LevelIdOffset) { builder.AddOffset(0, LevelIdOffset.Value, 0); }
  public static void AddBestTime(FlatBufferBuilder builder, float BestTime) { builder.AddFloat(1, BestTime, 0.0f); }
  public static Offset<LevelSave> EndLevelSave(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<LevelSave>(o);
  }
};

public sealed class GameSave : Table {
  public static GameSave GetRootAsGameSave(ByteBuffer _bb) { return GetRootAsGameSave(_bb, new GameSave()); }
  public static GameSave GetRootAsGameSave(ByteBuffer _bb, GameSave obj) { return (obj.__init(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public GameSave __init(int _i, ByteBuffer _bb) { bb_pos = _i; bb = _bb; return this; }

  public string Version { get { int o = __offset(4); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetVersionBytes() { return __vector_as_arraysegment(4); }
  public LevelSave GetLevelResults(int j) { return GetLevelResults(new LevelSave(), j); }
  public LevelSave GetLevelResults(LevelSave obj, int j) { int o = __offset(6); return o != 0 ? obj.__init(__indirect(__vector(o) + j * 4), bb) : null; }
  public int LevelResultsLength { get { int o = __offset(6); return o != 0 ? __vector_len(o) : 0; } }

  public static Offset<GameSave> CreateGameSave(FlatBufferBuilder builder,
      StringOffset VersionOffset = default(StringOffset),
      VectorOffset levelResultsOffset = default(VectorOffset)) {
    builder.StartObject(2);
    GameSave.AddLevelResults(builder, levelResultsOffset);
    GameSave.AddVersion(builder, VersionOffset);
    return GameSave.EndGameSave(builder);
  }

  public static void StartGameSave(FlatBufferBuilder builder) { builder.StartObject(2); }
  public static void AddVersion(FlatBufferBuilder builder, StringOffset VersionOffset) { builder.AddOffset(0, VersionOffset.Value, 0); }
  public static void AddLevelResults(FlatBufferBuilder builder, VectorOffset levelResultsOffset) { builder.AddOffset(1, levelResultsOffset.Value, 0); }
  public static VectorOffset CreateLevelResultsVector(FlatBufferBuilder builder, Offset<LevelSave>[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddOffset(data[i].Value); return builder.EndVector(); }
  public static void StartLevelResultsVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static Offset<GameSave> EndGameSave(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<GameSave>(o);
  }
  public static void FinishGameSaveBuffer(FlatBufferBuilder builder, Offset<GameSave> offset) { builder.Finish(offset.Value); }
};


}