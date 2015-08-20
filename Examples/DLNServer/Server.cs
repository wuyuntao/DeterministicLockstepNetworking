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
