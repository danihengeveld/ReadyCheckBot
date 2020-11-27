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

        private static IEmote emoji = new Emoji("✅");

        private static readonly IDictionary<ulong, IUserMessage> latestMessages = new Dictionary<ulong, IUserMessage>();
        private static readonly IDictionary<ulong, ReadyCheckEntity> latestRCEntities = new Dictionary<ulong, ReadyCheckEntity>();

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
            IUserMessage latestMessage = null;
            if (latestMessages.TryGetValue(Context.Channel.Id, out latestMessage))
            {
                await Context.Channel.DeleteMessageAsync(latestMessage);
                latestMessages.Remove(Context.Channel.Id);
                latestRCEntities.Remove(Context.Channel.Id);
            }

            ReadyCheckEntity rcEntity = new ReadyCheckEntity(Context, amount);

            var message = await ReplyAsync(null, false, rcEntity.GenerateEmbed());

            await message.AddReactionAsync(emoji);

            latestRCEntities.Add(Context.Channel.Id, rcEntity);
            latestMessages.Add(Context.Channel.Id, message);

            _logger.LogInformation($"{Context.User.Username} started a ready check for {amount} people!");
        }

        public async Task UpdateReadyCheck(Cacheable<IUserMessage, ulong> cache, ISocketMessageChannel channel, SocketReaction reaction)
        {
            if (reaction.User.Value.IsBot) return;
            if (emoji.Name != reaction.Emote.Name) return;
            if (latestMessages.ContainsKey(channel.Id))
            {
                IUserMessage message = latestMessages[channel.Id];
                var reactedUsersCollection = await message.GetReactionUsersAsync(ReadyCheck.emoji, 30).FlattenAsync();
                IUser[] reactedUsers = reactedUsersCollection.Where((user) => user.Id != _client.CurrentUser.Id).ToArray();
                ReadyCheckEntity rcEntity = latestRCEntities[channel.Id];
                rcEntity.readyUsers = reactedUsers;

                await message.ModifyAsync(msg => msg.Embed = rcEntity.UpdateEmbed());
            }
        }

        public Task CleanUpReadyCheck(Cacheable<IMessage, ulong> cache, ISocketMessageChannel channel)
        {
            if (latestMessages.ContainsKey(channel.Id))
            {
                latestMessages.Remove(channel.Id);
                latestRCEntities.Remove(channel.Id);
            }

            return Task.CompletedTask;
        }
    }
}