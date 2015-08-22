// automatically generated, do not modify

namespace DLNSchema.Messages
{

using FlatBuffers;

public sealed class InputCommand : Table {
  public static InputCommand GetRootAsInputCommand(ByteBuffer _bb) { return GetRootAsInputCommand(_bb, new InputCommand()); }
  public static InputCommand GetRootAsInputCommand(ByteBuffer _bb, InputCommand obj) { return (obj.__init(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public InputCommand __init(int _i, ByteBuffer _bb) { bb_pos = _i; bb = _bb; return this; }

  public CommandFrame Frame { get { return GetFrame(new CommandFrame()); } }
  public CommandFrame GetFrame(CommandFrame obj) { int o = __offset(4); return o != 0 ? obj.__init(__indirect(o + bb_pos), bb) : null; }

  public static Offset<InputCommand> CreateInputCommand(FlatBufferBuilder builder,
      Offset<CommandFrame> frame = default(Offset<CommandFrame>)) {
    builder.StartObject(1);
    InputCommand.AddFrame(builder, frame);
    return InputCommand.EndInputCommand(builder);
  }

  public static void StartInputCommand(FlatBufferBuilder builder) { builder.StartObject(1); }
  public static void AddFrame(FlatBufferBuilder builder, Offset<CommandFrame> frameOffset) { builder.AddOffset(0, frameOffset.Value, 0); }
  public static Offset<InputCommand> EndInputCommand(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<InputCommand>(o);
  }
  public static void FinishInputCommandBuffer(FlatBufferBuilder builder, Offset<InputCommand> offset) { builder.Finish(offset.Value); }
};


}
