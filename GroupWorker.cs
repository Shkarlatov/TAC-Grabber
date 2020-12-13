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

        private readonly IMEI.IMEIGenerator IMEIGenerator;
       

        private readonly TaskPool<BaseHTTPClient, int, Task<string>, string> pool;

        public GroupWorker(BaseHTTPClient[] clients,IMEI.IMEIGenerator generator)
        {
            this.IMEIGenerator = generator;
            GroupName = clients.First().GetType().Name;

            pool = new TaskPool<BaseHTTPClient, int, Task<string>, string>(
                clients,
                x=>x.GetHashCode(),
                x=>x.GetQueryAsync(IMEIGenerator.GetValidIMEI(SkipIMEICount++).First())
                );

        }

        public  Task<string[]> GetTask()
        {
            return  pool.NextAsync();
        }
    }
}
