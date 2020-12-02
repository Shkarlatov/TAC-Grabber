using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

using TAC_Grabber.IMEI;

namespace TAC_Grabber
{
    class Worker
    {
        private SimpleSettings Settings { get; set; }
        private FileStream fs;

        private Dictionary<int, BaseHTTPClient[]> clients = new Dictionary<int, BaseHTTPClient[]>();
        private IMEIGenerator IMEIGenerator = new IMEIGenerator();


        public Worker(int[] proxiesPorts)
        {
            Settings = SimpleSettings.Load();

            foreach(var port in proxiesPorts)
            {
                clients[port] = ClientsFactory.GetClients(new WebProxy("127.0.0.1", port));
            }

        }

        CancellationTokenSource cancellationTokenSource;
        public void Stop()
        {
            cancellationTokenSource.Cancel();
            cancellationTokenSource = null;
            Settings.Save();

            fs?.Close();
        }

        public void Start()
        {
            if (cancellationTokenSource != null)
                return;

            cancellationTokenSource = new CancellationTokenSource();

            fs = File.OpenWrite("tacs.csv");

            Task.Run(DoWork,cancellationTokenSource.Token);
        }

        private void ProcessingResult(string str)
        {
            if(!string.IsNullOrEmpty(str))
            {
                Console.WriteLine(str);
            }
        }
        private async Task DoWork()
        {
            var currentTasks = new List<Task<string[]>>();
            var tac = IMEIGenerator.GetValidIMEI(Settings.LastValue).GetEnumerator();
            while(tac.MoveNext())
            {
                foreach(var group in clients)
                {
                    var tasks = group.Value.Select(x => x.GetQueryAsync(tac.Current));
                    var whenAll = Task.WhenAll(tasks);
                    currentTasks.Add(whenAll);
                    Console.WriteLine(tac.Current);
                    tac.MoveNext();
                    Settings.LastValue++;
                }

                var completed = await Task.WhenAny(currentTasks);
                currentTasks.Remove(completed);
                var result = await completed;
                foreach(var res in result)
                {
                    ProcessingResult(res);
                }
            }
        }
    }
}