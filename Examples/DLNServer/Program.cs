using System;

namespace DLNServer
{
    class Program
    {
        static void Main(string[] args)
        {
            var server = new Server(4000);

            Console.WriteLine("Press any key to stop server");
            Console.ReadKey();

            server.Close();

            Console.ReadKey();
        }
    }
}