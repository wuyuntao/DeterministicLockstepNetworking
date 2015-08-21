using DLNSchema.Messages;
using FlatBuffers;
using FlatBuffers.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DLNSchema
{
    public static class Schemas
    {
        private static MessageSchema serverSchema;

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

            var framePtrs = CreateFramePtrs(fbb, frames);
            var frameVectorPtr = Messages.SyncFrame.CreateFramesVector(fbb, framePtrs);
            var syncFramePtr = Messages.SyncFrame.CreateSyncFrame(fbb, frameVectorPtr);

            Messages.SyncFrame.FinishSyncFrameBuffer(fbb, syncFramePtr);

            return fbb.ToProtocolMessage(MessageIds.SyncFrame);
        }

        private static int[] CreateFramePtrs(FlatBufferBuilder fbb, CommandFrame[] frames)
        {
            if (frames == null || frames.Length == 0)
                return new int[0];

            var framePtrs = new int[frames.Length];

            int i = 0;
            foreach (var f in frames)
            {
                int[] commandPtrs = null;
                if (f.Commands != null && f.Commands.Length > 0)
                {
                    commandPtrs = new int[f.Commands.Length];

                    int j = 0;
                    foreach (var c in f.Commands)
                    {
                        commandPtrs[j] = Messages.Command.CreateCommand(fbb, c.CommandId, c.SessionId);
                        j++;
                    }
                }
                else
                {
                    commandPtrs = new int[0];
                }

                var commandVectorPtr = Messages.CommandFrame.CreateCommandsVector(fbb, commandPtrs);
                framePtrs[i] = Messages.CommandFrame.CreateCommandFrame(fbb, f.Ticks, commandVectorPtr);
            }

            return framePtrs;
        }

        public static byte[] CreateInputCommand(CommandFrame frame)
        {
            var fbb = new FlatBufferBuilder(1024);

            var commandFramePtr = CreateCommandFramePtr(frame, fbb);
            var inputCommandPtr = Messages.InputCommand.CreateInputCommand(fbb, commandFramePtr);

            Messages.InputCommand.FinishInputCommandBuffer(fbb, inputCommandPtr);

            return fbb.ToProtocolMessage(MessageIds.InputCommand);
        }

        private static int CreateCommandFramePtr(CommandFrame frame, FlatBufferBuilder fbb)
        {
            int[] commandPtrs = null;
            if (frame.Commands != null && frame.Commands.Length > 0)
            {
                commandPtrs = new int[frame.Commands.Length];

                int j = 0;
                foreach (var c in frame.Commands)
                {
                    commandPtrs[j] = Messages.Command.CreateCommand(fbb, c.CommandId, c.SessionId);
                    j++;
                }
            }
            else
            {
                commandPtrs = new int[0];
            }

            var commandVectorPtr = Messages.CommandFrame.CreateCommandsVector(fbb, commandPtrs);

            return Messages.CommandFrame.CreateCommandFrame(fbb, frame.Ticks, commandVectorPtr);
        }
    }
}