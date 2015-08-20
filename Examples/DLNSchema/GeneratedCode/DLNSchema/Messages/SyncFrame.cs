// automatically generated, do not modify

namespace DLNSchema.Messages
{

using FlatBuffers;

public sealed class SyncFrame : Table {
  public static SyncFrame GetRootAsSyncFrame(ByteBuffer _bb) { return GetRootAsSyncFrame(_bb, new SyncFrame()); }
  public static SyncFrame GetRootAsSyncFrame(ByteBuffer _bb, SyncFrame obj) { return (obj.__init(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public SyncFrame __init(int _i, ByteBuffer _bb) { bb_pos = _i; bb = _bb; return this; }

  public CommandFrame GetFrames(int j) { return GetFrames(new CommandFrame(), j); }
  public CommandFrame GetFrames(CommandFrame obj, int j) { int o = __offset(4); return o != 0 ? obj.__init(__indirect(__vector(o) + j * 4), bb) : null; }
  public int FramesLength { get { int o = __offset(4); return o != 0 ? __vector_len(o) : 0; } }

  public static int CreateSyncFrame(FlatBufferBuilder builder,
      int frames = 0) {
    builder.StartObject(1);
    SyncFrame.AddFrames(builder, frames);
    return SyncFrame.EndSyncFrame(builder);
  }

  public static void StartSyncFrame(FlatBufferBuilder builder) { builder.StartObject(1); }
  public static void AddFrames(FlatBufferBuilder builder, int framesOffset) { builder.AddOffset(0, framesOffset, 0); }
  public static int CreateFramesVector(FlatBufferBuilder builder, int[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddOffset(data[i]); return builder.EndVector(); }
  public static void StartFramesVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static int EndSyncFrame(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return o;
  }
  public static void FinishSyncFrameBuffer(FlatBufferBuilder builder, int offset) { builder.Finish(offset); }
};


}
