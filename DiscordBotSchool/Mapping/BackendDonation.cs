namespace DiscordBotSchool.Mapping
{
    public enum DonationResult
    {
        DonationSuccesful,
        DonatorNoMoney,
        DonatorDoesntExist,
        ReceiverDoesntExist,
        UnknownError
    }


    public class BackendDonation
    {
        public BackendUser Donator;
        public BackendUser Receiver;
        public long Points;
        public DonationResult Result;
    }
}
