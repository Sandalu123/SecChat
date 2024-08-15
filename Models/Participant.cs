using System;

namespace SecureChat.Models
{
    public class Participant
    {
        public string Id { get; set; } = Guid.NewGuid().ToString("N");
        public string SessionId { get; set; }
        public string ConnectionId { get; set; }
        public string RandomName { get; set; }
        public string AvatarCode { get; set; }
        public DateTime JoinedAt { get; set; } = DateTime.UtcNow;
        public DateTime LastActivity { get; set; } = DateTime.UtcNow;
        
        public virtual ChatSession Session { get; set; }
    }
}