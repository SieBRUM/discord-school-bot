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
        [Command("points")]
        public async Task GetPoints()
        {
            var response = APIHelper.MakeGetRequest($"users/{Context.User.Id}");
            BackendUser dbuser = JsonConvert.DeserializeObject<BackendUser>(response);

            RankHelper.UpdateRank(dbuser, Context.Guild.GetUser(Context.User.Id), Context);

            await ReplyAsync($"{Context.User.Mention} has {string.Format("{0:C}", dbuser.Points)}!");
        }

        [Command("points")]
        public async Task GetPoints(SocketGuildUser user)
        {
            var response = APIHelper.MakeGetRequest($"users/{user.Id}");
            BackendUser dbuser = JsonConvert.DeserializeObject<BackendUser>(response);
            RankHelper.UpdateRank(dbuser, Context.Guild.GetUser(Context.User.Id), Context);

            await ReplyAsync($"{user.Mention} has {string.Format("{0:C}", dbuser.Points)}!");
        }

        [Command("givepoints"), RequireUserPermission(GuildPermission.Administrator)]
        public async Task GivePoints(SocketGuildUser user, [Remainder]long points)
        {
            await ReplyAsync($"{Context.User.Mention} fuck off");

            //BackendUser dbUser = new BackendUser()
            //{
            //    DiscordId = (long)user.Id,
            //    Points = points
            //};

            //string response = APIHelper.MakePostCall("users", dbUser);

            //BackendUser dbuser = JsonConvert.DeserializeObject<BackendUser>(response);
            //await ReplyAsync($"{user.Mention} has {string.Format("{0:C}", dbuser.Points)}!");
        }

        [Command("highscore")]
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

        [Command("pay")]
        public async Task PayMoney(SocketGuildUser user, long points)
        {
            if (points < 1)
            {
                await ReplyAsync($"{Context.User.Mention} Coinflip can't be lower than 0!");
                return;
            }

            BackendCoinflip coinflip = new BackendCoinflip()
            {
                Challenger = new BackendUser() { DiscordId = (long)Context.User.Id },
                Enemy = new BackendUser() { DiscordId = (long)user.Id },
                Points = points,
            };


            string response = APIHelper.MakePostCall("pay", coinflip);
            coinflip = JsonConvert.DeserializeObject<BackendCoinflip>(response);

            RankHelper.UpdateRank(coinflip.Challenger, Context.Guild.GetUser(Context.User.Id), Context);
            RankHelper.UpdateRank(coinflip.Enemy, Context.Guild.GetUser(Context.User.Id), Context);

            switch (coinflip.Result)
            {
                case CoinflipVsResults.ChallengeDoesntExist:
                    await ReplyAsync($"{Context.User.Mention} doesn't exist in database!");
                    break;
                case CoinflipVsResults.EnemyWon:
                    await ReplyAsync($"{user.Mention} doesn't exist in database!");
                    break;
                case CoinflipVsResults.ChallengerNoPoints:
                    await ReplyAsync($"{Context.User.Mention} can't affort to donate this much! (max {string.Format("{0:C}", coinflip.Challenger.Points)})");
                    break;
                case CoinflipVsResults.ChallengerWon:
                    await ReplyAsync($"{Context.User.Mention} has donated {string.Format("{0:C}", coinflip.Points)} to {user.Mention}!");
                    break;
                case CoinflipVsResults.UnknownError:
                    await ReplyAsync($"{Context.User.Mention} An unknown error occured.");
                    break;
                default:
                    await ReplyAsync($"{Context.User.Mention} An unknown error occured!");
                    break;
            }
        }
    }
}
