using System;
using System.Linq;

namespace TAC_Grabber
{
    class Program
    {
        static void Main(string[] args)
        {
            var proxiesPort = Enumerable.Range(8181, 4).ToArray();
            var worker = new Worker(proxiesPort);
            worker.Start();

            Console.WriteLine("Press enter to exit");
            Console.ReadLine();
            worker.Stop();
        }
    }
}
