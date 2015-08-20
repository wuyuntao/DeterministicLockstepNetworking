using DeterministicLockstepNetworking;
using DLNSchema;
using FlatBuffers.Schema;
using System;
using System.Net.Sockets;
using System.Threading;

namespace DLNServer
{
    class Session
    {
        private SessionManager manager;

        private uint id;

        private TcpClient tcpClient;

        private NetworkStream networkStream;

        public Session(SessionManager manager, TcpClient tcpClient)
        {
            this.manager = manager;
            this.id = manager.GetNextSessionId();
            this.tcpClient = tcpClient;
            this.networkStream = tcpClient.GetStream();

            ThreadPool.QueueUserWorkItem(ReadThread);
        }

        #region Read

        private void ReadThread(object state)
        {
            var queue = new MessageQueue(Schemas.ServerSchema);

            var buffer = new byte[this.tcpClient.ReceiveBufferSize];

            while (this.networkStream.CanRead)
            {
                var readSize = this.networkStream.Read(buffer, 0, buffer.Length);

                var bytes = new byte[readSize];
                Array.Copy(buffer, bytes, readSize);

                queue.Enqueue(bytes);

                foreach (var message in queue.DequeueAll())
                {
                    ProcessMessage(message);
                }
            }
        }

        private void ProcessMessage(Message m)
        {
            if (m.Id == (int)DLNSchema.Messages.MessageIds.InputCommand)
            {
                var message = (DLNSchema.Messages.InputCommand)m.Body;

                if (message.Frame != null)
                {
                    for (int i = 0; i < message.Frame.CommandsLength; i++)
                    {
                        var command = message.Frame.GetCommands(i);

                        this.manager.AddCommand(new Command(command.CommandId, command.SessionId));
                    }
                }
            }
        }
        
        #endregion

        #region Write

        public void SynchronizeCommandFrame(CommandFrame frame)
        {
            ThreadPool.QueueUserWorkItem(WriteThread, frame);
        }

        private void WriteThread(object state)
        {
            var bytes = Schemas.CreateSyncFrame((CommandFrame)state);

            this.networkStream.Write(bytes, 0, bytes.Length);
        }
        
        #endregion
    }
}