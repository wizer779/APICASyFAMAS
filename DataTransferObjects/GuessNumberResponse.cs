namespace APICASyFAMAS.DataTransferObjects
{
    public class GuessNumberResponse
    {
        public int GameId { get; set; }
        public string AttemptedNumber { get; set; }
        public string Message { get; set; }
    }
}
