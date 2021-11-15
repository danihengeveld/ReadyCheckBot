using Discord;
using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ReadyCheckBot.Entities
{
    public class ReadyCheckEntity
    {
        public IUser[] ReadyUsers { get; set; }
        private int Amount { get; }
        private readonly EmbedBuilder _embedBuilder;

        private readonly SocketCommandContext _ctx;

        public ReadyCheckEntity(SocketCommandContext ctx, int amount)
        {
            _ctx = ctx;
            Amount = amount;
            ReadyUsers = Array.Empty<IUser>();
            _embedBuilder = new EmbedBuilder();
        }

        public Embed GenerateEmbed()
        {

            _embedBuilder
                .WithAuthor(new EmbedAuthorBuilder()
                    .WithIconUrl(_ctx.Client.CurrentUser.GetAvatarUrl() ??
                                 _ctx.Client.CurrentUser.GetDefaultAvatarUrl()).WithName("ReadyCheck"))
                .WithDescription($"Waiting for {Amount} people.")
                .WithColor(new Color(255, 204, 0))
                .WithFooter(new EmbedFooterBuilder().WithText($"Started by: {_ctx.User.Username}"))
                .WithCurrentTimestamp();

            return _embedBuilder.Build();
        }

        public Embed UpdateEmbed()
        {
            if (ReadyUsers.Length > 0)
            {
                var readyUsernames = ReadyUsers.Aggregate(string.Empty, (current, user) => current + $"{user.Username}\n");

                _ = readyUsernames.Remove(readyUsernames.Length - 1);
                _embedBuilder.Fields = new List<EmbedFieldBuilder> { new EmbedFieldBuilder().WithName("Ready").WithValue(readyUsernames).WithIsInline(false) };
            }
            else
            {
                _embedBuilder.Fields = new List<EmbedFieldBuilder>();
            }

            if (ReadyUsers.Length >= Amount)
            {
                _embedBuilder.Description = "Everyone is ready!";
                _embedBuilder.Color = new Color(32, 192, 29);
            }
            else
            {
                _embedBuilder.Description = $"Waiting for { Amount - ReadyUsers.Length} people.";
                _embedBuilder.Color = new Color(255, 204, 0);
            }

            return _embedBuilder.Build();
        }
    }
}
