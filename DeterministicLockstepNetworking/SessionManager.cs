using System;
using System.Collections.Generic;

namespace DeterministicLockstepNetworking
{
    public sealed class SessionManager
    {
        private uint currentSessionId;

        private Dictionary<uint, Session> sessions = new Dictionary<uint, Session>();

        private List<Command> commands = new List<Command>();

        private uint currentTicks;

        public SessionManager(uint currentSessionId)
        {
            this.currentSessionId = currentSessionId;

            AddSession(currentSessionId);
        }

        public Session AddSession(uint sessionId)
        {
            var session = new Session(sessionId);
            this.sessions.Add(sessionId, session);
            return session;
        }

        public Session FindSession(uint sessionId)
        {
            Session session;
            this.sessions.TryGetValue(sessionId, out session);
            return session;
        }

        public bool ReceiveCommands(CommandFrame frame)
        {
            if (frame.Ticks != this.currentTicks + 1)
                return false;

            if (frame.Commands != null)
            {
                foreach (var command in frame.Commands)
                {
                    var session = FindSession(command.SessionId);
                    if (session != null)
                    {
                        session.ReceiveCommand(command);
                    }
                    else
                    {
                        throw new InvalidOperationException(string.Format("Session not exist: {0}", command.SessionId));
                    }
                }
            }

            this.currentTicks = frame.Ticks;
            return true;
        }

        public void SendCommand(uint commandId)
        {
            this.commands.Add(new Command(commandId, this.currentSessionId));
        }

        public CommandFrame FetchSendCommands()
        {
            if (this.commands.Count == 0)
                return null;

            var frame = new CommandFrame(this.currentTicks + 1, this.commands.ToArray());

            this.commands.Clear();

            return frame;
        }
    }
}