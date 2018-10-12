namespace DiscordBotSchool.Mapping
{
    public class BackendJackpot
    {
        public int Id;
        public int UserId;
        public long Points;
        public double WinChancePercentage;
        public long TotalPoints;
        public long DiscordId;
        public JackpotStatus Status;
        public BackendUser User;
    }

    public enum JackpotStatus
    {
        JackpotAlreadyExists,
        UserDoesntExist,
        UserNotEnoughPoints,
        JackpotEnded,
        InvalidPoints,
        UnknownError
    }
}
