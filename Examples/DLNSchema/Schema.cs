using DLNSchema.Messages;
using FlatBuffers;
using FlatBuffers.Schema;

namespace DLNSchema
{
    public static class Schemas
    {
        private static MessageSchema clientSchema;

        private static MessageSchema serverSchema;

        public static MessageSchema ClientSchema
        {
            get
            {
                if (clientSchema == null)
                {
                    clientSchema = new MessageSchema();
                    clientSchema.Register(MessageIds.SyncFrame, SyncFrame.GetRootAsSyncFrame);
                    clientSchema.Register(MessageIds.AssignSessionId, AssignSessionId.GetRootAsAssignSessionId);
                }

                return clientSchema;
            }
        }

        public static MessageSchema ServerSchema
        {
            get
            {
                if (serverSchema == null)
                {
                    serverSchema = new MessageSchema();
                    serverSchema.Register(MessageIds.InputCommand, InputCommand.GetRootAsInputCommand);
                }

                return serverSchema;
            }
        }

        public static byte[] CreateSyncFrame(CommandFrame[] frames)
        {
            var fbb = new FlatBufferBuilder(1024);

            var oFrames = CreateFrameOffsets(fbb, frames);
            var vFrames = Messages.SyncFrame.CreateFramesVector(fbb, oFrames);

            var oSyncFrame = Messages.SyncFrame.CreateSyncFrame(fbb, vFrames);
            Messages.SyncFrame.FinishSyncFrameBuffer(fbb, oSyncFrame);

            return fbb.ToProtocolMessage(MessageIds.SyncFrame);
        }

        private static Offset<Messages.CommandFrame>[] CreateFrameOffsets(FlatBufferBuilder fbb, CommandFrame[] frames)
        {
            if (frames == null || frames.Length == 0)
                return new Offset<Messages.CommandFrame>[0];

            var oFrames = new Offset<Messages.CommandFrame>[frames.Length];

            int i = 0;
            foreach (var f in frames)
            {
                Offset<Messages.Command>[] oCommands = null;
                if (f.Commands != null && f.Commands.Length > 0)
                {
                    oCommands = new Offset<Messages.Command>[f.Commands.Length];

                    int j = 0;
                    foreach (var c in f.Commands)
                    {
                        oCommands[j] = Messages.Command.CreateCommand(fbb, c.CommandId, c.SessionId);
                        j++;
                    }
                }
                else
                {
                    oCommands = new Offset<Messages.Command>[0];
                }

                var vCommands = Messages.CommandFrame.CreateCommandsVector(fbb, oCommands);
                oFrames[i] = Messages.CommandFrame.CreateCommandFrame(fbb, f.Ticks, vCommands);

                i++;
            }

            return oFrames;
        }

        public static byte[] CreateInputCommand(CommandFrame frame)
        {
            var fbb = new FlatBufferBuilder(1024);

            var oFrame = CreateCommandFrameOffset(frame, fbb);

            var oInputCommand = Messages.InputCommand.CreateInputCommand(fbb, oFrame);
            Messages.InputCommand.FinishInputCommandBuffer(fbb, oInputCommand);

            return fbb.ToProtocolMessage(MessageIds.InputCommand);
        }

        private static Offset<Messages.CommandFrame> CreateCommandFrameOffset(CommandFrame frame, FlatBufferBuilder fbb)
        {
            Offset<Messages.Command>[] oCommands = null;
            if (frame.Commands != null && frame.Commands.Length > 0)
            {
                oCommands = new Offset<Messages.Command>[frame.Commands.Length];

                int j = 0;
                foreach (var c in frame.Commands)
                {
                    oCommands[j] = Messages.Command.CreateCommand(fbb, c.CommandId, c.SessionId);
                    j++;
                }
            }
            else
            {
                oCommands = new Offset<Messages.Command>[0];
            }

            var vCommand = Messages.CommandFrame.CreateCommandsVector(fbb, oCommands);

            return Messages.CommandFrame.CreateCommandFrame(fbb, frame.Ticks, vCommand);
        }

        public static byte[] CreateAssignSessionId(uint sessionId)
        {
            var fbb = new FlatBufferBuilder(1024);

            var oAssignSessionId = Messages.AssignSessionId.CreateAssignSessionId(fbb, sessionId);
            Messages.AssignSessionId.FinishAssignSessionIdBuffer(fbb, oAssignSessionId);

            return fbb.ToProtocolMessage(MessageIds.AssignSessionId);
        }
    }
}