using Grpc.Core;
using System;
using System.Threading; 

namespace Client
{
    class Program 
    {
        static void Main(string[] args)
        {
            try
            {
                // creates the channel
                Channel channel = new Channel("localhost", 23232, ChannelCredentials.Insecure);
                    
                Console.WriteLine("Client started");

                int tasks, milliseconds;
                Random random = new Random();

                while (true)
                {
                    tasks = random.Next(1, 100001); // get random tasks number
                    new Thread(temp => Request(tasks, channel)).Start(); // send the request without blocking

                    // wait up to 3 seconds to send the next request
                    milliseconds = random.Next(0, 3001);
                    Thread.Sleep(milliseconds);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception at main: " + e.Message);
            }
        }

        /// <summary>
        /// <c>Request</c>
        /// sends the request to the scaling server.
        /// </summary>
        /// <param name="tasks">
        /// The number of tasks to request from the server.
        /// </param>
        /// <param name="channel">
        /// The channel for the call.
        /// </param>
        static void Request(int tasks, Channel channel)
        {
            try
            {
                ScalingService.Generated.ScalingService.ScalingServiceClient client = new ScalingService.Generated.ScalingService.ScalingServiceClient(channel);

                var data = new ScalingService.Generated.RequestData() { Id = "", Tasks = tasks, Close = false };

                var response = client.Request(data);

                if (response.Id == "$")
                    return;

                // wait 30 seconds - 2 minutes to send the close request 
                int seconds = new Random().Next(30, 121);
                Thread.Sleep(seconds * 1000);

                data.Close = true;
                data.Id = response.Id;
                client.Request(data);
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception at thread: " + e.Message);
            }
        }
    }
}
