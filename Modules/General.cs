using Discord;
using Discord.Addons.CommandsExtension;
using Discord.Commands;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace ReadyCheckBot.Modules
{
    public class General : ModuleBase<SocketCommandContext>
    {
        private readonly ILogger<General> _logger;
        private readonly CommandService _service;
        private readonly IConfiguration _config;

        public General(ILogger<General> logger, CommandService service, IConfiguration config)
        {
            _logger = logger;
            _service = service;
            _config = config;
        }

        [Command("ping")]
        [Summary("Sends Pong! in chat if the bot is online!")]
        public async Task Ping()
        {
            await ReplyAsync("Pong!");
            _logger.LogInformation($"{Context.User.Username} executed the ping command!");
        }

        [Command("help"), Alias("assits", "info"), Summary("Shows help menu.")]
        public async Task Help([Remainder] string command = null)
        {
            var botPrefix = _config["prefix"];
            var helpEmbed = _service.GetDefaultHelpEmbed(command, botPrefix).ToEmbedBuilder()
                .WithColor(new Color(30, 191, 29))
                .WithAuthor(new EmbedAuthorBuilder().WithIconUrl(Context.Client.CurrentUser.GetAvatarUrl() ?? Context.Client.CurrentUser.GetDefaultAvatarUrl()).WithName("ReadyCheck"))
                .Build();

            await ReplyAsync(null, false, helpEmbed);
            _logger.LogInformation($"{Context.User.Username} executed the help command!");
        }
    }
}
