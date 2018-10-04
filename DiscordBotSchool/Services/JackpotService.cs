using Discord;
using Discord.WebSocket;
using System;
using System.Threading;

namespace DiscordBotSchool.Services
{
    public class JackpotService
    {

        private readonly ulong CHANNEL_ID = 484640211519799308;
        private const int SECONDS = 5;
        private const int SECOND_ROTATIONS = 5;

        private readonly Timer _timer;
        private int TimerCompleted = 0;
                                                                   
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

        public void Restart()
        {
            _timer.Change(TimeSpan.FromSeconds(0), TimeSpan.FromSeconds(SECONDS));
        }
    }
}
