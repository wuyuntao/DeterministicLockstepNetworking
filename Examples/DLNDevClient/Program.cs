using DLNSchema;
using System;

namespace DLNDevClient
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Press any key to start.");
            Console.ReadKey();

            var client = new Client("127.0.0.1", 4000, new ClientHandler());

            Console.WriteLine("Press any key to stop.");
            Console.ReadKey();
        }

        #region Client Handler

        class ClientHandler : IClientHandler
        {
            public void Log(string msg, params object[] args)
            {
                Console.WriteLine(msg, args);
            }

            public void LogError(string msg, params object[] args)
            {
                Console.WriteLine(msg, args);
            }

            public void OnConnect()
            {
                Console.WriteLine("OnConnect");
            }

            public void OnSync()
            {
                Console.WriteLine("OnSync");
            }
        }
        
        #endregion
    }
}
