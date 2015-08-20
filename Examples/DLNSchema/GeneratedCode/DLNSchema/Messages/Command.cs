// automatically generated, do not modify

namespace DLNSchema.Messages
{

using FlatBuffers;

public sealed class Command : Table {
  public static Command GetRootAsCommand(ByteBuffer _bb) { return GetRootAsCommand(_bb, new Command()); }
  public static Command GetRootAsCommand(ByteBuffer _bb, Command obj) { return (obj.__init(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public Command __init(int _i, ByteBuffer _bb) { bb_pos = _i; bb = _bb; return this; }

  public uint CommandId { get { int o = __offset(4); return o != 0 ? bb.GetUint(o + bb_pos) : (uint)0; } }
  public uint SessionId { get { int o = __offset(6); return o != 0 ? bb.GetUint(o + bb_pos) : (uint)0; } }

  public static int CreateCommand(FlatBufferBuilder builder,
      uint commandId = 0,
      uint sessionId = 0) {
    builder.StartObject(2);
    Command.AddSessionId(builder, sessionId);
    Command.AddCommandId(builder, commandId);
    return Command.EndCommand(builder);
  }

  public static void StartCommand(FlatBufferBuilder builder) { builder.StartObject(2); }
  public static void AddCommandId(FlatBufferBuilder builder, uint commandId) { builder.AddUint(0, commandId, 0); }
  public static void AddSessionId(FlatBufferBuilder builder, uint sessionId) { builder.AddUint(1, sessionId, 0); }
  public static int EndCommand(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return o;
  }
};


}
