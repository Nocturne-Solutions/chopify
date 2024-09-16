namespace chopify.Models
{
    public class VotingSystemStatusDTO
    {
        public enum States
        {
            Stoped,
            RoundInProgress,
            InBetweenRounds
        }

        public States State { get; set; } = States.Stoped;
        public double? CurrentStateRemainingTimeSeconds { get; set; }
    }
}
