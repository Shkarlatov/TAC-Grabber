﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
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

        GroupWorker[] groupWorkers;
        public Worker()
        {
            Settings = SimpleSettings.Load();

            foreach (var port in Settings.ProxiesPort)
            {
                clients[port] = ClientsFactory.CreateClients(port);
            }

            groupWorkers = ClientsFactory.GetClientsGroupByType()
                .Select(x => new GroupWorker(x.Value, IMEIGenerator))
                .ToArray();

            foreach (var group in groupWorkers)
            {
                if( Settings.LastValues.TryGetValue(group.GroupName, out int last))
                    group.SkipIMEICount = last;
            }

        }

        CancellationTokenSource cancellationTokenSource;
        public void Stop()
        {
            foreach (var group in groupWorkers)
            {
                Settings.LastValues[group.GroupName] = group.SkipIMEICount;
            }
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

            Task.Run(DoWork, cancellationTokenSource.Token);
        }

        private void ProcessingResult(string str)
        {
            if (!string.IsNullOrEmpty(str))
            {
                var lines = str
                    .Split("\r\n")
                    .Where(x => !string.IsNullOrEmpty(x))
                    .Select(x => x.Trim());
                foreach (var line in lines)
                {

                    sw.WriteLine(line);
                    // HACK:
                    // clear last line
                    Console.Write("\r" + new string(' ', Console.WindowWidth-1)+"\r");
                    Console.WriteLine($"{DateTime.Now}: {line}");
                }
                sw.Flush();

            }
        }
        private async Task DoWork()
        {
            var pool = new TaskPool<GroupWorker,string,Task<string[]>,string[]>(
                groupWorkers,
                x=>x.GroupName,
                x=>x.GetTask()
                );

            var statusString = new StringBuilder(255);
            while(true)
            {
                var result=await pool.NextAsync();
                foreach(var res in result)
                    foreach(var str in res)
                    {
                        ProcessingResult(str);
                    }

                statusString.Clear();
                foreach(var group in groupWorkers)
                {
                    statusString.Append($"{group.GroupName.Replace("Client", "")}:{group.SkipIMEICount} ");
                }
                statusString.Append('\r');
                Console.Write(statusString);
            }
            
        }
    }
}