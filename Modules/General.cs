using Discord;
using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ReadyCheck.Modules
{
    public class General : ModuleBase
    {
        [Command("ping")]
        public async Task Ping()
        {
            await Context.Channel.SendMessageAsync("Pong!");
        }

        [Command("info")]
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

            await Context.Channel.SendMessageAsync(null, false, embed);
        }
    }
}
