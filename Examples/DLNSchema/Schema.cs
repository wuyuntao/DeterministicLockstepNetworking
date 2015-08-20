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

        public static byte[] CreateSyncFrame(DeterministicLockstepNetworking.CommandFrame frame)
        {
            throw new NotImplementedException();
        }
    }
}
