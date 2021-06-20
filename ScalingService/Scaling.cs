using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ScalingService
{
    /// <summary>
    /// Class
    /// <c>StreamServer</c>
    /// contains functions to handle the scaling process.
    /// </summary>
    public class Scaling
    {
        const string LOG_FILE = "log.txt"; // log file path
        static int[] machines = new int[300]; // all machines status (number of tasks in each machine)
        // key = request id, value = (machine index, number of tasks that belong to this request)
        static Dictionary<string, Dictionary<int, int>> idTasks = new Dictionary<string, Dictionary<int, int>>();
        static int runningMachines = 0; // number of running machines

        /// <summary>
        /// <c>AddTasks</c>
        /// assigns the tasks to machines.
        /// </summary>
        /// <param name="request">
        /// The data recevied from the client.
        /// </param>
        /// <returns>
        /// A new request id.
        /// </returns>
        public static string AddTasks(Generated.RequestData request)
        {
            if (runningMachines == 300)
                return "$";
            string id = "0";
            int openSpace, addedTasks, count = 0;
            int remainingTasks = request.Tasks;
            Dictionary<int, int> tasksDistrabution = new Dictionary<int, int>();
            Log("New request - Tasks = " + request.Tasks + ", Close = false");
            lock (machines)
            {
                for (int i = 0; i < machines.Length && remainingTasks > 0; ++i)
                {
                    // if the machine can handle new tasks
                    if (machines[i] < 100)
                    {
                        // if the machine is closed
                        if (machines[i] == 0)
                        {
                            runningMachines++;
                        }
                        openSpace = 100 - machines[i];
                        // if there are more tasks than machine can handle
                        if (remainingTasks > openSpace)
                        {
                            machines[i] = 100;
                            addedTasks = openSpace;
                        }
                        else
                        {
                            machines[i] = remainingTasks + machines[i];
                            addedTasks = remainingTasks;
                        }
                        remainingTasks -= openSpace;
                        tasksDistrabution.Add(i, addedTasks);
                    }
                }
                LogRunningMachines(machines.ToList());
            }
            lock (idTasks)
            {
                // create new request id
                while (idTasks.ContainsKey(id))
                {
                    count++;
                    id = "" + count;
                }
                idTasks.Add(id, tasksDistrabution);
            }
            return id; // return the request id to be sent to the client
        }

        /// <summary>
        /// <c>StopTasks</c>
        /// stops the tasks and closes machines if needed.
        /// </summary>
        /// <param name="id">
        /// The request id of the tasks.
        /// </param>
        public static void StopTasks(string id)
        {
            Log("New request - Id = " + id + ", Close = true");
            Dictionary<int, int> tasksDistribution;
            lock (idTasks)
            {
                // get the request's tasks distribution to close them
                if (!idTasks.ContainsKey(id))
                    return;
                idTasks.TryGetValue(id, out tasksDistribution);
                idTasks.Remove(id);
            }
            lock (machines)
            {
                foreach (KeyValuePair<int, int> item in tasksDistribution)
                {
                    machines[item.Key] -= item.Value;
                    // if the machine is now free, close it
                    if (machines[item.Key] == 0)
                        runningMachines--;
                }
                LogRunningMachines(machines.ToList());
            }
        }

        /// <summary>
        /// <c>LogRunningMachines</c>
        /// logs the number of running machines and the status of the each machine.
        /// </summary>
        /// <param name="machinesList">
        /// The machines list to log.
        /// </param>
        static void LogRunningMachines(List<int> machinesList)
        {
            Log("Running machines = " + runningMachines);
            machinesList.RemoveAll((x) => x == 0);
            Log("[" + string.Join("]\n[", machinesList) + "]");
        }

        /// <summary>
        /// <c>LogRunningMachines</c>
        /// logs the message with a timestamp.
        /// </summary>
        /// <param name="message">
        /// The message to be logged.
        /// </param>
        static void Log(string message)
        {
            string stampedMessage = "[" + DateTime.Now + "] : " + message;
            File.AppendAllText(LOG_FILE, "[" + DateTime.Now + "] : " + stampedMessage + "\n");
            Console.WriteLine(stampedMessage);
        }
    }
}
