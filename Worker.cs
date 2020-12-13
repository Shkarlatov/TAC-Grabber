using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using TAC_Grabber.IMEI;

namespace TAC_Grabber
{
    class Worker
    {
        private SimpleSettings Settings { get; set; }

        private StreamWriter sw;
        private IMEIGenerator IMEIGenerator = new IMEIGenerator();
        private GroupWorker[] groupWorkers;

        public Worker()
        {
            Settings = SimpleSettings.Load();

            foreach(var port in Settings.ProxiesPort)
            {
                 ClientsFactory.CreateClients(port);
            }

            groupWorkers = ClientsFactory.GetGroupClientsByType()
               .Select(x => new GroupWorker(x.Value, IMEIGenerator))
               .ToArray();

            foreach(var group in groupWorkers)
            {
                if(Settings.LastValues.TryGetValue(group.GroupName, out int last))
                {
                    group.SkipIMEICount = last;
                }

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
            var currentTasks= groupWorkers.Select(group => new TaskId
            {
                Id = group.GroupName,
                Task = group.GetTask()
            }).ToList();

            var statusString = new StringBuilder();
            while (true)
            {
                await Task.WhenAny(currentTasks.Select(x => x.Task));

                var completedTasks = currentTasks.Where(x => x.Task.IsCompleted).ToArray();
                foreach(var competed in completedTasks)
                {
                    var result = await competed.Task;
                    foreach(var str in result)
                    {
                        ProcessingResult(str);
                    }
                    currentTasks.Remove(competed);
                    var newTask = groupWorkers.Where(x => x.GroupName == competed.Id).Select(group => new TaskId
                    {
                        Id = group.GroupName,
                        Task = group.GetTask()
                    }).First();
                    currentTasks.Add(newTask);
                }

                
                foreach (var group in groupWorkers)
                {
                    statusString.Append($"{group.GroupName.Replace("Client", "")}: {group.SkipIMEICount} ");

                    Settings.LastValues[group.GroupName] = group.SkipIMEICount;
                }
                statusString.Append('\r');
                Console.Write(statusString);
                statusString.Clear();
            }
        }
    }
}