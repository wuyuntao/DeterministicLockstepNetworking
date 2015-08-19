using System;

namespace DeterministicLockstepNetworking
{
    public sealed class Command
    {
        public uint CommandId { get; private set; }

        public uint SessionId { get; private set; }

        internal Command(uint commandId, uint sessionId)
        {
            CommandId = commandId;
            SessionId = sessionId;
        }
    }
}