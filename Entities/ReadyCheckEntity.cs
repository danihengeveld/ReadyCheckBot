using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Text;

namespace ReadyCheck.Entities
{
    public class ReadyCheckEntity
    {
        public IUser[] readyUsers { get; set;  }
        private int amount;

        private SocketCommandContext _ctx;

        public ReadyCheckEntity(SocketCommandContext ctx, int amount)
        {
            this._ctx = ctx;
            this.amount = amount;
            readyUsers = new IUser[0];
        }

        public Embed GenerateEmbed()
        {

                var builder = new EmbedBuilder()
                    .WithAuthor(new EmbedAuthorBuilder()
                        .WithIconUrl(_ctx.Client.CurrentUser.GetAvatarUrl() ??
                                     _ctx.Client.CurrentUser.GetDefaultAvatarUrl()).WithName("ReadyCheck"))
                    .WithDescription($"Waiting for {amount - readyUsers.Length} people.")
                    .WithColor(new Color(30, 191, 29))
                    .WithFooter(new EmbedFooterBuilder().WithText($"Started by: {_ctx.User.Username}"))
                    .WithCurrentTimestamp();

                if (readyUsers.Length > 0)
                {
                    var readyUsernames = "";
                    foreach (IUser user in readyUsers)
                        readyUsernames += $"{user.Username}\n";

                    readyUsernames.Remove(readyUsernames.Length - 1);
                    builder.AddField("Ready", readyUsernames, true);
                }

                if (readyUsers.Length == amount) builder.Description = "Everyone is ready!";

                return builder.Build();
        }
    }
}
