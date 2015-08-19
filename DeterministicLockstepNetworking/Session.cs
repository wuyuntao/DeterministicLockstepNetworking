using System;
using System.Collections.Generic;

namespace DeterministicLockstepNetworking
{
    public sealed class Session
    {
        public uint SessionId { get; private set; }

        private List<Command> commands = new List<Command>();

        internal Session(uint sessionId)
        {
            SessionId = sessionId;
        }

        internal void ReceiveCommand(Command command)
        {
            if (command.SessionId != SessionId)
                throw new ArgumentException("SessionId does not match");

            this.commands.Add(command);
        }

        public IEnumerable<Command> FetchCommands()
        {
            if (this.commands.Count == 0)
            {
                return null;
            }

            var commands = this.commands;
            this.commands = new List<Command>();
            return commands;
        }
    }
}