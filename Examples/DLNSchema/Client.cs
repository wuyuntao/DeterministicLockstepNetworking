using FlatBuffers.Schema;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading;

namespace DLNSchema
{
    public interface IClientHandler
    {
        void Log(string msg, params object[] args);

        void LogError(string msg, params object[] args);

        void OnConnect();

        void OnSync();
    }

    public sealed class Client
    {
        private IClientHandler handler;

        private uint sessionId;

        private TcpClient tcpClient;

        private NetworkStream networkStream;

        private List<CommandFrame> frames = new List<CommandFrame>();

        private object framesLock = new object();

        public Client(IClientHandler handler)
        {
            this.handler = handler;
            this.tcpClient = new TcpClient();
            this.tcpClient.Connect("127.0.0.1", 4000);
            this.networkStream = this.tcpClient.GetStream();

            this.handler.Log("Connected");

            ThreadPool.QueueUserWorkItem(ReadThread);
        }

        private void ReadThread(object state)
        {
            var queue = new MessageQueue(Schemas.ClientSchema);

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

            this.handler.Log("End");
        }

        private void ProcessMessage(Message m)
        {
            var messageId = (DLNSchema.Messages.MessageIds)m.Id;

            switch (messageId)
            {
                case DLNSchema.Messages.MessageIds.AssignSessionId:
                    {
                        var message = (DLNSchema.Messages.AssignSessionId)m.Body;

                        this.sessionId = message.SessionId;

                        this.handler.OnConnect();
                    }
                    break;

                case DLNSchema.Messages.MessageIds.SyncFrame:
                    {
                        if (this.sessionId == 0)
                            throw new InvalidOperationException("SessionId is not assigned yet");

                        var message = (DLNSchema.Messages.SyncFrame)m.Body;
                        for (int i = 0; i < message.FramesLength; i++)
                        {
                            var f = message.GetFrames(i);

                            var commands = new Command[f.CommandsLength];
                            for (int j = 0; j < f.CommandsLength; j++)
                            {
                                var c = f.GetCommands(j);

                                commands[j] = new Command(c.CommandId, c.SessionId);
                            }

                            var frame = new CommandFrame(f.Ticks, commands);

                            lock (this.framesLock)
                            {
                                this.frames.Add(frame);
                            }

                            this.handler.OnSync();
                        }
                    }
                    break;

                default:
                    throw new InvalidOperationException(string.Format("Unexpected message: {0}", messageId));
            }
        }

        public void SendCommand(uint ticks, Command command)
        {
            var frame = new CommandFrame(ticks, new Command[] { command });

            ThreadPool.QueueUserWorkItem(WriteThread, frame);
        }

        private void WriteThread(object state)
        {
            var frame = (CommandFrame)state;
            var bytes = Schemas.CreateInputCommand(frame);

            this.networkStream.Write(bytes, 0, bytes.Length);
        }

        public CommandFrame[] FetchCommandFrames()
        {
            CommandFrame[] frames = null;

            lock (this.framesLock)
            {
                if (this.frames.Count > 0)
                {
                    frames = this.frames.ToArray();

                    this.frames.Clear();
                }
            }

            return frames;
        }

        public uint SessionId
        {
            get { return this.sessionId; }
        }
    }
}