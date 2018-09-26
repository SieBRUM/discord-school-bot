using Discord.Commands;
using Discord.WebSocket;
using System;
using System.IO;

namespace LoggingInfo
{
    public static class LogHelper
    {
        private static readonly string BASE_LOCATION = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), @"discord_bot\discord_logs\");

        public static void WriteLog(bool IsSuccess, SocketCommandContext Context, string Command, string ErrorReason)
        {
            var date = DateTime.Now;
            var datePrefix = "[" + date.TimeOfDay.Hours.ToString("00") + ":" + date.TimeOfDay.Minutes.ToString("00") + ":" + date.TimeOfDay.Seconds.ToString("00") + "]";
            var fileName = date.Day + "-" + date.Month + "-" + date.Year + ".txt";

            if(!File.Exists(BASE_LOCATION + fileName))
            {
                var file = File.Create(BASE_LOCATION + fileName);
                file.Close();
            }

            using (StreamWriter file = File.AppendText(BASE_LOCATION + fileName))
            {
                string text = datePrefix;
                text += IsSuccess ? "-SUCCESS- " : "-ERROR- ";

                text += Context.User.Username;
                if(Context.User.Username.Length < 7)
                    text += "\t \t executed '" + Command + "'";
                else
                    text += "\t executed '" + Command + "' " + ErrorReason;

                file.WriteLine(text);
            }
        }
    }
}
