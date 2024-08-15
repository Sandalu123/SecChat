using System;
using System.Collections.Generic;

namespace SecureChat.Models
{
    public class ChatSession
    {
        public string Id { get; set; } = Guid.NewGuid().ToString("N");
        public DateTime CreationTime { get; set; } = DateTime.UtcNow;
        public DateTime LastActivity { get; set; } = DateTime.UtcNow;
        public virtual ICollection<Participant> Participants { get; set; } = new List<Participant>();
    }
}