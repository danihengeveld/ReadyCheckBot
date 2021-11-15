using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;
using ReadyCheckBot.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ReadyCheckBot.Modules
{
    public class ReadyCheck : ModuleBase<SocketCommandContext>
    {
        private readonly ILogger<ReadyCheck> _logger;
        private readonly DiscordSocketClient _client;

        private static readonly IEmote Emoji = new Emoji("✅");

        private static readonly IDictionary<ulong, IUserMessage> LatestMessages = new Dictionary<ulong, IUserMessage>();
        private static readonly IDictionary<ulong, ReadyCheckEntity> LatestRcEntities = new Dictionary<ulong, ReadyCheckEntity>();

        public ReadyCheck(ILogger<ReadyCheck> logger, DiscordSocketClient client)
        {
            _logger = logger;
            _client = client;
            _client.ReactionAdded += UpdateReadyCheck;
            _client.ReactionRemoved += UpdateReadyCheck;
            _client.MessageDeleted += CleanUpReadyCheck;
        }

        [Command("readycheck"), Alias("rcheck", "rc", "check", "checkready")]
        [Summary("Starts the ready check.")]
        public async Task RCheck([Summary("The amount of players to check for.")] int amount = 10)
        {
            await Context.Message.DeleteAsync();

            if (LatestMessages.TryGetValue(Context.Channel.Id, out var latestMessage))
            {
                await Context.Channel.DeleteMessageAsync(latestMessage);
                LatestMessages.Remove(Context.Channel.Id);
                LatestRcEntities.Remove(Context.Channel.Id);
            }

            var rcEntity = new ReadyCheckEntity(Context, amount);

            var message = await ReplyAsync(null, false, rcEntity.GenerateEmbed());

            await message.AddReactionAsync(Emoji);

            LatestRcEntities.Add(Context.Channel.Id, rcEntity);
            LatestMessages.Add(Context.Channel.Id, message);

            _logger.LogInformation($"{Context.User.Username} started a ready check for {amount} people!");
        }

        private async Task UpdateReadyCheck(Cacheable<IUserMessage, ulong> cache, ISocketMessageChannel channel, SocketReaction reaction)
        {
            if (reaction.User.Value.IsBot) return;
            if (Emoji.Name != reaction.Emote.Name) return;
            if (LatestMessages.ContainsKey(channel.Id))
            {
                var message = LatestMessages[channel.Id];
                var reactedUsersCollection = await message.GetReactionUsersAsync(Emoji, 30).FlattenAsync();
                var reactedUsers = reactedUsersCollection.Where((user) => user.Id != _client.CurrentUser.Id).ToArray();
                var rcEntity = LatestRcEntities[channel.Id];
                rcEntity.ReadyUsers = reactedUsers;

                await message.ModifyAsync(msg => msg.Embed = rcEntity.UpdateEmbed());
            }
        }

        private static Task CleanUpReadyCheck(Cacheable<IMessage, ulong> cache, ISocketMessageChannel channel)
        {
            if (!LatestMessages.ContainsKey(channel.Id)) return Task.CompletedTask;

            LatestMessages.Remove(channel.Id);
            LatestRcEntities.Remove(channel.Id);

            return Task.CompletedTask;
        }
    }
}