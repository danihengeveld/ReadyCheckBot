using Discord;
using Discord.Commands;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace ReadyCheck.Modules
{
    public class GeneralCommands : ModuleBase<SocketCommandContext>
    {
        private readonly ILogger<GeneralCommands> _logger;

        public GeneralCommands(ILogger<GeneralCommands> logger)
            => _logger = logger;

        [Command("ping")]
        [Summary("Sends Pong! in chat if the bot is online!")]
        public async Task Ping()
        {
            await ReplyAsync("Pong!");
            _logger.LogInformation($"{Context.User.Username} executed the ping command!");
        }

        [Command("info")]
        [Summary("Sends a menu in chat with information about the bot and all commands.")]
        public async Task Info()
        {
            var commandsField = new EmbedFieldBuilder()
                .WithName("Command")
                .WithValue(
                ">info\n" +
                ">ping")
                .WithIsInline(true);

            var commandsDescriptionField = new EmbedFieldBuilder()
                .WithName("Description")
                .WithValue(
                "Gives you the current menu\n" +
                "Check if the bot is online.")
                .WithIsInline(true);

            var builder = new EmbedBuilder()
                .WithAuthor(new EmbedAuthorBuilder().WithIconUrl(Context.Client.CurrentUser.GetAvatarUrl() ?? Context.Client.CurrentUser.GetDefaultAvatarUrl()).WithName("ReadyCheck"))
                .WithDescription("I listen to '>'. Check if everyone is ready!")
                .WithColor(new Color(30, 191, 29))
                .AddField(commandsField)
                .AddField(commandsDescriptionField)
                .WithFooter(new EmbedFooterBuilder().WithText("Author: Dani Hengeveld"))
                .WithCurrentTimestamp();
            var embed = builder.Build();

            await ReplyAsync(null, false, embed);
        }
    }
}
