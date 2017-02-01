using System.IO;
using Microsoft.AspNetCore.Hosting;
using DSBroker.Platform.NETCore;

namespace DSBroker.ASP
{
    public class Program
    {
        public static Broker Broker;

        public static void Main(string[] args)
        {
            Broker = new Broker(new NETCorePlatform());

            var host = new WebHostBuilder()
                .UseKestrel()
                .UseUrls("http://0.0.0.0:5000")
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseIISIntegration()
                .UseStartup<Startup>()
                .Build();

            host.Run();
        }
    }
}
