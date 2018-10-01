using Discord;
using Discord.Commands;
using Discord.WebSocket;
using DiscordBotSchool.Services;
using System;
using System.Threading;
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
        public async Task AddJackpot(long points)
        {
            _service.Restart();
        }
    }
}
