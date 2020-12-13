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

        StreamWriter sw;

        private Dictionary<int, BaseHTTPClient[]> clients = new Dictionary<int, BaseHTTPClient[]>();
        private IMEIGenerator IMEIGenerator = new IMEIGenerator();


        public Worker()
        {
            Settings = SimpleSettings.Load();

            foreach(var port in Settings.ProxiesPort)
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

            sw?.Close();
            
        }

        public void Start()
        {
            if (cancellationTokenSource != null)
                return;

            cancellationTokenSource = new CancellationTokenSource();

            sw = new StreamWriter("tacs.csv", append: true);


            Task.Run(DoWork,cancellationTokenSource.Token);
        }

        private void ProcessingResult(string str)
        {
            if(!string.IsNullOrEmpty(str))
            {
                var lines = str
                    .Split("\r\n")
                    .Where(x => !string.IsNullOrEmpty(x))
                    .Select(x => x.Trim());
                foreach(var line in lines)
                {
                   
                    sw.WriteLine(line);
                    Console.WriteLine($"{DateTime.Now}: {line}");
                }
                sw.Flush();
                
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
                    Console.WriteLine("Current TAC: "+ tac.Current.Substring(0,8));
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