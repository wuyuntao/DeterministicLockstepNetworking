using System;
using System.Collections.Generic;

namespace DeterministicLockstepNetworking
{
    public sealed class SessionManager
    {
        private Dictionary<uint, Session> sessions = new Dictionary<uint, Session>();

        private List<ICommand> commands = new List<ICommand>();

        private uint currentTicks;

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

        public bool ReceiveCommands(uint ticks, ICommand[] commands)
        {
            if (ticks != this.currentTicks + 1)
                return false;

            if (commands != null)
            {
                foreach (var command in commands)
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

            this.currentTicks = ticks;
            return true;
        }

        public void SendCommand(ICommand command)
        {
            this.commands.Add(command);
        }

        public ICommand[] FetchCommands()
        {
            if (this.commands.Count == 0)
                return null;

            var commands = this.commands.ToArray();

            this.commands.Clear();

            return commands;
        }

        public uint CurrentTicks
        {
            get { return this.currentTicks; }
        }
    }
}