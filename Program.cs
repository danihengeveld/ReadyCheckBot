using System;
using System.Threading.Tasks;

namespace ReadyCheck
{
    class Program
    {
        public static async Task Main(string[] args)
            => await Startup.RunAsync(args);
    }
}
