using Discord.Commands;
using Discord.WebSocket;
using System.Threading.Tasks;
using Newtonsoft.Json;
using DiscordBotSchool.Modules.Helpers;
using DiscordBotSchool.Mapping;
using Discord;
using System;

namespace DiscordBotSchool.Modules.Commands
{
    public class Coinflipping : ModuleBase<SocketCommandContext>
    {
        [Command("coinflip")]
        public async Task Coinflip(string side, long points)
        {
            if (side.ToLower() != "heads" && side.ToLower() != "tails")
            {
                await ReplyAsync($"{Context.User.Mention} Please choose between heads or tails!");
                return;
            }

            if (points < 1)
            {
                await ReplyAsync($"{Context.User.Mention} Coinflip can't be lower than 0!");
                return;
            }

            CoinflipResult coinflipUser = new CoinflipResult()
            {
                User = new BackendUser() { DiscordId = (long)Context.User.Id, Points = points },
                ChosenSide = (side.ToLower() == "heads" ? 1 : 0),
            };

            string response = APIHelper.MakePostCall("gamble/coinflip", coinflipUser);
            coinflipUser = JsonConvert.DeserializeObject<CoinflipResult>(response);
            RankHelper.UpdateRank(coinflipUser.User, Context.Guild.GetUser(Context.User.Id), Context);

            if (coinflipUser.Result == CoinflipResults.Lost)
            {
                await ReplyAsync($"{Context.User.Mention} has lost and now has {string.Format("{0:C}", coinflipUser.User.Points)}!");
            }
            else if (coinflipUser.Result == CoinflipResults.Won)
            {
                await ReplyAsync($"{Context.User.Mention} has won and now has {string.Format("{0:C}", coinflipUser.User.Points)}!");
            }
            else if (coinflipUser.Result == CoinflipResults.NoPoints)
            {
                await ReplyAsync($"{Context.User.Mention} doesn't have the points to make that gamble (max {string.Format("{0:C}", coinflipUser.User.Points)})");
            }
        }

        [Command("coinflip")]
        public async Task CoinflipVs(SocketGuildUser enemy, string side, long points)
        {
            if (side.ToLower() != "heads" && side.ToLower() != "tails")
            {
                await ReplyAsync($"{Context.User.Mention} Please choose between heads or tails!");
                return;
            }

            if (points < 1)
            {
                await ReplyAsync($"{Context.User.Mention} Coinflip can't be lower than 0!");
                return;
            }

            if(enemy.Id == Context.Client.CurrentUser.Id)
            {
                await ReplyAsync("Please use 'coinflip [heads/tails] [amount]' instead");
                return;
            }

            BackendCoinflip coinflip = new BackendCoinflip()
            {
                Challenger = new BackendUser() { DiscordId = (long)Context.User.Id },
                Enemy = new BackendUser() { DiscordId = (long)enemy.Id },
                Points = points,
                Side = (side.ToLower() == "heads" ? 1 : 0)
            };

            string response = APIHelper.MakePostCall("gamble/coinflipvs", coinflip);
            coinflip = JsonConvert.DeserializeObject<BackendCoinflip>(response);
            RankHelper.UpdateRank(coinflip.Challenger, Context.Guild.GetUser(Context.User.Id), Context);
            RankHelper.UpdateRank(coinflip.Enemy, Context.Guild.GetUser(enemy.Id), Context);


            switch (coinflip.Result)
            {
                case CoinflipVsResults.ChallengerNoPoints:
                    await ReplyAsync($"{Context.User.Mention} doesn't have enough points (max {string.Format("{0:C}", coinflip.Challenger.Points)})!");
                    break;
                case CoinflipVsResults.EnemyNoPoints:
                    await ReplyAsync($"{Context.User.Mention} Your enemy ({enemy.Username}) doesn't have enough points (max {string.Format("{0:C}", coinflip.Enemy.Points)})!");
                    break;
                case CoinflipVsResults.ChallengeAlreadyExists:
                    await ReplyAsync($"{Context.User.Mention} you already have a challenge vs {enemy.Username} ({string.Format("{0:C}", coinflip.Points)})!");
                    break;
                case CoinflipVsResults.ChallengeRequestSet:
                    await ReplyAsync($"{Context.User.Mention} has challenged {enemy.Mention} for a coinflip battle ({string.Format("{0:C}", coinflip.Points)})!");
                    break;
                case CoinflipVsResults.UnknownError:
                    await ReplyAsync($"{Context.User.Mention} An unknown error occured.");
                    break;
                default:
                    await ReplyAsync($"{Context.User.Mention} An unknown error occured!");
                    break;
            }
        }

        [Command("accept")]
        public async Task AcceptCoinflipChallenge(SocketGuildUser challenger)
        {
            BackendCoinflip coinflip = new BackendCoinflip()
            {
                Challenger = new BackendUser() { DiscordId = (long)challenger.Id },
                Enemy = new BackendUser() { DiscordId = (long)Context.User.Id }
            };

            string response = APIHelper.MakePostCall("gamble/acceptcoinflip", coinflip);
            coinflip = JsonConvert.DeserializeObject<BackendCoinflip>(response);

            RankHelper.UpdateRank(coinflip.Challenger, Context.Guild.GetUser(Context.User.Id), Context);
            RankHelper.UpdateRank(coinflip.Enemy, Context.Guild.GetUser(challenger.Id), Context);

            switch (coinflip.Result)
            {
                case CoinflipVsResults.ChallengerWon:
                    await ReplyAsync($"{challenger.Mention} has won the coinflip vs {Context.User.Mention} ({string.Format("{0:C}", coinflip.Points)}) and now has {string.Format("{0:C}", coinflip.Challenger.Points)}!");
                    break;
                case CoinflipVsResults.EnemyWon:
                    await ReplyAsync($"{Context.User.Mention} has won the coinflip vs {challenger.Mention} ({string.Format("{0:C}", coinflip.Points)}) and now has {string.Format("{0:C}", coinflip.Enemy.Points)}!");
                    break;
                case CoinflipVsResults.ChallengerNoPoints:
                    await ReplyAsync($"{challenger.Mention} doesn't have the points to gamble {string.Format("{0:C}", coinflip.Points)} (max {coinflip.Challenger.Points})!");
                    break;
                case CoinflipVsResults.EnemyNoPoints:
                    await ReplyAsync($"{Context.User.Mention} doesn't have the points to gamble {string.Format("{0:C}", coinflip.Points)} (max {coinflip.Enemy.Points})!");
                    break;
                case CoinflipVsResults.ChallengeDoesntExist:
                    await ReplyAsync($"{Context.User.Mention} you don't have a challenge vs {challenger.Username}!");
                    break;
                case CoinflipVsResults.UnknownError:
                    await ReplyAsync($"{Context.User.Mention} An unknown error occured!");
                    break;
                case CoinflipVsResults.ChallengeDeclined:
                    await ReplyAsync($"{Context.User.Mention} Declined!");
                    break;
                default:
                    await ReplyAsync($"{Context.User.Mention} An unknown error occured!");
                    break;
            }
        }

        [Command("decline")]
        public async Task DeclineCoinflipChallenge(SocketGuildUser challenger)
        {
            BackendCoinflip coinflip = new BackendCoinflip()
            {
                Challenger = new BackendUser() { DiscordId = (long)challenger.Id },
                Enemy = new BackendUser() { DiscordId = (long)Context.User.Id }
            };

            string response = APIHelper.MakePostCall("gamble/declinecoinflip", coinflip);
            coinflip = JsonConvert.DeserializeObject<BackendCoinflip>(response);

            RankHelper.UpdateRank(coinflip.Challenger, Context.Guild.GetUser(Context.User.Id), Context);
            RankHelper.UpdateRank(coinflip.Enemy, Context.Guild.GetUser(challenger.Id), Context);

            switch (coinflip.Result)
            {
                case CoinflipVsResults.ChallengeDeclined:
                    await ReplyAsync($"{Context.User.Mention} has declined the duel vs {challenger.Mention}!");
                    break;
                case CoinflipVsResults.ChallengeDoesntExist:
                    await ReplyAsync($"{Context.User.Mention} you dont have a match vs {challenger.Mention}!");
                    break;
                case CoinflipVsResults.UnknownError:
                    await ReplyAsync($"{Context.User.Mention} An unknown error occured!");
                    break;
                default:
                    await ReplyAsync($"{Context.User.Mention} An unknown error occured!");
                    break;
            }
        }
    }
}
