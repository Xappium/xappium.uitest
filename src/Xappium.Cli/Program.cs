using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Xappium.Commands;

namespace Xappium
{
    internal class Program
    {
        public static Task<int> Main(string[] args) =>
            new HostBuilder()
            .ConfigureLogging(ConfigureLogging)
            .ConfigureServices(ConfigureServices)
            .RunCommandLineApplicationAsync<XappiumCommand>(args);

        private static void ConfigureLogging(HostBuilderContext ctx, ILoggingBuilder builder)
        {

        }

        private static void ConfigureServices(IServiceCollection services)
        {

        }
    }
}
