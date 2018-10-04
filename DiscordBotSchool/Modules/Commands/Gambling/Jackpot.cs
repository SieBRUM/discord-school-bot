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
                // Error?
            }
            else
            {
                // Win percentage etc..
                await ReplyAsync($"Total points: {jackpot.TotalPoints} | your total bet: {jackpot.Points} | your win percentage {jackpot.WinChancePercentage}");
            }
        }
    }
}
    