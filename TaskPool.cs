using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Linq;
using System.Threading.Tasks;

namespace TAC_Grabber
{
    public class TaskPool<TItem,TKey,TTask, TTaskRes> where TTask: Task<TTaskRes>
    {
        Dictionary<TKey, TTask> dic;
        IEnumerable<TItem> source;
        Func<TItem, TKey> key;
        Func<TItem, TTask> task;

        public TaskPool(IEnumerable<TItem> source, Func<TItem, TKey> key, Func<TItem, TTask> task) 
        {
            this.source = source;
            this.key = key;
            this.task = task;

            dic=source.ToDictionary(key, task);
        }

        public async Task<TTaskRes[]> NextAsync()
        {
            while(true)
            {
                await Task.WhenAny(dic.Values);
                var completed = dic.Where(x => x.Value.IsCompleted).ToArray();

                var lst = new List<TTaskRes>(completed.Length);
                foreach(var c in completed)
                {
                    var result = await c.Value;
                    lst.Add(result);

                    dic[c.Key] = task(source.First(x => key(x).Equals(c.Key)));
                }
                if (lst.Count > 0)
                    return lst.ToArray();

            }
        }

    }
}
