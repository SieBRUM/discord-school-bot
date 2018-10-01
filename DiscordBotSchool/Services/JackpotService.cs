using Discord;
using Discord.WebSocket;
using System;
using System.Threading;

namespace DiscordBotSchool.Services
{
    public class JackpotService
    {

        private readonly ulong CHANNEL_ID = 487166975794085888;
        private readonly Timer _timer;

        private int TimerCompleted = 0;
                                                                   
        public JackpotService(DiscordSocketClient client)
        {
            _timer = new Timer(async _ =>
            {
                var chan = client.GetChannel(CHANNEL_ID) as IMessageChannel;
                if (chan != null)
                {
                    TimerCompleted++;
                    if(TimerCompleted > 3)
                    {
                        _timer.Change(Timeout.Infinite, Timeout.Infinite);
                        await chan.SendMessageAsync("jackpot closed!");
                        TimerCompleted = 0;
                    }
                    else
                    {
                        await chan.SendMessageAsync($"Jackpot closing in {15 / TimerCompleted} seconds!");
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
            _timer.Change(TimeSpan.FromSeconds(0), TimeSpan.FromSeconds(5));
        }
    }
}
