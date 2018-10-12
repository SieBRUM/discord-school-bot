using Discord.Commands;
using DiscordBotSchool.Mapping;
using DiscordBotSchool.Services;
using System.Threading.Tasks;

namespace DiscordBotSchool.Modules.Commands.Gambling
{
    public class Jackpot : ModuleBase<SocketCommandContext>
    {
        private readonly JackpotService _service;

        public Jackpot(JackpotService service)
        {
            _service = service;
        }

        [Command("jackpot")]
        public async Task AddJackpot(long Points)
        {
            BackendJackpot jackpot = new BackendJackpot()
            {
                Points = Points,
                DiscordId = (long)Context.User.Id
            };

            jackpot = _service.RequestJackpot(jackpot);

            if(jackpot == null)
            {
                await ReplyAsync($"{Context.User.Mention} An unknown error occured!");
                return;
            }
            switch (jackpot.Status)
            {
                case JackpotStatus.UserNotEnoughPoints:
                    await ReplyAsync($"{Context.User.Mention} You do not have the points to make this bet (max {jackpot.User.Points})!");
                    break;
                case JackpotStatus.InvalidPoints:
                    await ReplyAsync($"{Context.User.Mention} Invalid bet!");
                    break;
                case JackpotStatus.UnknownError:
                    await ReplyAsync($"{Context.User.Mention} An unknown error occured!");
                    break;
                default:
                    await ReplyAsync($"{Context.User.Mention} Total points: {jackpot.TotalPoints} | your total bet: {jackpot.Points} | your win percentage {jackpot.WinChancePercentage} %");
                    break;
            }
        }
    }
}
    