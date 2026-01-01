using Microsoft.Extensions.Hosting;

namespace EstimoteBeaconReceiver
{
    class Program
    {
        static async Task Main(string[] args)
        {
            using IHost host = Host.CreateDefaultBuilder(args)
            .ConfigureServices((context, services) =>
            {
               

            })
            .Build();
            await host.StartAsync();
        }
    }
}