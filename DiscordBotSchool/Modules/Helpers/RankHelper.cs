using Discord.Commands;
using Discord.WebSocket;
using DiscordBotSchool.Mapping;
using System.Collections.Generic;
using System.Linq;

namespace DiscordBotSchool.Modules.Helpers
{
    static class RankHelper
    {
        public static void UpdateRank(BackendUser Backend, SocketGuildUser DiscordUser, SocketCommandContext Context)
        {

            if (Backend == null || DiscordUser == null || Context == null)
                return;

            RemoveRanks(Backend, DiscordUser, Context);

            if(Backend.Points < 100)
            {
                DiscordUser.AddRoleAsync(Context.Guild.Roles.Where(x => x.Name == "Quinten").FirstOrDefault());
            }
            else if(Backend.Points < 500)
            {
                DiscordUser.AddRoleAsync(Context.Guild.Roles.Where(x => x.Name == "Broke nibba").FirstOrDefault());
            }
            else if(Backend.Points < 1000)
            {
                DiscordUser.AddRoleAsync(Context.Guild.Roles.Where(x => x.Name == "Zwerrie").FirstOrDefault());
            }
            else if(Backend.Points < 1500)
            {
                DiscordUser.AddRoleAsync(Context.Guild.Roles.Where(x => x.Name == "Hyena").FirstOrDefault());
            }
            else if(Backend.Points < 2000)
            {
                DiscordUser.AddRoleAsync(Context.Guild.Roles.Where(x => x.Name == "Saafslaaf").FirstOrDefault());
            }
            else if(Backend.Points < 5000)
            {
                DiscordUser.AddRoleAsync(Context.Guild.Roles.Where(x => x.Name == "Co-Pimp").FirstOrDefault());
            }
            else
            {
                DiscordUser.AddRoleAsync(Context.Guild.Roles.Where(x => x.Name == "Pimp").FirstOrDefault());
            }
        }

        private static async void RemoveRanks(BackendUser Backend, SocketGuildUser user, SocketCommandContext Context)
        {
            List<SocketRole> list = Context.Guild.Roles.Where(x => x != null && x.Name != "Admin" && x.Name != "@everyone" && x.Name != "BotNews" && x.Name != "School bot" && user != null && user.Roles != null && user.Roles.Contains(x)).ToList();

            if(list.Contains(list.Where(x => x.Name == "Quinten").FirstOrDefault()) && !(Backend.Points > 100))
            {
                list.Remove(list.Where(x => x.Name == "Quinten").FirstOrDefault());
            }

            if (list.Contains(list.Where(x => x.Name == "Broke nibba").FirstOrDefault()) && !(Backend.Points > 499 || Backend.Points < 100))
            {
                list.Remove(list.Where(x => x.Name == "Broke nibba").FirstOrDefault());
            }

            if (list.Contains(list.Where(x => x.Name == "Zwerrie").FirstOrDefault()) && !(Backend.Points > 999 || Backend.Points < 500))
            {
                list.Remove(list.Where(x => x.Name == "Zwerrie").FirstOrDefault());
            }

            if (list.Contains(list.Where(x => x.Name == "Hyena").FirstOrDefault()) && !(Backend.Points > 1499 || Backend.Points < 1000))
            {
                list.Remove(list.Where(x => x.Name == "Hyena").FirstOrDefault());
            }

            if (list.Contains(list.Where(x => x.Name == "Saafslaaf").FirstOrDefault()) && !(Backend.Points > 1999 || Backend.Points < 1500))
            {
                list.Remove(list.Where(x => x.Name == "Saafslaaf").FirstOrDefault());
            }

            if (list.Contains(list.Where(x => x.Name == "Co-Pimp").FirstOrDefault()) && !(Backend.Points > 4999 || Backend.Points < 2000))
            {
                list.Remove(list.Where(x => x.Name == "Co-Pimp").FirstOrDefault());
            }

            if (list.Contains(list.Where(x => x.Name == "Pimp").FirstOrDefault()) && !(Backend.Points < 5000))
            {
                list.Remove(list.Where(x => x.Name == "Pimp").FirstOrDefault());
            }

            if (list.Count > 0)
                await user.RemoveRolesAsync(list);
        }
    }
}
