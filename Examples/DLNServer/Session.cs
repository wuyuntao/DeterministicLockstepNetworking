using DeterministicLockstepNetworking;
using DLNSchema;
using FlatBuffers.Schema;
using System;
using System.IO;
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

        public override string ToString()
        {
            return string.Format("{0} / {1} / {2}", base.ToString(), this.id, this.tcpClient.Client.RemoteEndPoint);
        }

        #region Read

        private void ReadThread(object state)
        {
            var login = Schemas.CreateAssignSessionId(this.id);
            this.networkStream.Write(login, 0, login.Length);
            Console.WriteLine("AssignSessionId #{0}", this.id);

            var frames = this.manager.FetchTotalFrames();
            this.SynchronizeCommandFrame(frames);
            Console.WriteLine("Synchronize initial frames: {0}", frames.Length);

            var queue = new MessageQueue(Schemas.ServerSchema);
            var buffer = new byte[this.tcpClient.ReceiveBufferSize];

            while (this.networkStream.CanRead)
            {
                int readSize;
                try
                {
                    readSize = this.networkStream.Read(buffer, 0, buffer.Length);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Disconnected: {0}", ex.GetType());
                    break;
                }

                Console.WriteLine("Received {0} bytes", readSize);

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

                if (message.Frame != null && message.Frame.CommandsLength > 0)
                {
                    for (int i = 0; i < message.Frame.CommandsLength; i++)
                    {
                        var command = message.Frame.GetCommands(i);

                        this.manager.AddCommand(new Command(command.CommandId, command.SessionId));

                        Console.WriteLine("Received input command: {0} / {1}", command.CommandId, command.SessionId);
                    }
                }
                else
                {
                    Console.WriteLine("Receieved empty input command");
                }
            }
            else
            {
                Console.WriteLine("Received unexpected message: {0}", m.Id);
            }
        }

        #endregion

        #region Write

        public void SynchronizeCommandFrame(params CommandFrame[] frames)
        {
            ThreadPool.QueueUserWorkItem(WriteThread, frames);
        }

        private void WriteThread(object state)
        {
            if (!this.networkStream.CanWrite)
                return;

            var frames = (CommandFrame[])state;
            var bytes = Schemas.CreateSyncFrame(frames);

            try
            {
                this.networkStream.Write(bytes, 0, bytes.Length);

                Console.WriteLine("{0} frames sent", frames.Length);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Disconnected: {0}", ex.GetType());
            }
        }

        #endregion
    }
}