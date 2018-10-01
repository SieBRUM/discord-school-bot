using Discord;
using Discord.Commands;
using Discord.WebSocket;
using DiscordBotSchool.Modules.Helpers;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DiscordBotSchool.Modules.Commands
{
    public class General: ModuleBase<SocketCommandContext>
    {

        [Command("help")]
        public async Task Help()
        {
            EmbedBuilder builder = new EmbedBuilder();

            builder.WithColor(3447003)
                .WithTitle("This bot used the '!' (exclamation mark) prefix.")
                .AddField("help", "This command!")
                .AddField("clear [mention user]", "Removes all messages of mentioned user in current channel. (ADMIN ONLY)")
                .AddField("clearall", "Removes all messages in current channel. (ADMIN ONLY)")
                .AddField("givepoints [mention] [amount] ", "Give points to user. (ADMIN ONLY)")
                .AddField("points", "See your current points")
                .AddField("points [mention]", "See points of user")
                .AddField("coinflip [amount]", "Coinflip vs bot")
                .AddField("coinflip [heads/tails] [amount]", "Coinflip vs bot")
                .AddField("coinflip [mention] [heads/tails] [amount]", "Coinflip vs user")
                .AddField("highscore", "Current highscore")
                .AddField("accept [mention]", "Accept coinflip duel")

                .WithFooter(CustomEmbedBuilder.EmbedFooter(Context));

            await ReplyAsync("", false, builder.Build());
        }

        [Command("clear"), RequireUserPermission(GuildPermission.ManageMessages)]
        public async Task ClearMessagesOfUser(SocketGuildUser user)
        {
            var messages = await Context.Channel.GetMessagesAsync().Flatten();
            messages = messages.Where(x => x.Author == user);
            await Context.Channel.DeleteMessagesAsync(messages);

            await ReplyAsync($"Removed messages of {user.Username}.");
        }

        [Command("clearall"), RequireUserPermission(GuildPermission.ManageMessages)]
        public async Task ClearAllMessages()
        {
            var messages = await Context.Channel.GetMessagesAsync().Flatten();
            await Context.Channel.DeleteMessagesAsync(messages);

            var message = await ReplyAsync("Removed messages. Removing this message in 1 second...");
            await Task.Delay(1000);
            messages = await Context.Channel.GetMessagesAsync().Flatten();
            await Context.Channel.DeleteMessagesAsync(messages);
        }

        [Command("emoji")]
        public async Task MakeEmojiAsync([Remainder] string text)
        {
            string emojiText = "";
            foreach (var letter in text)
            {
                bool isAlphaBet = Regex.IsMatch(letter.ToString(), "[a-z]", RegexOptions.IgnoreCase);

                if (isAlphaBet)
                    emojiText += ":regional_indicator_" + letter.ToString().ToLower() + ":";
                else if (letter.Equals('?'))
                    emojiText += ":question:";
                else if (letter.Equals('!'))
                    emojiText += ":exclamation:";
                else if (letter.Equals('#'))
                    emojiText += ":hash:";
                else
                    emojiText += letter;
            }

            await ReplyAsync(emojiText);
        }

        [Command("subscribe")]
        public async Task SubscribeToMessages()
        {
            var user = Context.Guild.GetUser(Context.User.Id);
            if (user.Roles.Contains(Context.Guild.Roles.Where(x => x.Name == "BotNews").FirstOrDefault()))
                await ReplyAsync($"{Context.User.Mention} already subscribed.");
            else
                await user.AddRoleAsync(Context.Guild.Roles.Where(x => x.Name == "BotNews").FirstOrDefault());

            await ReplyAsync($"{Context.User.Mention} subscribed.");
        }

        [Command("unsubscribe")]
        public async Task UnsubscribeToMessages()
        {
            var user = Context.Guild.GetUser(Context.User.Id);
            if (!user.Roles.Contains(Context.Guild.Roles.Where(x => x.Name == "BotNews").FirstOrDefault()))
                await ReplyAsync($"{Context.User.Mention} not subscribed.");
            else
                await user.RemoveRoleAsync(Context.Guild.Roles.Where(x => x.Name == "BotNews").FirstOrDefault());

            await ReplyAsync($"{Context.User.Mention} unsubscribed.");
        }
    }
}
