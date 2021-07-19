using Discord;
using Discord.Addons.Hosting;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Discord.Addons.Hosting.Util;
using Microsoft.Extensions.Logging;

namespace ReadyCheckBot.Services
{
    public class CommandHandler : DiscordClientService
    {
        private readonly IServiceProvider _provider;
        private readonly DiscordSocketClient _client;
        private readonly CommandService _service;
        private readonly IConfiguration _config;

        public CommandHandler(IServiceProvider provider, 
            DiscordSocketClient client, 
            CommandService service,
            IConfiguration config, 
            ILogger<CommandHandler> logger) : base(client, logger)
        {
            _provider = provider;
            _client = client;
            _service = service;
            _config = config;
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            _client.MessageReceived += OnMessageReceived;
            _service.CommandExecuted += OnCommandExecuted;
            await _service.AddModulesAsync(Assembly.GetEntryAssembly(), _provider);

            await _client.WaitForReadyAsync(cancellationToken);

            await _client.SetGameAsync("?rc | git.io/JkHdC", null, ActivityType.Listening);
        }

        private async Task OnMessageReceived(SocketMessage arg)
        {
            if (!(arg is SocketUserMessage {Source: MessageSource.User} message)) return;

            var argPos = 0;
            if (!(message.HasStringPrefix(_config["prefix"], ref argPos) ||
                  message.HasMentionPrefix(_client.CurrentUser, ref argPos)) || message.Author.IsBot) return;

            var context = new SocketCommandContext(_client, message);

            await _service.ExecuteAsync(context, argPos, _provider);
        }

        private static async Task OnCommandExecuted(Optional<CommandInfo> command, ICommandContext context, IResult result)
        {
            if (command.IsSpecified && !result.IsSuccess) await context.Channel.SendMessageAsync($"Error: {result.ErrorReason}");
        }
    }
}