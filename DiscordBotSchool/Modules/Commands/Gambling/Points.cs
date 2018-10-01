using Discord.Commands;
using Discord.WebSocket;
using System.Threading.Tasks;
using Newtonsoft.Json;
using DiscordBotSchool.Modules.Helpers;
using DiscordBotSchool.Mapping;
using Discord;
using System.Collections.Generic;

namespace DiscordBotSchool.Modules.Commands
{
    public class Points : ModuleBase<SocketCommandContext>
    {
        [Command("points"), Alias("Money", "Point")]
        public async Task GetPoints()
        {
            var response = APIHelper.MakeGetRequest($"users/{Context.User.Id}");
            BackendUser dbuser = JsonConvert.DeserializeObject<BackendUser>(response);

            RankHelper.UpdateRank(dbuser, Context.Guild.GetUser(Context.User.Id), Context);

            await ReplyAsync($"{Context.User.Mention} has {string.Format("{0:C}", dbuser.Points)}!");
        }

        [Command("points"), Alias("Money", "Point")]
        public async Task GetPoints(SocketGuildUser user)
        {
            var response = APIHelper.MakeGetRequest($"users/{user.Id}");
            BackendUser dbuser = JsonConvert.DeserializeObject<BackendUser>(response);
            RankHelper.UpdateRank(dbuser, user, Context);

            await ReplyAsync($"{user.Mention} has {string.Format("{0:C}", dbuser.Points)}!");
        }

        [Command("givepoints"), RequireUserPermission(GuildPermission.Administrator)]
        public async Task GivePoints(SocketGuildUser user, [Remainder]long points)
        {
            await ReplyAsync($"{Context.User.Mention} fuck off");
        }

        [Command("highscore"), Alias("highscores", "top")]
        public async Task GetHighscores()
        {
            EmbedBuilder builder = new EmbedBuilder();

            string response = APIHelper.MakeGetRequest("highscore");
            List<BackendUser> dbuser = JsonConvert.DeserializeObject<List<BackendUser>>(response);


            builder.WithColor(3447003)
                .WithTitle("Highscores")
                .WithFooter(CustomEmbedBuilder.EmbedFooter(Context));

            for (int i = 0; i < dbuser.Count; i++)
            {
                var user = Context.Guild.GetUser((ulong)dbuser[i].DiscordId);
                builder.AddField($"{i + 1}.", $"{user.Username} with {string.Format("{0:C}", dbuser[i].Points)}");
            }

            await ReplyAsync("", false, builder.Build());
        }

        [Command("pay"), Alias("donate", "give")]
        public async Task PayMoney(SocketGuildUser user, long points)
        {
            if (points < 1)
            {
                await ReplyAsync($"{Context.User.Mention} Coinflip can't be lower than 0!");
                return;
            }

            BackendDonation donation = new BackendDonation()
            {
                Donator = new BackendUser() { DiscordId = (long)Context.User.Id },
                Receiver = new BackendUser() { DiscordId = (long)user.Id },
                Points = points,
            };


            string response = APIHelper.MakePostCall("pay", donation);
            donation = JsonConvert.DeserializeObject<BackendDonation>(response);

            RankHelper.UpdateRank(donation.Donator, Context.Guild.GetUser(Context.User.Id), Context);
            RankHelper.UpdateRank(donation.Receiver, Context.Guild.GetUser(user.Id), Context);

            switch (donation.Result)
            {
                case DonationResult.DonatorDoesntExist:
                    await ReplyAsync($"{Context.User.Mention} doesn't exist in database!");
                    break;
                case DonationResult.ReceiverDoesntExist:
                    await ReplyAsync($"{user.Mention} doesn't exist in database!");
                    break;
                case DonationResult.DonatorNoMoney:
                    await ReplyAsync($"{Context.User.Mention} can't affort to donate this much! (max {string.Format("{0:C}", donation.Donator.Points)})");
                    break;
                case DonationResult.DonationSuccesful:
                    await ReplyAsync($"{Context.User.Mention} has donated {string.Format("{0:C}", donation.Points)} to {user.Mention}!");
                    break;
                case DonationResult.UnknownError:
                    await ReplyAsync($"{Context.User.Mention} An unknown error occured.");
                    break;
                default:
                    await ReplyAsync($"{Context.User.Mention} An unknown error occured!");
                    break;
            }
        }
    }
}
