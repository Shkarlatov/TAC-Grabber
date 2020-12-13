using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using TAC_Grabber.IMEI;
using System.Diagnostics;

namespace TAC_Grabber
{
    public class GroupWorker
    {
        private readonly BaseHTTPClient[] clients;
        private readonly IMEIGenerator IMEIGenerator;

        public int SkipIMEICount { get; set; }
        public string GroupName { get; private set; }

        public GroupWorker(BaseHTTPClient[] clients, IMEIGenerator generator)
        {
            this.clients = clients;
            IMEIGenerator = generator;

            GroupName = clients.FirstOrDefault().GetType().Name;
        }


        Dictionary<int, Task<string>> _cache = new Dictionary<int, Task<string>>();
        public async Task<string[]> GetTask()
        {
            if(_cache.Count==0)
            {
                 foreach(var client in clients)
                {
                    _cache[client.GetHashCode()] = client.GetQueryAsync(IMEIGenerator.GetValidIMEI(SkipIMEICount++).First());
                }
            }
          
            while(true)
            {

                await Task.Delay(1);
                var completedTasks = _cache.Where(x => x.Value.IsCompleted).ToArray();

                List<string> results = new List<string>(completedTasks.Length);

                foreach(var completedTask in completedTasks)
                {
                    _cache[completedTask.Key] = clients.First(x => x.GetHashCode() == completedTask.Key).GetQueryAsync(IMEIGenerator.GetValidIMEI(SkipIMEICount++).First());

                    var result = await completedTask.Value;
                    results.Add(result);
                }

                if (results.Count > 0)
                    return results.ToArray();
            }
           
        }

        //public Task<string[]> GetTask()
        //{
        //    var imei = IMEIGenerator.GetValidIMEI(SkipIMEICount).Take(clients.Length).ToArray();
        //    SkipIMEICount += clients.Length;

        //    var tasks = new Task<string>[clients.Length];
        //    for (int i = 0; i < clients.Length; i++)
        //    {
        //        tasks[i] = clients[i].GetQueryAsync(imei[i]);
        //    }
        //    return Task.WhenAll(tasks);
        //}
    }
}
