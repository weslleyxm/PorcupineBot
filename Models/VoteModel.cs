namespace PorcupineBot.Models
{
    public class VoteModel
    {
        public string UserId { get; set; } = string.Empty;
        public int Votes { get; set; }
        public string Reason { get; set; } = string.Empty;
    }
}
