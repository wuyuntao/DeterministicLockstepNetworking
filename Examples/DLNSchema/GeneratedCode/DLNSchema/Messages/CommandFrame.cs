// automatically generated, do not modify

namespace DLNSchema.Messages
{

using FlatBuffers;

public sealed class CommandFrame : Table {
  public static CommandFrame GetRootAsCommandFrame(ByteBuffer _bb) { return GetRootAsCommandFrame(_bb, new CommandFrame()); }
  public static CommandFrame GetRootAsCommandFrame(ByteBuffer _bb, CommandFrame obj) { return (obj.__init(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public CommandFrame __init(int _i, ByteBuffer _bb) { bb_pos = _i; bb = _bb; return this; }

  public uint Ticks { get { int o = __offset(4); return o != 0 ? bb.GetUint(o + bb_pos) : (uint)0; } }
  public Command GetCommands(int j) { return GetCommands(new Command(), j); }
  public Command GetCommands(Command obj, int j) { int o = __offset(6); return o != 0 ? obj.__init(__indirect(__vector(o) + j * 4), bb) : null; }
  public int CommandsLength { get { int o = __offset(6); return o != 0 ? __vector_len(o) : 0; } }

  public static Offset<CommandFrame> CreateCommandFrame(FlatBufferBuilder builder,
      uint ticks = 0,
      VectorOffset commands = default(VectorOffset)) {
    builder.StartObject(2);
    CommandFrame.AddCommands(builder, commands);
    CommandFrame.AddTicks(builder, ticks);
    return CommandFrame.EndCommandFrame(builder);
  }

  public static void StartCommandFrame(FlatBufferBuilder builder) { builder.StartObject(2); }
  public static void AddTicks(FlatBufferBuilder builder, uint ticks) { builder.AddUint(0, ticks, 0); }
  public static void AddCommands(FlatBufferBuilder builder, VectorOffset commandsOffset) { builder.AddOffset(1, commandsOffset.Value, 0); }
  public static VectorOffset CreateCommandsVector(FlatBufferBuilder builder, Offset<Command>[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddOffset(data[i].Value); return builder.EndVector(); }
  public static void StartCommandsVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static Offset<CommandFrame> EndCommandFrame(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<CommandFrame>(o);
  }
};


}
