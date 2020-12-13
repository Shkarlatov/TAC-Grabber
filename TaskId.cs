using System.Threading.Tasks;

namespace TAC_Grabber
{
    class TaskId
    {
        public string Id { get; set; }
        public Task<string[]> Task { get; set; }
    }
}
