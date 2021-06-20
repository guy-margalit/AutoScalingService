using System;

namespace ScalingService
{
    class Program
    {
        static void Main(string[] args)
        {
            ScalingServer server = new ScalingServer();
            server.Start();
            Console.WriteLine("Started server");
            Console.ReadKey();
        }
    }
}
