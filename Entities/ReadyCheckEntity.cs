using Discord;
using Discord.Commands;
using System.Collections.Generic;

namespace ReadyCheckBot.Entities
{
    public class ReadyCheckEntity
    {
        public IUser[] readyUsers { get; set; }
        public int amount { get; set; }
        private readonly EmbedBuilder embedBuilder;

        private SocketCommandContext _ctx;

        public ReadyCheckEntity(SocketCommandContext ctx, int amount)
        {
            this._ctx = ctx;
            this.amount = amount;
            this.readyUsers = new IUser[0];
            embedBuilder = new EmbedBuilder();
        }

        public Embed GenerateEmbed()
        {

            embedBuilder
                .WithAuthor(new EmbedAuthorBuilder()
                    .WithIconUrl(_ctx.Client.CurrentUser.GetAvatarUrl() ??
                                 _ctx.Client.CurrentUser.GetDefaultAvatarUrl()).WithName("ReadyCheck"))
                .WithDescription($"Waiting for {amount} people.")
                .WithColor(new Color(255, 204, 0))
                .WithFooter(new EmbedFooterBuilder().WithText($"Started by: {_ctx.User.Username}"))
                .WithCurrentTimestamp();

            return embedBuilder.Build();
        }

        public Embed UpdateEmbed()
        {
            if (readyUsers.Length > 0)
            {
                var readyUsernames = "";
                foreach (IUser user in readyUsers)
                    readyUsernames += $"{user.Username}\n";

                readyUsernames.Remove(readyUsernames.Length - 1);
                embedBuilder.Fields = new List<EmbedFieldBuilder> { new EmbedFieldBuilder().WithName("Ready").WithValue(readyUsernames).WithIsInline(false) };
            }
            else
            {
                embedBuilder.Fields = new List<EmbedFieldBuilder>();
            }

            if (readyUsers.Length >= amount)
            {
                embedBuilder.Description = "Everyone is ready!";
                embedBuilder.Color = new Color(32, 192, 29);
            }
            else
            {
                embedBuilder.Description = $"Waiting for { amount - readyUsers.Length} people.";
                embedBuilder.Color = new Color(255, 204, 0);
            }

            return embedBuilder.Build();
        }
    }
}
