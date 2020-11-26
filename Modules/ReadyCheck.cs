using Discord;
using Discord.Commands;
using Microsoft.Extensions.Logging;
using ReadyCheckBot.Entities;
using ReadyCheckBot.Services;
using System.Threading.Tasks;

namespace ReadyCheckBot.Modules
{
    public class ReadyCheck : ModuleBase<SocketCommandContext>
    {
        private readonly ILogger<ReadyCheck> _logger;

        public ReadyCheck(ILogger<ReadyCheck> logger)
            => _logger = logger;

        public static IEmote emoji = new Emoji("✅");

        [Command("readycheck"), Alias("rcheck", "rc", "check", "checkready")]
        [Summary("Starts the ready check.")]
        public async Task RCheck([Summary("The amount of players to check for.")] int amount = 10)
        {
            var rcEntity = new ReadyCheckEntity(Context, amount);

            var message = await ReplyAsync(null, false, rcEntity.GenerateEmbed());

            await message.AddReactionAsync(emoji);

            CommandHandler.messageIds.Add(message.Id);
            CommandHandler.rcEntity = rcEntity;
        }
    }
}