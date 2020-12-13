using System;

namespace TAC_Grabber
{
    class Program
    {
        static void Main(string[] args)
        {
            var worker = new Worker();
            worker.Start();

            Console.WriteLine("Press enter to exit");
            Console.ReadLine();
            worker.Stop();
        }

    }
}
