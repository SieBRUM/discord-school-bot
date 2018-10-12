using Discord;
using Discord.WebSocket;
using DiscordBotSchool.Mapping;
using DiscordBotSchool.Modules.Helpers;
using Newtonsoft.Json;
using System;
using System.Threading;

namespace DiscordBotSchool.Services
{
    public class JackpotService
    {

        private readonly ulong CHANNEL_ID = 487166975794085888;
        private const int SECONDS = 5;
        private const int SECOND_ROTATIONS = 5;
        private readonly Timer _timer;

        private int TimerCompleted = 0;
        private bool IsLocked = false;
                                                                   
        public JackpotService(DiscordSocketClient client)
        {
            _timer = new Timer(async _ =>
            {
                var chan = client.GetChannel(CHANNEL_ID) as IMessageChannel;
                if (chan != null)
                {
                    if(TimerCompleted > SECOND_ROTATIONS)
                    {
                        _timer.Change(Timeout.Infinite, Timeout.Infinite);
                        await chan.SendMessageAsync("Jackpot closed!");
                        TimerCompleted = 0;
                        var jackpot = CloseJackpot();
                        var user = await chan.GetUserAsync((ulong)jackpot.User.DiscordId);
                        await chan.SendMessageAsync($"{user.Mention} has won the jackpot of {jackpot.TotalPoints} with {jackpot.WinChancePercentage}% chance to win!");
                    }
                    else
                    {
                        await chan.SendMessageAsync($"Jackpot closing in {(SECONDS * SECOND_ROTATIONS) - (SECONDS * TimerCompleted) + SECONDS} seconds!");
                        TimerCompleted++;
                    }
                }
            },
            null,
            Timeout.Infinite,
            Timeout.Infinite);  
        }

        public void Stop()
        {
            _timer.Change(Timeout.Infinite, Timeout.Infinite);
        }

        private void Restart()
        {
            if(!IsLocked)
                _timer.Change(TimeSpan.FromSeconds(0), TimeSpan.FromSeconds(SECONDS));
        }

        public BackendJackpot RequestJackpot(BackendJackpot jackpot)
        {
            if (IsLocked)
                return null;

            // Do some init
            if(TimerCompleted == 0)
            {
                return UpdateJackpot(jackpot);
            }
            else
            {
                TimerCompleted = 0;
                return UpdateJackpot(jackpot);
            }
        }

        private BackendJackpot UpdateJackpot(BackendJackpot jackpot)
        {
            // API call
            Restart();
            string response = APIHelper.MakePostCall("gamble/updatejackpot", jackpot);
            return JsonConvert.DeserializeObject<BackendJackpot>(response);

        }

        private BackendJackpot CloseJackpot()
        {
            // Actions should be queue'd but just to be sure..
            IsLocked = true;
            // API call
            string response = APIHelper.MakeGetRequest("gamble/endjackpot");
            IsLocked = false;
            return JsonConvert.DeserializeObject<BackendJackpot>(response);
        }
    }
}
