using Discord;
using Discord.Addons.Hosting;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ReadyCheckBot.Services;
using System.IO;
using System.Threading.Tasks;

namespace ReadyCheckBot
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var builder = Host.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration(x =>
            {
                var configuration = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json", false, true)
                    .Build();

                x.AddConfiguration(configuration);
            })
            .ConfigureLogging(x =>
            {
                x.AddConsole();
                x.SetMinimumLevel(LogLevel.Information);
            })
            .ConfigureDiscordHost<DiscordSocketClient>((context, config) =>
            {
                config.SocketConfig = new DiscordSocketConfig
                {
                    LogLevel = LogSeverity.Info,
                    AlwaysDownloadUsers = true,
                    MessageCacheSize = 200,
                };

                config.Token = context.Configuration["token"];
            })
            .UseCommandService((context, config) =>
            {
                config.CaseSensitiveCommands = false;
                config.LogLevel = LogSeverity.Info;
                config.DefaultRunMode = RunMode.Async;
            })
            .ConfigureServices((context, services) =>
            {
                services.AddHostedService<CommandHandler>();
            })
            .UseConsoleLifetime();

            var host = builder.Build();
            using (host)
            {
                await host.RunAsync();
            }
        }
    }
}