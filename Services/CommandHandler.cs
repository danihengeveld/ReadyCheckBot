using Discord;
using Discord.Addons.Hosting;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using ReadyCheckBot.Entities;
using ReadyCheckBot.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace ReadyCheckBot.Services
{
    public class CommandHandler : InitializedService
    {
        private readonly IServiceProvider _provider;
        private readonly DiscordSocketClient _client;
        private readonly CommandService _service;
        private readonly IConfiguration _config;

        public static List<ulong> messageIds;
        public static ReadyCheckEntity rcEntity;

        public CommandHandler(IServiceProvider provider, DiscordSocketClient client, CommandService service, IConfiguration config)
        {
            _provider = provider;
            _client = client;
            _service = service;
            _config = config;
            messageIds = new List<ulong>();
        }

        public override async Task InitializeAsync(CancellationToken cancellationToken)
        {
            _client.MessageReceived += OnMessageReceived;
            _client.MessageDeleted += OnMessageDeleted;
            _client.ReactionAdded += OnReactionAdded;
            _client.ReactionRemoved += OnReactionRemoved;
            _service.CommandExecuted += OnCommandExecuted;
            await _service.AddModulesAsync(Assembly.GetEntryAssembly(), _provider);
        }

        private async Task OnMessageReceived(SocketMessage arg)
        {
            if (!(arg is SocketUserMessage message)) return;
            if (message.Source != MessageSource.User) return;

            var argPos = 0;
            if (!message.HasStringPrefix(_config["prefix"], ref argPos) && !message.HasMentionPrefix(_client.CurrentUser, ref argPos)) return;

            var context = new SocketCommandContext(_client, message);

            await _service.ExecuteAsync(context, argPos, _provider);
        }

        private async Task OnCommandExecuted(Optional<CommandInfo> command, ICommandContext context, IResult result)
        {
            if (command.IsSpecified && !result.IsSuccess) await context.Channel.SendMessageAsync($"Error: {result.ErrorReason}");
        }

        private async Task OnReactionAdded(Cacheable<IUserMessage, ulong> cache, ISocketMessageChannel channel, SocketReaction reaction)
        {
            if (reaction.UserId == _client.CurrentUser.Id) return;
            if (messageIds.Contains(reaction.MessageId))
            {
                var message = await cache.GetOrDownloadAsync();
                var reactedUsersCollection = await message.GetReactionUsersAsync(ReadyCheck.emoji, 30).FlattenAsync();
                var reactedUsers = reactedUsersCollection.Where((user) => user.Id != _client.CurrentUser.Id).ToArray();
                Embed embed = (Embed) message.Embeds.First();

                rcEntity.readyUsers = reactedUsers;

                await message.ModifyAsync(msg => msg.Embed = rcEntity.GenerateEmbed());
            }
        }

        private async Task OnReactionRemoved(Cacheable<IUserMessage, ulong> cache, ISocketMessageChannel channel, SocketReaction reaction)
        {
            if (reaction.UserId == _client.CurrentUser.Id) return;
            if (messageIds.Contains(reaction.MessageId))
            {
                var message = await cache.GetOrDownloadAsync();
                var reactedUsersCollection = await message.GetReactionUsersAsync(ReadyCheck.emoji, 30).FlattenAsync();
                var reactedUsers = reactedUsersCollection.Where((user) => user.Id != _client.CurrentUser.Id).ToArray();
                Embed embed = (Embed)message.Embeds.First();

                rcEntity.readyUsers = reactedUsers;

                await message.ModifyAsync(msg => msg.Embed = rcEntity.GenerateEmbed());
            }
        }

        private async Task OnMessageDeleted(Cacheable<IMessage, ulong> cache, ISocketMessageChannel channel)
        {
            var message = await cache.GetOrDownloadAsync();
            if (messageIds.Contains(message.Id)) messageIds.Remove(message.Id);
        }
    }
}