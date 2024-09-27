using System.Collections.Generic;

namespace VoteAppBackend.Models
{
    public class UserPreference
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        
        // Inicializar el diccionario para evitar valores nulos
        public Dictionary<string, int> Preferences { get; set; } = new Dictionary<string, int>();
    }
}
