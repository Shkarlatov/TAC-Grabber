using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TAC_Grabber
{
    public class GroupWorker
    {
        public int SkipIMEICount { get; set; }
        public string GroupName { get; private set; }

        private readonly BaseHTTPClient[] clients;
        private readonly IMEI.IMEIGenerator IMEIGenerator;
        private Dictionary<int, Task<string>> _cache = new Dictionary<int, Task<string>>();

        public GroupWorker(BaseHTTPClient[] clients,IMEI.IMEIGenerator generator)
        {
            this.clients = clients;
            this.IMEIGenerator = generator;
            GroupName = clients.First().GetType().Name;

            _cache = clients.ToDictionary(
                  x => x.GetHashCode()
                  , x => x.GetQueryAsync(
                       IMEIGenerator.GetValidIMEI(SkipIMEICount++)
                       .First()
                       ));
        }

        public async Task<string[]> GetTask()
        {
            if(_cache.Count==0)
            {
                throw new Exception();
            }
            while(true)
            {
                await Task.Delay(1);
                var completedTasks = _cache
                    .Where(x => x.Value.IsCompleted)
                    .ToArray();
                
                var lst = new List<string>(completedTasks.Length);
                
                foreach(var completedTask in completedTasks)
                {
                    var result = await completedTask.Value;
                    lst.Add(result);

                    try
                    {
                        _cache[completedTask.Key] = clients.First(x => x.GetHashCode() == completedTask.Key).GetQueryAsync(IMEIGenerator.GetValidIMEI(SkipIMEICount++)
                       .First());
                    }
                    catch
                    {
                        _cache.Remove(completedTask.Key);
                    }
                   
                }
                if (lst.Count > 0)
                    return lst.ToArray();
            }
        }
    }
}
