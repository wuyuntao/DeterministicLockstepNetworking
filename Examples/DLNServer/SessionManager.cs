using DeterministicLockstepNetworking;
using DLNSchema;
using System.Collections.Generic;
using System.Threading;

namespace DLNServer
{
    class SessionManager
    {
        private const int SyncInterval = 1000;

        private uint nextSessionId;

        private List<Session> sessions = new List<Session>();

        private object sessionsLock = new object();

        private bool isEnded = false;

        private uint currentTicks;

        private List<Command> commands = new List<Command>();

        private object commandsLock = new object();

        private List<CommandFrame> totalFrames = new List<CommandFrame>();

        private object totalFramesLock = new object();

        public SessionManager()
        {
            ThreadPool.QueueUserWorkItem(SynchronizeFrame);
        }

        #region Session

        public uint GetNextSessionId()
        {
            return ++this.nextSessionId;
        }

        public void AddSession(Session session)
        {
            lock (this.sessionsLock)
            {
                this.sessions.Add(session);
            }
        }

        public void RemoveSession(Session session)
        {
            lock (this.sessionsLock)
            {
                this.sessions.Remove(session);

                this.isEnded = true;
            }
        }

        public void RemoveAllSessions()
        {
            lock (this.sessionsLock)
            {
                this.sessions.Clear();
            }
        }

        #endregion

        #region Synchronization

        public void AddCommand(Command command)
        {
            lock (this.commandsLock)
            {
                this.commands.Add(command);
            }
        }

        private void SynchronizeFrame(object state)
        {
            while (!isEnded)
            {
                Thread.Sleep(SyncInterval);

                this.currentTicks++;

                CommandFrame frame;
                lock (this.commandsLock)
                {
                    frame = new CommandFrame(this.currentTicks, this.commands.ToArray());

                    this.commands.Clear();
                }
                
                lock(this.totalFramesLock)
                {
                    this.totalFrames.Add(frame);
                }

                lock (this.sessionsLock)
                {
                    foreach (var session in this.sessions)
                    {
                        session.SynchronizeCommandFrame(frame);
                    }
                }
            }
        }

        public CommandFrame[] FetchTotalFrames()
        {
            CommandFrame[] frames;
            lock (this.totalFramesLock)
            {
                frames = this.totalFrames.ToArray();
            }

            return frames;
        }

        #endregion
    }
}
