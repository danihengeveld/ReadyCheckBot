using Discord;
using Discord.Addons.Interactive;
using Discord.Commands;
using Microsoft.Extensions.Logging;
using ReadyCheck.Entities;
using System.Threading.Tasks;
using System.Linq;
using ReadyCheck.Services;

namespace ReadyCheck.Modules
{
    public class ReadyCheckCommand : ModuleBase<SocketCommandContext>
    {
        private readonly ILogger<ReadyCheckCommand> _logger;

        public ReadyCheckCommand(ILogger<ReadyCheckCommand> logger)
            => _logger = logger;

        public static IEmote emoji =  new Emoji("👍");

        [Command("rcheck")]
        [Summary("Starts the ready check.")]
        public async Task RCheck(int amount)
        {
            var rcEntity = new ReadyCheckEntity(Context, amount);

            var message = await ReplyAsync(null, false, rcEntity.GenerateEmbed());
            
            await message.AddReactionAsync(emoji);

            CommandHandler.messageIds.Add(message.Id);
            CommandHandler.rcEntity = rcEntity;
        }
    }
}