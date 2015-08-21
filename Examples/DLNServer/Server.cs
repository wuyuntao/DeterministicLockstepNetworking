using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace DLNServer
{
    class Server
    {
        private TcpListener tcpListener;

        private bool isClosed = false;

        public Server(int port)
        {
            this.tcpListener = new TcpListener(new IPAddress(0), port);
            this.tcpListener.Start();

            Console.WriteLine("Server started at {0}", this.tcpListener.Server.LocalEndPoint);

            ThreadPool.QueueUserWorkItem(WorkThread);
        }

        public void Close()
        {
            if (!this.isClosed)
            {
                this.tcpListener.Stop();

                this.isClosed = true;
            }
        }

        private void WorkThread(object state)
        {
            var sessionManager = new SessionManager();

            while (!this.isClosed)
            {
                try
                {
                    var tcpClient = this.tcpListener.AcceptTcpClient();

                    var session = new Session(sessionManager, tcpClient);

                    sessionManager.AddSession(session);

                    Console.WriteLine("New session #{0} created", session );
                }
                catch (SocketException)
                {
                    Console.WriteLine("Accept failure");
                    break;
                }
            }

            sessionManager.RemoveAllSessions();
        }
    }
}
