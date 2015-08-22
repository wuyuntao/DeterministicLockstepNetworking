// automatically generated, do not modify

namespace DLNSchema.Messages
{

using FlatBuffers;

public sealed class AssignSessionId : Table {
  public static AssignSessionId GetRootAsAssignSessionId(ByteBuffer _bb) { return GetRootAsAssignSessionId(_bb, new AssignSessionId()); }
  public static AssignSessionId GetRootAsAssignSessionId(ByteBuffer _bb, AssignSessionId obj) { return (obj.__init(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public AssignSessionId __init(int _i, ByteBuffer _bb) { bb_pos = _i; bb = _bb; return this; }

  public uint SessionId { get { int o = __offset(4); return o != 0 ? bb.GetUint(o + bb_pos) : (uint)0; } }

  public static Offset<AssignSessionId> CreateAssignSessionId(FlatBufferBuilder builder,
      uint sessionId = 0) {
    builder.StartObject(1);
    AssignSessionId.AddSessionId(builder, sessionId);
    return AssignSessionId.EndAssignSessionId(builder);
  }

  public static void StartAssignSessionId(FlatBufferBuilder builder) { builder.StartObject(1); }
  public static void AddSessionId(FlatBufferBuilder builder, uint sessionId) { builder.AddUint(0, sessionId, 0); }
  public static Offset<AssignSessionId> EndAssignSessionId(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<AssignSessionId>(o);
  }
  public static void FinishAssignSessionIdBuffer(FlatBufferBuilder builder, Offset<AssignSessionId> offset) { builder.Finish(offset.Value); }
};


}
