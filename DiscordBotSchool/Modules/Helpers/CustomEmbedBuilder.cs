using Discord;
using Discord.Commands;

namespace DiscordBotSchool.Modules.Helpers
{
    public static class CustomEmbedBuilder
    {

        public static EmbedFooterBuilder EmbedFooter(SocketCommandContext Context)
        {
            EmbedFooterBuilder footer = new EmbedFooterBuilder();
            footer.Text = $"This bot is made by {Context.Client.GetUser(223100696411635712).Username}!";
            footer.IconUrl = Context.Client.GetUser(223100696411635712).GetAvatarUrl();

            return footer;
        }
    }
}
