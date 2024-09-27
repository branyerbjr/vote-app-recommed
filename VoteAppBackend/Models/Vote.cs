using System;

namespace VoteAppBackend.Models
{
    public class Vote
    {
        public int Id { get; set; }
        public string Choice { get; set; } // "Cat" o "Dog"
        public DateTime Timestamp { get; set; }
    }
}
