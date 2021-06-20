using Grpc.Core;
using ScalingService.Generated;
using System;
using System.Threading.Tasks;

namespace ScalingService
{
    public class ServerImplementation : Generated.ScalingService.ScalingServiceBase
    {

        /// <summary>
        /// <c>Request</c>
        /// handles the scaling request.
        /// </summary>
        /// <param name="request">
        /// The data recevied from the client.
        /// </param>
        /// <param name="context">
        /// The data about the call.
        /// </param>
        /// <returns>
        /// The response for the client.
        /// </returns>
        public override Task<ResponseData> Request(RequestData request, ServerCallContext context)
        {
            try
            {
                string id = request.Id;
                Console.WriteLine("Id: " + id + ", Tasks: " + request.Tasks + ", Close: " + request.Close);
                if (request.Close)
                    Scaling.StopTasks(request.Id);
                else
                    id = Scaling.AddTasks(request);
                return Task.FromResult(new ResponseData() { Id = id });
            }
            catch (Exception e)
            {
                Console.WriteLine("Error at server while handling request: " + e.Message);
                return Task.FromResult(new ResponseData() { Id = "$" });
            }
        }
    }
}
