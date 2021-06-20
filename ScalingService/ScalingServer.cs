using Grpc.Core;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ScalingService
{
    /// <summary>
    /// Class
    /// <c>ScalingServer</c>
    /// wraps the server object.
    /// </summary>
    public class ScalingServer
    {
        private readonly Server _server; // actaul server object

        /// <summary>
        /// <c>ScalingServer</c>
        /// constructor that creates the server object with the ServerImplementation.
        /// </summary>
        public ScalingServer() 
        {
            _server = new Server()
            {
                Services = { Generated.ScalingService.BindService(new ServerImplementation()) },
                Ports = { new ServerPort("localhost", 23232, ServerCredentials.Insecure) }
            };
        }

        public void Start()
        {
            _server.Start();
        }
    }
}
